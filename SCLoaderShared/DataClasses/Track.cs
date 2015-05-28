using SCLoaderShared.Interfaces;
using System;

namespace SCLoaderShared.DataClasses
{
    public class Track
    {

        public string SoundCloudID { get; set; }

        public string Album { get; set; }
        public string Artist { get; set; }
        public float BPM { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string Title { get; set; }
        public int TrackNumber { get; set; }
        public string TrackUrl { get; set; }
        public string TrackUserUrl { get; set; }
        public int Year { get; set; }

        public string CoverFileName { get; set; }
        public string Mp3FileName { get; set; }

    }
}
