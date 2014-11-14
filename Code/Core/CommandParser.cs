using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class CommandParser
    {
        private byte[] cmdBytes=new byte[0];
        private object cmdBytesLock = new object();

        public void ProcessReceive(byte[] buffer, int length)
        {
            lock (cmdBytesLock)
            {
                byte[] newCmdBytes = new byte[cmdBytes.Length + length];

                Buffer.BlockCopy(cmdBytes, 0, newCmdBytes, 0, cmdBytes.Length);
                Buffer.BlockCopy(buffer, 0, newCmdBytes, cmdBytes.Length, length);

                this.cmdBytes = newCmdBytes;
            }
        }

        public List<CommandWrapper> GetCommandWrappers()
        {
            lock (cmdBytesLock)
            {
                if (this.cmdBytes.Length <= CommandParserUtils.tag4ContentSize)
                    return null;

                List<CommandWrapper> cmds = new List<CommandWrapper>();

                while (true)
                {
                    if (!CommandParserUtils.IsSizeOKForOneCommand(this.cmdBytes))
                        break;

                    CommandWrapper cmd = CommandParserUtils.ParseCommand(this.cmdBytes);
                    cmds.Add(cmd);

                    this.cmdBytes = CommandParserUtils.TruncateBuffer(this.cmdBytes);
                }

                return cmds;
            }
        }
    }
}