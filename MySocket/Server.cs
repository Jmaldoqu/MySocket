using System;
using System.Text.Json;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class Server : ICommunicator {

    IPHostEntry host;
    IPAddress IPAddrs;
    IPEndPoint endpoint;

    Socket sServer;
    Socket sClient;

    JsonMessageSerializer serializer;

    public Server(string ip, int port)
    {
        try
        {
            host = Dns.GetHostEntry(ip);
            IPAddrs = host.AddressList[0];
            endpoint = new IPEndPoint(IPAddrs, port);
            serializer = new JsonMessageSerializer();

            sServer = new Socket(IPAddrs.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sServer.Bind(endpoint);
            sServer.Listen(10);
        }
        catch (SocketException e)
        {
            Console.WriteLine("Server error: " + e.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.ToString());
        }
    }

    public void Start() {
        try {
            sClient = sServer.Accept();
        }
        catch (SocketException e) {
            Console.WriteLine("Connection error: " + e.ToString());
        }
    }

    public void Send(Object data)
    {
        try
        {
            byte[] buffer = serializer.Serializate(data);
            sClient.Send(buffer);
        }
        catch (SocketException e)
        {
            Console.WriteLine("Connection error: " + e.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to send the data: " + e.ToString());
        }
    }
    public Object Receive()
    {
        try
        {
            byte[] buffer = new byte[1024];

            int received = sClient.Receive(buffer);
            byte[] data = buffer.Take(received).ToArray();

            Object message = serializer.Deserializate(data);

            return message;
        }
        catch (SocketException e)
        {
            Console.WriteLine("Connection error: " + e.ToString());
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.ToString());
            return null;
        }

    }

    public void Close() {
        try {
            sServer.Close();
            sClient.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Sockets closure failed: " + e.Message);
        }
    }
}



