using Sword.CommandBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword.Clients
{
    public class Sword<T>:IDisposable
        where T: class
    {
        private CommandBusClient commandBusClient;

        private T _proxy;
        public T Proxy
        {
            get
            {
                return this._proxy;
            }
        }

        public Sword()
        {
            this.commandBusClient = CommandBusFactory.CreateCommandBus();

            this._proxy = CommandBusILEmitAdapter.Create<T>(this.commandBusClient);
        }

        public void Dispose()
        {
            if (this.commandBusClient != null)
            {
                this.commandBusClient.Dispose();
                this.commandBusClient = null;
            }
        }
    }
}
