using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword.Clients
{
    public static class SwordConfiguration
    {
        public static void SetServerInfo(string server, int port)
        {
            Server = server;
            Port = port;
        }

        public static string Server { get; set; }

        public static int Port { get; set; }
    }
}
