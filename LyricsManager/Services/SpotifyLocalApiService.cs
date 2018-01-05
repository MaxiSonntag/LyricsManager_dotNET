using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SpotifyAPI.Local;

namespace LyricsManager.Services
{
    class SpotifyLocalApiService
    {

        private readonly SpotifyLocalAPI _spotifyLocalApi;

        public SpotifyLocalApiService()
        {
            _spotifyLocalApi = new SpotifyLocalAPI();
        }

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
