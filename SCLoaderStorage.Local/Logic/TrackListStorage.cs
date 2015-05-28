using Newtonsoft.Json;
using SCLoaderShared.Helpers;
using SCLoaderShared.DataClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCLoaderStorage.Local.Logic
{
    class TrackListStorage
    {

        private string jsonFile = "";

        internal TrackListStorage(string targetFolder)
        {

            this.jsonFile = PathHelpers.GetWorkingCombinedPath(targetFolder, "SCLoaderTrackList.json");

        }


        internal StorageTrackList GetTrackList()
        {

            if (!File.Exists(this.jsonFile))
            {
                // Create a new database file
                File.Create(this.jsonFile).Dispose();
            }

            var json = File.ReadAllText(this.jsonFile, Encoding.UTF8);
            return JsonConvert.DeserializeObject<StorageTrackList>(json);

        }

        internal void UpdateTrackList(StorageTrackList trackList)
        {

            if (!File.Exists(this.jsonFile))
            {
                throw new FileNotFoundException("The track list file was not found at path: " + this.jsonFile);
            }

            var json = JsonConvert.SerializeObject(trackList, Formatting.Indented);
            File.WriteAllText(this.jsonFile, json, Encoding.UTF8);

        }

    }
}
