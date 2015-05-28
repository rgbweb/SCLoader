using Newtonsoft.Json;
using SCLoaderShared.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCLoaderStorage.Local.Logic
{
    class TrackListHelper
    {


        internal IStorageTrackList GetTrackList()
        {

            var jsonFile = GetTrackListFilePath();
            if (!File.Exists(jsonFile))
            {
                throw new FileNotFoundException("The tracklist file was not found at path: " + jsonFile);
            }

            var json = File.ReadAllText(jsonFile, Encoding.UTF8);
            return JsonConvert.DeserializeObject<IStorageTrackList>(json);

        }

        internal void UpdateTrackList(IStorageTrackList trackList)
        {

            var jsonFile = GetTrackListFilePath();
            if (!File.Exists(jsonFile))
            {
                throw new FileNotFoundException("The tracklist file was not found at path: " + jsonFile);
            }

            var json = JsonConvert.SerializeObject(trackList, Formatting.Indented);

            File.WriteAllText(jsonFile, json, Encoding.UTF8);

        }


        private static string GetTrackListFilePath()
        {

            var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(exePath, "SCLoaderTrackList.json");

        }


    }
}
