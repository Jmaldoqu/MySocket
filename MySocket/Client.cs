
using System;
using System.Text.Json;
using System.Text;
using System.Net;
using System.Net.Sockets;


public class Client : ICommunicator
{
    IPHostEntry host;
    IPAddress IPAddrs;
    IPEndPoint endpoint;

    Socket sClient;
    JsonMessageSerializer serializer;

    public Client(string ip, int port)
    {
        try {
            host = Dns.GetHostEntry(ip);
            IPAddrs = host.AddressList[0];
            endpoint = new IPEndPoint(IPAddrs, port);
            serializer = new JsonMessageSerializer();

            sClient = new Socket(IPAddrs.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        catch (SocketException e)
        {
            Console.WriteLine("Socket error: " + e.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine("Client error: " + e.ToString());
        }
    }

    public void Start(int maxRetries = 10, int delay = 1000)
    {
        int tries = 0;

        while (tries < maxRetries) {
            try {
                sClient = new Socket(IPAddrs.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sClient.Connect(endpoint);
                break;
            }
            catch (Exception e) {
                Console.WriteLine("Connection error: " + e.Message);
                tries++;

                if (tries < maxRetries) {
                    Thread.Sleep(delay);
                }
            }
        }

        if (!sClient.Connected) {
            Console.WriteLine("Connection error. Failed to reconnect.");
        }
    }

    public void Send(Object data)
    {
        try {
            byte[] buffer = serializer.Serializate(data);
            sClient.Send(buffer);
        }
        catch (SocketException e) {
            Console.WriteLine("Connection error: " + e.ToString());
        }
        catch (Exception e) {
            Console.WriteLine("Failed to send the data: " + e.ToString());
        }
    }

    public Object Receive() {
        try {
            byte[] buffer = new byte[1024];
            int received = sClient.Receive(buffer);
            byte[] data = buffer.Take(received).ToArray();
            Object message = serializer.Deserializate(data);

            return message;
        }
        catch (SocketException e) {
            Console.WriteLine("Connection error: " + e.ToString());
            return null;
        }
        catch (Exception e) {
            Console.WriteLine("Error: " + e.ToString());
            return null;
        }
    }
    public void Close() {
        try {
            sClient.Close();
        }
        catch (Exception e) {
            Console.WriteLine("Sockets closure failed: " + e.Message);
        }
    }
}
