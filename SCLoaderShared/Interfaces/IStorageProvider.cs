using SCLoaderShared.DataClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCLoaderShared.Interfaces
{
    public interface IStorageProvider
    {

        string StorageProviderName { get; }

        void Initialize();

        bool TryApplyInstanceLock(TimeSpan lifetime);
        void ReleaseInstanceLock();

        StorageTrackList GetTrackList();
        void UpdateTrackList(StorageTrackList trackList);

        void SaveMp3(FileStream mp3File, Track trackInfo);
        void SaveCover(FileStream jpegFile, Track trackInfo);

    }
}
