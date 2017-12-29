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
        private static string downloadLyricByNameBaseUrl = "http://api.chartlyrics.com/apiv1.asmx/SearchLyricDirect?";
        private static string searchBaseUrl = "http://api.chartlyrics.com/apiv1.asmx/SearchLyric?";
        private static string downloadLyricByIdBaseUrl = "http://api.chartlyrics.com/apiv1.asmx/GetLyric?";

        public static async Task<Song> DownloadSongAsync(string artist, string name)
        {
            await Task.Delay(0);
            var fullUrl = downloadLyricByNameBaseUrl + "artist=" + artist + "&song=" + name;

            try
            {
                var client = new WebClient();
                var result = client.DownloadString(fullUrl);
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

        public static async Task<List<Song>> DownloadSearchResultsAsync(string artist, string songName)
        {
            await Task.Delay(0);
            var searchUrl = searchBaseUrl + "artist=" + artist + "&song=" + songName;
            Console.WriteLine(searchUrl);
            var searchResults = new List<Song>();
            try
            {
                var client = new WebClient();
                var result = client.DownloadString(searchUrl);
                var doc = new XmlDocument();
                doc.LoadXml(result);
                Console.WriteLine("Anzahl: " +doc.ChildNodes[1].ChildNodes.Count);
                for (int i = 0; i < doc.ChildNodes[1].ChildNodes.Count-1; i++)
                {
                    var trackId = doc.ChildNodes[1].ChildNodes[i].ChildNodes[0].InnerText;
                    
                    var lyricChecksum = doc.ChildNodes[1].ChildNodes[i].ChildNodes[1].InnerText;
                    var lyricId = Int32.Parse(doc.ChildNodes[1].ChildNodes[i].ChildNodes[2].InnerText);
                    var lyricUrl = doc.ChildNodes[1].ChildNodes[i].ChildNodes[3].InnerText;
                    var lyricArtist = doc.ChildNodes[1].ChildNodes[i].ChildNodes[5].InnerText;
                    var lyricSong = doc.ChildNodes[1].ChildNodes[i].ChildNodes[6].InnerText;
                    var lyric = "";

                    if (lyricId != 0)
                    {
                        var song = new Song
                        {
                            TrackId = trackId,
                            LyricChecksum = lyricChecksum,
                            LyricId = lyricId,
                            LyricSong = lyricSong,
                            LyricArtist = lyricArtist,
                            LyricUrl = lyricUrl,
                            Lyric = lyric,
                            LyricRank = 0
                        };
                        searchResults.Add(song);
                    }
                    
                    
                    
                }
                
                return searchResults;
            }
            catch (Exception e)
            {
                Console.WriteLine("exception: " + e);
                throw;
            }
        }

        public static async Task<Song> DownloadSongByIdAsync(int id, string checksum)
        {
            await Task.Delay(0);
            var fullUrl = downloadLyricByIdBaseUrl + "lyricId=" + id + "&lyricCheckSum=" + checksum;
            Console.WriteLine("*#*#*#*#*#* URL: "+fullUrl);
            try
            {
                var client = new WebClient();
                var result = client.DownloadString(fullUrl);
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
