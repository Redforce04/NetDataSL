// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         PluginControl.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 5:15 PM
//    Created Date:     01/28/2023 5:15 PM
// -----------------------------------------

namespace NetDataSL.API;

public static class PluginControl
{
    internal static void DisablePlugin()
    {
        Log.Debug($"Disabling Plugin.");
        Log.Line("DISABLE");
    }
}