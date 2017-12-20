using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using LyricsManager.Models;

namespace LyricsManager.Services
{
    class DownloadService
    {
        private static string url = "http://api.chartlyrics.com/apiv1.asmx/SearchLyricDirect?";

        public static async Task<Song> DownloadSongAsync(string artist, string name)
        {
            await Task.Delay(0);
            url += "artist=" + artist + "&song=" + name;

            try
            {
                var client = new WebClient();
                var result = client.DownloadString(url);
                //Console.Write(result);
                var doc = new XmlDocument();
                doc.LoadXml(result);
                

                var trackId = doc.ChildNodes[1].ChildNodes[0].InnerText;
                var lyricChecksum = doc.ChildNodes[1].ChildNodes[1].InnerText;
                var lyricId = Int32.Parse(doc.ChildNodes[1].ChildNodes[2].InnerText);
                var lyricSong = doc.ChildNodes[1].ChildNodes[3].InnerText;
                var lyricArtist = doc.ChildNodes[1].ChildNodes[4].InnerText;
                var lyricUrl = doc.ChildNodes[1].ChildNodes[5].InnerText;
                var lyric = doc.ChildNodes[1].ChildNodes[9].InnerText;

                var song = new Song
                {
                    TrackId = trackId,
                    LyricChecksum = lyricChecksum,
                    LyricId = lyricId,
                    LyricSong = lyricSong,
                    LyricArtist = lyricArtist,
                    LyricUrl = lyricUrl,
                    Lyric = lyric
                    
                };
                return song;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        
    }
}
