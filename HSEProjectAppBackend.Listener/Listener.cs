using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using HSEProjectAppBackend.Context;
using HSE_Finance_App_Backend.Core;
using HSEProjectAppBackend.Context.Entities;

namespace HSEProjectAppBackend.Listener;

public class ClientObject : object
{
    public TcpClient client;

    public ClientObject(TcpClient tcpClient)
    {
        client = tcpClient;
    }

    public void Process()
    {
        NetworkStream stream = null;
        try
        {
            stream = client.GetStream();
            var data = new byte[64];
            while (true)
            {
                var builder = new StringBuilder();
                var bytes = 0;
                do
                {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (stream.DataAvailable);

                var message = builder.ToString();

                Execute exe = new Execute();
                string response = exe.ExecuteRequest(message);

                byte[] comp = Encoding.Unicode.GetBytes(response);

                stream.Write(comp, 0, comp.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
        }
    }
}

public class MainListener
{
    private static readonly IConfigurationBuilder Builder =
        new ConfigurationBuilder().AddJsonFile(@"Config\ListenerSettings.json");

    private static TcpListener listener;

    static void Main(string[] args)
    {
        var root = Builder.Build();

        var port = int.Parse(root["Port"]);
        var address = root["Address"];

        try
        {
            listener = new TcpListener(IPAddress.Parse(address), port);
            listener.Start();
            Console.WriteLine("Waiting for client to connect...");

            while (true)
            {
                var client = listener.AcceptTcpClient();

                var clientObject = new ClientObject(client);

                var clientThread = new Thread(clientObject.Process);
                clientThread.Start();
            }
        }
        catch (Exception e)
        {
            Console.Write($"[ERROR]: {e.Message}");
        }
        finally
        {
            listener?.Stop();
        }
    }
}