using Core.CommandBus;
using Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Communication
{
    internal class QueueDispatcher
    {
        public QueueDispatcher(Socket socket)
        {
            this.Socket = socket;

            this.sendingQueue = new BlockingCollection<CommandResult>();

            StartSendTask();
        }

        public Socket Socket { get; set; }

        private SocketAsyncEventArgs saea_send { get; set; }
        private BlockingCollection<CommandResult> sendingQueue;

        private void StartSendTask()
        {
            var task = new Task(() => {
                SendInQueue();
            });

            task.Start();
        }

        private void SendInQueue()
        {
            while (true)
            {
                var cmd = this.sendingQueue.Take();
                if (cmd == null)
                {
                    Thread.Sleep(200);
                    continue;
                }

                MemoryStream ms = new MemoryStream();
                BufferedStream bs = new BufferedStream(ms);
                BinaryWriter bw = new BinaryWriter(bs, UnicodeEncoding.UTF8);

                var cmdBytes = SerializerUtility.BinSerialize(cmd);
                bw.Write(cmdBytes.Length);
                bw.Write(cmdBytes);

                bw.Flush();

                this.Socket.Send(ms.ToArray());
            }
        }

        public void Dispatch(CommandResult result)
        {
            sendingQueue.Add(result);
        }
    }
}
