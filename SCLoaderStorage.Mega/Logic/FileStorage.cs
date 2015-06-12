using CG.Web.MegaApiClient;
using SCLoaderShared.DataClasses;
using SCLoaderShared.Helpers;
using SCLoaderStorage.Mega.ApiClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCLoaderStorage.Mega.Logic
{
    class FileStorage
    {

        private INode directoryNode;

        private MegaClient megaClient;

        internal FileStorage(string targetPath, MegaClient megaClient)
        {

            this.megaClient = megaClient;
            this.directoryNode = megaClient.GetOrAddDirectoryNode(targetPath);

        }


        internal void SaveMp3(FileStream mp3File, Track trackInfo)
        {

            this.megaClient.SaveFileStream(this.directoryNode, trackInfo.Mp3FileName, mp3File);

        }

        internal void SaveCover(FileStream jpegFile, Track trackInfo)
        {

            this.megaClient.SaveFileStream(this.directoryNode, trackInfo.CoverFileName, jpegFile);

        }

    }
}
