using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyricsManager.Models;
using LyricsManager.MVVM;
using LyricsManager.Services;

namespace LyricsManager.ViewModels
{
    class EditWindowViewModel : ViewModelBase
    {
        public event EventHandler OnCloseRequest;
        public int index;

        public DelegateCommand SaveCommand { get; set; }

        private SongViewModel _song;
        public SongViewModel Song {
            get => _song;
            set => Set(ref _song, value);
        }

        public EditWindowViewModel(SongViewModel songViewModel)
        {
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            _song = songViewModel;
        }

        public EditWindowViewModel(){ }

        private async void SaveCommandExecute(object obj)
        {
            var allSongs = await PersistencyService.LoadLyricsAsync();
            var song = new Song()
            {
                LyricId = _song.LyricId,
                LyricArtist = _song.LyricArtist,
                LyricSong = _song.LyricSong,
                LyricChecksum = _song.LyricChecksum,
                LyricRank = _song.LyricRank,
                LyricUrl = _song.LyricUrl,
                Lyric = _song.Lyric
            };
            allSongs[index] = song;
            await PersistencyService.SaveLyricsAsync(allSongs);
            OnCloseRequest(this, EventArgs.Empty);
        }

    }
}
