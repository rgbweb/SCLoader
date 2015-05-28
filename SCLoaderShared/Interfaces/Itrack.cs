using System;

namespace SCLoaderShared.Interfaces
{
    public interface ITrack
    {

        string SoundCloudID { get; set; }

        string Album { get; set; }
        string Artist { get; set; }
        float BPM { get; set; }
        string CoverUrl { get; set; }
        string Description { get; set; }
        string Genre { get; set; }
        string Title { get; set; }
        int TrackNumber { get; set; }
        string TrackUrl { get; set; }
        string TrackUserUrl { get; set; }
        int Year { get; set; }

        string CoverFileName { get; set; }
        string Mp3FileName { get; set; }

    }
}
