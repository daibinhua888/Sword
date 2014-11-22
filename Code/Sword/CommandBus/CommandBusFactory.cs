using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword.CommandBus
{
    public class CommandBusFactory
    {
        public static CommandBusClient CreateCommandBus()
        {
            var cmdBusClient = new CommandBusClient();
            return cmdBusClient;
        }
    }
}
