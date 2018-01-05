using LyricsManager.Models;

namespace LyricsManager.ViewModels
{
    public class SongViewModel
    {
        public SongViewModel() { }

        public SongViewModel(Song song)
        {
            LyricChecksum = song.LyricChecksum;
            LyricId = song.LyricId;
            LyricUrl = song.LyricUrl;
            LyricArtist = song.LyricArtist;
            LyricSong = song.LyricSong;
            LyricRank = song.LyricRank;
            Lyric = song.Lyric;
            ImageUri = song.ImageUri;
        }

        public string Lyric { get; set; }
        public string LyricChecksum { get; }
        public int LyricId { get; set; }
        public string LyricUrl { get; }
        public string LyricArtist { get; set; }
        public string LyricSong { get; set; }
        public int LyricRank { get; }
        public string ImageUri { get; set; }
    }
}
