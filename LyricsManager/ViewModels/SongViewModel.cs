using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyricsManager.Models;

namespace LyricsManager.ViewModels
{
    public class SongViewModel
    {
        public SongViewModel() { }

        public SongViewModel(Song song)
        {
            TrackId = song.TrackId;
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
        public string TrackId { get; set; }
        public string LyricChecksum { get; set; }
        public int LyricId { get; set; }
        public string LyricUrl { get; set; }
        public string LyricArtist { get; set; }
        public string LyricSong { get; set; }
        public int LyricRank { get; set; }
        public string ImageUri { get; set; }
    }
}
