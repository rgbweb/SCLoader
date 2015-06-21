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

            ILogger logger = null;

            try
            {

                // Note:
                // The settings file is automatically overwritten if an SCLoader.exe.custom.config file exists in the application directory
                // Settings can be configured by using the ApplicationSettings or the (older) AppSettings sections in AppConfig
                // The AppSettings value is used if both have a value with the same name
                var settings = new SettingsManager();

                // Load providers using MEF
                var providerManager = new ProviderManager();

                // Load an initialize the LoggingProvider
                string loggerName = settings.GetSetting("LoggerName");
                string loggerSettings = settings.GetSetting(loggerName + "Settings");

                logger = providerManager.GetLogger(loggerName);
                logger.Initialize(loggerSettings);
                logger.LogVerbose("Logger [{0}] initialized.", logger.LoggerName);

                // Load an initialize the StorageProvider
                string storageProviderName = settings.GetSetting("StorageProviderName");
                string storageProviderSettings = settings.GetSetting(storageProviderName + "Settings");

                IStorageProvider storageProvider = providerManager.GetStorageProvider(storageProviderName);
                storageProvider.Initialize(storageProviderSettings);
                logger.LogVerbose("StorageProvider [{0}] initialized.", storageProvider.StorageProviderName);

                // Initialize business logic objects
                SCLoader scLoader = new SCLoader(settings, logger, storageProvider);
                SCLoaderTimer scLoaderTimer = new SCLoaderTimer(TimeSpan.FromMinutes(settings.GetSetting("CheckIntervalMinutes")), scLoader, logger);
                InstanceLock instanceLock = new InstanceLock(TimeSpan.FromMinutes(settings.GetSetting("InstanceLockLifetimeMinutes")), storageProvider, logger);

                // Apply the instance lock and start to work
                instanceLock.ApplyAsync(() =>
                {
                    scLoaderTimer.Start();
                });

                // Run the application until someone wants to stop it
                WaitForApplicationCloseCommand();

                scLoaderTimer.Stop();

                instanceLock.Release();

            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.LogException("Unhandled application exception thrown", ex);
                }
            }

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
