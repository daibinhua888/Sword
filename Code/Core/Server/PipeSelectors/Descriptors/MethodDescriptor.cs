using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sword.Server.PipeSelectors.Descriptors
{
    public class MethodDescriptor
    {
        public MethodDescriptor()
        {
            this.ParameterDescriptors = new List<ParameterDescriptor>();
        }

        public string MethodName { get; set; }

        public MethodInfo MethodInfo { get; set; }

        public List<ParameterDescriptor> ParameterDescriptors { get; set; }
    }
}
