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

        public string Lyric { get; set; }
        public string LyricChecksum { get; }
        public long LyricId { get; set; }
        public string TrackId { get; set; }
        public string LyricUrl { get; }
        public string LyricArtist { get; set; }
        public string LyricSong { get; set; }
        public int LyricRank { get; }
        public string ImageUri { get; set; }
        public bool ChartLyricsApi { get; set; }
    }
}
