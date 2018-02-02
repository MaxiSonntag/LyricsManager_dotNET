using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using LyricsManager.MVVM;
using LyricsManager.Services;

namespace LyricsManager.ViewModels
{
    /// <summary>
    ///     Controller für die Verbindung der Spotify-Dienste (Web und Local)
    /// </summary>
    internal class SpotifyController : ViewModelBase
    {
        private readonly SpotifyLocalApiService _spotifyLocalApiService;
        private readonly SpotifyWebApiService _spotifyWebApiService;
        
        public string SearchedArtist { private get; set; }
        public string SearchedSong { private get; set; }

        private bool _isLocalConnected;
        private bool _isWebConnected;

        public bool IsLocalConnected
        {
            get => _isLocalConnected;
            set => Set(ref _isLocalConnected, value);
        }

        public bool IsWebConnected
        {
            get => _isWebConnected;
            set => Set(ref _isWebConnected, value);
        }

        public SpotifyController()
        {
            _spotifyLocalApiService = new SpotifyLocalApiService();
            _spotifyWebApiService = new SpotifyWebApiService("", "");
            IsLocalConnected = false;
            IsWebConnected = false;
        }

        /// <summary>
        ///     Verbindet bzw. Authentifiziert den Nutzer mit Spotify (Web)
        /// </summary>
        /// <returns>Information ob Verbindung erfolgreich</returns>
        public bool ConnectWebApi()
        {
            Task.Run(() => _spotifyWebApiService.Authenticate()).Wait();
            return IsWebConnected = _spotifyWebApiService.IsConnected();
        }

        /// <summary>
        ///     Verbindet die lokale Spotify-Installation
        /// </summary>
        /// <returns>Information ob Verbindung erfolgreich</returns>
        public bool ConnectLocalApi()
        {
            return IsLocalConnected = _spotifyLocalApiService.Connect();
        }

        /// <summary>
        ///     Sucht ein Lied auf Spotify und spielt dieses ab
        /// </summary>
        public void SearchAndPlay()
        {
            if (!IsWebConnected)
            {
                Console.WriteLine("SpotifyWebApi not connected");
                return;
            }

            _spotifyWebApiService.SearchedArtist = SearchedArtist;
            _spotifyWebApiService.SearchedSong = SearchedSong;

            var playableUrl = _spotifyWebApiService.SearchSong();
            if (string.IsNullOrEmpty(playableUrl))
            {
                Console.WriteLine("URL NOT PLAYABLE");
                return;
            }
            if (IsLocalConnected)
            {
                _spotifyLocalApiService.PlaySongByUrl(playableUrl);
            }
            else
            {
                Process.Start(new ProcessStartInfo(playableUrl));
            }
            
        }

        /// <summary>
        ///     Pausiert das aktuell abgespielte Lied
        /// </summary>
        public void PauseLocalSpotify()
        {
            if (IsLocalConnected)
            {
                _spotifyLocalApiService.PauseSong();
            }
        }

        /// <summary>
        ///     Lädt Künstler und Titel zum aktuell abgespielten Lied
        /// </summary>
        /// <returns></returns>
        public List<string> GetInfoForCurrentTrack()
        {
            var infos = Task.Run(_spotifyLocalApiService.GetCurrentTrackInfos).Result;
            return infos;
        }
    }
}
