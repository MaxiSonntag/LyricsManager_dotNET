﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;
using LyricsManager.Models;

namespace LyricsManager.Services
{
    class DownloadService
    {
        //private static string _downloadLyricByNameBaseUrl = "http://api.chartlyrics.com/apiv1.asmx/SearchLyricDirect?";
        private const string SearchBaseUrl = "http://api.chartlyrics.com/apiv1.asmx/SearchLyric?";

        private const string DownloadLyricByIdBaseUrl = "http://api.chartlyrics.com/apiv1.asmx/GetLyric?";

        /*public static async Task<Song> DownloadSongAsync(string artist, string name)
        {
            await Task.Delay(0);
            var fullUrl = _downloadLyricByNameBaseUrl + "artist=" + artist + "&song=" + name;

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

        }*/

        public static async Task<List<Song>> DownloadSearchResultsAsync(string artist, string songName)
        {
            await Task.Delay(0);
            var searchUrl = SearchBaseUrl + "artist=" + artist + "&song=" + songName;
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
            var fullUrl = DownloadLyricByIdBaseUrl + "lyricId=" + id + "&lyricCheckSum=" + checksum;
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
                var imageUrl = doc.ChildNodes[1].ChildNodes[6].InnerText;
                var lyric = doc.ChildNodes[1].ChildNodes[9].InnerText;

                var song = new Song
                {
                    TrackId = trackId,
                    LyricChecksum = lyricChecksum,
                    LyricId = lyricId,
                    LyricSong = lyricSong,
                    LyricArtist = lyricArtist,
                    LyricUrl = lyricUrl,
                    ImageUri = imageUrl,
                    Lyric = lyric

                };
                

                if (!string.IsNullOrEmpty(song.ImageUri))
                {
                    song = await DownloadImageFromUrl(song);
                }
                else
                {
                    song.ImageUri = "/Images/PlaceholderPicture.png";
                }

                return song;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static async Task<Song> DownloadImageFromUrl(Song song)
        {
            const string path = "C:/Users/Public/Pictures/LyricsManagerImages";
            await Task.Delay(0);
            var client = new WebClient();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }else if (!File.Exists(path + "/" + song.LyricId + "_image.jpg"))
            {
                client.DownloadFile(new Uri(song.ImageUri), path + "/" + song.LyricId + "_image.jpg");

            }
            song.ImageUri = path + "/" + song.LyricId + "_image.jpg";
            return song;
        }

    }
}
