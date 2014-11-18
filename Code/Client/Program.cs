using Core.Clients;
using Core.CommandBus;
using Core.Communication;
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
            CommandBusClient cmdBus = CommandBusFactory.GetCommandBus();

            cmdBus.Start();

            Console.Title = "Client1 " + cmdBus.LocalEndPoint.ToString();

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


            cmdBus.Stop();

            Console.WriteLine("done");
            Console.ReadKey();

        }
    }
}
