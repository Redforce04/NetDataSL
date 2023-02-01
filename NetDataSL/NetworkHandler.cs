// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         NetworkHandler.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/01/2023 10:26 AM
//    Created Date:     02/01/2023 10:26 AM
// -----------------------------------------

using System.Net;
using System.Net.Sockets;

namespace NetDataSL;

public class NetworkHandler
{
    public static NetworkHandler? Singleton;

    internal NetworkHandler()
    {
        if (Singleton != null)
            return;
        Singleton = this;
        _initSocket();
        
    }
    private SimpleListener _server;
    private const string Ip = "127.0.0.1";
    private const int Port = 12000;
    private const int MaxChunkSize = 128;
    private void _initSocket()
    {
        try
        {
            _server = new SimpleListener(Ip, Port); //tempIp is the ip address of the server.
            //add handler for new client connections
            _server.new_tcp_client += server_new_tcp_client;
            // Start listening for client requests.
            _server.Listen();
        }
        catch(Exception e)
        {
            Log.Error($"Socket error: {e}");
        }
        finally
        {
            _server.Stop();
        }
    }

    void server_new_tcp_client(System.Net.Sockets.TcpClient tcpclient)
    {
        // Buffer for reading data
        Byte[] bytes = new Byte[MaxChunkSize];

        // Get a stream object for reading and writing
        System.IO.Stream stream = tcpclient.GetStream();

        // now that the connection is established start listening though data
        // sent through the stream..
        int i;
        try
        {
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                var data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                
                // Process the data sent by the client.
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error has occured while reading the socket {e}");
        }
    }
}
    
