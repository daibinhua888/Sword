using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core.Server
{
    internal class ConnectionAcceptor
    {
        private ConnectionMaster master;
        public SocketAsyncEventArgs SocketAsyncEventArgs { get; set; }
        private Socket serverSocket { get; set; }

        public ConnectionAcceptor(ConnectionMaster master, Socket socket)
        {
            this.master = master;
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

                Console.WriteLine("New client[{0}] connected.", e.AcceptSocket.RemoteEndPoint.ToString());

                ProcessAccept(e.AcceptSocket);
            }

            Start();
        }

        private void ProcessAccept(Socket acceptSocket)
        {
            var worker = new ConnectionWorker(acceptSocket);
            worker.LastActiveTime = DateTime.Now;

            this.master.AddWorker(worker);

            worker.StartReceive();
        }
    }
}
