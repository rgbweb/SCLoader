using CG.Web.MegaApiClient;
using SCLoaderShared.Helpers;
using SCLoaderStorage.Mega.ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SCLoaderStorage.Mega.Logic
{
    class InstanceLock
    {

        private const string fileName = "SCLoaderInstanceLock.txt";

        private INode directoryNode;
        private TimeSpan lockLifetime;
        private Timer lockRefreshTimer = null;

        private MegaClient megaClient;

        private string lockId;

        internal InstanceLock(string targetPath, MegaClient megaClient)
        {

            this.megaClient = megaClient;
            this.directoryNode = megaClient.GetOrAddDirectoryNode(targetPath);

            // Unique lock id
            this.lockId = Guid.NewGuid().ToString("N");

        }


        internal bool TryApplyLock(TimeSpan lifetime)
        {

            this.lockLifetime = lifetime;

            var lockApplied = ApplyOrRefreshLock();

            if (lockApplied && this.lockRefreshTimer == null)
            {
                CreateLockRefreshTimer();
            }

            return lockApplied;

        }


        internal void ReleaseLock()
        {

            if (this.lockRefreshTimer != null)
            {
                this.lockRefreshTimer.Stop();
                this.lockRefreshTimer = null;
            }

            megaClient.DeleteFile(this.directoryNode, InstanceLock.fileName, false);

        }


        private bool ApplyOrRefreshLock()
        {

            // Returns an empty string if the file does not exist
            var lockFileContent = this.megaClient.GetFileContent(this.directoryNode, InstanceLock.fileName);
            if (lockFileContent.Length > 0)
            {

                var lockFileId = lockFileContent.Split('|').FirstOrDefault();
                var lockFileTime = lockFileContent.Split('|').LastOrDefault();

                DateTime lockTimeout;
                if (DateTime.TryParse(lockFileTime, out lockTimeout))
                {
                    // Check if a lock from another instance is still active
                    if (lockTimeout > DateTime.UtcNow && !lockFileId.Equals(this.lockId, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return false;
                    }
                }
            }

            // Add or update the LockFile
            var newLockTimeout = DateTime.UtcNow.Add(this.lockLifetime);
            var newLockFileContent = this.lockId + "|" + newLockTimeout.ToString("s");
            this.megaClient.SaveFileContent(this.directoryNode, InstanceLock.fileName, newLockFileContent, true);

            return true;

        }


        private void CreateLockRefreshTimer()
        {
            this.lockRefreshTimer = new Timer();

            // Half of the actual lifetime should be enough
            this.lockRefreshTimer.Interval = this.lockLifetime.TotalMilliseconds / 2;

            // Restart the timer from within the elapsed event
            this.lockRefreshTimer.AutoReset = false;

            this.lockRefreshTimer.Elapsed += (sender, e) =>
            {

                ApplyOrRefreshLock();

                this.lockRefreshTimer.Start();
            };

            this.lockRefreshTimer.Start();
        }


    }
}
