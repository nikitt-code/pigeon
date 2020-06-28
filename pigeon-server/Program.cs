using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pigeon_server
{
    class Program
    {
        static void Main(string[] args)
        {
            string Welcome = "\r\n  =================================\r\n" +
                             "    WELCOME TO PIGEON FILE SENDER\r\n" +
                             "  =================================";
            foreach (char c in Welcome)
            {
                Console.Write(c);
                Thread.Sleep(5);
            }
            Console.ReadKey();
            // start listener

            TcpListener TcpListener = new TcpListener(55387);


        }
    }
}
