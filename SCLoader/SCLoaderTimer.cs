using SCLoader.Properties;
using SCLoaderShared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCLoader
{
    class SCLoaderTimer
    {

        public TimeSpan Interval;
        public SCLoader SCLoader;
        public ILogger Logger;

        private Timer timer;


        public SCLoaderTimer(TimeSpan interval, SCLoader scLoader, ILogger logger)
        {

            this.Interval = interval;
            this.SCLoader = scLoader;
            this.Logger = logger;

            // Prepare the timer
            this.timer = new Timer(TimerCallback, this, Timeout.Infinite, Timeout.Infinite);

        }


        /// <summary>
        /// Starts the first execution immediately and the afterwards in the given interval.
        /// </summary>
        public void Start()
        {

            // Set only the due time until first start
            // The period is handled manually by restarting the timer at the end of the callback method
            this.timer.Change(0, Timeout.Infinite);

        }

        /// <summary>
        /// Disposes the timer
        /// </summary>
        public void Stop()
        {

            this.timer.Dispose();

        }


        private void TimerCallback(object state)
        {

            // The state object is an instance of the current class
            var instance = (SCLoaderTimer)state;

            instance.Logger.LogVerbose("Check for new downloads...");
            try
            {
                instance.SCLoader.ExeuteDownloader();
            }
            catch (Exception ex)
            {
                instance.Logger.LogException("Failed to execute the downloader.", ex);
            }

            instance.Logger.LogVerbose("Wait {0} minutes for the next check...", this.Interval.TotalMinutes);

            // Reset the timer for the next run
            this.timer.Change(instance.Interval, TimeSpan.FromMilliseconds(-1));

        }



    }
}
