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

namespace NetDataSL.Networking;

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetDataService;

// ReSharper disable twice RedundantNameQualifier
using NetDataSL.Networking.Classes;
using NetDataSL.Structs;
using Newtonsoft.Json;

/// <summary>
/// The network handler for starting gRPC events.
/// </summary>
public class NetworkHandler
{
    private static NetworkHandler? _singleton;

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private static Thread _gRpcThread = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkHandler"/> class.
    /// </summary>
    /// <param name="host">The host to bind to.</param>
    [RequiresUnreferencedCode("Calls NetDataSL.Networking.NetworkHandler.InitSocket()")]
    internal NetworkHandler(string host = "")
    {
        if (_singleton != null)
        {
            return;
        }

        Log.Debug($"Starting App");

        _singleton = this;
        ParameterizedThreadStart start = _ => { this.InitSocket(host == string.Empty ? "http://localhost:11011" : host); };
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
        app.MapGrpcService<NetDataPacketSender.NetDataPacketSenderBase>();
        app.MapPost("/packet", async (httpContext) =>
        {
            try
            {
                httpContext.Request.Body.Flush();
                StreamReader reader = new StreamReader(httpContext.Request.Body);
                var body = await reader.ReadToEndAsync();
                Log.Debug($"body: {body}");
                var packet = JsonConvert.DeserializeObject<NetDataPacketHandler>(body);
                Log.Debug($"Packet Received.");
                var data = new Dictionary<string, object>();
                data.Add("status", 200);
                data.Add("message", "packet receieved");
                await Results.Json(data).ExecuteAsync(httpContext);
            }
            catch (Exception e)
            {
                Log.Debug($"Invalid Packet Received.");
                Log.Debug(e.ToString());
                var data = new Dictionary<string, object>();
                data.Add("status", 400);
                data.Add("message", "bad packet");
                await Results.Json(data).ExecuteAsync(httpContext);
            }
        });
        app.Run(host);
    }
}
