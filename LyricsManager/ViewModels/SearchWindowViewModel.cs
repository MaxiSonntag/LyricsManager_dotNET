using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyricsManager.Models;
using LyricsManager.MVVM;
using LyricsManager.Services;

namespace LyricsManager.ViewModels
{
    class SearchWindowViewModel : ViewModelBase
    {
        public event EventHandler OnCloseRequest;

        private List<SongViewModel> _resultList;
        private ObservableCollection<SongViewModel> _searchResults;
        private SongViewModel _selectedSong;

        public string EnteredArtist { get; set; }
        public string EnteredSong { get; set; }

        private Song downloadedSong;
        private string selectedLyricChecksum;
        private int selectedLyricId;

        public ObservableCollection<SongViewModel> SearchResults
        {
            get => _searchResults;
            set => Set(ref _searchResults, value);
        }

        public SongViewModel SelectedSearchViewModel
        {
            get => _selectedSong;
            set => Set(ref _selectedSong, value);
        }

        public DelegateCommand SearchCommand { get; set; }
        public DelegateCommand ApplyCommand { get; set; }
        public DelegateCommand ManualCommand { get; set; }

        public SearchWindowViewModel()
        {
            SearchCommand = new DelegateCommand(SearchCommandExecute);
            ApplyCommand = new DelegateCommand(ApplyCommandExecute);
            ManualCommand = new DelegateCommand(ManualCommandExecute);
            _searchResults = new ObservableCollection<SongViewModel>();
            _resultList = new List<SongViewModel>();
            downloadedSong = new Song();
            selectedLyricChecksum = "";
            selectedLyricId = 0;
            
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
            selectedLyricId = SelectedSearchViewModel.LyricId;
            selectedLyricChecksum = SelectedSearchViewModel.LyricChecksum;
            Task.Run(DownloadSongAsync).Wait();
            Task.Run(SaveDownloadedSong).Wait();
            OnCloseRequest(this, EventArgs.Empty);
        }
        

        private void ManualCommandExecute(object obj)
        {
            var resultSong = new Song();

            if (EnteredArtist != "")
            {
                resultSong.LyricArtist = EnteredArtist;
            }
            if (EnteredSong != "")
            {
                resultSong.LyricSong = EnteredSong;
            }

            downloadedSong = resultSong;
            Task.Run(SaveDownloadedSong).Wait();
            OnCloseRequest(this, EventArgs.Empty);
        }

        private async Task SearchSongsAsync()
        {
            List<Song> list = await DownloadService.DownloadSearchResultsAsync(EnteredArtist, EnteredSong);
            list.ForEach(s => _resultList.Add(new SongViewModel(s)));
            SearchResults = new ObservableCollection<SongViewModel>(_resultList);
        }

        private async Task DownloadSongAsync()
        {
            var song = await DownloadService.DownloadSongByIdAsync(selectedLyricId, selectedLyricChecksum);
            downloadedSong = song;
            
        }

        private async Task SaveDownloadedSong()
        {
            var savedSongs = await PersistencyService.LoadLyricsAsync();
            savedSongs.Add(downloadedSong);
            await PersistencyService.SaveLyricsAsync(savedSongs);
        }
        
    }
}
