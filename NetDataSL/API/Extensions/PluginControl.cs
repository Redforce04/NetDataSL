// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         PluginControl.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/28/2023 5:15 PM
// -----------------------------------------

namespace NetDataSL.API.Extensions;

/// <summary>
/// The class for controlling a plugin.
/// </summary>
public static class PluginControl
{
    /// <summary>
    /// Disables a plugin.
    /// </summary>
    internal static void DisablePlugin()
    {
        Log.Debug($"Disabling Plugin.");
        Log.Line("DISABLE");
    }
}