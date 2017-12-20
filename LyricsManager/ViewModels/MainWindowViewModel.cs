using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public MainWindowViewModel()
        {
            Task.Run(LoadDataAsync);
            DeleteCommand = new DelegateCommand(DeleteCommandExecute);
            NewCommand = new DelegateCommand(NewCommandExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
        }

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

        private async Task LoadDataAsync()
        {
            //await DownloadService.DownloadSongAsync("michael jackson", "bad");

            
            _allSongs = new List<SongViewModel>();
            var songs = await PersistencyService.LoadLyricsAsync();
            /*List<Song> songs = new List<Song>
            {
                await DownloadService.DownloadSongAsync("michael jackson", "bad")
            };*/

            songs.ToList().ForEach(s => _allSongs.Add(new SongViewModel(s)));
            Songs = new ObservableCollection<SongViewModel>(_allSongs);
            if (Songs.Count > 0)
                SelectedSong = Songs[0];
                

            /*
            _allSongs = new List<SongViewModel>
            {
                new SongViewModel(new Song
                {
                    LyricArtist = "Penis",
                    LyricSong = "PenisSong",
                    LyricChecksum = "1",
                    TrackId = "1",
                    LyricId = 1,
                    LyricRank = 1,
                    LyricUrl = "www.penis.com",
                    Lyric = "penis penis Penis"
                })
            };
            Songs = new ObservableCollection<SongViewModel>(_allSongs);
            SelectedSong = Songs[0];

            await Task.Run(SaveDataAsync);*/
        }

        private async Task SaveDataAsync()
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
            Console.WriteLine("#########################");
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
            Console.WriteLine("-------------------------" + newSong.LyricId);

            _allSongs.Add(newSong);
            SelectedSong = newSong;
            Console.WriteLine("-------------------------" + newSong.LyricId);*/
        }

        private void DeleteCommandExecute(object obj)
        {
            Console.WriteLine("+-+-+-+-+--+-+-+-+-+-+");
            if (SelectedSong == null) return;

            _allSongs.Remove(SelectedSong);
            Songs.Remove(SelectedSong);
        }
    }
}
