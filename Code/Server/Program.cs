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
        private static ConnectionAcceptor serverAcceptor;

        static void Main(string[] args)
        {
            Console.Title = "Server";

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            EndPoint listenAddress = new IPEndPoint(IPAddress.Any, 888);
            serverSocket.Bind(listenAddress);
            serverSocket.Listen(5);

            serverAcceptor = new ConnectionAcceptor(serverSocket);
            serverAcceptor.Start();

            Console.WriteLine("Server started.");
            Console.ReadKey();
        }

        //private static void sae_Completed(object sender, SocketAsyncEventArgs e)
        //{
        //    Console.WriteLine(e.LastOperation.ToString());
        //    switch(e.LastOperation)
        //    {
        //        case SocketAsyncOperation.Accept:
        //            var socket = e.AcceptSocket;
        //            Console.WriteLine(socket.RemoteEndPoint.ToString());
        //            Console.WriteLine(e.UserToken==null?"null":"not null");

        //            byte[] data=new byte[1024*1024];
        //            var len=socket.Receive(data);

        //            MemoryStream ms = new MemoryStream(data);
        //            BufferedStream bs = new BufferedStream(ms);
        //            BinaryReader br = new BinaryReader(bs, UnicodeEncoding.UTF8);


        //            var cmdLength=br.ReadInt32();
        //            var cmdBytes = br.ReadBytes(cmdLength);
        //            var cmd = CLFramework.SerializerUtility.BinDeserialize<CommandWrapper>(cmdBytes);

        //            Console.WriteLine("New command received:");
        //            Console.WriteLine("         "+cmd.Command);
        //            Console.WriteLine("         " + cmd.Tag);
        //            break;
        //    }
        //}
    }
}
