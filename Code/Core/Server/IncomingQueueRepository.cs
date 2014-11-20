using Core.CommandBus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Server
{
    internal class IncomingQueueRepository
    {
        private BlockingCollection<Command> innerQueue = new BlockingCollection<Command>();

        public void Enqueue(Command cmd)
        {
            innerQueue.Add(cmd);
        }

        public Command DequeueNoWait()
        {
            if (innerQueue.Count == 0)
                return null;

            return DequeueBlock();
        }

        public Command DequeueBlock()
        {
            return innerQueue.Take();
        }
    }
}
