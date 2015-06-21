using Newtonsoft.Json;
using SCLoaderShared.DataClasses;
using SCLoaderShared.Interfaces;
using SCLoaderStorage.Mega.ApiClient;
using SCLoaderStorage.Mega.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCLoaderStorage.Mega
{

    [Export(typeof(IStorageProvider))]
    public class MegaStorageProvider : IStorageProvider
    {

        private InstanceLock instanceLockStorage;
        private TrackListStorage trackListStorage;
        private FileStorage fileStorage;

        string IStorageProvider.StorageProviderName
        {
            get
            {
                return "MegaStorageProvider";
            }
        }


        void IStorageProvider.Initialize(string serializedConfig)
        {

            // Parse the configuration
            Configuration config;
            try
            {
                config = JsonConvert.DeserializeObject<Configuration>(serializedConfig);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to parse the configuration", ex);
            }

            var megaClient = new MegaClient(config.Email, config.Password);

            this.instanceLockStorage = new InstanceLock(config.InstanceLockFilePath, megaClient);
            this.trackListStorage = new TrackListStorage(config.TrackListFilePath, megaClient);
            this.fileStorage = new FileStorage(config.Mp3TargetPath, megaClient);

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

    }
}
