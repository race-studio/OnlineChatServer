using System;

using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;

using System.Threading;

namespace OnlineChatServer
{

    public class Client
    {
        public TcpClient client;
        public Server server;
        public Client(Server servers,TcpClient tcpClient)
        {
            server = servers;
            client = tcpClient;
        }


        public void Process()
        {
           
            NetworkStream stream =  client.GetStream();

            server.streamList.Add(stream);
            //server.clientArray[0] = stream;
            try
            {

                
               
                byte[] data = new byte[64]; // буфер для получаемых данных
                while (true)
                {
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                       
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    
                    }

                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    string messageExit = message.Substring(message.IndexOf(':') + 1);
                    if (messageExit.Equals("exit") == true)
                    {
                        stream.Close();
                        client.Close();
                    }
                    if (message.Equals("") == false) {
                 
                        Console.WriteLine(message);
                        
                    }
                    else {
                        continue;
                    }

                   
                    data = Encoding.Unicode.GetBytes(message);

                    foreach (NetworkStream i in server.streamList) { 
                    i.Write(data, 0, data.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
                if (client != null)
                {
                    client.Close();
                }
            }
         }
        
    }

    public class Server
    {
     public  List<NetworkStream> streamList = new List<NetworkStream>();
     

    }
    class ServerProgram
    {
       
        static TcpListener listener;
        static void Main(string[] args)
        {
            int port = 8005;
            string ip = "127.0.0.1";
            try
            {
                Server server = new Server();
                listener = new TcpListener(IPAddress.Parse(ip), port);
                listener.Start();
                Console.WriteLine("Ожидание подключений...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    
                    Client clientObject = new Client(server ,client);

                    // создаем новый поток для обслуживания нового клиента
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
            
        }
    }
}
