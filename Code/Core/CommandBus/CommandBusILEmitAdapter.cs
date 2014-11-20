using Core.CommandBus;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Core.CommandBus
{
    public class CommandBusILEmitAdapter
    {
        private static Dictionary<Type, Type> createdProxies = new Dictionary<Type, Type>();

        public static T Create<T>()
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
                    var methodBlfr = typeBldr.DefineMethod(methods[i].Name, MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.Standard, methods[i].ReturnType, paramTypes);

                    il = methodBlfr.GetILGenerator();

                    //声明各个局部变量
                    var requestLocal = il.DeclareLocal(typeof(Command));
                    var argsLocal = il.DeclareLocal(typeof(List<object>));
                    var responseLocal = il.DeclareLocal(typeof(CommandResult));
                    var responseResult = il.DeclareLocal(typeof(byte[]));
                    var responseResultCasted = il.DeclareLocal(methods[i].ReturnType);
                    var value2Object = il.DeclareLocal(typeof(object));

                    #region Command requestLocal=new Command();
                    il.Emit(OpCodes.Newobj, typeof(Command).GetConstructor(new Type[0]));
                    il.Emit(OpCodes.Stloc, requestLocal);
                    #endregion

                    #region  List<object> argsLocal=new List<object>();
                    il.Emit(OpCodes.Newobj, typeof(List<object>).GetConstructor(new Type[0]));
                    il.Emit(OpCodes.Stloc, argsLocal);
                    #endregion

                    #region 将传入方法的参数全部Add到List object 中
                    MethodInfo mi = typeof(List<object>).GetMethod("Add");
                    if (paramTypes != null)
                    {
                        for (var index = 0; index < paramTypes.Length; index++)
                        {
                            if (paramTypes[index].IsValueType)
                            {
                                //box
                                il.Emit(OpCodes.Ldarg, 1 + index);
                                il.Emit(OpCodes.Box, paramTypes[index]);
                                il.Emit(OpCodes.Stloc, value2Object);


                                il.Emit(OpCodes.Ldloc, argsLocal);
                                il.Emit(OpCodes.Ldloc, value2Object);
                                il.Emit(OpCodes.Callvirt, mi);
                            }
                            else
                            {
                                il.Emit(OpCodes.Ldloc, argsLocal);
                                il.Emit(OpCodes.Ldarg, 1 + index);
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

                    #region responseResult=responseLocal.Result;
                    mi = typeof(CommandResult).GetMethod("get_Result");
                    il.Emit(OpCodes.Ldloc, responseLocal);
                    il.Emit(OpCodes.Callvirt, mi);
                    il.Emit(OpCodes.Stloc, responseResult);
                    #endregion

                    #region SerializerUtility.BinDeserialize<T>(cmdResult.Result)
                    mi = typeof(SerializerUtility).GetMethod("BinDeserialize");
                    mi = mi.MakeGenericMethod(methods[i].ReturnType);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, serializeField);
                    il.Emit(OpCodes.Ldloc, responseResult);
                    il.Emit(OpCodes.Callvirt, mi);
                    il.Emit(OpCodes.Stloc, responseResultCasted);
                    il.Emit(OpCodes.Ldloc, responseResultCasted);
                    #endregion

                    if (methods[i].ReturnType == typeof(void))
                        il.Emit(OpCodes.Pop);
                    il.Emit(OpCodes.Ret);
                }

                proxyType=typeBldr.CreateType();

                //asmBuilder.Save(proxyDllName);

                createdProxies[typeof(T)] = proxyType;
            }

            proxyType = createdProxies[typeof(T)];

            return (T)Activator.CreateInstance(proxyType, new object[] { CommandBusFactory.GetCommandBus(), SerializerUtility.Instance() });
        }

        private static Type[] GetParametersType(MethodInfo method)
        {
            Type[] paramTypes = null;

            if (method != null)
            {
                var parameters = method.GetParameters();
                if (parameters.Length > 0)
                {

                    paramTypes = new Type[parameters.Length];

                    for (var i = 0; i < parameters.Length; i++)
                    {
                        paramTypes[i] = parameters[i].ParameterType;
                    }
                }
            }
            return paramTypes;
        }
    }
}