using Core.CommandBus;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core.Communication
{
    public class ConnectionObject
    {
        public ConnectionObject(Socket socket)
        {
            this.Socket = socket;

            this.parser = new CommandParser<Command>();

            this.saea_receive = new SocketAsyncEventArgs();

            this.receiveBuffer = new byte[1024*1024];

            this.saea_receive.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);

            this.saea_receive.Completed += SocketAsyncEventArgs_Completed;

            this.saea_send = new SocketAsyncEventArgs();
            this.saea_send.Completed += saea_send_Completed;
        }

        private byte[] receiveBuffer;
        public Socket Socket { get; set; }
        public SocketAsyncEventArgs saea_receive { get; set; }
        public SocketAsyncEventArgs saea_send { get; set; }
        public DateTime LastActiveTime { get; set; }
        private CommandParser<Command> parser;

        public void StartReceive()
        {
            var delayed=this.Socket.ReceiveAsync(this.saea_receive);

            if (!delayed)
            {
                if (this.saea_receive.BytesTransferred>0)
                    ProcessReceive(this.saea_receive.Buffer, this.saea_receive.BytesTransferred);
            }
        }

        private void SocketAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                if (e.SocketError != SocketError.Success)
                {
                    Console.WriteLine("Error!");
                    return;
                }

                if (e.BytesTransferred == 0)
                {
                    Console.WriteLine("Client disconnected. "+this.Socket.RemoteEndPoint.ToString());
                    return;
                }

                this.LastActiveTime = DateTime.Now;
                ProcessReceive(e.Buffer, e.BytesTransferred);

                StartReceive();
            }
        }

        private void ProcessReceive(byte[] buffer, int length)
        {
            parser.ProcessReceive(buffer, length);

            var cmds = parser.GetDTOs();

            if (cmds != null && cmds.Count > 0)
            {
                cmds.ForEach(cmd =>
                {
                    Console.WriteLine("[{0}] New Command: " + cmd.AppID+", "+cmd.CallContract, cmd.SessionID);

                    MemoryStream ms = new MemoryStream();
                    BufferedStream bs = new BufferedStream(ms);
                    BinaryWriter bw = new BinaryWriter(bs, UnicodeEncoding.UTF8);

                    CommandResult cmdResult = new CommandResult();

                    cmdResult.Result = Utils.SerializerUtility.BinSerialize("Server: " + DateTime.Now.ToString());

                    var cmdBytes = SerializerUtility.BinSerialize(cmdResult);
                    bw.Write(cmdBytes.Length);
                    bw.Write(cmdBytes);

                    bw.Flush();

                    byte[] sendBuffer = ms.ToArray();
                    saea_send.SetBuffer(sendBuffer, 0, sendBuffer.Length);

                    this.Socket.SendAsync(saea_send);
                });
            }
        }


        private void saea_send_Completed(object sender, SocketAsyncEventArgs e)
        {
            //Nothing to do
        }
    }
}
