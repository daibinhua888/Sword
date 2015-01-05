using Sword.CommandBus;
using Sword.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sword.CommandBus
{
    public class CommandBusILEmitAdapter
    {
        private static Dictionary<Type, Type> createdProxies = new Dictionary<Type, Type>();

        public static T Create<T>(CommandBusClient commandBusClient)
        {
            string id = string.Format("Commonization{0}", typeof(T).FullName.Replace(".", ""));
            string proxyTypeString = "Commonization" + id;
            string proxyDllName = id + ".dll";

            Type proxyType = null;

            if (!createdProxies.ContainsKey(typeof(T)))
            {
                AssemblyBuilder asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(id), AssemblyBuilderAccess.RunAndSave);
                var mdlBldr = asmBuilder.DefineDynamicModule(id, proxyDllName);

                var typeBldr = mdlBldr.DefineType(proxyTypeString, TypeAttributes.Public, null, new Type[] { typeof(T) });

                //嵌入ESB私有变量
                var esbSrvField = typeBldr.DefineField("esbSrv", typeof(CommandBusClient), FieldAttributes.Private);
                var serializeField = typeBldr.DefineField("serializeSrv", typeof(SerializerUtility), FieldAttributes.Private);

                //构造函数
                var constructor = typeBldr.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(CommandBusClient), typeof(SerializerUtility) });
                var il = constructor.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Stfld, serializeField);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, esbSrvField);
                il.Emit(OpCodes.Ret);

                //改写方法
                var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance);
                for (var i = 0; i < methods.Length; i++)
                {
                    var paramTypes = GetParametersType(methods[i]);

                    Type[] parameterTypes = null;
                    if (paramTypes != null && paramTypes.Length > 0)
                        parameterTypes = paramTypes.Select(f => f.ParameterType).ToArray();
                    else
                        parameterTypes = new Type[] { };

                    var methodBlfr = typeBldr.DefineMethod(methods[i].Name, MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.Standard, methods[i].ReturnType, parameterTypes);

                    il = methodBlfr.GetILGenerator();

                    //声明各个局部变量
                    var requestLocal = il.DeclareLocal(typeof(Command));
                    var argsLocal = il.DeclareLocal(typeof(Dictionary<string, object>));
                    var responseLocal = il.DeclareLocal(typeof(CommandResult));
                    var value2Object = il.DeclareLocal(typeof(object));

                    #region Command requestLocal=new Command();
                    il.Emit(OpCodes.Newobj, typeof(Command).GetConstructor(new Type[0]));
                    il.Emit(OpCodes.Stloc, requestLocal);
                    #endregion

                    #region  Dictionary<string, object> argsLocal=new Dictionary<string, object>();
                    il.Emit(OpCodes.Newobj, typeof(Dictionary<string, object>).GetConstructor(new Type[0]));
                    il.Emit(OpCodes.Stloc, argsLocal);
                    #endregion

                    #region 将传入方法的参数全部Add到List object 中
                    MethodInfo mi = typeof(Dictionary<string, object>).GetMethod("Add");
                    if (paramTypes != null)
                    {
                        for (var index = 0; index < paramTypes.Length; index++)
                        {
                            if (paramTypes[index].ParameterType.IsValueType)
                            {
                                //box
                                il.Emit(OpCodes.Ldarg, 1 + index);
                                il.Emit(OpCodes.Box, paramTypes[index].ParameterType);
                                il.Emit(OpCodes.Stloc, value2Object);


                                il.Emit(OpCodes.Ldloc, argsLocal);
                                il.Emit(OpCodes.Ldstr, paramTypes[index].ParameterName);    //Dictionary->key
                                il.Emit(OpCodes.Ldloc, value2Object);                       //Dictionary->value
                                il.Emit(OpCodes.Callvirt, mi);
                            }
                            else
                            {
                                il.Emit(OpCodes.Ldloc, argsLocal);
                                il.Emit(OpCodes.Ldstr, paramTypes[index].ParameterName);    //Dictionary->key
                                il.Emit(OpCodes.Ldarg, 1 + index);                          //Dictionary->value
                                il.Emit(OpCodes.Callvirt, mi);
                            }
                        }
                    }
                    #endregion

                    #region requestLocal.Arguments=argsLocal
                    mi = typeof(Command).GetMethod("set_Arguments");
                    il.Emit(OpCodes.Ldloc, requestLocal);
                    il.Emit(OpCodes.Ldloc, 1);
                    il.Emit(OpCodes.Callvirt, mi);
                    #endregion

                    #region requestLocal.Method2Invoke=methods[i].Name
                    mi = typeof(Command).GetMethod("set_Method2Invoke");
                    il.Emit(OpCodes.Ldloc, requestLocal);
                    il.Emit(OpCodes.Ldstr, methods[i].Name);
                    il.Emit(OpCodes.Callvirt, mi);
                    #endregion

                    #region requestLocal.CallContractNamespace=typeof(T).Namespace
                    mi = typeof(Command).GetMethod("set_CallContractNamespace");
                    il.Emit(OpCodes.Ldloc, requestLocal);
                    il.Emit(OpCodes.Ldstr, typeof(T).Namespace);
                    il.Emit(OpCodes.Callvirt, mi);
                    #endregion

                    #region requestLocal.CallContract=typeof(T).Name
                    mi = typeof(Command).GetMethod("set_CallContract");
                    il.Emit(OpCodes.Ldloc, requestLocal);
                    il.Emit(OpCodes.Ldstr, typeof(T).Name);
                    il.Emit(OpCodes.Callvirt, mi);
                    #endregion

                    #region esbSrvField.Send(requestLocal)
                    mi = typeof(CommandBusClient).GetMethod("Send");
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, esbSrvField);
                    il.Emit(OpCodes.Ldloc, requestLocal);
                    il.Emit(OpCodes.Call, mi);
                    #endregion

                    #region esbSrvField.WaitForResult()
                    mi = typeof(CommandBusClient).GetMethod("WaitForResult");
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, esbSrvField);
                    il.Emit(OpCodes.Callvirt, mi);
                    il.Emit(OpCodes.Stloc, responseLocal);
                    #endregion


                    if (methods[i].ReturnType != typeof(void))
                    {
                        var response_Result_Bytes = il.DeclareLocal(typeof(byte[]));
                        var responseResultCasted = il.DeclareLocal(methods[i].ReturnType);

                        #region responseResult=responseLocal.Result/Successful/Exception;
                        mi = typeof(CommandResult).GetMethod("get_Result");
                        il.Emit(OpCodes.Ldloc, responseLocal);
                        il.Emit(OpCodes.Callvirt, mi);
                        il.Emit(OpCodes.Stloc, response_Result_Bytes);
                        #endregion

                        #region SerializerUtility.BinDeserialize<T>(cmdResult.Result)
                        mi = typeof(SerializerUtility).GetMethod("BinDeserialize");
                        mi = mi.MakeGenericMethod(methods[i].ReturnType);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldfld, serializeField);
                        il.Emit(OpCodes.Ldloc, response_Result_Bytes);
                        il.Emit(OpCodes.Callvirt, mi);
                        il.Emit(OpCodes.Stloc, responseResultCasted);
                        il.Emit(OpCodes.Ldloc, responseResultCasted);
                        #endregion
                    }

                    il.Emit(OpCodes.Ret);
                }

                proxyType=typeBldr.CreateType();

                //asmBuilder.Save(proxyDllName);

                createdProxies[typeof(T)] = proxyType;
            }

            proxyType = createdProxies[typeof(T)];

            return (T)Activator.CreateInstance(proxyType, new object[] { commandBusClient, SerializerUtility.Instance() });
        }

        private static WrappedMethodInfo[] GetParametersType(MethodInfo method)
        {
            WrappedMethodInfo[] paramTypes = null;

            if (method != null)
            {
                var parameters = method.GetParameters();
                if (parameters.Length > 0)
                {

                    paramTypes = new WrappedMethodInfo[parameters.Length];

                    for (var i = 0; i < parameters.Length; i++)
                    {
                        paramTypes[i] = new WrappedMethodInfo();

                        paramTypes[i].ParameterType = parameters[i].ParameterType;
                        paramTypes[i].ParameterName = parameters[i].Name;
                    }
                }
            }
            return paramTypes;
        }
    }
}