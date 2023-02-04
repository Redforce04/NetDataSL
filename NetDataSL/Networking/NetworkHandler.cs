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

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NetDataSL.Networking;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NetDataService;

public class NetworkHandler
{
    private static NetworkHandler? _singleton;
    private static Thread gRPCThread;

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkHandler"/> class.
    /// </summary>
    internal NetworkHandler()
    {
        if (_singleton != null)
        {
            return;
        }

        Log.Debug($"Starting App");

        _singleton = this;
        gRPCThread = new Thread(this.InitSocket);
        gRPCThread.Start();
        gRPCThread.IsBackground = false;
        Log.Debug($"Running App");
    }

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


/// <summary>
/// A logger without StdOut logging to prevent conflicts.
/// </summary>
[ProviderAlias("NoStdOutLogger")]
public class NoStdOutLoggerProvider : ILoggerProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoStdOutLoggerProvider"/> class.
    /// </summary>
    internal NoStdOutLoggerProvider()
    {
    }


    /// <inheritdoc/>
    public void Dispose()
    {
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName)
    {
        return new NoStdOutLogger(this);
    }
}

public class NoStdOutLogger : ILogger
{
    protected readonly NoStdOutLoggerProvider _noStdOutLoggerProvider;

    /// <summary>
    /// The log level that will be logged.
    /// </summary>
    public LogLevel LogLevel;

    /// <summary>
    /// Initializes a new instance of the <see cref="NoStdOutLogger"/> class.
    /// </summary>
    /// <param name="noStdOutLoggerProvider">The provider</param>
    public NoStdOutLogger([NotNull] NoStdOutLoggerProvider noStdOutLoggerProvider)
    {
        _noStdOutLoggerProvider = noStdOutLoggerProvider;
        LogLevel = LogLevel.Information;
    }

    /// <inheritdoc/>
    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel)
    {
        return (int)logLevel < (int)this.LogLevel;
    }

    /// <inheritdoc/>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
    {
        if (!this.IsEnabled(logLevel))
        {
            return;
        }

        NetDataSL.Log.Debug("log");
        NetDataSL.Log.Singleton.AddLogMessage($"[{DateTime.Now:G}] [AspNet] ({logLevel}) {formatter(state, exception)} {(exception != null ? exception.StackTrace : string.Empty)}");
    }
}




