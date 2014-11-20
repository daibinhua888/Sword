using Core.Clients;
using Core.CommandBus;
using Core.Server;
using Core.Utils;
using ServiceImpls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Client1";

            SwordConfiguration.SetServerInfo("localhost", 888);

            using (var proxy = new Sword<ITest>())
            {
                for (var i = 0; i < 500; i++)
                {
                    var result = proxy.Proxy.Test1("fff");

                    Console.WriteLine(i+"===="+result);
                }
            }

            using (var proxy = new Sword<ITest2>())
            {
                for (var i = 0; i < 500; i++)
                {
                    var result = proxy.Proxy.Test2("fff");

                    Console.WriteLine(i + "====" + result);
                }
            }

            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
