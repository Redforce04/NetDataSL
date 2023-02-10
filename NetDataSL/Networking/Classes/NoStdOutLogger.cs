// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         NoStdOutLogger.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 7:12 PM
//    Created Date:     02/03/2023 7:12 PM
// -----------------------------------------

namespace NetDataSL.Networking.Classes;

using Microsoft.Extensions.Logging;
#pragma warning disable SA1401

/// <summary>
/// The logger that prevents logging into the std out for asp.net.
/// </summary>
public class NoStdOutLogger : ILogger
{
    /// <summary>
    /// THe instance of the provider.
    /// </summary>
    // ReSharper disable once NotAccessedField.Global
    public readonly NoStdOutLoggerProvider NoStdOutLoggerProvider;

    /// <summary>
    /// The log level that will be logged.
    /// </summary>
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public LogLevel LogLevel;

    /// <summary>
    /// Initializes a new instance of the <see cref="NoStdOutLogger"/> class.
    /// </summary>
    /// <param name="noStdOutLoggerProvider">The provider.</param>
    public NoStdOutLogger(NoStdOutLoggerProvider noStdOutLoggerProvider)
    {
        this.NoStdOutLoggerProvider = noStdOutLoggerProvider;
        this.LogLevel = LogLevel.Information;
    }

    /// <inheritdoc/>
#pragma warning disable CS8633, CS8603
    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
#pragma warning restore CS8633, CS8603
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
        if (NetDataSL.Log.Singleton != null)
        {
            if (exception != null)
            {
                NetDataSL.Log.Singleton.AddLogMessage(
                    $"[{DateTime.Now:G}] [AspNet] ({logLevel}) {formatter(state, exception)} {exception.StackTrace}");
            }
        }
    }
}
#pragma warning restore SA1401
