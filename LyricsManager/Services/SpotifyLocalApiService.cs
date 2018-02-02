using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyAPI.Local;

namespace LyricsManager.Services
{
    /// <summary>
    ///     Zugriffsklasse für die Spotify-LocalAPI
    /// </summary>
    class SpotifyLocalApiService
    {

        private readonly SpotifyLocalAPI _spotifyLocalApi;

        /// <summary>
        ///     Initialisiert eine Instanz der Spotify-LocalAPI
        /// </summary>
        public SpotifyLocalApiService()
        {
            _spotifyLocalApi = new SpotifyLocalAPI();
        }

        /// <summary>
        ///     Baut eine Verbindung zur lokalen Spotify-Installation auf
        /// </summary>
        /// <returns>Information ob der Verbindungsaufbau erfolgreich war</returns>
        public bool Connect()
        {
            if (!SpotifyLocalAPI.IsSpotifyRunning())
            {
                return false;
            }

            bool successful = _spotifyLocalApi.Connect();
            

            return successful;
        }

        /// <summary>
        ///     Spielt einen Song von Anfang an in Spotify ab
        /// </summary>
        /// <param name="url">Abspielbare URL</param>
        public async void PlaySongByUrl(string url)
        {
            await _spotifyLocalApi.PlayURL(url);
        }

        /// <summary>
        ///     Pausiert den aktuell abgespielten Song
        /// </summary>
        public async void PauseSong()
        {
            await _spotifyLocalApi.Pause();
        }

        /// <summary>
        ///     Ruft Titel und Künstler des aktuell spielenden Songs ab
        /// </summary>
        /// <returns>Liste aus Strings (0 - Künstler; 1 - Titel</returns>
        public async Task<List<string>> GetCurrentTrackInfos()
        {
            await Task.Delay(0);
            var result = new List<string>();
            try
            {
                var artist = _spotifyLocalApi.GetStatus().Track.ArtistResource.Name;
                var track = _spotifyLocalApi.GetStatus().Track.TrackResource.Name;
                result = new List<string> {artist, track};
            }
            catch (Exception e)
            {
                Console.WriteLine("Current Track could not be loaded\n"+e.Message);
            }
            
            return result;
        }

    }
}
