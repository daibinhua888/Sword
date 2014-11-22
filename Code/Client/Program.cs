using Sword.Clients;
using Sword.CommandBus;
using Sword.Server;
using Sword.Utils;
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

            //using (var proxy = new Sword<ITest>())
            //{
            //    for (var i = 0; i < 500; i++)
            //    {
            //        try
            //        {
            //            var result = proxy.Proxy.Test1("fff");

            //            Console.WriteLine(i + "====" + result.P1);
            //        }
            //        catch(Exception ex)
            //        {
            //            Console.WriteLine(ex.Message);
            //        }
            //    }
            //}

            //using (var proxy = new Sword<ITest2>())
            //{
            //    for (var i = 0; i < 500; i++)
            //    {
            //        var result = proxy.Proxy.Test2("fff");

            //        Console.WriteLine(i + "====" + result);
            //    }
            //}

            using (var proxy = new Sword<ITest3>())
            {
                //try
                //{
                    var result1 = proxy.Proxy.Test3_1("fff");

                    Console.WriteLine(result1);
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}

                //try
                //{
                    var result2 = proxy.Proxy.Test3_2("fff");

                    Console.WriteLine(result2);
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}

                //try
                //{
                    proxy.Proxy.Test3_3("fff");

                    Console.WriteLine("executed");
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}
            }
            

            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
