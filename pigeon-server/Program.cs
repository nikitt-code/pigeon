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
            public ulong FileSize;
            public byte[] FileContents;
        }

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

            IPAddress ip = IPAddress.Parse("0.0.0.0");
            int port = 55387;
            TcpListener tcpListener = new TcpListener(ip, port);
            tcpListener.Start();
            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            NetworkStream ns = tcpClient.GetStream();
            ns.ReadTimeout = 300000;
            ns.WriteTimeout = 1000;
            Console.Clear();
            Console.Write(" [ Connected ] \r\n");

            byte[] buffer = ReadContents(ns);
            Packet[] files = null;
            var Result = DispatchPacket(ref files, buffer);
            if (Result == 0)
            {
                Console.Write("  Files successfully transferred.\r\n");
                SaveFiles(files);
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

        /// <summary>
        /// Saves all files from Packet array
        /// </summary>
        /// <param name="files">Array of dispatched packets</param>
        private static void SaveFiles(Packet[] files)
        {
            Console.Write(" Files will be saved to /save/. Press ENTER to continue."); Console.ReadLine();
            foreach (Packet p in files)
            {
                File.WriteAllBytes("save/" + p.Filename, p.FileContents);
                Console.Write("Saved " + p.Filename + "!\r\n");
            }
        }

        /// <summary>
        /// Dispatches packet into a readable form
        /// </summary>
        /// <param name="files">Array of Packet structures</param>
        /// <param name="buffer">Array of bytes to dispatch</param>
        /// <returns>Status code, 0 being success and 1 being corruption.</returns>
        private static int DispatchPacket(ref Packet[] files, byte[] buffer)
        {
            List<Packet> PacketsDispatched = new List<Packet>();
            int Position = 0;
            List<byte> BufferList = buffer.ToList();
            while (true)
            {
                var NewPacket = new Packet();
                string FileName;
                try
                {
                    FileName = ReadNT_UTF8_String(buffer, Position);
                } catch
                {
                    return 1;
                }
                Position += FileName.Length + 1; //string + 0x00
                byte[] LengthBytes = BufferList.GetRange(Position, 8).ToArray(); //after filename, takes 8 bytes
                ulong FileLength = BitConverter.ToUInt64(LengthBytes, 0); //... and converts them to ulong
                Position += 8;
                byte[] FileBytes = BufferList.GetRange(Position, (int)FileLength).ToArray(); //TODO: Fix ulong/int bottleneck
                Position += (int)FileLength; //Fix ulong/int bottleneck

                NewPacket.FileSize = FileLength;
                NewPacket.Filename = FileName;
                NewPacket.FileContents = FileBytes;

                PacketsDispatched.Add(NewPacket);

                // (vvv) Checks if FileName is small or contains invalid chars
                if (FileName.Length < 1 || FileName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1 || FileName.Contains("..") || FileName.Contains("/")) { files = PacketsDispatched.ToArray(); return 1; }

                if (Position >= buffer.Length) break;
            }
            files = PacketsDispatched.ToArray();
            return 0;
        }
        
        /// <summary>
        /// Reads null terminated UTF-8 string
        /// </summary>
        /// <param name="array">Array to read from</param>
        /// <param name="offset">Offset to start reading from</param>
        /// <param name="limit">Limit in bytes</param>
        /// <returns>String</returns>
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

        /// <summary>
        /// Reads all contents of NetworkStream
        /// </summary>
        /// <param name="str">Stream to read from</param>
        /// <returns>Byte array of stream contents</returns>
        public static byte[] ReadContents(NetworkStream str)
        {
            using (var memoryStream = new MemoryStream())
            {
                str.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
