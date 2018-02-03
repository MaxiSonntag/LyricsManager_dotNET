using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using LyricsManager.Models;
using LyricsManager.ViewModels;
using Newtonsoft.Json.Linq;

namespace LyricsManager.Services
{
    /// <summary>
    ///     Klasse für die Durchführung von Downloads
    ///     Enthält alle Funktionalitäten die mit Netzwerkaktivität erfordern
    /// </summary>
    static class DownloadService
    {
    
        // ################# Lyric Suche ####################
        /// <summary>
        ///     Durchsucht die ChartLyrics-API und die Musixmatch-API nach passenden Songs für die Suchanfrage.
        ///     Diese wird aus Künstler und Name des gewünschten Songs zusammengesetzt.
        /// </summary>
        /// <param name="artist">Der Name des Künstlers</param>
        /// <param name="songName">Der Name des Songs</param>
        /// <returns>Liste der gefundenen Songs</returns>
        public static async Task<List<Song>> DownloadSearchResultsAsync(string artist, string songName)
        {
            var results = new List<Song>();
            var chartLyricResults = await DownloadChartLyricsSearchResultAsync(artist, songName, null);
            var musixmatchResults = await DownloadMusixmatchSearchResultAsync(artist, songName, null);

            foreach (var res in musixmatchResults)
            {
                results.Add(res);
            }

            foreach (var res in chartLyricResults)
            {
                results.Add(res);
            }
            
            return results;
        }


