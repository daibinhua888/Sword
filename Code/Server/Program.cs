using Sword;
using Sword.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Server";

            //ServerRuntime.Setup(10);
            ServerRuntime.Start(888);

            Console.WriteLine("Server started.");
            Console.ReadKey();
        }
    }
}
