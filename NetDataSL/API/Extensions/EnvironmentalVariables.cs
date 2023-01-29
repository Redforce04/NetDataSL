// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         EnvironmentalVariables.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/28/2023 1:39 PM
//    Created Date:     01/28/2023 1:39 PM
// -----------------------------------------

namespace NetDataSL.API.Extensions;

public class EnvironmentalVariables
{
    /// <summary>
    /// The directory where all Netdata-related user configuration should be stored.
    /// If the plugin requires custom user configuration, this is the place the user
    /// has saved it (normally under /etc/netdata).
    /// </summary>
    public static string? NetDataUserConfigDir => Environment.GetEnvironmentVariable("NETDATA_USER_CONFIG_DIR");
    
    /// <summary>
    /// The directory where all Netdata -related stock configuration should be stored.
    /// If the plugin is shipped with configuration files, this is the place they can
    /// be found (normally under /usr/lib/netdata/conf.d).
    /// </summary>
    public static string? NetDataStockConfigDir => Environment.GetEnvironmentVariable("NETDATA_STOCK_CONFIG_DIR");
    
    /// <summary>
    /// The directory where all Netdata plugins are stored.
    /// </summary>
    public static string? NetDataPluginsDir => Environment.GetEnvironmentVariable("NETDATA_PLUGINS_DIR");
    
    /// <summary>
    /// The list of directories where custom plugins are stored.
    /// </summary>
    public static string? NetDataUserPluginsDirs => Environment.GetEnvironmentVariable("NETDATA_USER_PLUGINS_DIRS");
    
    /// <summary>
    /// The directory where the web files of Netdata are saved.
    /// </summary>
    public static string? NetDataWebDir => Environment.GetEnvironmentVariable("NETDATA_WEB_DIR");
    
    /// <summary>
    /// The directory where the cache files of Netdata are stored.
    /// Use this directory if the plugin requires a place to store data.
    /// A new directory should be created for the plugin for this purpose,
    /// inside this directory.
    /// </summary>
    public static string? NetDataCacheDir => Environment.GetEnvironmentVariable("NETDATA_CACHE_DIR");
    
    /// <summary>
    /// The directory where the log files are stored.
    /// By default the stderr output of the plugin will be saved in
    /// the error.log file of Netdata.
    /// </summary>
    public static string? NetDataLogDir => Environment.GetEnvironmentVariable("NETDATA_LOG_DIR");
    
    /// <summary>
    /// The directory where the log files are stored.
    /// By default the stderr output of the plugin will be
    /// saved in the error.log file of Netdata.
    /// </summary>
    public static string? NetDataHostPrefix => Environment.GetEnvironmentVariable("NETDATA_HOST_PREFIX");
    
    /// <summary>
    /// This is a number (probably in hex starting with 0x),
    /// that enables certain Netdata debugging features.
    /// Check [[Tracing Options]] for more information.
    /// </summary>
    public static string? NetDataDebugFlags => Environment.GetEnvironmentVariable("NETDATA_DEBUG_FLAGS");
    
    /// <summary>
    /// The minimum number of seconds between chart refreshes.
    /// This is like the internal clock of Netdata
    /// (it is user configurable, defaulting to 1).
    /// There is no meaning for a plugin to update
    /// its values more frequently than this number of seconds.
    /// </summary>
    public static string? NetDataUpdateEvery => Environment.GetEnvironmentVariable("NETDATA_UPDATE_EVERY");
}