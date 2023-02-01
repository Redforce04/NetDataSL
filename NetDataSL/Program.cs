﻿// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Program.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 1:34 PM
//    Created Date:     01/25/2023 12:20 PM
// -----------------------------------------

using System.Reflection;
using Sentry;
namespace NetDataSL
{
    class Program
    {
        public static void Main(string[] args)
        {
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

            _getVersionInstances();
            using (SentrySdk.Init(o =>
                   {
                       o.Dsn = "https://841ce728bc284365be420b1fce6e133e@sentry.peanutworshipers.net/2";
                       // When configuring for the first time, to see what the SDK is doing:
                       o.Debug = false;
                       o.Release = GitCommitHash;

                       o.AutoSessionTracking = true;

                       // Set traces_sample_rate to 1.0 to capture 100% of transactions for performance monitoring.
                       // We recommend adjusting this value in production.
                       o.TracesSampleRate = 1.0;
                       // Enable Global Mode if running in a client app
                       o.IsGlobalModeEnabled = true;
                   }))
            {
                // App code goes here. Dispose the SDK before exiting to flush events.
                var unused = new Plugin(refreshTime);
            }
        }

        private static void _getVersionInstances()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream("PWProfiler.version.txt")!)
                using (StreamReader reader = new StreamReader(stream))
                {
                    GitCommitHash = reader.ReadToEnd();
                }

                using (Stream stream = assembly.GetManifestResourceStream("PWProfiler.versionIdentifier.txt")!)
                using (StreamReader reader = new StreamReader(stream))
                    VersionIdentifier = reader.ReadToEnd();

            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }

        }

        public static string GitCommitHash = String.Empty;
        public static string VersionIdentifier = String.Empty;
    }
}
