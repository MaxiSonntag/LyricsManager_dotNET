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

        public async void PlaySongByUrl(string url)
        {
            await _spotifyLocalApi.PlayURL(url);
        }

        public async void PauseSong()
        {
            await _spotifyLocalApi.Pause();
        }

    }
}
