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

using System.Text;

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

    /// <summary>
    /// Should Logs output directly into stdout - note that this may mess with the plugin so try to avoid it.
    /// </summary>
    private const bool DebugModeEnabled = true;
    private string _logPath = string.Empty;
    private List<string> _logMessages = null!;
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

        Singleton = this;
        this.Init();
    }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="x">The debug message to log.</param>
    public static void Debug(string x)
    {
        string log = $"[{DateTime.Now:G}] [Debug] {x}    ";
#pragma warning disable CS0162
        if (DebugModeEnabled)
        {
            // ReSharper disable once HeuristicUnreachableCode
            Singleton!._stdOut.Write(log.Replace("\n", string.Empty).Replace(Environment.NewLine, string.Empty));
            Singleton._stdOut.Flush();
            Thread.Sleep(50);
        }
#pragma warning restore CS0162

        Singleton._logMessages.Add(log);
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="x">The error message to log.</param>
    public static void Error(string x)
    {
        if (Singleton is null)
        {
            var unused = new Log();
        }

        string log = $"[{DateTime.Now:G}] [Error] {x}    ";

        // Singleton!._stdErr.Write(log.Replace("\n", "").Replace(Environment.NewLine, ""));
        // Singleton!._stdErr.Flush();
        Singleton!._logMessages.Add(log);
        Thread.Sleep(50);
    }

    /// <summary>
    /// Writes a line to the StdOut for the NetData API to receive.
    /// </summary>
    /// <param name="x">The information to send to StdOut.</param>
    public static void Line(string x)
    {
        Singleton!._stdOut.Write($"{x}    ".Replace("\n", string.Empty).Replace(Environment.NewLine, string.Empty));
        Singleton._stdOut.Flush();
        Thread.Sleep(50);
    }

    /// <summary>
    /// Adds a log message to the file log.
    /// </summary>
    /// <param name="message">The message to add to the file log.
    /// </param>
    public void AddLogMessage(string message)
    {
        this._logMessages.Add(message);
        if (DebugModeEnabled)
        {
            Line(message);
        }
    }

    /// <summary>
    /// Logs Messages.
    /// </summary>
    internal void LogMessages()
    {
        string concatLog = string.Empty;
        foreach (string log in this._logMessages)
        {
            concatLog += log + Environment.NewLine;
        }

        try
        {
            using FileStream fs = new FileStream(this._logPath, FileMode.Append, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.Write(concatLog);
            sw.Close();
            fs.Close();
            File.WriteAllText(this._logPath, concatLog);
        }
        catch (Exception e)
        {
            Log.Error($"Could not write to logfile. Error: \n{e}");
        }

        this._logMessages.Clear();
    }

    private void Init()
    {
        string directory = "/var/log/";
        this._logPath = directory + "ScpslPlugin.log";

        this._logMessages = new List<string>();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(this._logPath))
        {
            File.Create(this._logPath).Close();
        }

        this._stdOut = new StreamWriter(Console.OpenStandardOutput());
        this._stdErr = new StreamWriter(Console.OpenStandardError());

        // Log.Error($"Info: Log filepath: {_logPath}");
    }
}