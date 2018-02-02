using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricsManager
{
    class Constants
    {
        //Download
        public const string ChartLyricsSearchBaseUrl = "http://api.chartlyrics.com/apiv1.asmx/SearchLyric?";
        public const string ChartLyricsDownloadLyricByIdBaseUrl = "http://api.chartlyrics.com/apiv1.asmx/GetLyric?";
        public const string MusixmatchSearchBaseUrl = "https://api.musixmatch.com/ws/1.1/track.search?";
        public const string MusixmatchDownloadLyricByIdBaseUrl = "https://api.musixmatch.com/ws/1.1/track.lyrics.get?";

        //Api Keys
        public const string YouTubeApiKey = "AIzaSyDaRJC0kzLibXTn-B2d5iciHBN5v9069ic";
        public const string SpotifyApiKey = "8e00c31b945e4a45b213041652b4a30c";
        public const string MusixmatchApiKey = "daf36035e9c68d96de11addcd19c3df6";
    }
}
