// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         NoStdOutLoggerProvider.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 7:13 PM
//    Created Date:     02/03/2023 7:13 PM
// -----------------------------------------

namespace NetDataSL.Networking.Classes;

using Microsoft.Extensions.Logging;

/// <summary>
/// A logger without StdOut logging to prevent conflicts.
/// </summary>
[ProviderAlias("NoStdOutLogger")]

// ReSharper disable once ClassNeverInstantiated.Global
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