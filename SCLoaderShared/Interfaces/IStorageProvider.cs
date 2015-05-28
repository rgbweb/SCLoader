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

        bool TryApplyInstanceLock();
        void ReleaseInstanceLock();

        IStorageTrackList GetTrackList();
        void UpdateTrackList(IStorageTrackList trackList);

        void SaveMp3(FileStream mp3File, ITrack trackInfo);
        void SaveCover(FileStream jpegFile, ITrack trackInfo);

    }
}
