using SCLoader.Properties;
using SCLoader.SoundCloud;
using SCLoaderShared.Helpers;
using SCLoaderShared.DataClasses;
using SCLoaderShared.Interfaces;
using SoundCloud.API.Client.Objects;
using SoundCloud.API.Client.Objects.TrackPieces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCLoader
{
    class SCLoader
    {

        private Settings settings;
        private ILogger logger;
        private IStorageProvider storage;


        public SCLoader(Settings settings, ILogger logger, IStorageProvider storage)
        {

            this.settings = settings;
            this.logger = logger;
            this.storage = storage;

        }


        public void ExeuteDownloader()
        {

            // Initialize the "real" SoundCloud client
            var scClient = new SoundCloudClient(settings.SoundCloudClientID,
                settings.SoundCloudClientSecret,
                settings.SoundCloudUserName,
                settings.SoundCloudUserPassword);

            var trackList = storage.GetTrackList();
            if (trackList == null)
            {
                trackList = new StorageTrackList();
            }

            var scTracks = scClient.GetAllFavorites();

            logger.LogVerbose("{0} tracks found in the SoundCloud favorites.", scTracks.Count);

            var newTracks = FilterNewTracks(scTracks, trackList.Tracks);
            logger.LogVerbose("{0} tracks are new.", newTracks.Count);

            if (settings.DownloadableTracksOnly)
            {
                newTracks = FilterDownloadableTracks(newTracks);
                logger.LogVerbose("{0} tracks are downloadable.", newTracks.Count);
            }

            if (newTracks.Any())
            {
                foreach (var scTrack in newTracks)
                {

                    logger.LogVerbose("Processing track \"{0}\" [{1}]...", scTrack.Title, scTrack.Id);

                    // Download the track and cover, write ID3Tag and save to storage
                    var track = ExecuteTrackDwonload(scTrack);

                    // Update and save track list
                    trackList.Tracks.Add(track);
                    trackList.LastUpdateUTC = DateTime.UtcNow;
                    storage.UpdateTrackList(trackList);

                }
            }
            else
            {
                // Update time stamp and save
                trackList.LastUpdateUTC = DateTime.UtcNow;
                storage.UpdateTrackList(trackList);
            }

            logger.LogVerbose("All new tracks were saved successfully.");

        }


        private ICollection<SCTrack> FilterNewTracks(IEnumerable<SCTrack> allTracks, IEnumerable<Track> existingTracks)
        {

            ICollection<string> existingTrackIDs = existingTracks.Select(x => x.SoundCloudID).ToList();

            var result = new List<SCTrack>();

            foreach (var scTrack in allTracks)
            {
                if (!existingTrackIDs.Contains(scTrack.Id))
                {
                    result.Add(scTrack);
                }
            }

            return result;

        }

        private ICollection<SCTrack> FilterDownloadableTracks(IEnumerable<SCTrack> tracks)
        {

            var result = new List<SCTrack>();

            foreach (var scTrack in tracks)
            {
                if (scTrack.Downloadable.HasValue && scTrack.Downloadable.Value)
                {
                    result.Add(scTrack);
                }
            }

            return result;

        }

        private Track ExecuteTrackDwonload(SCTrack scTrack)
        {

            // Start mp3 download
            var tempMp3Filename = GetTempFilename(settings.CustomTempFilePath);
            SoundCloudDownloader.DownloadSoundToTemp(tempMp3Filename, scTrack, settings.PreferOriginalMp3, settings.SoundCloudClientID);

            // Start cover JPEG download (500x500)
            var tempCoverFilename = GetTempFilename(settings.CustomTempFilePath);
            SoundCloudDownloader.DownloadCoverToTemp(tempCoverFilename, scTrack);

            // Copy the meta infos for the ID3-Tag
            Track track = GetTrackInfo(scTrack);

            // Add the ID3-Tag meta infos and cover image
            ID3Tagger.WriteID3Tag(track, tempMp3Filename, tempCoverFilename);

            // Save the mp3 to the storage
            using (var fileStream = File.OpenRead(tempMp3Filename))
            {
                storage.SaveMp3(fileStream, track);
            }

            // Save the cover to the storage
            using (var fileStream = File.OpenRead(tempCoverFilename))
            {
                storage.SaveCover(fileStream, track);
            }

            // Free up the temp space
            File.Delete(tempMp3Filename);
            File.Delete(tempCoverFilename);

            return track;

        }

        private Track GetTrackInfo(SCTrack scTrack)
        {

            var track = new Track();

            track.SoundCloudID = scTrack.Id;

            track.Album = "SoundCloud " + scTrack.Title;
            track.Artist = scTrack.User.UserName;
            if (scTrack.Bpm.HasValue)
            {
                track.BPM = scTrack.Bpm.Value;
            }
            else
            {
                track.BPM = 0;
            }
            track.Description = scTrack.Description;
            track.Genre = "SoundCloud Downloads";
            track.Title = scTrack.Title;
            track.TrackNumber = 1;
            track.TrackUrl = scTrack.PermalinkUrl;
            track.TrackUserUrl = scTrack.User.PermalinkUrl;
            if (scTrack.ReleaseDate.HasValue)
            {
                track.Year = scTrack.ReleaseDate.Value.Year;
            }
            else
            {
                track.Year = DateTime.Today.Year;
            }

            track.CoverFileName = track.Artist + " " + track.Title + ".jpg";
            track.Mp3FileName = track.Artist + " " + track.Title + ".mp3";

            return track;

        }

        private string GetTempFilename(string customTempFolder)
        {

            if (string.IsNullOrEmpty(customTempFolder))
            {
                return Path.GetTempFileName();
            }
            else
            {
                var tempFileName = Guid.NewGuid().ToString("N") + ".tmp";
                return PathHelpers.GetWorkingCombinedPath(customTempFolder, tempFileName);
            }

        }

    }
}
