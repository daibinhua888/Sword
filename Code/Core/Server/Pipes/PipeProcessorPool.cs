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
            var pipe = this.idlePipeProcessors.Take();

            Console.WriteLine("PickOneIdle");

            this.busyPipeProcessors.Add(pipe);

            return pipe;
        }

        public void BackIntoPool(PipeProcessor pipeProcessor)
        {
            Console.WriteLine("BackIntoPool");

            this.busyPipeProcessors.Remove(pipeProcessor);

            this.idlePipeProcessors.Add(pipeProcessor);
        }
    }
}
