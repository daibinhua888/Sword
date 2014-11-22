using Sword.Server;
using Sword.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sword.CommandBus
{
    public class CommandBusClient:IDisposable
    {
        private ClientConnectionManager mgr;
        private string SessionID { get; set; }
        private bool started = false;

        public CommandBusClient()
        {
            Start();
        }

        private void Start()
        {
            if (started)
                return;

            this.SessionID = Guid.NewGuid().ToString();

            if (mgr == null)
                mgr = new ClientConnectionManager();

            mgr.Connect();
        }

        private void Stop()
        {
            if (mgr != null)
                mgr.Close();

            mgr = null;
        }

        public void Send(Command cmd)
        {
            //validate first, ignored

            //send command
            cmd.SessionID = this.SessionID;
            mgr.Send(cmd);
        }

        public CommandResult WaitForResult()
        {
            var commandResult=mgr.Receive();

            if (!commandResult.Sucessful)
            {
                Exception exception = SerializerUtility.Instance().BinDeserialize<Exception>(commandResult.Exception);
                throw exception;
            }

            return commandResult;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
