using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
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
                Set(ref _selectedSong, value);
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

        public bool IsSelectionValid => SelectedSong != null;

        public MainWindowViewModel()
        {
            Task.Run(LoadDataAsync).Wait();
            DeleteCommand = new DelegateCommand(DeleteCommandExecute);
            NewCommand = new DelegateCommand(NewCommandExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            EditCommand = new DelegateCommand(EditCommandExecute);
        }


        private async Task LoadDataAsync()
        {
            
            _allSongs = new List<SongViewModel>();
            var songs = await PersistencyService.LoadLyricsAsync();
            songs.ToList().ForEach(s => _allSongs.Add(new SongViewModel(s)));
            Songs = new ObservableCollection<SongViewModel>(_allSongs);
            if (Songs.Count > 0)
            {
                SelectedSong = Songs[0];
            }
                
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
                Lyric = vm.Lyric
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
            Console.WriteLine("+-+-+-+-+--+-+-+-+-+-+");
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
            int idx = _allSongs.IndexOf(SelectedSong);
            vm.Index = idx;
            editWindow.ShowDialog();
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
            Task.Run(LoadDataAsync);
        }

    }
}
