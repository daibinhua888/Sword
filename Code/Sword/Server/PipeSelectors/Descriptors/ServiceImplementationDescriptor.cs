using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sword.Server.PipeSelectors.Descriptors
{
    public class ServiceImplementationDescriptor
    {
        public Type ServiceType { get; set; }
        public string Namespace { get; set; }

        public ServiceImplementationDescriptor()
        {
            this.MethodDescriptors = new List<MethodDescriptor>();
        }

        public List<MethodDescriptor> MethodDescriptors { get; set; }
    }
}
