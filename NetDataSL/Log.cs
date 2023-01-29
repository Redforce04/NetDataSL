// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Log.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 1:34 PM
//    Created Date:     01/27/2023 9:47 PM
// -----------------------------------------

namespace NetDataSL;

public static class Log
{
    private const bool DebugModeEnabled = false;

    public static void Debug(string x)
    {
        if (DebugModeEnabled)
            Console.WriteLine($"[Debug] {x}");
    }

    public static void Error(string x)
    {
        Console.Error.WriteLine($"[{DateTime.UtcNow.ToString("G")}] [Error] {x}");
    }

    public static void Line(string x)
    {
        Console.WriteLine(x);
    }
}