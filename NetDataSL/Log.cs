// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Log.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/03/2023 1:18 PM
//    Created Date:     01/27/2023 9:47 PM
// -----------------------------------------

namespace NetDataSL;

using System.Collections.Concurrent;
using Sentry;

/// <summary>
/// The logging system.
/// </summary>
public class Log
{
    /// <summary>
    /// The instance of the <see cref="Log"/> Instance.
    /// </summary>
#pragma warning disable CA2211, SA1401
    public static Log? Singleton;
#pragma warning restore SA1401, CA2211

    // ReSharper disable HeuristicUnreachableCode

    /// <summary>
    /// Should Logs output directly into stdout - note that this may mess with the plugin so try to avoid it.
    /// </summary>
    private static bool _debugModeEnabled = true;
    private string _logPath = string.Empty;
    private string _debugLineOutPath = string.Empty;
    private ConcurrentQueue<string> _logMessages = null!;
    private ConcurrentQueue<string> _debugLineOutMessages = null!;
    private StreamWriter _stdOut = null!;

    // ReSharper disable once NotAccessedField.Local
    private StreamWriter _stdErr = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="Log"/> class.
    /// </summary>
    public Log()
    {
        if (Singleton != null)
        {
            return;
        }

        _debugModeEnabled = Config.Singleton!.DebugMode;
        Singleton = this;
        this.Init();
    }

    /// <summary>
    /// Adds a breadcrumb if debugging is enabled or the level is info or higher.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category to log.</param>
    /// <param name="dictionary">Any values to be included.</param>
    /// <param name="level">The level of the debug. Debug is only logged if debug mode is enabled.</param>
    /// <param name="type">Should be default.</param>
    public static void AddBreadcrumb(string message, string category, Dictionary<string, string>? dictionary = null, BreadcrumbLevel level = BreadcrumbLevel.Debug, string type = "default")
    {
        if (_debugModeEnabled && level == BreadcrumbLevel.Debug)
        {
            SentrySdk.AddBreadcrumb(message, category, type, dictionary, level);
        }
        else if ((int)level >= 0)
        {
            SentrySdk.AddBreadcrumb(message, category, type, dictionary, level);
        }
    }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="x">The debug message to log.</param>
    public static void Debug(string x)
    {
        // ReSharper disable once RedundantAssignment
        string log = $"[{DateTime.Now:G}] [Debug] {x}    ";
#pragma warning disable CS0162
        if (_debugModeEnabled)
        {
            if (Singleton is not null)
            {
                // Singleton._stdOut.Write(log.Replace("\n", string.Empty).Replace(Environment.NewLine, string.Empty));
                /*Singleton._stdOut.Write(log + "\n");
                Singleton._stdOut.Flush();
                Thread.Sleep(50); */
                Singleton._logMessages.Enqueue(log);
            }
            else
            {
                // Console.Write(log);
            }

            SentrySdk.CaptureMessage(log, SentryLevel.Debug);
        }

#pragma warning restore CS0162

    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="x">The error message to log.</param>
    public static void Error(string x)
    {
        string log = $"[{DateTime.Now:G}] [Error] {x}    ";
        SentrySdk.CaptureMessage(log, SentryLevel.Error);
        if (Singleton is null)
        {
            var unused = new Log();
        }

        SentrySdk.CaptureMessage(log);

        // Singleton!._stdErr.Write(log.Replace("\n", "").Replace(Environment.NewLine, ""));
        // Singleton!._stdErr.Flush();
        Singleton!._logMessages.Enqueue(log);

        // Thread.Sleep(50);
    }

    /// <summary>
    /// Writes a line to the StdOut for the NetData API to receive.
    /// </summary>
    /// <param name="x">The information to send to StdOut.</param>
    public static void Line(string x)
    {
        // Singleton!._stdOut.Write($"{x}    ".Replace("\n", string.Empty).Replace(Environment.NewLine, string.Empty));
        Singleton!._stdOut.Write($"{x}\n");
        Singleton!._stdOut.Flush();

        // Singleton._stdOut.Flush();
        // Thread.Sleep(10);
        if (_debugModeEnabled)
        {
            Singleton.AddLogMessage($"[{DateTime.Now:G}] {x}", true);
        }

        // Thread.Sleep(50);
    }

    /// <summary>
    /// Adds a log message to the file log.
    /// </summary>
    /// <param name="message">The message to add to the file log.</param>
    /// <param name="lineOut">If the message is sent from the line() method.</param>
    public void AddLogMessage(string message, bool lineOut = false)
    {
        if (!lineOut)
        {
            this._logMessages.Enqueue(message);
        }
        else if (_debugModeEnabled && lineOut)
#pragma warning disable CS0162
        {
            this._debugLineOutMessages.Enqueue(message);
        }
#pragma warning restore CS0162
    }

    /// <summary>
    /// Logs Messages.
    /// </summary>
    internal void LogMessages()
    {
        try
        {
            File.AppendAllLines(this._logPath, this._logMessages);
            if (_debugModeEnabled)
            {
                File.AppendAllLines(this._debugLineOutPath, this._debugLineOutMessages);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Could not write to logfile. Error: \n{e}");
            Environment.Exit(128);
            SentrySdk.CaptureException(e);
        }

        this._logMessages.Clear();
        this._debugLineOutMessages.Clear();
    }

    private void Init()
    {
        try
        {
            this._logPath = Config.Singleton!.LogPath; // + "/NetDataSL.log";
            string directory = this._logPath.Substring(0, this._logPath.LastIndexOf("/", StringComparison.Ordinal));
            this._debugLineOutPath = directory + "/debug-output.log";
            this._debugLineOutMessages = new ConcurrentQueue<string>();
            this._debugLineOutMessages.Enqueue("\n");
            this._logMessages = new ConcurrentQueue<string>();
            this._logMessages.Enqueue("\n");
            this._logMessages.Enqueue($"Loading NetDataSL Integration on commit {AssemblyInfo.CommitHash}, and branch {AssemblyInfo.CommitBranch}.");

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            try
            {
                if (!File.Exists(this._logPath))
                {
                    File.Create(this._logPath).Close();
                }

                if (_debugModeEnabled && !File.Exists(this._debugLineOutPath))
                {
                    File.Create(this._debugLineOutPath).Close();
                }
            }
            catch (Exception e)
            {
                Log.Error("Error at Log.Init() => Creating logging file.");
                SentrySdk.CaptureException(e);
            }

            this._stdOut = new StreamWriter(Console.OpenStandardOutput());
            this._stdErr = new StreamWriter(Console.OpenStandardError());

            // Sentry.SentrySdk.CaptureMessage($"(Commit {AssemblyInfo.CommitHash}, branch {AssemblyInfo.CommitBranch}) Debug mode: {_debugModeEnabled}, debug output: {this._debugLineOutPath}, log output: {this._logPath}, directory({directory})");
            // Log.Error($"Info: Log filepath: {_logPath}");
        }
        catch (Exception e)
        {
            Log.Error("Error at Log.Init()");
            SentrySdk.CaptureException(e);
        }
    }
}
