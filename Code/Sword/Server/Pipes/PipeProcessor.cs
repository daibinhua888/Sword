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
            });
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
            //管道开始
            var serviceDescriptor= ServiceRegistry.ResolveServiceDescriptor(command.CallContract);
            var methodDescriptor = ServiceRegistry.ResolveMethodDescriptor(command.CallContract, command.Method2Invoke);

            List<object> parameters = new List<object>();
            
            foreach (var pd in methodDescriptor.ParameterDescriptors)
                parameters.Add(ParameterParser.GetValue(command, pd));

            object serviceInstance = Activator.CreateInstance(serviceDescriptor.ServiceType);

            object resultValue = methodDescriptor.MethodInfo.Invoke(serviceInstance, parameters.ToArray());
            //管道结束

            //装配返回对象
            CommandResult result = new CommandResult();

            result.ConnectionWorker = command.ConnectionWorker;

            result.Result = SerializerUtility.Instance().BinSerialize(resultValue);

            return result;
        }
    }
}
