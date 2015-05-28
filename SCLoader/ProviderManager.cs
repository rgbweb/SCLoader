using SCLoader.Properties;
using SCLoaderShared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCLoader
{
    class ProviderManager
    {

        [ImportMany(typeof(IStorageProvider))]
        private IEnumerable<IStorageProvider> storageProviders = null;

        [ImportMany(typeof(ILogger))]
        private IEnumerable<ILogger> loggers = null;


        public ProviderManager()
        {

            var catalog = new DirectoryCatalog(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

        }


        public IStorageProvider GetStorageProvider(string name)
        {

            var storageProvider = this.storageProviders
                .Where(x => x.StorageProviderName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            if (storageProvider == null)
            {
                throw new DllNotFoundException("Failed to find a StorageProvider with name '" + name + "'. Check if the name is correct and the DLL is in place.");
            }

            return storageProvider;

        }

        public ILogger GetLogger(string name)
        {

            var logger = this.loggers
                .Where(x => x.LoggerName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            if (logger == null)
            {
                throw new DllNotFoundException("Failed to find a Logger with name '" + name + "'. Check if the name is correct and the DLL is in place.");
            }

            return logger;

        }

    }
}
