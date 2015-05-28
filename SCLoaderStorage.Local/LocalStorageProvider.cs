using SCLoaderShared.Interfaces;
using SCLoaderStorage.Local.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCLoaderStorage.Local
{
    public class LocalStorageProvider : IStorageProvider
    {

        private InstanceLockHelper instanceLockHelper;
        private TrackListHelper trackListHelper;
        private FileHelper fileHelper;


        public LocalStorageProvider(dynamic config)
        {

            var instanceLockLifetime = config.InstaneLockLifetime;
            var mp3TargetFolder = config.Mp3TargetFolder;

            this.instanceLockHelper = new InstanceLockHelper(instanceLockLifetime);
            this.trackListHelper = new TrackListHelper();
            this.fileHelper = new FileHelper(mp3TargetFolder);

        }


        #region Interface implementation

        public bool TryApplyInstanceLock()
        {
            return this.instanceLockHelper.TryApplyInstanceLock();
        }

        public void ReleaseInstanceLock()
        {
            this.instanceLockHelper.ReleaseInstanceLock();
        }

        public IStorageTrackList GetTrackList()
        {
            return this.trackListHelper.GetTrackList();
        }

        public void UpdateTrackList(IStorageTrackList trackList)
        {
            this.trackListHelper.UpdateTrackList(trackList);
        }

        public void SaveMp3(FileStream mp3File, ITrack trackInfo)
        {
            this.fileHelper.SaveMp3(mp3File, trackInfo);
        }

        public void SaveCover(FileStream jpegFile, ITrack trackInfo)
        {
            this.fileHelper.SaveCover(jpegFile, trackInfo);
        }

        #endregion

    }
}
