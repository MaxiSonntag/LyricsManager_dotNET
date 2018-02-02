using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LyricsManager.MVVM;
using LyricsManager.Services;

namespace LyricsManager.ViewModels
{
    /// <summary>
    ///     View Model des YouTube-Suchergebnis-Fensters
    /// </summary>
    internal class YoutubeWindowViewModel : ViewModelBase
    {
        private ObservableCollection<YoutubeSearchResultViewModel> _searchResults;
        private YoutubeSearchResultViewModel _selectedSong;

        public YoutubeWindowViewModel(string artist, string song)
        {
            Task.Run(() => LoadDataAsync(artist, song)).Wait();
            
        }

        public YoutubeWindowViewModel()
        {
            
        }

        public ObservableCollection<YoutubeSearchResultViewModel> SearchResults
        {
            get => _searchResults;
            set => Set(ref _searchResults, value);
        }

        public YoutubeSearchResultViewModel SelectedSong
        {
            get => _selectedSong;
            set => Set(ref _selectedSong, value);
        }

        /// <summary>
        ///     Lädt YouTube Suchergebnisse von der YouTube-API
        /// </summary>
        /// <param name="artist">Der Name des Künstlers</param>
        /// <param name="song">Der Name des Songs</param>
        private async Task LoadDataAsync(string artist, string song)
        {

            _searchResults = new ObservableCollection<YoutubeSearchResultViewModel>();
            var songs = await DownloadService.LoadYoutubeVideosAsync(artist, song);
            songs.ToList().ForEach(s => _searchResults.Add(new YoutubeSearchResultViewModel(s)));
            SearchResults = new ObservableCollection<YoutubeSearchResultViewModel>(_searchResults);
            SelectedSong = SearchResults[0];
        }


    }
}
