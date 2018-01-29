using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LyricsManager.Models;
using LyricsManager.MVVM;
using LyricsManager.Services;

namespace LyricsManager.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private List<SongViewModel> _allSongs;
        private ObservableCollection<SongViewModel> _songs;
        private SongViewModel _selectedSong;
        private string _filterText;

        public SpotifyViewModel SpotifyViewModel { get; }
        

        public ObservableCollection<SongViewModel> Songs
        {
            get => _songs;
            set => Set(ref _songs, value);
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

        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand NewCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand ConnectLocalSpotifyCommand { get; set; }
        public DelegateCommand ConnectWebSpotifyCommand { get; set; }
        public DelegateCommand SearchAndPlaySpotifyCommand { get; set; }
        public DelegateCommand PauseSpotifyCommand { get; set; }
        public DelegateCommand ShowYoutubeCommand { get; set; }

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
            PauseSpotifyCommand = new DelegateCommand(PauseSpotifyCommandExecute);
            ShowYoutubeCommand = new DelegateCommand(ShowYoutubeCommandExecute);
            SpotifyViewModel = new SpotifyViewModel();

            if (Songs != null && Songs.Count > 0)
            {
                SelectedSong = Songs[0];
            }
        }

        


        private async Task LoadDataAsync()
        {
            
            _allSongs = new List<SongViewModel>();
            var songs = await PersistencyService.LoadLyricsAsync();
            songs.ToList().ForEach(s => _allSongs.Add(new SongViewModel(s)));
            Songs = new ObservableCollection<SongViewModel>(_allSongs);
                
        }

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

        private void DeleteCommandExecute(object obj)
        {
            if (SelectedSong == null) return;
            
            _allSongs.Remove(SelectedSong);
            Songs.Remove(SelectedSong);
        }

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

        private void SearchAndPlaySpotifyCommandExecute(object obj)
        {
            if (SpotifyViewModel.IsWebConnected)
            {
                SpotifyViewModel.SearchedArtist = SelectedSong.LyricArtist;
                SpotifyViewModel.SearchedSong = SelectedSong.LyricSong;

                SpotifyViewModel.SearchAndPlay();
            }
            
        }

        private void PauseSpotifyCommandExecute(object obj)
        {
            if (SpotifyViewModel.IsLocalConnected)
            {
                SpotifyViewModel.PauseLocalSpotify();
            }
        }

        private void ConnectWebSpotifyCommandExecute(object obj)
        {
            SpotifyViewModel.ConnectWebApi();
        }

        private void ConnectLocalSpotifyCommandExecute(object obj)
        {
            SpotifyViewModel.ConnectLocalApi();
        }

        private void ShowYoutubeCommandExecute(object obj)
        {
            var vm = new YoutubeWindowViewModel(SelectedSong.LyricArtist, SelectedSong.LyricSong);
            var youtubeWindow = new YoutubeWindow
            {
                DataContext = vm
            };
            youtubeWindow.ShowDialog();
        }

        private void Filter()
        {
            Songs = string.IsNullOrWhiteSpace(FilterText)
                ? new ObservableCollection<SongViewModel>(_allSongs)
                : new ObservableCollection<SongViewModel>(_allSongs.Where(s => s.LyricSong?.IndexOf(FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0
                                                                                     || s.LyricArtist?.IndexOf(FilterText, StringComparison.CurrentCultureIgnoreCase) >=
                                                                                     0));
        }

        private void HandleDialogWindowClose(Window window)
        {
            
            window.Close();
            Task.Run(LoadDataAsync).Wait();
            if (window.GetType() == typeof(SearchWindow))
            {
                SelectedSong = Songs[Songs.Count - 1];
            }
        }

    }
}
