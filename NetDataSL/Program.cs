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

            using (SentrySdk.Init(o =>
                   {
                       o.Dsn = "https://841ce728bc284365be420b1fce6e133e@sentry.peanutworshipers.net/2";

                       // When configuring for the first time, to see what the SDK is doing:
                       o.Debug = false;
                       o.Release = AssemblyInfo.CommitHash;
                       o.Environment = AssemblyInfo.CommitBranch;
                       o.AutoSessionTracking = true;

                       // Set traces_sample_rate to 1.0 to capture 100% of transactions for performance monitoring.
                       // We recommend adjusting this value in production.
                       o.TracesSampleRate = 1.0;

                       // Enable Global Mode if running in a client app
                       o.IsGlobalModeEnabled = true;
                   }))
            {
                SentrySdk.CaptureMessage($"Starting on commit {AssemblyInfo.CommitHash}, branch {AssemblyInfo.CommitBranch}");
                var refreshTime = 5f;
                string configFilePath = string.Empty;
                SentrySdk.AddBreadcrumb("Parsing Args", "Startup", "default", new Dictionary<string, string>(), BreadcrumbLevel.Debug);
                if (args.Length > 0)
                {
                    try
                    {
                        for (int i = 0; i < args.Length; i++)
                        {
                            InterpretArgument(args[i], i, out refreshTime, out configFilePath);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Could not parse startup args. Args: Exception: \n{e}");
                    }
                }

                SentrySdk.AddBreadcrumb("Args Parsed", "Startup", "default", new Dictionary<string, string>(), BreadcrumbLevel.Debug);

                // App code goes here. Dispose the SDK before exiting to flush events.
                if (configFilePath != string.Empty)
                {
                    SentrySdk.AddBreadcrumb("Loading Config", "Startup", "default", new Dictionary<string, string>() { { "path", $"{configFilePath}" } }, BreadcrumbLevel.Debug);
                    var unused = new Config(configFilePath);
                    SentrySdk.AddBreadcrumb(
                        "Config Loaded",
                        "Startup",
                        "default",
                        new Dictionary<string, string>()
                        {
                            { $"address", $"{unused.ServerAddress}" },
                            { $"config path", $"{unused.ConfigPath}" },
                            { $"directory path", $"{unused.DirectoryPath}" },
                            { $"debug mode", $"{unused.DebugMode}" },
                            { $"send rate", $"{unused.SendRate}" },
                            { $"servers", $"{unused.ServerInstances.Count}" },
                        },
                        BreadcrumbLevel.Debug);
                }
                else
                {
                    SentrySdk.AddBreadcrumb(
                        "No Config Detected",
                        "Startup",
                        "default",
                        new Dictionary<string, string>()
                        {
                            {
                                "args",
                                args.Aggregate((x, y) =>
                                {
                                    return x + $"\"{y}\", ";
                                })
                            },
                        },
                        BreadcrumbLevel.Error);
                    SentrySdk.CaptureMessage($"No config detected.", SentryLevel.Error);
                    Console.Error.Write($"No Config Detected.");
                    Environment.Exit(128);
                    return;
                }

                // ReSharper disable once RedundantNameQualifier
                SentrySdk.AddBreadcrumb("Loading Plugin", "Startup", "default", new Dictionary<string, string>() { { "server address", $"{Config.Singleton!.ServerAddress}" } }, BreadcrumbLevel.Debug);
                var unused2 = new Plugin(refreshTime, Config.Singleton.ServerAddress);
                SentrySdk.AddBreadcrumb("Plugin Loaded", "Startup", "default", new Dictionary<string, string>(), BreadcrumbLevel.Debug);
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
