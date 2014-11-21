using Sword.CommandBus;
using Sword.Server.PipeSelectors.Descriptors;
using Sword.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword.Server.PipeSelectors
{
    public class ParameterParser
    {
        public static object GetValue(Command cmd, ParameterDescriptor pd)
        {
            object value = null;

            if (pd.SourceLocation == SourceLocation.FromCommand)
            {
                value = cmd.Arguments[pd.KeyInSource];
            }
            else
            {
                throw new Exception("Unknow SourceLocation");
            }
            return value;
        }
    }
}
