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
    using System.Reflection;
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
        public static string GitCommitHash = string.Empty;
#pragma warning restore SA1401

        /// <summary>
        /// The version identifier. This is the branch that this build was made with.
        /// </summary>
        // ReSharper disable once NotAccessedField.Global
#pragma warning disable SA1401
        public static string VersionIdentifier = string.Empty;
#pragma warning restore SA1401

        /// <summary>
        /// The main startup method.
        /// </summary>
        /// <param name="args">The arguments. Should be one float for the refresh rate.</param>
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

            GetVersionInstances();
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
                string host = string.Empty;
                for (int i = 1; i < args.Length - 1; i++)
                {
                    host += args[i] + " ";
                }

                // App code goes here. Dispose the SDK before exiting to flush events.
                var unused = new Plugin(refreshTime, host);
            }
        }

        private static void GetVersionInstances()
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
                {
                    VersionIdentifier = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}
