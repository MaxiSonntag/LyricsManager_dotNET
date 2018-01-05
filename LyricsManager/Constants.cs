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
        public const string SearchBaseUrl = "http://api.chartlyrics.com/apiv1.asmx/SearchLyric?";
        public const string DownloadLyricByIdBaseUrl = "http://api.chartlyrics.com/apiv1.asmx/GetLyric?";

        //Api Keys
        public const string YouTubeApiKey = "AIzaSyDaRJC0kzLibXTn-B2d5iciHBN5v9069ic";
        public const string SpotifyApiKey = "8e00c31b945e4a45b213041652b4a30c";
    }
}
