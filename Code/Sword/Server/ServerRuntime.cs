using Sword.CommandBus;
using Sword.Server.Cleaner;
using Sword.Server.Pipes;
using Sword.Server.PipeSelectors;
using Sword.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sword.Server
{
    public static class ServerRuntime
    {
        internal static ConnectionMaster master;
        private static IncomingQueueRepository incomingQueueRepository = new IncomingQueueRepository();
        private static OutgoingQueueRepository outgoingQueueRepository = new OutgoingQueueRepository();
        private static OfflineConnectionCleanWorker offlineConnectionCleanWorker;
        private static PipeProcessorPool pipeProcessorPool;

        static ServerRuntime()
        {
            ServiceRegistry.RegisterSwordServices();
        }

        private static bool _setupOk=false;
        public static void Setup(int maxPoolSize, int timeoutSeconds)
        {
            pipeProcessorPool = new PipeProcessorPool(maxPoolSize);
            offlineConnectionCleanWorker = new OfflineConnectionCleanWorker(TimeSpan.FromSeconds(timeoutSeconds));

            _setupOk=true;
        }

        public static void Start(int port)
        {
            if (master != null)
                throw new Exception("已经Start了");

            if (!_setupOk)
                Setup(10, 30);

            master = new ConnectionMaster(port);
            master.Start();

            pipeProcessorPool.PrepareIdlePipeProcessors();

            Task.Factory.StartNew(() => {
                StartDispatchIncomeQueue();
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(() =>
            {
                StartDispatchOutgoingQueue();
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(() =>
            {
                StartCleanWorker();
            }, TaskCreationOptions.LongRunning);
        }

        private static void StartCleanWorker()
        {
            while (true)
            {
                offlineConnectionCleanWorker.DetectAndTagInactiveConnectionWorkers();

                offlineConnectionCleanWorker.CleanTaggedConnectionWorkers();
            }
        }

        internal static void StartDispatchIncomeQueue()
        {
            while (true)
            {
                var incomingMsg = incomingQueueRepository.DequeueBlock();

                if (incomingMsg.ConnectionWorker.IsTagged)
                    continue;

                PipeProcessor pipe = pipeProcessorPool.PickOneIdle();

                pipe.GiveTask(incomingMsg);
            }
        }

        internal static void StartDispatchOutgoingQueue()
        {
            while (true)
            {
                var outgoingMsg = outgoingQueueRepository.DequeueBlock();

                if (outgoingMsg.ConnectionWorker.IsTagged)
                    continue;

                outgoingMsg.ConnectionWorker.SendResponse(outgoingMsg);
            }
        }

        internal static void AddCommandToIncomingQueueRepository(Command cmd)
        {
            incomingQueueRepository.Enqueue(cmd);
        }

        internal static void AddCommandResultToOutgoingQueueRepository(CommandResult cmdResult)
        {
            outgoingQueueRepository.Enqueue(cmdResult);
        }
    }
}
