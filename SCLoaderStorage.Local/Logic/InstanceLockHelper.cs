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
    class InstanceLockHelper
    {

        private TimeSpan lockLifetime = new TimeSpan();
        private Timer lockRefreshTimer = null;


        internal InstanceLockHelper(TimeSpan lifetime)
        {
            this.lockLifetime = lifetime;
        }


        internal bool TryApplyInstanceLock()
        {

            var lockApplied = ApplyOrRefreshLock();

            if (lockApplied && this.lockRefreshTimer == null)
            {
                CreateLockRefreshTimer();
            }

            return lockApplied;

        }


        internal void ReleaseInstanceLock()
        {

            var lockFile = GetLockFilePath();

            if (this.lockRefreshTimer != null)
            {
                this.lockRefreshTimer.Enabled = false;
                this.lockRefreshTimer = null;
            }

            if (File.Exists(lockFile))
            {
                File.Delete(lockFile);
            }

        }


        private static string GetLockFilePath()
        {

            var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(exePath, "SCLoaderInstanceLock.txt");

        }


        private bool ApplyOrRefreshLock()
        {

            var lockFile = GetLockFilePath();

            if (File.Exists(lockFile))
            {
                var lockFileContent = File.ReadAllText(lockFile, Encoding.ASCII);

                DateTime lockTimeout;
                if (DateTime.TryParse(lockFileContent, out lockTimeout))
                {
                    if (lockTimeout > DateTime.UtcNow)
                    {
                        return false;
                    }
                }
            }

            var newLockTimeout = DateTime.UtcNow.Add(this.lockLifetime);
            File.WriteAllText(lockFile, newLockTimeout.ToString("s"), Encoding.ASCII);

            return true;

        }


        private void CreateLockRefreshTimer()
        {
            this.lockRefreshTimer = new Timer();

            // Half of the actual lifetime should be enough
            this.lockRefreshTimer.Interval = this.lockLifetime.TotalMilliseconds / 2;

            this.lockRefreshTimer.Elapsed += (sender, e) =>
            {
                this.lockRefreshTimer.Enabled = false;

                ApplyOrRefreshLock();

                this.lockRefreshTimer.Enabled = true;
            };

            this.lockRefreshTimer.Enabled = true;
        }


    }
}
