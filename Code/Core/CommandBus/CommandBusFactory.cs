using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CommandBus
{
    public class CommandBusFactory
    {
        private static CommandBusClient cmdBusClient;
        private static object lk_cmdBusClient = new object();

        public static CommandBusClient GetCommandBus()
        {
            lock (lk_cmdBusClient)
            {
                if (cmdBusClient != null)
                    return cmdBusClient;

                cmdBusClient = new CommandBusClient();
                cmdBusClient.Start();

                return cmdBusClient;
            }
        }
    }
}
