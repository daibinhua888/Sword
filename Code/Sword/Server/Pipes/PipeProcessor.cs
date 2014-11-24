using Sword.CommandBus;
using Sword.Server.PipeSelectors;
using Sword.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sword.Server.Pipes
{
    public class PipeProcessor
    {
        private PipeProcessorPool pipeProcessorPool;
        private BlockingCollection<Command> incomeCommand=new BlockingCollection<Command>();

        public PipeProcessor(PipeProcessorPool pipeProcessorPool)
        {
            this.pipeProcessorPool = pipeProcessorPool;

            Task.Factory.StartNew(() =>
            {
                InnerProcessLogic();
            }, TaskCreationOptions.LongRunning);
        }

        public void GiveTask(Command task)
        {
            incomeCommand.Add(task);
        }

        private void InnerProcessLogic()
        {
            while (true)
            {
                var cmd = this.incomeCommand.Take();

                var result=Process(cmd);

                if (result.ConnectionWorker.IsTagged)
                    return;

                ServerRuntime.AddCommandResultToOutgoingQueueRepository(result);

                this.pipeProcessorPool.BackIntoPool(this);
            }
        }

        private CommandResult Process(Command command)
        {
            var serviceDescriptor= ServiceRegistry.ResolveServiceDescriptor(command.CallContract);
            var methodDescriptor = ServiceRegistry.ResolveMethodDescriptor(command.CallContract, command.Method2Invoke);

            List<object> parameters = new List<object>();
            
            foreach (var pd in methodDescriptor.ParameterDescriptors)
                parameters.Add(ParameterParser.GetValue(command, pd));

            object serviceInstance = Activator.CreateInstance(serviceDescriptor.ServiceType);

            bool successful = false;
            Exception exc=null;
            bool noReturnValue = false;
            object resultValue = null;

            if (methodDescriptor.MethodInfo.ReturnType.Equals(typeof(void)))
                noReturnValue = true;

            try
            {
                if (noReturnValue)
                    methodDescriptor.MethodInfo.Invoke(serviceInstance, parameters.ToArray());
                else
                    resultValue = methodDescriptor.MethodInfo.Invoke(serviceInstance, parameters.ToArray());
                successful = true;
            }
            catch(Exception ex)
            {
                exc = ex.InnerException;
                successful = false;
            }

            CommandResult result = new CommandResult();

            result.Sucessful = successful;
            result.ConnectionWorker = command.ConnectionWorker;

            if (successful)
            {
                if (!noReturnValue)
                    result.Result = SerializerUtility.Instance().BinSerialize(resultValue);
            }
            else
            {
                result.Exception = SerializerUtility.Instance().BinSerialize(exc);
            }

            result.ConnectionWorker.LastActiveTime = DateTime.Now; //reset timeout

            return result;
        }
    }
}
