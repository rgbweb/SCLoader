using SCLoaderShared.Interfaces;
using SoundCloud.API.Client;
using SoundCloud.API.Client.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCLoader.SoundCloud
{
    class SoundCloudClient
    {


        private static ISoundCloudClient scClient = null;

        internal SoundCloudClient(string clientID, string clientSecret, string userName, string userPass)
        {

            var scConnector = new SoundCloudConnector();
            scClient = scConnector.DirectConnect(clientID, clientSecret, userName, userPass);

        }

        internal ICollection<SCTrack> GetAllFavorites()
        {

            var result = new List<SCTrack>();

            var favCount = scClient.Me.GetUser().FavoriteCount;

            // We get max 50 tracks per request            
            int favBlocks = favCount / 50;
            for (var i = 0; i < favBlocks; i++)
            {

                // Get the track block
                var favs = scClient.Me.GetFavorites(i * 50, 50);

                // Ad to result list
                result.AddRange(favs);

            }

            // SC sorts the tracks -> newest to oldest
            // We want to start with the oldest
            result.Reverse();

            return result;

        }

        internal SCTrack ResolveTrack(string soundCloudUrl)
        {

            return scClient.Resolve.GetTrack(soundCloudUrl);

        }


    }
}
