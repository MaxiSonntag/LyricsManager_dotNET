using System;
using LyricsManager.Models;
using LyricsManager.MVVM;
using LyricsManager.Services;

namespace LyricsManager.ViewModels
{
    internal class EditWindowViewModel : ViewModelBase
    {
        private SongViewModel _song;
        public int Index;

        public EditWindowViewModel(SongViewModel songViewModel)
        {
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            _song = songViewModel;
        }

        public EditWindowViewModel()
        {
        }

        public DelegateCommand SaveCommand { get; set; }

        public SongViewModel Song
        {
            get => _song;
            set => Set(ref _song, value);
        }

        public event EventHandler OnCloseRequest;

        private async void SaveCommandExecute(object obj)
        {
            var allSongs = await PersistencyService.LoadLyricsAsync();
            var song = new Song
            {
                LyricId = Song.LyricId,
                LyricArtist = Song.LyricArtist,
                LyricSong = Song.LyricSong,
                LyricChecksum = Song.LyricChecksum,
                LyricRank = Song.LyricRank,
                LyricUrl = Song.LyricUrl,
                Lyric = Song.Lyric,
                ImageUri = Song.ImageUri
            };
            allSongs[Index] = song;
            await PersistencyService.SaveLyricsAsync(allSongs);
            OnCloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}