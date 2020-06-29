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
        public struct Packet
        {
            public string Filename;
            public int FileSize;
            public byte[] FileContents;
        }
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

            IPAddress ip = IPAddress.Parse("0.0.0.0");
            int port = 55387;
            TcpListener tcpListener = new TcpListener(ip, port);
            tcpListener.Start();
            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            NetworkStream ns = tcpClient.GetStream();
            ns.ReadTimeout = 300000;
            ns.WriteTimeout = 36000;
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true;
            Console.Clear();
            Console.Write(" [ Connected ] \r\n");

            byte[] buffer = ReadContents(ns);
            Packet[] files = null;
            var Result = DispatchPacket(ref files, buffer);
            if (Result == 0)
            {
                Console.Write("  File successfully transferred.\r\n");
            } else
            {
                Console.Write("  Some files were corrupted. Do you still want to save them?\r\n");
                Console.Write("(Y/N): ");
                string Answer = Console.ReadLine();
                switch (Answer)
                {
                    case "Y": SaveFiles(files); break;
                    default: return;
                }
            }

        }

        private static void SaveFiles(Packet[] files)
        {
            
        }

        private static int DispatchPacket(ref Packet[] files, byte[] buffer)
        {
            List<Packet> PacketsDispatched = new List<Packet>();
            int Position = 0;
            while (true)
            {
                var NewPacket = new Packet();
                string FileName = ReadNT_UTF8_String(buffer, Position);
            }
        }

        public static string ReadNT_UTF8_String(byte[] array, int offset, int limit=128)
        {
            if (array.Length <= offset) throw new ArgumentException("Offset is higher than array length");
            string Output = "";
            for (int i = offset; i<array.Length; i++)
            {
                if (limit < 0) break;
                limit--;
                if (array[i] == 0x00) break;
                Output += (char)array[i];
            }
            return Output;
        }

        public static byte[] ReadContents(Stream str)
        {
            using (var memoryStream = new MemoryStream())
            {
                str.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
