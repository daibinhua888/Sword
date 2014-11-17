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

            for (var i = 0; i < 5000; i++)
            {
                Command cmd = new Command();
                cmd.SessionID = Console.Title;
                cmd.AppID = "testing";
                cmd.CallContract = "ssssssss";

                cmdBus.Send(cmd);
                
                Console.WriteLine("Sent");

                CommandResult cmdResult = cmdBus.WaitForResult();

                Console.WriteLine("[{0}] {1}", cmdBus.RemoteEndPoint.ToString(), Core.Utils.SerializerUtility.BinDeserialize<string>(cmdResult.Result));

                Thread.Sleep(500);
            }

            cmdBus.Stop();

            Console.WriteLine("done");
            Console.ReadKey();


            using (var proxy = new Sword<TestService>())
            {
            }
        }
    }
}
