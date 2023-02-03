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

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NetDataService;

public class NetworkHandler
{
    private static NetworkHandler? _singleton;

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkHandler"/> class.
    /// </summary>
    internal NetworkHandler()
    {
        if (_singleton != null)
        {
            return;
        }

        _singleton = this;
        this.InitSocket();
    }

    private void InitSocket()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddGrpc();

        var app = builder.Build();

        app.MapGrpcService<NetDataPacketSender.NetDataPacketSenderBase>();
        app.Run();
    }
}




