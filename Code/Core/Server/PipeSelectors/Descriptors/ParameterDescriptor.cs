using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sword.Server.PipeSelectors.Descriptors
{
    public class ParameterDescriptor
    {
        public SourceLocation SourceLocation { get; set; }
        public string KeyInSource { get; set; }
        public Type ParameterType { get; set; }
    }
}
