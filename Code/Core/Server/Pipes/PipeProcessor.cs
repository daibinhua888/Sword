using Core.CommandBus;
using Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Server.Pipes
{
    public class PipeProcessor
    {
        private PipeProcessorPool pipeProcessorPool;
        private BlockingCollection<Command> incomeCommand=new BlockingCollection<Command>();
        private BlockingCollection<CommandResult> outgoingCommand=new BlockingCollection<CommandResult>();

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

        public CommandResult WaitForResult()
        {
            var result=this.outgoingCommand.Take();

            this.pipeProcessorPool.BackIntoPool(this);

            return result;
        }

        private void SetResult(CommandResult result)
        {
            this.outgoingCommand.Add(result);
        }


        private void InnerProcessLogic()
        {
            while (true)
            {
                var cmd = this.incomeCommand.Take();

                SetResult(Process(cmd));
            }
        }

        private CommandResult Process(Command command)
        {
            CommandResult result = new CommandResult();

            result.Result = SerializerUtility.Instance().BinSerialize("Server: " + DateTime.Now.ToString());

            return result;
        }
    }
}
