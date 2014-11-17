using Core.Communication;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.CommandBus
{
    public class CommandBusClient
    {
        private ClientConnectionManager mgr;

        public void Start()
        {
            if (mgr == null)
                mgr = new ClientConnectionManager();

            mgr.Connect();
        }

        public void Stop()
        {
            if (mgr != null)
                mgr.Close();

            mgr = null;
        }

        public void Send(Command cmd)
        {
            //validate first, ignored

            //send command
            mgr.Send(cmd);
        }

        public CommandResult WaitForResult()
        {
            return mgr.Receive();
        }

        public EndPoint LocalEndPoint
        {
            get
            {
                return mgr.LocalEndPoint;
            }
        }

        public EndPoint RemoteEndPoint
        {
            get
            {
                return mgr.RemoteEndPoint;
            }
        }

        
    }
}
