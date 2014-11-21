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
    public class TestService : ITest
    {
        public string Test1(string input)
        {
            return DateTime.Now.ToString();
        }
    }
}
