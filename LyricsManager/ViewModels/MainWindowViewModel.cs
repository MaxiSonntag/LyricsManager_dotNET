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

        public ObservableCollection<SongViewModel> Songs
        {
            get => _songs;
            set => Set(ref _songs, value);
        }

        public SongViewModel SelectedSong
        {
            get => _selectedSong;
            set => Set(ref _selectedSong, value);
        }

        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand NewCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }

        public MainWindowViewModel()
        {
            Task.Run(LoadDataAsync);
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

        /*private async Task SaveDataAsync()
        {
            List<Song> list = new List<Song>();
            foreach (var song in _allSongs)
            {
                Song s = new Song
                {
                    LyricSong = song.LyricSong,
                    LyricArtist = song.LyricArtist,
                    LyricChecksum = song.LyricChecksum,
                    LyricId = song.LyricId,
                    LyricRank = song.LyricRank,
                    LyricUrl = song.LyricUrl,
                    Lyric = song.Lyric
                };
                list.Add(s);
            }
            await PersistencyService.SaveLyricsAsync(list);
        }*/

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
            /*var newSong = new SongViewModel(new Song
            {
                LyricSong = "",
                LyricArtist = "",
                LyricChecksum = "",
                LyricId = 0,
                LyricRank = 0,
                LyricUrl = "",
                Lyric = ""
            });
            //_songs & _allSongs nicht verknüpft -> Binding klappt nicht wie es soll
            _songs.Add(newSong);
            _allSongs.Add(newSong);
            SelectedSong = newSong;*/
        }

        private void DeleteCommandExecute(object obj)
        {
            Console.WriteLine("+-+-+-+-+--+-+-+-+-+-+");
            if (SelectedSong == null) return;

            //_songs & _allSongs nicht verknüpft -> Binding klappt nicht wie es soll
            _allSongs.Remove(SelectedSong);
            _songs.Remove(SelectedSong);
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
            vm.index = idx;
            //vm.Song = SelectedSong;
            editWindow.ShowDialog();
        }

        private void HandleDialogWindowClose(Window window)
        {
            window.Close();
            Task.Run(LoadDataAsync);
        }

    }
}
