﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LyricsManager.Models;
using LyricsManager.MVVM;
using LyricsManager.Services;
using MahApps.Metro.Controls.Dialogs;

namespace LyricsManager.ViewModels
{
    /// <summary>
    ///     View Model des Hauptfensters
    /// </summary>
    internal class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        ///     Beinhaltet alle Songs
        /// </summary>
        private List<SongViewModel> _allSongs;

        /// <summary>
        ///     Beinhaltet die Songs, die aktuell angezeigt werden
        /// </summary>
        private ObservableCollection<SongViewModel> _songs;
        /// <summary>
        ///     Aktuell ausgewählter Song
        /// </summary>
        private SongViewModel _selectedSong;
        /// <summary>
        ///     Text im Suchfeld
        /// </summary>
        private string _filterText;
        /// <summary>
        ///     Information über den Authentifizierungsstatus von Spotify
        /// </summary>
        private bool _isSpotifyAuthorized;
        /// <summary>
        ///     Controller für die Verbindung mit Spotify (Web und Local)
        /// </summary>
        public SpotifyController SpotifyController { get; }
        

        public ObservableCollection<SongViewModel> Songs
        {
            get => _songs;
            set => Set(ref _songs, new ObservableCollection<SongViewModel>(value.OrderBy(s=>s.LyricSong)));
        }

        public SongViewModel SelectedSong
        {
            get => _selectedSong;
            set
            {
                if (value != null)
                {
                    Set(ref _selectedSong, value);
                }
                OnPropertyChanged(nameof(IsSelectionValid));
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                Set(ref _filterText, value);
                Filter();
            }
        }

        /// <summary>
        ///     Command zum Löschen eines Songs
        /// </summary>
        public DelegateCommand DeleteCommand { get; set; }
        /// <summary>
        ///     Command zum Erstellen eines neuen Songs
        /// </summary>
        public DelegateCommand NewCommand { get; set; }
        /// <summary>
        ///     Command zum Speichern der aktuellen Songliste
        /// </summary>
        public DelegateCommand SaveCommand { get; set; }
        /// <summary>
        ///     Command zum Bearbeiten des selektierten Songs
        /// </summary>
        public DelegateCommand EditCommand { get; set; }
        /// <summary>
        ///     Command zum Verbinden mit der lokalen Spotify Installation
        /// </summary>
        public DelegateCommand ConnectLocalSpotifyCommand { get; set; }
        /// <summary>
        ///     Command zum Verbinden und Authentifizieren mit Spotify (Web)
        /// </summary>
        public DelegateCommand ConnectWebSpotifyCommand { get; set; }
        /// <summary>
        ///     Command zum Suchen und abspielen eines Songs in Spotify
        /// </summary>
        public DelegateCommand SearchAndPlaySpotifyCommand { get; set; }
        /// <summary>
        ///     Command zum Pausieren des aktuell abgespielten Songs in Spotify
        /// </summary>
        public DelegateCommand PauseSpotifyCommand { get; set; }
        /// <summary>
        ///     Command zum Erstellen eines neuen Songs auf Basis des aktuell in Spotify abgespielten Songs
        /// </summary>
        public DelegateCommand LyricsForCurrentTrackCommand { get; set; }
        /// <summary>
        ///     Command zum Anzeigen der Suchergebnisse zum selektieren Song auf YouTube
        /// </summary>
        public DelegateCommand ShowYoutubeCommand { get; set; }

        /// <summary>
        ///     Information, ob ein Song selektiert ist
        /// </summary>
        public bool IsSelectionValid => SelectedSong != null;

        

