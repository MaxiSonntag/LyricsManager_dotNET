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
        private List<SongViewModel> _resultList;
        private ObservableCollection<SongViewModel> _searchResults;
        private SongViewModel _selectedSong;

        public string EnteredArtist { get; set; }
        public string EnteredSong { get; set; }

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

        public DelegateCommand AcceptCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public SearchWindowViewModel()
        {
            AcceptCommand = new DelegateCommand(AcceptCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);
            _searchResults = new ObservableCollection<SongViewModel>();
            _resultList = new List<SongViewModel>();
        }

        private void AcceptCommandExecute(object obj)
        {
            Console.WriteLine("+-+-+-+-+-+-+-+- AcceptCalled");
            if (SearchResults.Count != 0)
            {
                SearchResults = new ObservableCollection<SongViewModel>();
            }
            Task.Run(SearchSongsAsync);
        }

        private void CancelCommandExecute(object obj)
        {
            Console.WriteLine("+-+-+-+-+-+-+-+- CancelCalled");
        }

        private async Task SearchSongsAsync()
        {
            List<Song> list = await DownloadService.DownloadSearchResultsAsync(EnteredArtist, EnteredSong);
            list.ForEach(s => _resultList.Add(new SongViewModel(s)));
            SearchResults = new ObservableCollection<SongViewModel>(_resultList);
        }
    }
}
