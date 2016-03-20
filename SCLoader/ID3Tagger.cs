using Id3;
using Id3.Frames;
using SCLoaderShared.DataClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCLoader
{
    class ID3Tagger
    {

        internal static void WriteID3Tag(Track track, string mp3FilePath, string jpgFilePath)
        {

            // Open the MP3 file
            using (var fileStream = File.Open(mp3FilePath, FileMode.Open, FileAccess.ReadWrite))
            {

                // Open file in tag editor
                var m = new Mp3Stream(fileStream, Mp3Permissions.ReadWrite);

                // Clear possible existing tags
                m.DeleteAllTags();

                // Fill the tag
                var tag = new Id3.Id3v2.v23.Id3v23Tag();
                tag.Album.Value = track.Album;
                tag.Artists.Value = track.Artist;
                tag.ArtistUrls.Add(new ArtistUrlFrame() { Url = track.TrackUserUrl });
                tag.AudioFileUrl.Url = track.TrackUrl;
                if (track.BPM > 0) tag.BeatsPerMinute.Value = track.BPM.ToString(CultureInfo.InvariantCulture);
                tag.Comments.Add(new CommentFrame() { Comment = track.Description });
                tag.Genre.Value = track.Genre;
                tag.Title.Value = track.Title;
                tag.Track.Value = track.TrackNumber.ToString();
                tag.Year.Value = track.Year.ToString();

                // Add the local JPEG cover
                if (!string.IsNullOrEmpty(jpgFilePath))
                {
                    using (var jpgStream = File.Open(jpgFilePath, FileMode.Open, FileAccess.Read))
                    {
                        tag.Pictures.Add(new PictureFrame()
                        {
                            PictureType = PictureType.FrontCover,
                            MimeType = "image/jpeg",
                            PictureData = GetByteArrayFromFileStream(jpgStream)
                        });
                    }
                }

                // Save the new tag
                m.WriteTag(tag, 2, 3, WriteConflictAction.Replace);

            }

        }

        private static byte[] GetByteArrayFromFileStream(FileStream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

    }
}
