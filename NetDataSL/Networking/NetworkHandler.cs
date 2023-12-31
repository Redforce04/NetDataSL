﻿// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         NetworkHandler.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     02/01/2023 10:26 AM
// -----------------------------------------

namespace NetDataSL.Networking;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

// using Newtonsoft.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

// ReSharper disable twice RedundantNameQualifier
using NetDataSL.Networking.Classes;
using NetDataSL.StructsAndClasses;
using Sentry;

/// <summary>
/// The network handler for starting gRPC events.
/// </summary>
public class NetworkHandler
{
    /// <summary>
    /// The amount of time a request is checked for rate-limit samples.
    /// </summary>
    internal const int RateLimitSampleTime = 5;

    /// <summary>
    /// The amount of requests to get rate-limited.
    /// </summary>
    internal const float RateLimitSampleAmount = 10;

    /// <summary>
    /// The max invalid api key requests before a sender gets blacklisted.
    /// </summary>
    internal const float MaxInvalidApiKeyRequests = 10;

    /// <summary>
    /// The timeout duration for a sender blacklist.
    /// Currently 15 minutes.
    /// </summary>
    internal const int TimeoutDuration = 900;

    /// <summary>
    /// The single instance of the NetworkHandler.
    /// </summary>
#pragma warning disable SA1401
    internal static NetworkHandler? Singleton;
#pragma warning restore SA1401

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    // ReSharper disable once InconsistentNaming
    private static Thread gRpcThread = null!;

    // ReSharper disable once InconsistentNaming
    private WebApplication app = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkHandler"/> class.
    /// </summary>
    /// <param name="host">The host to bind to.</param>
    internal NetworkHandler(string host = "")
    {
        if (Singleton != null)
        {
            return;
        }

        Log.AddBreadcrumb("Starting Web Application Thread", "Network Handler", new Dictionary<string, string>());

        Singleton = this;
#pragma warning disable IL2026
        ParameterizedThreadStart start = _ => { this.InitSocket(host == string.Empty ? "127.0.0.1:11011" : host); };
#pragma warning restore IL2026
        gRpcThread = new Thread(start);
        gRpcThread.Start();
        gRpcThread.IsBackground = false;
        Log.AddBreadcrumb("Web Application Thread is Running", "Network Handler", new Dictionary<string, string>());
    }

    /// <summary>
    /// Stops the web application.
    /// </summary>
    internal void Stop()
    {
        this.app.StopAsync();
    }

    [RequiresUnreferencedCode(
        "Calls Microsoft.AspNetCore.Builder.EndpointRouteBuilderExtensions.MapGet(String, Delegate)")]
    private void InitSocket(string host = "127.0.0.1:11011")
    {
        Log.AddBreadcrumb("Creating Builder with host", "Network Handler", new Dictionary<string, string>() { { "Host", $"http://{host}" } });

        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        this.app = builder.Build();

        // ReSharper disable once ArrangeThisQualifier
        this.app.MapPost("/packet", async (httpContext) => await this.ProcessPostRequest(httpContext));
        this.app.Run("http://" + host);
    }

    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
    private async Task ProcessPostRequest(HttpContext httpContext)
    {
        var sender = Sender.Get(httpContext.Request.Host.Host, httpContext.Request.Host.Port);
        if (sender.IsBlacklisted())
        {
            await this.SendResult(httpContext, StatusCodes.Status429TooManyRequests, "too many requests", sender);
            return;
        }

        try
        {
            // Flush the stream and make the stream reader.
            httpContext.Request.Body.Flush();
            StreamReader reader = new StreamReader(httpContext.Request.Body);

            // Get the body.
            var body = await reader.ReadToEndAsync();
            Log.AddBreadcrumb("Body of Packet", $"Network handler", new Dictionary<string, string>() { { "Body", body } });

            // Get the packet.
            Debug.Assert(PacketSerializerContext.Default.NetDataPacket != null, "PacketSerializerContext.Default.NetDataPacket != null");
            NetDataPacket? packet = System.Text.Json.JsonSerializer.Deserialize(body, PacketSerializerContext.Default.NetDataPacket);
            if (packet is null)
            {
                await this.SendResult(httpContext, StatusCodes.Status400BadRequest, "no packet received", sender);
                sender.ProcessRequest(true);
                return;
            }

            if (!this.ApiKeyIsValid(packet.Port, packet.ApiKey))
            {
                await this.SendResult(httpContext, StatusCodes.Status401Unauthorized, "invalid api key", sender);
                sender.ProcessRequest(true);
                return;
            }

            sender.ProcessRequest(false);

            try
            {
                float late = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (packet.Epoch + Plugin.ServerRefreshTime);
                if (late > 1f)
                {
                    Log.AddBreadcrumb("An old packet has been received. It will still be processed.", "Network Handler", new Dictionary<string, string>()
                    {
                        { "Port", packet.Port.ToString() },
                        { "Server Refresh Speed", Plugin.ServerRefreshTime.ToString("F") },
                        { "Late", late.ToString("F") },
                    });

                    // return;
                }

                UpdateProcessor.Singleton!.ProcessUpdate(packet);
                if (packet.RefreshSpeed > Plugin.ServerRefreshTime)
                {
                    await this.SendResult(httpContext, StatusCodes.Status200OK, "slow refresh time", sender, new Dictionary<string, object> { { "server refresh", Plugin.ServerRefreshTime } });
                    return;
                }

                UpdateProcessor.Singleton.ProcessUpdate(packet);
            }
            catch (Exception e)
            {
                Log.Error($"Could not capture packet.");
                SentrySdk.CaptureException(e);
            }

            await this.SendResult(httpContext, StatusCodes.Status200OK, "packet receieved", sender);
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);

            // If an error occurs.
            Log.AddBreadcrumb("Invalid Packet Received", "Network Handler", new Dictionary<string, string>() { { "Packet Body", e.ToString() } });
            Log.Error($"Invalid Packet Received from: {httpContext.Request.Host}.");

            await this.SendResult(httpContext, StatusCodes.Status400BadRequest, "bad packet receieved", sender);
        }
    }

    private bool ApiKeyIsValid(int port, string key)
    {
        Plugin.Singleton!.Servers.TryGetValue(port, out ServerConfig? confg);
        if (confg == null)
        {
            return false;
        }

        if (confg.Key != key)
        {
            return false;
        }

        return true;
    }

    private async Task SendResult(HttpContext httpContext, int response, string message, Sender sender, Dictionary<string, object>? optionalValues = null)
    {
        if (response != 200)
        {
            switch (response)
            {
                case 400:
                    await Results.BadRequest(message).ExecuteAsync(httpContext);
                    Log.AddBreadcrumb("Bad Request", "Packet Handler", new Dictionary<string, string>() { { "Sender", sender.Ip }, });
                    return;
                case 401:
                    await Results.Unauthorized().ExecuteAsync(httpContext);
                    Log.AddBreadcrumb("Unauthorized Request", "Packet Handler", new Dictionary<string, string>() { { "Sender", sender.Ip }, });
                    return;
                default:
                    await Results.Problem(message, null, response).ExecuteAsync(httpContext);
                    Log.AddBreadcrumb("Invalid Request", "Packet Handler", new Dictionary<string, string>()
                    {
                        { "Sender", sender.Ip },
                        { "Status", response.ToString() },
                        { "Message", message },
                    });
                    return;
            }
        }

        // Return the status.
        var data = optionalValues == null ? new Dictionary<string, object>() : optionalValues;
        data.Add("status", response);
        data.Add("message", message);
        await Results.Json(data).ExecuteAsync(httpContext);
    }
}
