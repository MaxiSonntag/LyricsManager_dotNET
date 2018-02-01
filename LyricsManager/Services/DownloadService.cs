using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using LyricsManager.Models;
using LyricsManager.ViewModels;

namespace LyricsManager.Services
{
    /// <summary>
    ///     Klasse für die Durchführung von Downloads
    ///     Enthält alle Funktionalitäten die mit Netzwerkaktivität erfordern
    /// </summary>
    static class DownloadService
    {
    
        /// <summary>
        ///     Durchsucht die ChartLyrics-API nach passenden Songs für die Suchanfrage.
        ///     Diese wird aus Künstler und Name des gewünschten Songs zusammengesetzt.
        /// </summary>
        /// <param name="artist">Der Name des Künstlers</param>
        /// <param name="songName">Der Name des Songs</param>
        /// <returns>Liste der gefundenen Songs</returns>
        public static async Task<List<Song>> DownloadSearchResultsAsync(string artist, string songName)
        {
            await Task.Delay(0);
            var searchUrl = Constants.SearchBaseUrl + "artist=" + artist + "&song=" + songName;
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
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        ///     Lädt alle Details zu einem spezifischen Song herunter.
        /// </summary>
        /// <param name="id">Die Id des gesuchten Songs</param>
        /// <param name="checksum">Die Checksumme des gesuchten Songs</param>
        /// <returns>Einen einzelnen Song inkl. Lyrics</returns>
        public static async Task<Song> DownloadSongByIdAsync(int id, string checksum)
        {
            await Task.Delay(0);
            var fullUrl = Constants.DownloadLyricByIdBaseUrl + "lyricId=" + id + "&lyricCheckSum=" + checksum;
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

        /// <summary>
        ///     Lädt ein Bild für einen Song herunter. Ist ein Bild bei ChartLyrics verfügbar,
        ///     wird es heruntergeladen, falls nciht wird ein Placeholder-Bild verwendet.
        /// </summary>
        /// <param name="song">Der Song, zu dem ein Bild hinzugefügt werden soll</param>
        /// <returns>Den Song inkl. Bild</returns>
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

        /// <summary>
        ///     Lädt auf Spotify verfügbare Lieder zu einem Song herunter.
        /// </summary>
        /// <param name="query">Der Suchparameter (Zusammensetzung aus Künstler und Songname).</param>
        /// <param name="accessToken">OAuth Access Token der Spotify-WebAPI</param>
        /// <returns>JSON String der Suchergebnisse</returns>
        public static async Task<string> DownloadSpotifySearchResults(string query, string accessToken)
        {
            await Task.Delay(0);
            var client = new WebClient();
            client.Headers.Add("Host", "api.spotify.com");
            client.Headers.Add("Accept", "application/json");
            client.Headers.Add("Content-Type", "application/json");
            client.Headers.Add("Authorization", "Bearer "+accessToken);
            client.Headers.Add("User-Agent", "Spotify API Console v0.1");
            var escapeUri = Uri.EscapeUriString("https://api.spotify.com/v1/search?q=" + query + "&type=track");
            
        
            
            Console.WriteLine("URL: " + escapeUri);
            string response = "";
            try
            {
                response = client.DownloadString(escapeUri);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return response;
        }

        /// <summary>
        ///     Lädt YouTube-Suchergebnisse zu einem Song.
        /// </summary>
        /// <param name="artist">Der Name des Künstlers</param>
        /// <param name="song">Der Name des Songs</param>
        /// <returns>Liste aus Suchergebnissen</returns>
        public static async Task<List<YoutubeSearchResultViewModel>> LoadYoutubeVideosAsync(string artist, string song)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Constants.YouTubeApiKey,
                ApplicationName = "LyricsManager"
            });

            var searchRequest = youtubeService.Search.List("snippet");
            searchRequest.Q = artist + " " + song;
            searchRequest.MaxResults = 30;
            searchRequest.Type = "video";

            var response = await searchRequest.ExecuteAsync();

            var resultList = new List<YoutubeSearchResultViewModel>();
            foreach (var searchResult in response.Items)
            {
                var id = searchResult.Id.VideoId;
                var imageUrl = searchResult.Snippet.Thumbnails.Medium.Url;
                var title = searchResult.Snippet.Title;
                var desc = searchResult.Snippet.Description;
                
                resultList.Add(new YoutubeSearchResultViewModel(imageUrl, id, title, desc));
            }

            return resultList;

        }
    }
}
