﻿using SCLoaderShared;
using SCLoaderShared.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCLoaderStorage.Local.Logic
{
    class FileHelper
    {

        private string targetFolder = "";

        internal FileHelper(string targetFolder)
        {

            this.targetFolder = targetFolder;

        }


        internal void SaveMp3(FileStream mp3File, ITrack trackInfo)
        {

            var fileName = PathHelpers.GetWorkingCombinedPath(this.targetFolder, trackInfo.Mp3FileName);

            using (var fileStream = File.Create(fileName))
            {
                mp3File.CopyTo(fileStream);
            }

        }

        internal void SaveCover(FileStream jpegFile, ITrack trackInfo)
        {

            var fileName = PathHelpers.GetWorkingCombinedPath(this.targetFolder, trackInfo.CoverFileName);

            using (var fileStream = File.Create(fileName))
            {
                jpegFile.CopyTo(fileStream);
            }

        }

    }
}
