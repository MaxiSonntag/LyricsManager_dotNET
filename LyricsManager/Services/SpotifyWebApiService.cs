using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;

namespace LyricsManager.Services
{
    class SpotifyWebApiService
    {
        private SpotifyWebAPI _spotifyWebApi;
        public string SearchedArtist { get; set; }
        public string SearchedSong { get; set; }
        

        public SpotifyWebApiService(string artist, string song)
        {
            SearchedArtist = artist;
            SearchedSong = song;
        }

        public async void Authenticate()
        {
            WebAPIFactory webApiFactory = new WebAPIFactory(
                "http://localhost",
                8000,
                Constants.SpotifyApiKey,
                Scope.UserReadPrivate | Scope.UserReadEmail | Scope.PlaylistReadPrivate | Scope.UserLibraryRead |
                Scope.UserReadPrivate | Scope.UserFollowRead | Scope.UserReadBirthdate | Scope.UserTopRead | Scope.PlaylistReadCollaborative |
                Scope.UserReadRecentlyPlayed | Scope.UserReadPlaybackState | Scope.UserModifyPlaybackState);

            try
            {
                _spotifyWebApi = await webApiFactory.GetWebApi();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (_spotifyWebApi == null)
            {
                return;
            }
            

        }

        public string GetAccessToken()
        {
            if (_spotifyWebApi != null)
            {
                return _spotifyWebApi.AccessToken;
            }
            return "";
        }

        public bool IsConnected()
        {
            if (_spotifyWebApi != null)
            {
                return true;
            }
            return false;
        }

        public string SearchSong()
        {
            var query = SearchedArtist + " " + SearchedSong;
            string response = "";
            try
            {
                response = DownloadService.DownloadSpotifySearchResults(query, _spotifyWebApi.AccessToken).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (string.IsNullOrEmpty(response))
            {
                return "";
            }

            var trackUrl = CreatePlayableUrlFromJson(response);

            return trackUrl;
        }

        private string CreatePlayableUrlFromJson(string json)
        {
            var url = ExtractUrl(json);

            if (string.IsNullOrEmpty(url))
            {
                return "";
            }

            var idx = url.LastIndexOf('/')+1;
            var songId = url.Substring(idx);
            var playableUrl = "https://open.spotify.com/track/" + songId;
            return playableUrl;
        }

        private string ExtractUrl(string jsonResponse)
        {
            var parsed = JObject.Parse(jsonResponse);
            var itemsCnt = parsed.SelectToken("tracks.items").Children().Count();
            var url = "";

            for (int i = 0; i < itemsCnt; i++)
            {
                var name = parsed.SelectToken("tracks.items[" + i + "].name").Value<string>();
                if (name.ToLower().Contains(SearchedSong.ToLower()))
                {
                    url = parsed.SelectToken("tracks.items[" + i + "].href").Value<string>();
                    break;
                }
            }
            
            return url;

        }

    }
}
