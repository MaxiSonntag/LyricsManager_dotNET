using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private List<SongViewModel> _resultListLyric;
        private ObservableCollection<SongViewModel> _searchResultsLyric;
        private SongViewModel _selectedSong;
        private SongViewModel _selectedSongLyric;
        private string _artist;
        private string _song;
        private string _lyric;
        private Song _downloadedSong;

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

        public string EnteredLyric
        {
            get => _lyric;
            set
            {
                Set(IsSongValid, ref _lyric, value);
                OnPropertyChanged(nameof(IsSearchByLyricEnabled));
            }
        }


        public ObservableCollection<SongViewModel> SearchResults
        {
            get => new ObservableCollection<SongViewModel>(_searchResults.OrderBy(s=>s.LyricSong));
            set => Set(ref _searchResults, value);
        }

        public ObservableCollection<SongViewModel> SearchResultsLyric
        {
            get => new ObservableCollection<SongViewModel>(_searchResultsLyric.OrderBy(s=>s.LyricSong));
            set => Set(ref _searchResultsLyric, value);
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

        public SongViewModel SelectedSearchLyricViewModel
        {
            get => _selectedSongLyric;
            set
            {
                Set(ref _selectedSongLyric, value);
                OnPropertyChanged(nameof(IsApplyLyricEnabled));
            }
        }

        /// <summary>
        ///     Command zum Suchen von verfügbaren Lyrics
        /// </summary>
        public DelegateCommand SearchCommand { get; set; }
        /// <summary>
        ///     Command zum Downloaden der Lyrics des selektierten Songs und Rückkehr zum MainWindow
        /// </summary>
        public DelegateCommand ApplyCommand { get; set; }
        /// <summary>
        ///     Command zum Erstellen eines neuen Songs ohne Lyrics herunterzuladen
        /// </summary>
        public DelegateCommand ManualCommand { get; set; }
        /// <summary>
        ///     Command zum Suchen von verfügbaren Lyrics auf Basis der eingegebenen Lyric
        /// </summary>
        public DelegateCommand SearchByLyricCommand { get; set; }

        /// <summary>
        ///     Information ob sowohl Künstler als auch Titel nicht leer sind -> Aktiviert Möglichkeit zur Suche nach verfügbaren Lyrics
        /// </summary>
        public bool IsSearchEnabled => !string.IsNullOrWhiteSpace(EnteredArtist) && !string.IsNullOrWhiteSpace(EnteredSong);
        /// <summary>
        ///     Information ob ein valides Sucherergebnis selektiert ist
        /// </summary>
        public bool IsApplyEnabled => SelectedSearchViewModel != null && SelectedSearchViewModel.LyricId != -1;

        public bool IsApplyLyricEnabled =>
            SelectedSearchLyricViewModel != null && SelectedSearchLyricViewModel.LyricId != -1;
        /// <summary>
        ///     Information ob Lyric nicht leer ist -> Aktiviert Möglichkeit zur Suche nach verfügbaren Lyrics
        /// </summary>
        public bool IsSearchByLyricEnabled => !string.IsNullOrWhiteSpace(EnteredLyric);

        public bool IsLyricTabEnabled { get; set; }

        public SearchWindowViewModel()
        {
            SearchCommand = new DelegateCommand(SearchCommandExecute);
            SearchByLyricCommand = new DelegateCommand(SearchByLyricCommandExecute);
            ApplyCommand = new DelegateCommand(ApplyCommandExecute);
            ManualCommand = new DelegateCommand(ManualCommandExecute);
            _searchResults = new ObservableCollection<SongViewModel>();
            _searchResultsLyric = new ObservableCollection<SongViewModel>();
            _resultList = new List<SongViewModel>();
            _resultListLyric = new List<SongViewModel>();
            _downloadedSong = new Song();
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

        private void SearchByLyricCommandExecute(object obj)
        {
            if (SearchResults.Count != 0)
            {
                SearchResultsLyric = new ObservableCollection<SongViewModel>();
                _resultListLyric = new List<SongViewModel>();
            }
            Task.Run(SearchSongByLyricAsync);
        }

        /// <summary>
        ///     Lädt Lyrics zum selektierten Suchergebnis herunter, speichert es und kehrt zum MainWindow zurück
        /// </summary>
        private void ApplyCommandExecute(object obj)
        {
            Task.Run(DownloadSongAsync).Wait();
            Task.Run(SaveDownloadedSong).Wait();
            OnCloseRequest?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        ///     Übernimmt eingegebenen Künstler und/oder Titel, speichert das Objekt und kehrt zum MainWindow zurück
        /// </summary>
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

        /// <summary>
        ///     Sucht nach verfügbaren Lyrics
        /// </summary>
        /// <returns></returns>
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

        private async Task SearchSongByLyricAsync()
        {
            List<Song> list = await DownloadService.DownloadSearchResultsForLyricAsync(_lyric);
            if (list.Count == 0 || list.Count == 1)
            {
                _resultListLyric.Add(new SongViewModel
                {
                    LyricArtist = "Please try another query",
                    LyricSong = "No results found",
                    LyricId = -1
                });

            }
            else
            {
                list.ForEach(s => _resultListLyric.Add(new SongViewModel(s)));
            }
            SearchResultsLyric = new ObservableCollection<SongViewModel>(_resultListLyric);
        }

        /// <summary>
        ///     Lädt Lyrics zum selektierten Suchergebnis herunter
        /// </summary>
        /// <returns></returns>
        private async Task DownloadSongAsync()
        {
            if (!IsLyricTabEnabled)
            {
                var song = await DownloadService.DownloadSongByIdAsync(SelectedSearchViewModel);
                _downloadedSong = song;
            }
            else
            {
                var song = await DownloadService.DownloadSongByIdAsync(SelectedSearchLyricViewModel);
                _downloadedSong = song;
            }
            
        }

        /// <summary>
        ///     Speichert den neu erstellten Song
        /// </summary>
        /// <returns></returns>
        private async Task SaveDownloadedSong()
        {
            List<Song> savedSongs = await PersistencyService.LoadLyricsAsync();
            savedSongs.Add(_downloadedSong);
            await PersistencyService.SaveLyricsAsync(savedSongs);
        }

        /// <summary>
        ///     Überprüft ob ein Künstler eingegeben wurde
        /// </summary>
        /// <param name="artist">Eingegebener Künstler</param>
        /// <returns>Information ob die Eingabe valide ist (nicht unzugewiesen oder leer)</returns>
        private bool IsArtistValid(string artist, [CallerMemberName] string propertyName = null)
        {
            string error = "Please enter an Artist.";
            return SetError(() => !string.IsNullOrWhiteSpace(artist), propertyName, error);
        }

        /// <summary>
        ///     Überprüft ob ein Titel eingegeben wurde
        /// </summary>
        /// <param name="song">Eingegebener Titel</param>
        /// <returns>Information ob die Eingabe valide ist (nicht unzugewiesen oder leer)</returns>
        private bool IsSongValid(string song, [CallerMemberName] string propertyName = null)
        {
            string error = "Please enter a Song.";
            return SetError(() => !string.IsNullOrWhiteSpace(song), propertyName, error);
        }
        

    }
}
