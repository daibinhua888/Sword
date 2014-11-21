using Sword;
using Sword.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceImpls
{
    [SwordService]
    public class TestService2 : ITest2
    {
        public string Test2(string input)
        {
            return string.Format("I'm TestService2 {0}", DateTime.Now.ToString());
        }
    }
}
