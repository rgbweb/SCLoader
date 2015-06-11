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

            // Load and initialize providers
            var providerManager = new ProviderManager();

            ILogger logger = providerManager.GetLogger(settings.LoggerName);
            logger.Initialize();

            IStorageProvider storageProvider = providerManager.GetStorageProvider(settings.StorageProviderName);
            storageProvider.Initialize();

            // Initialize business logic objects
            SCLoader scLoader = new SCLoader(settings, logger, storageProvider);
            SCLoaderTimer scLoaderTimer = new SCLoaderTimer(TimeSpan.FromMinutes(settings.CheckIntervalMinutes), scLoader, logger);
            InstanceLock instanceLock = new InstanceLock(TimeSpan.FromMinutes(settings.InstanceLockLifetimeMinutes), storageProvider, logger);

            // Apply the instance lock and start to work
            instanceLock.ApplyAsync(() =>
            {
                scLoaderTimer.Start();
            });

            // Wait for ESC-key while the application runs...
            do
            {
                logger.LogInformation("Press [ESC] to close SCLoader.");
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            scLoaderTimer.Stop();

            instanceLock.Release();

            // ExitCode != 0 will cause an automatic restart in some services like AppHarbor Background Workers
            Environment.ExitCode = 1;

        }

    }
}
