
namespace LyricsManager.Models
{
    /// <summary>
    ///     Model-Klasse eines Songs
    /// </summary>
    public class Song
    {
        /// <summary>
        ///     Die Id eines Liedes(von ChartLyrics)
        /// </summary>
        public string TrackId { get; set; }
        /// <summary>
        ///     Die Checksumme eines Liedes (von ChartLyrics)
        /// </summary>
        public string LyricChecksum { get; set; }
        /// <summary>
        ///     Die Id einer Lyric (entspricht Details eines Songs - von ChartLyrics).
        /// </summary>
        public int LyricId { get; set; }
        /// <summary>
        ///     Die Web-URL einer Lyric
        /// </summary>
        public string LyricUrl { get; set; }
        /// <summary>
        ///     Der Name des Künstlers
        /// </summary>
        public string LyricArtist { get; set; }
        /// <summary>
        ///     Der ´Name des Liedes
        /// </summary>
        public string LyricSong { get; set; }
        /// <summary>
        ///     Der Rang einer Lyric (von ChartLyrics)
        /// </summary>
        public int LyricRank { get; set; }
        /// <summary>
        ///     Die Lyric zu einem Lied
        /// </summary>
        public string Lyric { get; set; }
        /// <summary>
        ///     Der Ort des Bildes zu einem Lied
        /// </summary>
        public string ImageUri { get; set; }
        
    }
}
