using Core.Utils;
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
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("localhost", 888);

            Console.Title = "Client1 " + socket.LocalEndPoint.ToString();

            for (var i = 0; i < 5000; i++)
            {
                MemoryStream ms = new MemoryStream();
                BufferedStream bs = new BufferedStream(ms);
                BinaryWriter bw = new BinaryWriter(bs, UnicodeEncoding.UTF8);

                Core.CommandWrapper cmd = new Core.CommandWrapper();
                cmd.SessionID = Console.Title;
                cmd.Command = "UPDATE";
                cmd.Tag = DateTime.Now.ToString();
                var cmdBytes = SerializerUtility.BinSerialize(cmd);
                bw.Write(cmdBytes.Length);
                bw.Write(cmdBytes);

                bw.Flush();

                socket.Send(ms.ToArray());
                Console.WriteLine("Sent");

                byte[] buffer=new byte[1024*16];
                
                int len = socket.Receive(buffer);
                var response=UTF8Encoding.UTF8.GetString(buffer, 0, len);

                Console.WriteLine("[{0}] {1}", socket.RemoteEndPoint.ToString(), response);

                Thread.Sleep(500);
            }

            socket.Shutdown(SocketShutdown.Send);
            socket.Close();

            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
