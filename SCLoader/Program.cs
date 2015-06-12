using SCLoader.Properties;
using SCLoaderShared.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SCLoader
{
    class Program
    {

        static void Main(string[] args)
        {

            // Note: The App.config file is overwritten by the post-build event if an "MyDebugApp.config" file exists in the project folder

            var settings = new Settings();

            // Load providers using MEF
            var providerManager = new ProviderManager();

            ILogger logger = providerManager.GetLogger(settings.LoggerName);
            logger.Initialize();
            logger.LogVerbose("Logger [{0}] initialized.", logger.LoggerName);

            IStorageProvider storageProvider = providerManager.GetStorageProvider(settings.StorageProviderName);
            storageProvider.Initialize();
            logger.LogVerbose("StorageProvider [{0}] initialized.", storageProvider.StorageProviderName);

            // Initialize business logic objects
            SCLoader scLoader = new SCLoader(settings, logger, storageProvider);
            SCLoaderTimer scLoaderTimer = new SCLoaderTimer(TimeSpan.FromMinutes(settings.CheckIntervalMinutes), scLoader, logger);
            InstanceLock instanceLock = new InstanceLock(TimeSpan.FromMinutes(settings.InstanceLockLifetimeMinutes), storageProvider, logger);

            // Apply the instance lock and start to work
            instanceLock.ApplyAsync(() =>
            {
                scLoaderTimer.Start();
            });

            // Run the application until anyone wants to stop it
            WaitForApplicationCloseCommand();

            scLoaderTimer.Stop();

            instanceLock.Release();

            // ExitCode != 0 will cause an automatic restart in some services like AppHarbor Background Workers
            Environment.ExitCode = 1;

        }

        static void WaitForApplicationCloseCommand()
        {

            try
            {

                // If we have a console, we wait for the ESC key to be pressed
                do
                {
                    Console.WriteLine("Press [ESC] to close SCLoader.");
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            }
            catch (InvalidOperationException)
            {

                // No console available
                // Could be a worker process in a cloud environment or something
                // Just let the current thread sleep to run the application forever..
                new ManualResetEvent(false).WaitOne();

            }

        }

    }
}
