using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Server.Pipes
{
    public class PipeProcessorPool
    {
        private int pipeProcessorCount;
        private int idlePipeProcessorCount;

        public List<PipeProcessor> PipeProcessors { get; set; }
        private object lk_pipes = new object();

        public PipeProcessorPool(int count)
        {
            this.pipeProcessorCount = count;
            this.idlePipeProcessorCount = this.pipeProcessorCount;

            this.PipeProcessors = new List<PipeProcessor>();
        }

        public void PrepareIdlePipeProcessors()
        {
            for (var i = 0; i < this.pipeProcessorCount; i++)
            {
                var pipe = new PipeProcessor();

                this.PipeProcessors.Add(pipe);
            }
        }

        public PipeProcessor PickOneIdle()
        {
            //lock (lk_pipes)
            //{ 

            //}
            return this.PipeProcessors.First();
        }
    }
}
