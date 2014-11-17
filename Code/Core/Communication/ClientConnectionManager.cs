using Core.CommandBus;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core.Communication
{
    internal class ClientConnectionManager:IDisposable
    {
        public Socket clientSocket;
        private CommandParser<CommandResult> parser;
        private byte[] buffer = new byte[16 * 1024];

        public ClientConnectionManager()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            parser = new CommandParser<CommandResult>();
        }

        public void Connect()
        {
            clientSocket.Connect("localhost", 888);
        }

        public void Send(Command cmd)
        {
            MemoryStream ms = new MemoryStream();
            BufferedStream bs = new BufferedStream(ms);
            BinaryWriter bw = new BinaryWriter(bs, UnicodeEncoding.UTF8);


            var cmdBytes = SerializerUtility.BinSerialize(cmd);
            bw.Write(cmdBytes.Length);
            bw.Write(cmdBytes);

            bw.Flush();

            clientSocket.Send(ms.ToArray());
        }

        public CommandResult Receive()
        {
            while (true)
            {
                int length = this.clientSocket.Receive(buffer);
                parser.ProcessReceive(buffer, length);
                var results = parser.GetDTOs();
                if (results != null && results.Count > 0)
                    return results.First();
            }
        }

        public void Close()
        {
            if (clientSocket != null)
            {
                try
                {
                    clientSocket.Close();
                }
                catch
                { 
                }
            }

            clientSocket = null;
        }

        public void Dispose()
        {
            this.Close();
        }

        public EndPoint LocalEndPoint
        {
            get
            {
                return clientSocket.LocalEndPoint;
            }
        }

        public EndPoint RemoteEndPoint
        {
            get
            {
                return clientSocket.RemoteEndPoint;
            }
        }
    }
}
