using Core.CommandBus;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Server.Pipes
{
    public class PipeProcessor
    {
        public bool Idle { get; set; }

        public PipeProcessor()
        {
            this.Idle = true;
        }

        public CommandResult Process(Command command)
        {
            CommandResult result = new CommandResult();

            result.Result = SerializerUtility.Instance().BinSerialize("Server: " + DateTime.Now.ToString());

            return result;
        }
    }
}
