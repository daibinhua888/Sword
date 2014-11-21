using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword.Utils
{
    public class TryCatchHelper
    {
        public static void Do(Action action)
        {
            if (action == null)
                return;

            try
            {
                action();
            }
            catch
            { 
            }
        }
    }
}
