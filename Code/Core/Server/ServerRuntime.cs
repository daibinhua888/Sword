using Core.CommandBus;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Server
{
    public static class ServerRuntime
    {
        private static ConnectionMaster master;
        private static IncomingQueueRepository incomingQueueRepository = new IncomingQueueRepository();
        private static OutgoingQueueRepository outgoingQueueRepository = new OutgoingQueueRepository();

        public static void Start(int port)
        {
            if (master != null)
                throw new Exception("已经Start了");

            master = new ConnectionMaster(port);
            master.Start();

            Task.Factory.StartNew(() => {
                StartDispatchIncomeQueue();
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(() =>
            {
                StartDispatchOutgoingQueue();
            }, TaskCreationOptions.LongRunning);
        }

        internal static void StartDispatchIncomeQueue()
        {
            while (true)
            {
                var incomingMsg = incomingQueueRepository.DequeueBlock();

                Console.WriteLine("Incoming: "+ incomingMsg.Method2Invoke);


                CommandResult result = new CommandResult();
                result.Result = SerializerUtility.Instance().BinSerialize("Server: " + DateTime.Now.ToString());

                result.ConnectionWorker = incomingMsg.ConnectionWorker;
                ServerRuntime.AddCommandResultToOutgoingQueueRepository(result);
            }
        }

        internal static void StartDispatchOutgoingQueue()
        {
            while (true)
            {
                var outgoingMsg = outgoingQueueRepository.DequeueBlock();

                Console.WriteLine("Outgoing: " + outgoingMsg.Result.Length);

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
