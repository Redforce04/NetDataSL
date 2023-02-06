// <copyright file="Log.cs" company="Redforce04#4091">
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

using Sentry;

namespace NetDataSL.Networking;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

// using Newtonsoft.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable twice RedundantNameQualifier
using NetDataSL.Networking.Classes;
using NetDataSL.StructsAndClasses;

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

    private static NetworkHandler? _singleton;

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private static Thread _gRpcThread = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkHandler"/> class.
    /// </summary>
    /// <param name="host">The host to bind to.</param>
    internal NetworkHandler(string host = "")
    {
        if (_singleton != null)
        {
            return;
        }

        Log.Debug($"Starting App");

        _singleton = this;
#pragma warning disable IL2026
        ParameterizedThreadStart start = _ => { this.InitSocket(host == string.Empty ? "http://localhost:11011" : host); };
#pragma warning restore IL2026
        _gRpcThread = new Thread(start);
        _gRpcThread.Start();
        _gRpcThread.IsBackground = false;
        Log.Debug($"Running App");
    }

    [RequiresUnreferencedCode(
        "Calls Microsoft.AspNetCore.Builder.EndpointRouteBuilderExtensions.MapGet(String, Delegate)")]
    private void InitSocket(string host = "http://localhost:11011")
    {
        Log.Debug($"Creating Builder with host {host}");
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddGrpc();

        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new NoStdOutLoggerProvider());
        var app = builder.Build();

        // ReSharper disable once ArrangeThisQualifier
        app.MapPost("/packet", async (httpContext) => await this.ProcessPostRequest(httpContext));
        app.Run(host);
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
            Log.Debug($"body: {body}");

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

            Plugin.Singleton!.ProcessPacket(packet);
            await this.SendResult(httpContext, StatusCodes.Status200OK, "packet receieved", sender);
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);

            // If an error occurs.
            Log.Debug($"Invalid Packet Received from: {httpContext.Request.Host}.");
            Log.Debug(e.ToString());

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

    private async Task SendResult(HttpContext httpContext, int response, string message, Sender sender)
    {
        if (response != 200)
        {
            switch (response)
            {
                case 400:
                    await Results.BadRequest(message).ExecuteAsync(httpContext);
                    Log.Debug($"Bad request from sender {sender}");
                    return;
                case 401:
                    await Results.Unauthorized().ExecuteAsync(httpContext);
                    Log.Debug($"Unauthorized request from sender {sender}");
                    return;
                default:
                    await Results.Problem(message, null, response).ExecuteAsync(httpContext);
                    Log.Debug($"Invalid request (status {response}, message: {message}), from sender {sender}");
                    return;
            }
        }

        // Return the status.
        var data = new Dictionary<string, object>();
        data.Add("status", response);
        data.Add("message", message);
        await Results.Json(data).ExecuteAsync(httpContext);
    }
}
