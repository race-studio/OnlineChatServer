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
        public Client(TcpClient tcpClient)
        {
            client = tcpClient;
        }
        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
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

                    Console.WriteLine(message);
                    // отправляем обратно сообщение в верхнем регистре
                    message = message.Substring(message.IndexOf(':') + 1).Trim().ToUpper();
                    data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
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
    class ServerProgram
    {
        static TcpListener listener;
        static void Main(string[] args)
        {
            int port = 8005;
            string ip = "127.0.0.1";
            try
            {
                listener = new TcpListener(IPAddress.Parse(ip), port);
                listener.Start();
                Console.WriteLine("Ожидание подключений...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Client clientObject = new Client(client);

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
        /*do
        {
            bytes = handler.Receive(data);
            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
        }
        while (handler.Available > 0);

        Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

        // отправляем ответ
        string message = builder.ToString() + "B";
        data = Encoding.Unicode.GetBytes(message);
        handler.Send(data);
        // закрываем сокет
        handler.Shutdown(SocketShutdown.Both);
        handler.Close();

        Console.WriteLine("Hello World!");*/
    }
}
