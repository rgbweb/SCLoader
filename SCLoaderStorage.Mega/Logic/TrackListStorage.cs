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
using CG.Web.MegaApiClient;
using SCLoaderStorage.Mega.ApiClient;

namespace SCLoaderStorage.Mega.Logic
{
    class TrackListStorage
    {

        private const string fileName = "SCLoaderTrackList.json";

        private INode directoryNode;

        private MegaClient megaClient;


        internal TrackListStorage(string targetPath, MegaClient megaClient)
        {

            this.megaClient = megaClient;
            this.directoryNode = megaClient.GetOrAddDirectoryNode(targetPath);

        }


        internal StorageTrackList GetTrackList()
        {

            // Returns an empty string if the file does not exist yet
            var json = this.megaClient.GetFileContent(this.directoryNode, TrackListStorage.fileName);
            return JsonConvert.DeserializeObject<StorageTrackList>(json);

        }

        internal void UpdateTrackList(StorageTrackList trackList)
        {

            var json = JsonConvert.SerializeObject(trackList, Formatting.Indented);
            this.megaClient.SaveFileContent(this.directoryNode, TrackListStorage.fileName, json);

        }

    }
}
