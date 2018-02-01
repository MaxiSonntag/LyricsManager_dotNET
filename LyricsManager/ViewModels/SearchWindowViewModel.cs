using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LyricsManager.Models;
using LyricsManager.MVVM;
using LyricsManager.Services;

namespace LyricsManager.ViewModels
{
    /// <summary>
    ///     View Model des Fensters zum Erstellen eines neuen Songs
    /// </summary>
    internal class SearchWindowViewModel : ViewModelBaseWithValidation
    {
        /// <summary>
        ///     Event Handler, der beim Schließen des Fensters aufgerufen wird
        /// </summary>
        public event EventHandler OnCloseRequest;

        private List<SongViewModel> _resultList;
        private ObservableCollection<SongViewModel> _searchResults;
        private SongViewModel _selectedSong;
        private string _artist;
        private string _song;
        private Song _downloadedSong;
        private string _selectedLyricChecksum;
        private int _selectedLyricId;

        public string EnteredArtist
        {
            get => _artist;
            set
            {
                Set(IsArtistValid, ref _artist, value);
                OnPropertyChanged(nameof(IsSearchEnabled));
            }
        }

        public string EnteredSong
        {
            get => _song;
            set
            {
                Set(IsSongValid, ref _song, value);
                OnPropertyChanged(nameof(IsSearchEnabled));
            }
        }

        

        public ObservableCollection<SongViewModel> SearchResults
        {
            get => _searchResults;
            set => Set(ref _searchResults, value);
        }

        public SongViewModel SelectedSearchViewModel
        {
            get => _selectedSong;
            set
            {
                Set(ref _selectedSong, value);
                OnPropertyChanged(nameof(IsApplyEnabled));
            }
        }

        public DelegateCommand SearchCommand { get; set; }
        public DelegateCommand ApplyCommand { get; set; }
        public DelegateCommand ManualCommand { get; set; }

        public bool IsSearchEnabled => !string.IsNullOrWhiteSpace(EnteredArtist) && !string.IsNullOrWhiteSpace(EnteredSong);
        public bool IsApplyEnabled => SelectedSearchViewModel != null && SelectedSearchViewModel.LyricId != -1;
        

        public SearchWindowViewModel()
        {
            SearchCommand = new DelegateCommand(SearchCommandExecute);
            ApplyCommand = new DelegateCommand(ApplyCommandExecute);
            ManualCommand = new DelegateCommand(ManualCommandExecute);
            _searchResults = new ObservableCollection<SongViewModel>();
            _resultList = new List<SongViewModel>();
            _downloadedSong = new Song();
            _selectedLyricChecksum = "";
            _selectedLyricId = 0;
        }

        private void SearchCommandExecute(object obj)
        {
            if (SearchResults.Count != 0)
            {
                SearchResults = new ObservableCollection<SongViewModel>();
                _resultList = new List<SongViewModel>();
            }
            Task.Run(SearchSongsAsync);
        }
        

        private void ApplyCommandExecute(object obj)
        {
            _selectedLyricId = SelectedSearchViewModel.LyricId;
            _selectedLyricChecksum = SelectedSearchViewModel.LyricChecksum;
            Task.Run(DownloadSongAsync).Wait();
            Task.Run(SaveDownloadedSong).Wait();
            OnCloseRequest?.Invoke(this, EventArgs.Empty);
        }
        

        private void ManualCommandExecute(object obj)
        {
            var resultSong = new Song();

            if (_artist != "")
            {
                resultSong.LyricArtist = _artist;
            }
            if (_song != "")
            {
                resultSong.LyricSong = _song;
            }

            resultSong.ImageUri = "/Images/PlaceholderPicture.png";
            _downloadedSong = resultSong;
            Task.Run(SaveDownloadedSong).Wait();
            OnCloseRequest?.Invoke(this, EventArgs.Empty);
        }

        private async Task SearchSongsAsync()
        {
            List<Song> list = await DownloadService.DownloadSearchResultsAsync(_artist, _song);
            if (list.Count == 0 || list.Count == 1)
            {
                _resultList.Add(new SongViewModel
                {
                    LyricArtist = "Please try another query",
                    LyricSong = "No results found",
                    LyricId = -1
                });
                
            }
            else
            {
                list.ForEach(s => _resultList.Add(new SongViewModel(s)));
            }
            SearchResults = new ObservableCollection<SongViewModel>(_resultList);
            
        }

        private async Task DownloadSongAsync()
        {
            var song = await DownloadService.DownloadSongByIdAsync(_selectedLyricId, _selectedLyricChecksum);
            _downloadedSong = song;
            
        }

        private async Task SaveDownloadedSong()
        {
            List<Song> savedSongs = await PersistencyService.LoadLyricsAsync();
            savedSongs.Add(_downloadedSong);
            await PersistencyService.SaveLyricsAsync(savedSongs);
        }

        private bool IsArtistValid(string artist, [CallerMemberName] string propertyName = null)
        {
            string error = "Please enter an Artist.";
            return SetError(() => !string.IsNullOrWhiteSpace(artist), propertyName, error);
        }

        private bool IsSongValid(string song, [CallerMemberName] string propertyName = null)
        {
            string error = "Please enter a Song.";
            return SetError(() => !string.IsNullOrWhiteSpace(song), propertyName, error);
        }
        

    }
}
