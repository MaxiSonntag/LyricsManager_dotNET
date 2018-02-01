using System;
using LyricsManager.Models;
using LyricsManager.MVVM;
using LyricsManager.Services;

namespace LyricsManager.ViewModels
{
    /// <summary>
    ///     View Model des Edit-Fensters
    /// </summary>
    internal class EditWindowViewModel : ViewModelBase
    {
        /// <summary>
        ///     Der Song, der bearbeitet werden soll
        /// </summary>
        private SongViewModel _song;
        /// <summary>
        ///     Die Listenposition des betroffenen Songs
        /// </summary>
        public int Index;
        /// <summary>
        ///     Command zum persistenten Abspeichern der Songs
        /// </summary>
        public DelegateCommand SaveCommand { get; set; }
        /// <summary>
        ///     Event Handler, der beim Schließen des Fensters aufgerufen wird um das Hauptfenster zu aktualisieren
        /// </summary>
        public event EventHandler OnCloseRequest;

        public EditWindowViewModel(SongViewModel songViewModel)
        {
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            _song = songViewModel;
        }

        public EditWindowViewModel()
        {
        }

        public SongViewModel Song
        {
            get => _song;
            set => Set(ref _song, value);
        }

        
        /// <summary>
        ///     Speichert die Änderungen ab und ruft den Event Handler auf, der das Fenster schließen soll.
        /// </summary>
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