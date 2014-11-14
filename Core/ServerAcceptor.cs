using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ServerAcceptor
    {
        public SocketAsyncEventArgs SocketAsyncEventArgs { get; set; }
        private Socket serverSocket { get; set; }

        public ServerAcceptor(Socket socket)
        {
            this.serverSocket = socket;

            this.SocketAsyncEventArgs = new SocketAsyncEventArgs();
            this.SocketAsyncEventArgs.Completed += SocketAsyncEventArgs_Completed;
        }

        public void Start()
        {
            this.SocketAsyncEventArgs.AcceptSocket = null;

            var delayed=this.serverSocket.AcceptAsync(this.SocketAsyncEventArgs);

            if (!delayed)
            {
                ProcessAccept(this.SocketAsyncEventArgs.AcceptSocket);
            }
        }

        private void SocketAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            Console.WriteLine("ServerAcceptor==>" + e.LastOperation.ToString());

            if (e.LastOperation == SocketAsyncOperation.Accept)
            {
                if (e.SocketError != SocketError.Success)
                {
                    Console.WriteLine("Error!");
                    return;
                }

                ProcessAccept(e.AcceptSocket);
            }

            Start();
        }

        private void ProcessAccept(Socket acceptSocket)
        {
            var cObj = new ConnectionObject(acceptSocket);
            cObj.LastActiveTime = DateTime.Now;

            ConnectionManager.ConnectionObjects.Add(cObj);

            Console.WriteLine("New client[{0}] connected.", acceptSocket.RemoteEndPoint.ToString());

            cObj.StartReceive();
        }
    }
}
