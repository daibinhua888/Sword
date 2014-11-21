using Sword.CommandBus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword.Server
{
    internal class OutgoingQueueRepository
    {
        private BlockingCollection<CommandResult> innerQueue = new BlockingCollection<CommandResult>();

        public void Enqueue(CommandResult cmd)
        {
            innerQueue.Add(cmd);
        }

        public CommandResult DequeueNoWait()
        {
            if (innerQueue.Count == 0)
                return null;

            return DequeueBlock();
        }

        public CommandResult DequeueBlock()
        {
            return innerQueue.Take();
        }
    }
}
