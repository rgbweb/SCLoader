using SCLoaderShared.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SCLoaderStorage.Local.Logic
{
    class InstanceLock
    {

        private string lockFile;
        private TimeSpan lockLifetime;
        private Timer lockRefreshTimer = null;

        private string lockId;

        internal InstanceLock(string targetFolder)
        {

            this.lockFile = PathHelpers.GetWorkingCombinedPath(targetFolder, "SCLoaderInstanceLock.txt");
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

            if (File.Exists(this.lockFile))
            {
                File.Delete(this.lockFile);
            }

        }


        private bool ApplyOrRefreshLock()
        {

            if (File.Exists(this.lockFile))
            {
                var lockFileContent = File.ReadAllText(this.lockFile, Encoding.ASCII);

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
            File.WriteAllText(this.lockFile, newLockFileContent, Encoding.ASCII);

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
