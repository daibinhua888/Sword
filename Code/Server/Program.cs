using Core;
using Core.Communication;
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

            ConnectionMaster master = new ConnectionMaster(888);
            master.Start();

            Console.WriteLine("Server started.");
            Console.ReadKey();
        }
    }
}
