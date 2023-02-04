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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetDataService;

// ReSharper disable once RedundantNameQualifier
using NetDataSL.Networking.Classes;

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
    [RequiresUnreferencedCode("Calls NetDataSL.Networking.NetworkHandler.InitSocket()")]
    internal NetworkHandler()
    {
        if (_singleton != null)
        {
            return;
        }

        Log.Debug($"Starting App");

        _singleton = this;
        _gRpcThread = new Thread(this.InitSocket);
        _gRpcThread.Start();
        _gRpcThread.IsBackground = false;
        Log.Debug($"Running App");
    }

    [RequiresUnreferencedCode("Calls Microsoft.AspNetCore.Builder.EndpointRouteBuilderExtensions.MapGet(String, Delegate)")]
    private void InitSocket()
    {
        Log.Debug($"Creating Builder");
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddGrpc();

        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new NoStdOutLoggerProvider());
        var app = builder.Build();
        app.MapGrpcService<NetDataPacketSender.NetDataPacketSenderBase>();
        app.MapGet("/", () => "{ \"status\": 200, \"message\": \"app running normally\" }");
        app.Run("http://localhost:11011");
    }
}
