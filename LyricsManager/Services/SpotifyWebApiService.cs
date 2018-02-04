using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;

namespace LyricsManager.Services
{
    /// <summary>
    ///     Zugriffsklasse auf die Spotify-WebAPI.
    ///     Umfasst Authentifizierung und Suche nach Songs.
    /// </summary>
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

        /// <summary>
        ///     Leitet die Authentifizierung des Nutzers auf der Spotify-Authentifizierungsseite ein
        /// </summary>
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
                Console.WriteLine("Spotify could not connect: "+e.Message);
            }
            

        }
        
        /// <summary>
        ///     Information ob der Nutzer mit der SpotifyWeb-API verbunden ist
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            if (_spotifyWebApi != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Sucht nach verfügbaren Liedern auf Spotify.
        /// </summary>
        /// <returns>URL eines abspielbaren Liedes, das den Suchkriterien entspricht</returns>
        public string SearchSong()
        {
            var query = SearchedArtist + " " + SearchedSong;
            string response;
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

        /// <summary>
        ///     Erstellt eine abspielbare URL eines Songs.
        /// </summary>
        /// <param name="json">Die JSON-Response der Suche</param>
        /// <returns>Die abspielbare URL</returns>
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

        /// <summary>
        ///     Extrahiert die URL eines Suchergebnisses aus JSON.
        /// </summary>
        /// <param name="jsonResponse">Die JSON-Response der Suche</param>
        /// <returns>Eine (nicht abspielbare) URL</returns>
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
