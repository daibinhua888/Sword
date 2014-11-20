using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Server.Cleaner
{
    public class OfflineConnectionCleanWorker
    {
        private TimeSpan timeout = TimeSpan.FromSeconds(10);

        public void DetectAndTagInactiveConnectionWorkers()
        {
            lock (ServerRuntime.master.lock_connectionObjects)
            {
                ServerRuntime.master.connectionObjects.ForEach(c =>
                {
                    if (c.LastActiveTime.Add(timeout) < DateTime.Now)
                        c.IsTagged = true;
                });
            }
        }

        public void CleanTaggedConnectionWorkers()
        { 
            lock(ServerRuntime.master.lock_connectionObjects)
            {
                foreach (var c in ServerRuntime.master.connectionObjects)
                    if(c.IsTagged)
                        c.Dispose();

                ServerRuntime.master.connectionObjects.RemoveAll(w=>w.IsTagged==true);
            }
        }
    }
}
