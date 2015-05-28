using SCLoaderShared.DataClasses;
using SCLoaderShared.Interfaces;
using SCLoaderStorage.Local.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCLoaderStorage.Local
{

    [Export(typeof(IStorageProvider))]
    public class LocalStorageProvider : IStorageProvider
    {

        private const string ConfigurationSection = "storageSettings/LocalStorageProvider";

        private InstanceLock instanceLockStorage;
        private TrackListStorage trackListStorage;
        private FileStorage fileStorage;


        public LocalStorageProvider()
        {

            // Load custom configuration from host App.config
            var config = (Configuration)ConfigurationManager.GetSection(ConfigurationSection);
            if (config == null)
            {
                throw new ConfigurationErrorsException("Failed to load configuration section [" + ConfigurationSection + "] from host App.config file.");
            }

            this.instanceLockStorage = new InstanceLock(config.InstanceLockFilePath);
            this.trackListStorage = new TrackListStorage(config.TrackListFilePath);
            this.fileStorage = new FileStorage(config.Mp3TargetPath);

        }


        #region Interface implementation

        string IStorageProvider.StorageProviderName
        {
            get
            {
                return "LocalStorageProvider";
            }
        }

        bool IStorageProvider.TryApplyInstanceLock(TimeSpan lifetime)
        {
            return this.instanceLockStorage.TryApplyLock(lifetime);
        }

        void IStorageProvider.ReleaseInstanceLock()
        {
            this.instanceLockStorage.ReleaseLock();
        }

        StorageTrackList IStorageProvider.GetTrackList()
        {
            return this.trackListStorage.GetTrackList();
        }

        void IStorageProvider.UpdateTrackList(StorageTrackList trackList)
        {
            this.trackListStorage.UpdateTrackList(trackList);
        }

        void IStorageProvider.SaveMp3(FileStream mp3File, Track trackInfo)
        {
            this.fileStorage.SaveMp3(mp3File, trackInfo);
        }

        void IStorageProvider.SaveCover(FileStream jpegFile, Track trackInfo)
        {
            this.fileStorage.SaveCover(jpegFile, trackInfo);
        }

        #endregion

    }
}
