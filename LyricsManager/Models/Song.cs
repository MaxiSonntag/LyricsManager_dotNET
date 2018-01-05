
namespace LyricsManager.Models
{
    public class Song
    {
        public string TrackId { get; set; }
        public string LyricChecksum { get; set; }
        public int LyricId { get; set; }
        public string LyricUrl { get; set; }
        public string LyricArtist { get; set; }
        public string LyricSong { get; set; }
        public int LyricRank { get; set; }
        public string Lyric { get; set; }
        public string ImageUri { get; set; }
        

        public override string ToString()
        {
            return $"{LyricArtist} - {LyricSong}"; 
        }
    }
}
