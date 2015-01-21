using Sword.Clients;
using Sword.Server.PipeSelectors.Descriptors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sword.Server.PipeSelectors
{
    public static class ServiceRegistry
    {
        private static Dictionary<string, ServiceImplementationDescriptor> serviceTypeDescriptors = new Dictionary<string, ServiceImplementationDescriptor>();

        /// <summary>
        /// 找到所有dll，分解class，后缀为Controller的则加入Dictionary
        /// </summary>
        public static void RegisterSwordServices()
        {
            var dlls = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            foreach (var dll in dlls)
                Assembly.LoadFile(dll);

            var lst = AppDomain.CurrentDomain.GetAssemblies().ToList().OrderBy(o => o.FullName).ToList();
            lst = lst.Where(w => !w.FullName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)).ToList();
            lst = lst.Where(w => !w.FullName.StartsWith("System", StringComparison.OrdinalIgnoreCase)).ToList();
            lst = lst.Where(w => !w.FullName.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase)).ToList();
            lst = lst.Where(w => !w.FullName.StartsWith("vshost", StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var item in lst)
            {
                List<Type> types=null;
                try
                {
                    types = item.GetTypes().ToList().Where(w =>
                    {
                        var attrs = w.GetCustomAttributes<SwordServiceAttribute>();
                        if (attrs != null && attrs.Count() > 0)
                            return true;
                        return false;
                    }).ToList();
                }
                catch(Exception ex)
                { 
                }

                if (types == null)
                    continue;

                types.ForEach(serviceType => {

                    ServiceImplementationDescriptor serviceTypeDescriptor = new ServiceImplementationDescriptor()
                    {
                        ServiceType = serviceType,
                        Namespace = serviceType.Namespace
                    };

                    #region MethodInfos
                    MethodInfo[] methods = serviceType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var method in methods)
                    {
                        MethodDescriptor methodDescriptor = new MethodDescriptor()
                        {
                            MethodName = method.Name,
                            MethodInfo = method
                        };

                        #region Parameters
                        ParameterInfo[] parameters = method.GetParameters();
                        foreach (var parameter in parameters)
                        {
                            ParameterDescriptor parameterDescriptor = new ParameterDescriptor()
                            {
                                ParameterType = parameter.ParameterType,
                                SourceLocation = SourceLocation.FromCommand,
                                KeyInSource = parameter.Name
                            };

                            methodDescriptor.ParameterDescriptors.Add(parameterDescriptor);
                        }
                        #endregion

                        serviceTypeDescriptor.MethodDescriptors.Add(methodDescriptor);
                    }
                    #endregion

                    var interfaces=serviceType.GetInterfaces();
                    if (interfaces != null)
                    {
                        foreach (var interf in interfaces)
                        {
                            serviceTypeDescriptors[interf.Name.ToLower()] = serviceTypeDescriptor;
                        }
                    }
                });
            }

        }


        internal static ServiceImplementationDescriptor ResolveServiceDescriptor(string key)
        {
            key = key.ToLower();
            if (!serviceTypeDescriptors.ContainsKey(key))
                return null;

            return serviceTypeDescriptors[key];
        }

        internal static MethodDescriptor ResolveMethodDescriptor(string key, string methodName)
        {
            var serviceDescriptor = ResolveServiceDescriptor(key);

            if (serviceDescriptor == null)
                return null;

            return serviceDescriptor.MethodDescriptors.Where(w => w.MethodName.Equals(methodName.ToLower(), StringComparison.OrdinalIgnoreCase)).First();
        }
    }
}
