using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pigeon_client
{
    class Program
    {
        public static void W(string T)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(T);
            Console.ResetColor();
        }
        public static void G(string T)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(T);
            Console.ResetColor();
        }
        public static void R(string T)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(T);
            Console.ResetColor();
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

            if (args.Length != 2)
            {
                Console.WriteLine("[Info] Usage: client.exe ip file/dir_to_send");
                Console.WriteLine("[Info] File -> file to send");
                Console.WriteLine("[Info] Dir -> directory to send (NON-RECURISVE!!!!!!!)");
            } else
            {
                string ip = args[0];
                string selection = args[1];

                if (Directory.Exists(selection))
                {
                    SendDirectory(selection, ip);
                } else
                {
                    FilesToSend.Add(selection);
                    SendFile(ip);
                }
            }
        }

        public static List<string> FilesToSend = new List<string>();
        private static void SendFile(string ip)
        {
            
            W("[Log] Initializing sender...");
            TcpClient tcpClient = null;
            NetworkStream ns = null;
            try
            {
                tcpClient = new TcpClient(ip, 55387);
                ns = tcpClient.GetStream();
                ns.WriteTimeout = 600000;
                ns.ReadTimeout = 1000;
            } catch
            {
                R("[ERROR] Error occured while connecting."); return;
            }

            foreach (string SFile in FilesToSend)
            {
                try
                {
                    byte[] BytesOfFileToSend = File.ReadAllBytes(SFile);
                    string RealFilename = Path.GetFileName(SFile);
                    W("[Log] Sending " + RealFilename);
                    ulong FileSize = (ulong)new FileInfo(SFile).Length;

                    List<byte> Constructor = new List<byte>();
                    Constructor.AddRange(Encoding.UTF8.GetBytes(RealFilename));
                    Constructor.Add(0x00);
                    Constructor.AddRange(BitConverter.GetBytes(FileSize));
                    Constructor.AddRange(BytesOfFileToSend);

                    byte[] ToSend = Constructor.ToArray();
                    Constructor.Clear();
                    ns.Write(ToSend, 0, ToSend.Length);
                    G("[Log] Sent " + RealFilename);
                } catch
                {
                    R("[ERROR] Error occured while sending " + SFile); return;
                }
            }
            tcpClient.Close();
        }


        private static void SendDirectory(string selection, string ip)
        {
            List<string> files = Directory.EnumerateFiles(selection, "*", SearchOption.TopDirectoryOnly).ToList();
            foreach (string file in files)
            {
                FilesToSend.Add(file);
            }
            SendFile(ip);
        }
    }
}
