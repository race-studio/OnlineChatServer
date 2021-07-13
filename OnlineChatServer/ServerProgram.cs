﻿using System;

using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace OnlineChatServer
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            int port = 8005;
            string ip = "127.0.0.1";
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse( ip ), port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                Socket handler = listenSocket.Accept();
                // получаем сообщение
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байтов
                byte[] data = new byte[256]; // буфер для получаемых данных

                do
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

                Console.WriteLine("Hello World!");
            }
        }
    }
}
