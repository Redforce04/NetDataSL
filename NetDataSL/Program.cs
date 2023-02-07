﻿// <copyright file="Log.cs" company="Redforce04#4091">
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
        /// The hash of the git commit when this plugin was built. Used for API reference, update tracking, and error tracking.
        /// </summary>
#pragma warning disable SA1401
        public static string GitCommitHash = AssemblyInfo.CommitHash;
#pragma warning restore SA1401

        /// <summary>
        /// The version identifier. This is the branch that this build was made with.
        /// </summary>
        // ReSharper disable once NotAccessedField.Global
#pragma warning disable SA1401
        public static string VersionIdentifier = AssemblyInfo.CommitBranch;
#pragma warning restore SA1401

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
                if (configFilePath != string.Empty)
                {
                    Log.Debug($"creating config.");
                    var unused = new Config(configFilePath);
                }
                else
                {
                    Console.Error.Write($"No Config Detected.");
                    Environment.Exit(128);
                    return;
                }

                Log.Debug($"Sentry message.");

                // ReSharper disable once RedundantNameQualifier
                Sentry.SentrySdk.CaptureMessage($"Starting on commit {AssemblyInfo.CommitHash}, branch {AssemblyInfo.CommitBranch}");
                Log.Debug($"Plugin time.");
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
            foreach (string x in result)
            {
                properArguments.Add(x.Replace("\"", string.Empty));
            }
        }

        private static void InterpretArgument(string arg, int i, out float refreshTime, out string configFilePath)
        {
            Log.Debug($"Parsing argument {i} \'{arg}\'.");
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
