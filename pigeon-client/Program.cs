using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pigeon_client
{
    class Program
    {
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

            if (args.Length != 3)
            {
                Console.WriteLine("Usage: client.exe file/dir_to_send ip port");
                Console.WriteLine("File -> file to send");
                Console.WriteLine("Dir -> directory to send (NON-RECURISVE!!!!!!!)");
            } else
            {
                string selection = args[0];
                string ip = args[1];
                int port = int.Parse(args[2]);
                
                if (Directory.Exists(selection))
                {
                    SendDirectory(selection);
                } else
                {
                    SendFile(selection);
                }
            }
        }

        private static void SendFile(string selection)
        {
            throw new NotImplementedException();
        }

        private static void SendDirectory(string selection)
        {
            throw new NotImplementedException();
        }
    }
}