        /// <summary>
        ///     Lädt die Suchergebnisse zu einem Song von der ChartLyrics-API herunter.
        /// </summary>
        /// <param name="artist">Der Name des Künstlers</param>
        /// <param name="songName">Der name des Songs</param>
        /// <returns>Liste aus Suchergebnissen</returns>
        private static async Task<List<Song>> DownloadChartLyricsSearchResultAsync(string artist, string songName, string lyric)
        {
            await Task.Delay(0);
            var searchUrl = "";
            if (string.IsNullOrEmpty(lyric))
            {
                searchUrl = Constants.ChartLyricsSearchBaseUrl + "artist=" + artist + "&song=" + songName;
            }
            else
            {
                searchUrl = Constants.ChartLyricsSearchLyricBaseUrl + "lyricText=" + lyric;
            }

            searchUrl = Uri.EscapeUriString(searchUrl);
            Console.WriteLine(searchUrl);
            var searchResults = new List<Song>();
            try
            {
                var client = new WebClient();
                var result = client.DownloadString(searchUrl);
                var doc = new XmlDocument();
                doc.LoadXml(result);
                Console.WriteLine("Anzahl: " + (doc.ChildNodes[1].ChildNodes.Count-1));
                for (int i = 0; i < doc.ChildNodes[1].ChildNodes.Count - 1; i++)
                {
                    var trackId = doc.ChildNodes[1].ChildNodes[i].ChildNodes[0].InnerText;

                    var lyricChecksum = doc.ChildNodes[1].ChildNodes[i].ChildNodes[1].InnerText;
                    var lyricId = Int32.Parse(doc.ChildNodes[1].ChildNodes[i].ChildNodes[2].InnerText);
                    var lyricUrl = doc.ChildNodes[1].ChildNodes[i].ChildNodes[3].InnerText;
                    var lyricArtist = doc.ChildNodes[1].ChildNodes[i].ChildNodes[5].InnerText;
                    var lyricSong = doc.ChildNodes[1].ChildNodes[i].ChildNodes[6].InnerText;
                    var trackLyric = "";

                    byte[] bytes = Encoding.Default.GetBytes(lyricArtist);
                    lyricArtist = Encoding.UTF8.GetString(bytes);

                    bytes = Encoding.Default.GetBytes(lyricSong);
                    lyricSong = Encoding.UTF8.GetString(bytes);


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
                            Lyric = trackLyric,
                            LyricRank = 0,
                            ChartLyricsApi = true
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
        ///     Lädt die Suchergebnisse zu einem Song von der Musixmatch-API herunter.
        /// </summary>
        /// <param name="artist">Der Name des Künstlers</param>
        /// <param name="song">Der name des Songs</param>
        /// <returns>Liste aus Suchergebnissen</returns>
        private static async Task<List<Song>> DownloadMusixmatchSearchResultAsync(string artist, string song, string lyric)
        {
            await Task.Delay(0);
            var searchUrl = "";
            if (String.IsNullOrEmpty(lyric))
            {
                searchUrl = Constants.MusixmatchSearchBaseUrl + "format=jsonp&callback=callback&q_track=" + song +
                            "&q_artist=" + artist + "&f_has_lyrics=1&apikey=" + Constants.MusixmatchApiKey;

            }
            else
            {
                searchUrl = Constants.MusixmatchSearchBaseUrl + "format=jsonp&callback=callback&q_lyrics="+lyric+"&f_has_lyrics=1&apikey=" + Constants.MusixmatchApiKey;

            }
            searchUrl = Uri.EscapeUriString(searchUrl);
            Console.WriteLine(searchUrl);
            var searchResults = new List<Song>();

            try
            {
                var client = new WebClient();
                var result = client.DownloadString(searchUrl);
                var cuttedJsonString = result.Substring(9, result.Length - 11);
                var doc = JObject.Parse(cuttedJsonString);
                var trackList = doc.Property("message").First.First.Next.First.First.First;
                Console.WriteLine("Anzahl MusixMatch: " + trackList.Count());
                for (var i = 0; i < trackList.Children().Count(); i++)
                {
                    var currentTrack = trackList[i]["track"];

                    var trackId = currentTrack["track_id"].Value<long>().ToString();
                    var lyricChecksum = "";
                    var lyricId = currentTrack["lyrics_id"].Value<long>();
                    var lyricUrl = currentTrack["track_share_url"].Value<string>();
                    var lyricArtist = currentTrack["artist_name"].Value<string>();
                    var lyricSong = currentTrack["track_name"].Value<string>();
                    var imageUri = currentTrack["album_coverart_100x100"].Value<string>();
                    var trackLyric = "";

                    if (lyricId != 0)
                    {
                        var resSong = new Song
                        {
                            TrackId = trackId,
                            LyricChecksum = lyricChecksum,
                            LyricId = lyricId,
                            LyricSong = lyricSong,
                            LyricArtist = lyricArtist,
                            LyricUrl = lyricUrl,
                            Lyric = trackLyric,
                            LyricRank = 0,
                            ImageUri = imageUri,
                            ChartLyricsApi = false
                        };
                        searchResults.Add(resSong);
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

        public static async Task<List<Song>> DownloadSearchResultsForLyricAsync(string lyric)
        {
            var result = new List<Song>();
            
            var searchResultMusixmatch = await DownloadMusixmatchSearchResultAsync("", "", lyric);
            var searchResultChartLyrics = await DownloadChartLyricsSearchResultAsync("", "", lyric);
            foreach (var item in searchResultMusixmatch)
            {
                result.Add(item);
            }
            foreach (var item in searchResultChartLyrics)
            {
                result.Add(item);
            }
            return result;
        }


        //################# Lyric Download #################

        /// <summary>
        ///     Lädt alle Details zu einem spezifischen Song herunter.
        /// </summary>
        /// <param name="s">Das ViewModel des gesuchten Songs</param>
        /// <returns>Einen einzelnen Song inkl. Lyrics</returns>
        public static async Task<Song> DownloadSongByIdAsync(SongViewModel s)
        {
            Song song;
            if (s.ChartLyricsApi)
            {
                song = await DownloadChartLyricsSongById(s.LyricId, s.LyricChecksum);
            }
            else
            {
                song = await DownloadMusixmatchSongById(s);
            }
            
            return song;

        }

        /// <summary>
        ///     Lädt alle Suchergebnisse zu einem Song von der ChartLyrics-API herunter.
        /// </summary>
        /// <param name="id">Die TrackId eines Songs</param>
        /// <param name="checksum">Die Lyric-Checksumme eines Songs</param>
        /// <returns>Liste aus Suchergebnissen</returns>
        private static async Task<Song> DownloadChartLyricsSongById(long id, string checksum)
        {
            await Task.Delay(0);
            var fullUrl = Constants.ChartLyricsDownloadLyricByIdBaseUrl + "lyricId=" + id + "&lyricCheckSum=" + checksum;
            fullUrl = Uri.EscapeUriString(fullUrl);
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

                byte[] bytes = Encoding.Default.GetBytes(lyricArtist);
                lyricArtist = Encoding.UTF8.GetString(bytes);

                bytes = Encoding.Default.GetBytes(lyricSong);
                lyricSong = Encoding.UTF8.GetString(bytes);

                bytes = Encoding.Default.GetBytes(lyric);
                lyric = Encoding.UTF8.GetString(bytes);

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
        ///     Lädt die Lyrics zu einem Song von der Musixmatch-API herunter.
        /// </summary>
        /// <param name="s">Der gersuchte Song</param>
        /// <returns>Song inkl. Lyrics und Bild</returns>
        private static async Task<Song> DownloadMusixmatchSongById(SongViewModel s)
        {
            var url = Constants.MusixmatchDownloadLyricByIdBaseUrl + "format=jsonp&callback=callback&track_id=" + s.TrackId +
                      "&apikey=" + Constants.MusixmatchApiKey;
            url = Uri.EscapeUriString(url);
            
            try
            {
                var client = new WebClient();
                var result = client.DownloadString(url);
                var cuttedJsonString = result.Substring(9, result.Length - 11);
                var doc = JObject.Parse(cuttedJsonString);
                var lyricObj = doc["message"]["body"]["lyrics"];


                var trackId = s.TrackId;
                var lyricChecksum = s.LyricChecksum;
                var lyricId = lyricObj["lyrics_id"].Value<long>();
                var lyricSong = s.LyricSong;
                var lyricArtist = s.LyricArtist;
                var lyricUrl = s.LyricUrl;
                var imageUrl = s.ImageUri;
                var lyric = lyricObj["lyrics_body"].Value<string>();
                
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

        //##################### Bild Download ###################

        /// <summary>
        ///     Lädt ein Bild für einen Song herunter. Ist ein Bild bei verfügbar,
        ///     wird es heruntergeladen, falls nicht wird ein Placeholder-Bild verwendet.
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

       

        

        

        // ############### YouTube #################

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


        // ############### Spotify ##################
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
            client.Headers.Add("Authorization", "Bearer " + accessToken);
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
    }
}
