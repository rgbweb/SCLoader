using SCLoaderShared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCLoaderShared.DataClasses
{
    public class StorageTrackList
    {

        public StorageTrackList()
        {

            this.LastUpdateUTC = DateTime.MinValue;
            this.Tracks = new List<Track>();

        }

        public DateTime LastUpdateUTC { get; set; }

        public ICollection<Track> Tracks { get; private set; }

    }
}
