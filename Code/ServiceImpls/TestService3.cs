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
    public class TestService3 : ITest3
    {
        public int Test3_1(string input)
        {
            return 100;
        }

        public Guid Test3_2(string input)
        {
            return Guid.NewGuid();
        }

        public void Test3_3(string input)
        {
            Console.WriteLine(DateTime.Now.ToString());
        }
    }
}
