﻿// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Log.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 1:34 PM
//    Created Date:     01/27/2023 9:47 PM
// -----------------------------------------

using System.Text;

namespace NetDataSL;

public class Log
{
    internal static Log? Singleton;
    public Log()
    {
        if (Singleton != null)
            return;
        Singleton = this;
        _init();
    }

    private string _logPath = "";
    private void _init()
    {
        string directory = "/var/log/NetDataSL/";
        _logPath = directory + "ScpslPlugin.log";
        
        _logMessages = new List<string>();
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        if (!File.Exists(_logPath))
            File.Create(_logPath).Close();
        
        _stdOut = new StreamWriter(Console.OpenStandardOutput());
        _stdErr = new StreamWriter(Console.OpenStandardError());
        //Log.Error($"Info: Log filepath: {_logPath}");
    }

    private List<string> _logMessages = null!;
    private StreamWriter _stdOut;
    private StreamWriter _stdErr;
    internal void LogMessages()
    {
        string concatLog = "";
        foreach (string log in _logMessages)
        {
            concatLog += log + Environment.NewLine;
        }

        try
        {
            using FileStream fs = new FileStream(_logPath, FileMode.Append, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.Write(concatLog);
            sw.Close();
            fs.Close();
            File.WriteAllText(_logPath, concatLog);
        }
        catch (Exception e)
        {
            Log.Error($"Could not write to logfile. Error: \n{e}");
        }

        _logMessages.Clear();
    }
    
    /// <summary>
    /// Should it output directly into stdout - note that this may mess with the plugin so try to avoid it.
    /// </summary>
    private const bool DebugModeEnabled = false;
    public static void Debug(string x)
    {
        string log = $"[{DateTime.Now:G}] [Debug] {x}    ";
#pragma warning disable CS0162
        if (DebugModeEnabled)
        {
            // ReSharper disable once HeuristicUnreachableCode
            Singleton!._stdOut.Write(log.Replace("\n", "").Replace(Environment.NewLine, ""));
            Singleton!._stdOut.Flush();
            Thread.Sleep(50);
        }
#pragma warning restore CS0162
        
        Singleton!._logMessages.Add(log);
    }
    
    public static void Error(string x)
    {
        string log = $"[{DateTime.Now:G}] [Error] {x}    ";
        //Singleton!._stdErr.Write(log.Replace("\n", "").Replace(Environment.NewLine, ""));
        //Singleton!._stdErr.Flush();
        
        Singleton!._logMessages.Add(log);
        Thread.Sleep(50);
    }

    public static void Line(string x)
    {
        Singleton!._stdOut.Write($"{x}    ".Replace("\n", "").Replace(Environment.NewLine, ""));
        Singleton!._stdOut.Flush();
        Thread.Sleep(50);
        
    }
}