        public MainWindowViewModel()
        {
            Task.Run(LoadDataAsync).Wait();
            DeleteCommand = new DelegateCommand(DeleteCommandExecute);
            NewCommand = new DelegateCommand(NewCommandExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            EditCommand = new DelegateCommand(EditCommandExecute);
            ConnectWebSpotifyCommand = new DelegateCommand(ConnectWebSpotifyCommandExecute);
            ConnectLocalSpotifyCommand = new DelegateCommand(ConnectLocalSpotifyCommandExecute);
            SearchAndPlaySpotifyCommand = new DelegateCommand(SearchAndPlaySpotifyCommandExecute);
            PauseSpotifyCommand = new DelegateCommand(StopSpotifyCommandExecute);
            LyricsForCurrentTrackCommand = new DelegateCommand(LyricsForCurrentTrackCommandExecute);
            ShowYoutubeCommand = new DelegateCommand(ShowYoutubeCommandExecute);
            SpotifyController = new SpotifyController();
            _isSpotifyAuthorized = false;

            if (Songs != null && Songs.Count > 0)
            {
                SelectedSong = Songs[0];
            }
        }
        

        /// <summary>
        ///     Lädt gespeicherte Daten aus dem Dateisystem in das View Model
        /// </summary>
        /// <returns></returns>
        private async Task LoadDataAsync()
        {
            
            _allSongs = new List<SongViewModel>();
            var songs = await PersistencyService.LoadLyricsAsync();
            songs.ToList().ForEach(s => _allSongs.Add(new SongViewModel(s)));
            Songs = new ObservableCollection<SongViewModel>(_allSongs);
                
        }

        /// <summary>
        ///     Command zum Speichern der Songs auf das Dateisystem
        /// </summary>
        private async void SaveCommandExecute(object obj)
        {
            await PersistencyService.SaveLyricsAsync(_allSongs.Select(vm => new Song
            {
                LyricArtist = vm.LyricArtist,
                LyricSong = vm.LyricSong,
                LyricChecksum = vm.LyricChecksum,
                LyricRank = vm.LyricRank,
                LyricId = vm.LyricId,
                LyricUrl = vm.LyricUrl,
                Lyric = vm.Lyric,
                ImageUri = vm.ImageUri
            }));
        }

        /// <summary>
        ///     Command zum Erstellen eines neuen Songs
        /// </summary>
        private void NewCommandExecute(object obj)
        {
            
            SearchWindowViewModel vm = new SearchWindowViewModel();
            SearchWindow searchWindow = new SearchWindow
            {
                DataContext = vm
            };
            vm.OnCloseRequest += (s, e) => HandleDialogWindowClose(searchWindow);
            searchWindow.ShowDialog();
        }

        /// <summary>
        ///     Command zum Löschen eines Songs
        /// </summary>
        private void DeleteCommandExecute(object obj)
        {
            if (SelectedSong == null) return;
            
            _allSongs.Remove(SelectedSong);
            Songs.Remove(SelectedSong);
            SelectedSong = Songs[0];
            SaveCommandExecute(null);
        }

        /// <summary>
        ///     Command zum Bearbeiten eines Songs
        /// </summary>
        private void EditCommandExecute(object obj)
        {
            EditWindowViewModel vm = new EditWindowViewModel(SelectedSong);
            EditWindow editWindow = new EditWindow
            {
                DataContext = vm
            };
            vm.OnCloseRequest += (s, e) => HandleDialogWindowClose(editWindow);
            var idx = _allSongs.IndexOf(SelectedSong);
            vm.Index = idx;
            editWindow.ShowDialog();
        }

        /// <summary>
        ///     Command zum Suchen und Abspielen eines Liedes auf Spotify
        /// </summary>
        private void SearchAndPlaySpotifyCommandExecute(object obj)
        {
            if (SpotifyController.IsWebConnected)
            {
                SpotifyController.SearchedArtist = SelectedSong.LyricArtist;
                SpotifyController.SearchedSong = SelectedSong.LyricSong;

                SpotifyController.SearchAndPlay();
            }
            
        }

        /// <summary>
        ///     Command zum Stoppen eines aktuell spielenden Liedes auf Spotify
        /// </summary>
        private void StopSpotifyCommandExecute(object obj)
        {
            if (SpotifyController.IsLocalConnected)
            {
                SpotifyController.PauseLocalSpotify();
            }
        }

        /// <summary>
        ///     Command zum Laden von Lyrics eines aktuell spielenden Liedes auf Spotify
        /// </summary>
        private void LyricsForCurrentTrackCommandExecute(object obj)
        {
            var infos = SpotifyController.GetInfoForCurrentTrack();

            if (infos.Count == 0)
            {
                if (!SpotifyController.IsWebConnected)
                {
                    ShowErrorFlyout("You haven't authorized :(", "You have to connect your Spotify account first to use this feature");
                }else if (!SpotifyController.IsLocalConnected)
                {
                    ShowErrorFlyout("Spotify is not running :o",
                        "Please start your local Spotify installation to use this feature");
                }
                else
                {
                    ShowErrorFlyout("Oops, something went wrong...",
                        "Sorry, but that's all we know");
                }
                return;
            }
            SearchWindowViewModel vm = new SearchWindowViewModel();
            vm.EnteredArtist = infos[0];
            vm.EnteredSong = infos[1];
            SearchWindow searchWindow = new SearchWindow
            {
                DataContext = vm
            };
            vm.OnCloseRequest += (s, e) => HandleDialogWindowClose(searchWindow);
            searchWindow.ShowDialog();

        }

        /// <summary>
        ///     Command zum Authentifizieren und Verbinden von Spotify (Web)
        /// </summary>
        private void ConnectWebSpotifyCommandExecute(object obj)
        {
            _isSpotifyAuthorized = SpotifyController.ConnectWebApi();
        }

        /// <summary>
        ///     Command zum Verbinden der lokalen Spotify-Installation
        /// </summary>
        private void ConnectLocalSpotifyCommandExecute(object obj)
        {
            if (!_isSpotifyAuthorized)
            {
                ShowErrorFlyout("You haven't authorized :(",
                    "You have to connect your Spotify account first to use this feature");
                return;
            }
            var isConnected = SpotifyController.ConnectLocalApi();
            if (!isConnected)
            {
                ShowErrorFlyout("Spotify is not running :o",
                    "Please start your local Spotify installation to use this feature");
            }
        }

        /// <summary>
        ///     Command zum Anzeigen der YouTube-Suchergebnisse
        /// </summary>
        private void ShowYoutubeCommandExecute(object obj)
        {
            var vm = new YoutubeWindowViewModel(SelectedSong.LyricArtist, SelectedSong.LyricSong);
            var youtubeWindow = new YoutubeWindow
            {
                DataContext = vm
            };
            youtubeWindow.ShowDialog();
        }

        /// <summary>
        ///     Filtert die angezeigten Songs auf Basis des Suchtextes
        /// </summary>
        private void Filter()
        {
            Songs = string.IsNullOrWhiteSpace(FilterText)
                ? new ObservableCollection<SongViewModel>(_allSongs)
                : new ObservableCollection<SongViewModel>(_allSongs.Where(s => s.LyricSong?.IndexOf(FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0
                                                                                     || s.LyricArtist?.IndexOf(FilterText, StringComparison.CurrentCultureIgnoreCase) >=
                                                                                     0));
        }

        /// <summary>
        ///     Event Handler beim Schließen eines Fensters
        /// </summary>
        /// <param name="window">Das Fenster, das geschlossen wird</param>
        private void HandleDialogWindowClose(Window window)
        {
            
            window.Close();
            Task.Run(LoadDataAsync).Wait();
            if (window.GetType() == typeof(SearchWindow))
            {
                SelectedSong = Songs[Songs.Count - 1];
            }
        }

        /// <summary>
        ///     Öffnet ein Overlay (Flyout) mit Errorhandling-Informationen
        /// </summary>
        /// <param name="title">Der Titel des Flyouts</param>
        /// <param name="message">Die Errorhandling-Nachricht des Flyouts</param>
        private void ShowErrorFlyout(string title, string message)
        {
            var settings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Okay",
                AnimateShow = true,
                AnimateHide = true
            };
            var window = (MainWindow)Application.Current.MainWindow;
            window.ShowMessageAsync(title,
                message,
                MessageDialogStyle.Affirmative, settings);
        }

    }
}
