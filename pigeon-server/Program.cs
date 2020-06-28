using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pigeon_server
{
    class Program
    {
        public static NetworkStream NetworkStream { get; private set; }

        static void Main(string[] args)
        {
            string Welcome = "\r\n  =================================\r\n" +
                             "    WELCOME TO PIGEON FILE SENDER\r\n" +
                             "  =================================\r\n\r\n";
            foreach (char c in Welcome)
            {
                Console.Write(c);
                Thread.Sleep(5);
            }
            Console.ReadKey();
            // start listener

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 55387;
            TcpListener tcpListener = new TcpListener(ip, port);
            tcpListener.Start();

            Boolean isConnected = false;
            TcpClient tcpClient = null;
            NetworkStream ns    = null;
            StreamReader sr     = null;
            StreamWriter sw     = null;

            while (true)
            {
                if (!isConnected)
                {
                    tcpClient = tcpListener.AcceptTcpClient();
                    ns = tcpClient.GetStream();
                    ns.ReadTimeout = 36000;
                    ns.WriteTimeout = 36000;
                    sr = new StreamReader(ns);
                    sw = new StreamWriter(ns);
                    sw.AutoFlush = true;
                    isConnected = true;
                    Console.Clear();
                    Console.Write(" [ Connected ] \r\n");
                    break;
                }
            }

            Console.WriteLine();

        }
    }
}
