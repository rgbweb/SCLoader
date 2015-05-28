using SCLoaderShared.Helpers;
using SCLoaderShared.Interfaces;
using SoundCloud.API.Client.Objects;
using SoundCloud.API.Client.Objects.TrackPieces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SCLoader.SoundCloud
{

    class SoundCloudDownloader
    {

        public static void DownloadSoundToTemp(string targetFileName, SCTrack scTrack, bool preferOriginalMp3, string scClientID)
        {

            if (preferOriginalMp3 &&
                scTrack.Downloadable.HasValue &&
                scTrack.Downloadable.Value &&
                scTrack.DownloadsRemaining.HasValue &&
                scTrack.DownloadsRemaining.Value > 0 &&
                !string.IsNullOrEmpty(scTrack.DownloadUrl))
            {
                // Download the original mp3 file
                new WebClient().DownloadFile(scTrack.DownloadUrl, targetFileName);
            }
            else if (
                scTrack.Streamable.HasValue &&
                scTrack.Streamable.Value &&
                !string.IsNullOrEmpty(scTrack.StreamUrl))
            {
                // Save the mp3 stream to a file
                using (var scStream = new WebClient().OpenRead(scTrack.StreamUrl + "?client_id=" + scClientID))
                {
                    using (var fileStream = File.Create(targetFileName))
                    {
                        scStream.CopyTo(fileStream);
                    }
                }
            }
            else
            {
                throw new Exception("Cannot download track without download URL or ability to stream.");
            }

        }

        public static void DownloadCoverToTemp(string targetFileName, SCTrack scTrack)
        {

            new WebClient().DownloadFile(scTrack.Artwork.Url(SCArtworkFormat.T500X500), targetFileName);

        }

    }
}
