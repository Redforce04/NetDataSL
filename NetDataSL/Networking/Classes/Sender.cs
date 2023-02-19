// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         Request.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/05/2023 2:22 PM
//    Created Date:     02/05/2023 2:22 PM
// -----------------------------------------

namespace NetDataSL.Networking.Classes;

/// <summary>
/// Represents a sender sending information.
/// </summary>
public class Sender
{
    /// <summary>
    /// Gets or sets the list of senders.
    /// </summary>
    private static List<Sender> _senders = new List<Sender>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Sender"/> class.
    /// </summary>
    /// <param name="ip">The ip of the sender.</param>
    /// <param name="port">The port of the sender.</param>
    private Sender(string ip, int? port)
    {
        this.Ip = ip;
        this.Port = port;
        this.Requests = new Dictionary<TimeSpan, bool>();
        this.IsBlackListed = false;
        this.BlacklistExpiration = DateTime.Now;
    }

    /// <summary>
    /// Gets or sets the ip of the sender.
    /// </summary>
    internal string Ip { get; set; }

    /// <summary>
    /// Gets or sets the port of the sender.
    /// </summary>
    internal int? Port { get; set; }

    /// <summary>
    /// Gets or sets the time that the blacklist will expire.
    /// </summary>
    internal DateTime BlacklistExpiration { get; set; }

    /// <summary>
    /// Gets or sets a timed list of the requests from the past 5 seconds.
    /// The timespan is the time of the request. The bool is whether the api key was valid or invalid.
    /// </summary>
    internal Dictionary<TimeSpan, bool> Requests { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets whether the sender is blacklisted.
    /// </summary>
    private bool IsBlackListed { get; set; }

    /// <summary>
    /// Gets or creates an instance of the sender.
    /// </summary>
    /// <param name="ip">The ip of the sender.</param>
    /// <param name="port">The port of the sender.</param>
    /// <returns>An instance of the sender.</returns>
    public static Sender Get(string ip, int? port)
    {
        Log.Debug($"Getting ip {ip}, port {port}");
        if (_senders.Any(x => x.Ip == ip || (port != null && x.Port == port)))
        {
            var sender = _senders.FirstOrDefault(x => x.Ip == ip || (port != null && x.Port == port));
            if (sender != null)
            {
                return sender;
            }
        }

        var newSender = new Sender(ip, port);
        _senders.Add(newSender);
        return newSender;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.Ip}{(this.Port == null ? " (no port defined)" : $":{this.Port}")} {(this.IsBlackListed ? $"[Blacklisted for {this.BlacklistExpiration.Subtract(DateTime.UtcNow).TotalSeconds} more seconds]" : string.Empty)}";
    }

    /// <summary>
    /// Gets a value indicating whether gets whether the blacklist has expired.
    /// </summary>
    /// <returns>True if the sender is blacklisted. False if the sender can send messages.</returns>
    internal bool IsBlacklisted()
    {
        return false;
#pragma warning disable CS0162

        // ReSharper disable once HeuristicUnreachableCode
        if (this.IsBlackListed == false)
        {
            return false;
        }

        if (this.BlacklistExpiration < DateTime.Now)
        {
            this.IsBlackListed = false;
            return false;
        }

        return true;
#pragma warning restore CS0162
    }

    /// <summary>
    /// Processes a request.
    /// </summary>
    /// <param name="apiKeyIsValid">Is the api key valid.</param>
    internal void ProcessRequest(bool apiKeyIsValid)
    {
        return;
#pragma warning disable CS0162

        // ReSharper disable once HeuristicUnreachableCode
        foreach (var request in this.Requests)
        {
            if (request.Key.TotalSeconds <
                DateTime.UtcNow.TimeOfDay.Add(new TimeSpan(0, 0, NetworkHandler.RateLimitSampleTime)).TotalSeconds)
            {
                this.Requests.Remove(request.Key);
            }

            this.Requests.Add(DateTime.UtcNow.TimeOfDay, apiKeyIsValid);
            if (this.Requests.Count >= NetworkHandler.RateLimitSampleAmount ||
                this.Requests.Count(x => x.Value) >= NetworkHandler.MaxInvalidApiKeyRequests)
            {
                this.IsBlackListed = true;
                this.BlacklistExpiration = DateTime.UtcNow.AddSeconds(NetworkHandler.TimeoutDuration);
            }
        }
#pragma warning restore CS0162
    }
}