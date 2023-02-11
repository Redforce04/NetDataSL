
# NetDataSL

An SCP-SL Server NetData integration.

Uses the [PWProfiler](https://git.peanutworshipers.net/Redforce04/PWProfiler) plugin to interface with Scp SL servers.


## Authors

- [Redforce04#4091](https://git.peanutworshipers.net/Redforce04)


## **Features:**
#### -  Logs these stats:
- Checks **TPS / delta time** from an **average** of 120 frames (configurable)
    - Most commands only take the TPS from the very last frame, which can miss short lag spikes.
- Checks the **Memory allocation size** 
    - This is a rough memory usage estimate, that is accurate to around 50 - 100 mb.
- Checks the rough **CPU usage** 
    - Currently this is pretty inaccurate but I'm working on it.
- Checks the **player count**
    - Per server player count
- Checks how many times server **falls below a certain tps**
    - Configurable option for what qualifies as "low tps" (default is 30 ticks per second)
    - Logs how many times the server fell below that tps in the past 5 seconds.
- Works inside of  **Docker / Pterodactyl**
    - Should work fine on windows and linux
- Stats log every 5 seconds.
    - Tps is still accurately measured over 120 frames, however it is only logged every 5 seconds


## **Wip:**
- Adding a feature to pull cpu and memory stats from docker api
- Auto-generating a self-signed TLS certificate for https.
- Fix logging.
## FAQ

#### I have a suggestion. How can I suggest it?

Reach out to me over discord or leave an issue and I will tag it with the suggestion tag.

#### How do I leave a bug report?

Create an issue and I will look at it.


## Installation

### Profiler Plugin
#### Install the profiler plugin from here onto the sl server.

Profiler Plugin Location:
```bash
(root directory)/.config/SCP Secret Laboratory/PluginAPI/plugins/global/PWProfiler.dll
```
#### Configure Profiler Plugin
Configuration location:
```bash
(root directory)/.config/SCP Secret Laboratory/PluginAPI/plugins/global/PWProfiler/config.yml
```
NetData Integration Configuration:
```yml
# Make sure to enable the integration
net_data_integration_enabled: true
# The address and port that the NetData integration is hosted on. Ie: localhost:11011
# *Note:* if your server is hosted through docker / pterodactyl, you may have to use the machine's LAN ip instead of the loopback adapter. (192.168.1.2) 
net_data_integration_address: 127.0.0.1:11011
# The name of the server in the panel.
server_name: Test Net
# Used to communicate with the NetData integration and verify that this is a registered server with permission to update stats. 
# You will generate this in the next step.
api_key: '[api key]'
```

#### Generate the Api Key:
1. Run the following command in the **Server Console** after the profiler plugin is installed:
*`RegisterServer`*

This will output the following:
```json
{"Port":9017,"ServerName":"Test Net","Key":"[API-Key]"}
```
2. Copy the API-Key into the Profile config in the `api_key` section. Make sure to save the rest of this config, as you will need it later for configuring the integration.

### NetData Plugin
#### Installing the netdata plugin:
3. Find the netdata installation directory. It will have a folder named *`plugins.d`* (***Do not use  `custom-plugins.d`*** - that is a separate directory.)
Potential paths include:
- `/usr/libexec/netdata/plugins.d/`
- `/opt/netdata/netdata-plugins/plugins.d`

4. Put the `scpsl.plugin` file found on this repo in `Download -> Previous Artifacts -> Build`

5. Give the `scpsl.plugin` file execution perms, and change the owner to root, and group to netdata.

6. Find the netdata config. Potential paths include:
- `/etc/netdata/netdata.conf`
- `/opt/netdata/netdata-configs/netdata.conf`
7. Configure netdata and add a configuration location. *The actual integration configuration is separate from the netdata configuration.*
*You specify where this config lives.*
Add this to the config:
```
[plugin:scpsl]
         update every = 5
         command options = /your/path/to/the/NetDataSL/config.json
```
Netdata should automatically detect and load the plugin within a minute or two. 

The plugin will create the config for you when it is loaded. *Note: If the plugin does not have a config, it will fail to load.*

8. Configure the integration. Go to the config location that you specified. The config will look like this:
```json
{
  "LogPath": "/your/path/to/the/NetDataSL/log.log",
  "ServerInstances": [
    {
      "Port": 7777,
      "ServerName": "Server 1",
      "Key": "[Server 1 Api Key]"
    },
    {
      "Port": 7778,
      "ServerName": "Server 2",
      "Key": "[Server 2 Api Key]"
    }
  ],
  "ServerAddress": "127.0.0.1:11011"
}
```

9. In this config file, paste the server info from 2 into the ServerInstances.

If you need, you can restart netdata with the `systemctl restart netdata` command.

10. Ensure that no errors are being thown in the plugin. You can check the netdata log and it should say something like this: 
```
2023-02-11 07:09:19: netdata INFO  : PD[scpsl] : thread created with task id 3210316
2023-02-11 07:09:19: netdata INFO  : PD[scpsl] : set name of thread 3210316 to PD[scpsl]
```
If there are any errors related to `PD[scpsl]` contact **Redforce04#4091** for support.


### The plugin should now work!
## Configuration
### Profiler Plugin Config
```yml
# Make sure to enable the integration
net_data_integration_enabled: true
# The address and port that the NetData integration is hosted on. Ie: localhost:11011
# *Note:* if your server is hosted through docker / pterodactyl, you may have to use the machine's LAN ip instead of the loopback adapter. (192.168.1.2) 
net_data_integration_address: 127.0.0.1:11011
# The name of the server in the panel.
server_name: Test Net
# Used to communicate with the NetData integration and verify that this is a registered server with permission to update stats. 
# You will generate this in the next step.
api_key: '[api key]'
```

### Netdata Config
```
[plugin:scpsl]
         update every = 5
         command options = /your/path/to/the/NetDataSL/config.json
```

### Netdata Integration Config
```json
{
  "LogPath": "/your/path/to/the/NetDataSL/log.log",
  "ServerInstances": [
    {
      "Port": 7777,
      "ServerName": "Server 1",
      "Key": "[Server 1 Api Key]"
    },
    {
      "Port": 7778,
      "ServerName": "Server 2",
      "Key": "[Server 2 Api Key]"
    }
  ],
  "ServerAddress": "127.0.0.1:11011"
}

# Log path - the path to where the integration is logged.
# Server instances - a list of all the servers that netdata will configure
#   Port - The port of the server.
#   ServerName - The Name of the server to show up in NetDatal.
#   Key - The Api Key of the server to communicate with the plugin.
```
## Other Information

If a server is in idle mode, it won't send data to the integration, and won't show up in netdata. 

If stats don't work, or you need help with setting up the plugin contact **Redforce04#4091** for support.
## Contributing

Contributions are always welcome!

Reach out to Redforce04#4091 on discord for ways to get started.

Please adhere to this project's `code of conduct`.


## License

[MIT](https://choosealicense.com/licenses/mit/)

