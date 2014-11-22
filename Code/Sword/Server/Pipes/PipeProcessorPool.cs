using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword.Server.Pipes
{
    public class PipeProcessorPool
    {
        private int pipeProcessorCount;
        private int idlePipeProcessorCount;

        private BlockingCollection<PipeProcessor> idlePipeProcessors = new BlockingCollection<PipeProcessor>();
        private List<PipeProcessor> busyPipeProcessors = new List<PipeProcessor>();
        private object lk4Swap = new object();

        public PipeProcessorPool(int count)
        {
            this.pipeProcessorCount = count;
            this.idlePipeProcessorCount = this.pipeProcessorCount;
        }

        public void PrepareIdlePipeProcessors()
        {
            for (var i = 0; i < this.pipeProcessorCount; i++)
            {
                var pipe = new PipeProcessor(this);

                this.idlePipeProcessors.Add(pipe);
            }
        }

        public PipeProcessor PickOneIdle()
        {
            lock (lk4Swap)
            {
                var pipe = this.idlePipeProcessors.Take();

                this.busyPipeProcessors.Add(pipe);

                return pipe;
            }
        }

        public void BackIntoPool(PipeProcessor pipeProcessor)
        {
            lock (lk4Swap)
            {
                this.busyPipeProcessors.Remove(pipeProcessor);

                this.idlePipeProcessors.Add(pipeProcessor);
            }
        }
    }
}
