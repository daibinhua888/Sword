using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    [Serializable]
    public class CommandWrapper
    {
        public string SessionID { get; set; }
        public string Command { get; set; }
        public string Tag { get; set; }
    }
}
