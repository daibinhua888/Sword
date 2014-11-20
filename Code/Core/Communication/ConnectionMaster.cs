using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core.Communication
{
    public class ConnectionMaster
    {
        private int listenPort;
        private int listenBackLog=5;
        private List<ConnectionWorker> connectionObjects = new List<ConnectionWorker>();
        private object lock_connectionObjects = new object();

        private ConnectionAcceptor serverAcceptor;

        public ConnectionMaster(int listenPort)
        {
            this.listenPort = listenPort;
        }

        public void Start()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            EndPoint listenAddress = new IPEndPoint(IPAddress.Any, this.listenPort);
            serverSocket.Bind(listenAddress);
            serverSocket.Listen(this.listenBackLog);

            serverAcceptor = new ConnectionAcceptor(this, serverSocket);
            serverAcceptor.Start();
        }

        public void AddWorker(ConnectionWorker worker)
        {
            lock (lock_connectionObjects)
            {
                if (this.connectionObjects.Contains(worker))
                    return;

                this.connectionObjects.Add(worker);
            }
        }
    }
}
