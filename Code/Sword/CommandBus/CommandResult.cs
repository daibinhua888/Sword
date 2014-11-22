using Sword.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword.CommandBus
{
    [Serializable]
    public class CommandResult
    {
        public byte[] Result { get; set; }

        public bool Sucessful { get; set; }

        public byte[] Exception { get; set; }

        [NonSerialized]
        public ConnectionWorker ConnectionWorker;
    }
}
