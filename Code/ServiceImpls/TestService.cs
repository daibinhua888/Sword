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
        public TestResultDTO Test1(string input)
        {
            TestResultDTO dto = new TestResultDTO();

            dto.P1 = string.Format("I'm TestService {0}", DateTime.Now.ToString());
            dto.P2 = "111111111";
            dto.P3 = DateTime.Now;

            throw new Exception("fffffffffff");

            return dto;
        }
    }
}
