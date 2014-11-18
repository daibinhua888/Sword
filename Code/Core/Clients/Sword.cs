using Core.EmitCommandBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Clients
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
            this._proxy=Commonization.Create<T>();
        }

        public void Dispose()
        {
        }
    }
}
