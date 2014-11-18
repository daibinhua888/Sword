using Core;
using Core.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceImpls
{
    [SwordService]
    public class TestService
    {
        public string Test1()
        {
            return DateTime.Now.ToString();
        }
    }
}
