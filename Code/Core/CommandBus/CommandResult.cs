using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CommandBus
{
    [Serializable]
    public class CommandResult
    {
        public byte[] Result { get; set; }
    }
}
