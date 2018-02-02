using LyricsManager.Models;

namespace LyricsManager.ViewModels
{
    /// <summary>
    ///     View Model eines Songs
    /// </summary>
    internal class SongViewModel
    {
        public SongViewModel() { }

        public SongViewModel(Song song)
        {
            LyricChecksum = song.LyricChecksum;
            LyricId = song.LyricId;
            TrackId = song.TrackId;
            LyricUrl = song.LyricUrl;
            LyricArtist = song.LyricArtist;
            LyricSong = song.LyricSong;
            LyricRank = song.LyricRank;
            Lyric = song.Lyric;
            ImageUri = song.ImageUri;
            ChartLyricsApi = song.ChartLyricsApi;
        }

        /// <summary>
        ///     Die Lyric eines Songs
        /// </summary>
        public string Lyric { get; set; }
        /// <summary>
        ///     Die Checksumme eines Songs (von ChartLyrics-API für Download gefordert)
        /// </summary>
        public string LyricChecksum { get; }
        /// <summary>
        ///     Die Id der Lyric
        /// </summary>
        public long LyricId { get; set; }
        /// <summary>
        ///     Die Id des Songs
        /// </summary>
        public string TrackId { get; set; }
        /// <summary>
        ///     Die URL zur Webansicht eines Songs
        /// </summary>
        public string LyricUrl { get; }
        /// <summary>
        ///     Der Künstler des Songs
        /// </summary>
        public string LyricArtist { get; set; }
        /// <summary>
        ///     Der Titel des Songs
        /// </summary>
        public string LyricSong { get; set; }
        /// <summary>
        ///     Der Rang einer Lyric (Bewertungssystem von ChartLyrics - aktuell unbenutzt)
        /// </summary>
        public int LyricRank { get; }
        /// <summary>
        ///     Die Url zu einem Bild eines Songs (entweder von API geliefert oder Placeholder-Bild)
        /// </summary>
        public string ImageUri { get; set; }
        /// <summary>
        ///     Information ob der Song von ChartLyrics heruntergeladen wurde oder anderweitig
        /// </summary>
        public bool ChartLyricsApi { get; set; }
    }
}
