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
        public void Dispose()
        {
        }
    }
}
