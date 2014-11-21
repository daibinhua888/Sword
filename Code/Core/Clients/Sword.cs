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
            this._proxy = CommandBusILEmitAdapter.Create<T>();
        }

        public void Dispose()
        {
        }
    }
}
