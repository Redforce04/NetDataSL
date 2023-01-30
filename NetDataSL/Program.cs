// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Program.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 1:34 PM
//    Created Date:     01/25/2023 12:20 PM
// -----------------------------------------

using NetDataSL;
Thread.Sleep(1000);
var refreshTime = 5f;
if (args.Length > 0)
{

    try
    {
        float.TryParse(args[0], out refreshTime);
    }
    catch (Exception e)
    {
        Log.Error($"Could not parse startup args. Args: {args[0]} Exception: \n{e}");
    }
}

var unused = new Plugin(refreshTime);