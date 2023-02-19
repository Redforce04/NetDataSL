// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Config.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/05/2023 12:30 PM
//    Created Date:     02/05/2023 12:30 PM
// -----------------------------------------

namespace NetDataSL;

using System.Text.Json;
using System.Text.Json.Serialization;

// ReSharper disable once RedundantNameQualifier
using NetDataSL.StructsAndClasses;

/// <summary>
/// The main config for the plugin.
/// </summary>
public class Config
{
    /// <summary>
    /// Gets or sets the main instance of the config.
    /// </summary>
    [JsonIgnore]
#pragma warning disable SA1401
    public static Config? Singleton;
#pragma warning restore SA1401

    /// <summary>
    /// Gets the path to this instance of the config.
    /// </summary>
    [JsonIgnore]
    public string ConfigPath { get; private set; }

    /// <summary>
    /// Gets the path to the directory which this config is stored.
    /// </summary>
    [JsonIgnore]
    public string DirectoryPath { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Config"/> class.
    /// </summary>
    /// <param name="path">The path of the main config to load.</param>
#pragma warning disable CS8618,SA1201
    public Config(string path)
#pragma warning restore CS8618,SA1201
    {
        if (Singleton != null)
        {
            return;
        }

        this.ConfigPath = path;
        string indexKey = path.Contains('/') ? "/" : "\\";
        this.DirectoryPath = path.Substring(0, path.LastIndexOf(indexKey, StringComparison.Ordinal));
        Singleton = this;
        if (!Directory.Exists(this.DirectoryPath))
        {
            Directory.CreateDirectory(this.DirectoryPath);
        }

        bool createFile = !File.Exists(this.ConfigPath);
        FileStream stream = new FileStream(
            this.ConfigPath,
            FileMode.OpenOrCreate,
            createFile ? FileAccess.Write : FileAccess.Read,
            createFile ? FileShare.Write : FileShare.Read);

        if (createFile)
        {
            StreamWriter writer = new StreamWriter(stream);
            this.LogPath = this.DirectoryPath + "/scpsl.log";
            this.ServerAddress = "127.0.0.1:11011";
            this.DebugMode = false;
            this.SendRate = 5f;
            this.ServerInstances = new List<ServerConfig>()
            {
                new ServerConfig(7777, "Server 1", "[insert server 1 key here]"),
                new ServerConfig(7778, "Server 2", "[insert server 2 key here]"),
            };
            string config = JsonSerializer.Serialize(this, ConfigSerializerContext.Default.Config);
            Log.Debug($"New config loaded.");
            writer.Write(config);
            writer.Flush();
            writer.Close();
            stream.Close();
        }
        else
        {
            StreamReader reader = new StreamReader(stream);
            string json = reader.ReadToEnd();
            reader.Close();
            stream.Close();
            Config? config = JsonSerializer.Deserialize(json, ConfigSerializerContext.Default.Config);
            if (config == null)
            {
                Log.Error($"Config not valid. Unable to load config.");
                Environment.Exit(128);
                return;
            }

            this.LogPath = config.LogPath;
            try
            {
                var fileStream = File.Open(this.LogPath, FileMode.OpenOrCreate);
                var writer = new StreamWriter(fileStream);
                writer.WriteLine("\n");
                writer.Flush();
                writer.Close();
                fileStream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not write to log path. Exception {e}");
                Environment.Exit(128);
            }

            this.DebugMode = config.DebugMode;
            this.ServerAddress = config.ServerAddress;
            this.ServerInstances = config.ServerInstances;
            this.SendRate = config.SendRate;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Config"/> class via json serialization.
    /// </summary>
    /// <param name="LogPath">The log path for this plugin.</param>
    /// <param name="ServerInstances">The server instances for this plugin.</param>
    /// <param name="ServerAddress">The address for the webapi.</param>
    /// <param name="DebugMode">The setting for enabling or disabling Debug Mode.</param>
    /// <param name="SendRate">The rate at which the plugin will send updates to netdata.</param>
    [JsonConstructor]
#pragma warning disable CS8618,SA1313

    // ReSharper disable InconsistentNaming
    public Config(string LogPath, List<ServerConfig> ServerInstances, string ServerAddress, bool DebugMode, float SendRate)
#pragma warning restore CS8618,SA1313
    {
        this.LogPath = LogPath;
        this.ServerInstances = ServerInstances;
        this.ServerAddress = ServerAddress;
        this.DebugMode = DebugMode;
        this.SendRate = SendRate;
    }

    /// <summary>
    /// Gets a value indicating whether gets or sets the debug mode for output tracing.
    /// </summary>
    [JsonPropertyName("DebugMode")]
    public bool DebugMode { get; private set; }

    /// <summary>
    /// Gets the path for file logging.
    /// </summary>
    [JsonPropertyName("LogPath")]
    public string LogPath { get; private set; }

    /// <summary>
    /// Gets a list of server instances.
    /// </summary>
    [JsonPropertyName("ServerInstances")]
    public List<ServerConfig> ServerInstances { get; private set; }

    /// <summary>
    /// Gets the server address to use for the api.
    /// </summary>
    [JsonPropertyName("ServerAddress")]
    public string ServerAddress { get; private set; }

    /// <summary>
    /// Gets how often to send data to the netdata plugin.
    /// </summary>
    [JsonPropertyName("SendRate")]
    public float SendRate { get; private set; }
}

[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true)]
[JsonSerializable(typeof(Config))]
#pragma warning disable SA1402, SA1601

// ReSharper disable once UnusedType.Global
internal partial class ConfigSerializerContext
        : JsonSerializerContext
    {
    }
#pragma warning restore SA1402, SA1601

