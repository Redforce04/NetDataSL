// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Program.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/25/2023 12:20 PM
// -----------------------------------------

namespace NetDataSL
{
    using System.Text.RegularExpressions;
    using Sentry;

    /// <summary>
    /// The main program. Where everything is started.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main startup method.
        /// </summary>
        /// <param name="args">The arguments. Should be one float for the refresh rate.</param>
        public static void Main(string[] args)
        {
            Thread.Sleep(1000);

            var refreshTime = 5f;
            string configFilePath = string.Empty;
            if (args.Length > 0)
            {
                ProcessArguments(args, out List<string> properArguments);

                try
                {
                    for (int i = 0; i < properArguments.Count; i++)
                    {
                        InterpretArgument(properArguments[i], i, out refreshTime, out configFilePath);
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Could not parse startup args. Args: Exception: \n{e}");
                }
            }

            using (SentrySdk.Init(o =>
                   {
                       o.Dsn = "https://841ce728bc284365be420b1fce6e133e@sentry.peanutworshipers.net/2";

                       // When configuring for the first time, to see what the SDK is doing:
                       o.Debug = false;
                       o.Release = AssemblyInfo.CommitHash;

                       o.AutoSessionTracking = true;

                       // Set traces_sample_rate to 1.0 to capture 100% of transactions for performance monitoring.
                       // We recommend adjusting this value in production.
                       o.TracesSampleRate = 1.0;

                       // Enable Global Mode if running in a client app
                       o.IsGlobalModeEnabled = true;
                   }))
            {
                // App code goes here. Dispose the SDK before exiting to flush events.
                if (configFilePath != string.Empty)
                {
                    var unused = new Config(configFilePath);
                }
                else
                {
                    Console.Error.Write($"No Config Detected.");
                    Environment.Exit(128);
                    return;
                }

                // ReSharper disable once RedundantNameQualifier
                Sentry.SentrySdk.CaptureMessage($"Starting on commit {AssemblyInfo.CommitHash}, branch {AssemblyInfo.CommitBranch}");
                var unused2 = new Plugin(refreshTime, Config.Singleton!.ServerAddress);
            }
        }

        private static void ProcessArguments(string[] args, out List<string> properArguments)
        {
            properArguments = new List<string>();
            string combinedargs = string.Empty;
            foreach (string arg in args)
            {
                combinedargs += $"{arg} ";
            }

            Regex regex = new Regex("(\"[^\"]+\"|[^\\s\"]+)");
            var result = regex.Matches(combinedargs);
            foreach (Match match in result)
            {
                properArguments.Add(match.Value.Replace("\"", string.Empty));
            }
        }

        private static void InterpretArgument(string arg, int i, out float refreshTime, out string configFilePath)
        {
            refreshTime = 5f;
            configFilePath = string.Empty;
            switch (i)
            {
                // Argument 1 - Refresh rate.
                case 0:
                    float.TryParse(arg, out refreshTime);
                    break;

                // Argument 2 - config file.
                case 1:
                    configFilePath = arg;
                    break;
            }
        }
    }
}
