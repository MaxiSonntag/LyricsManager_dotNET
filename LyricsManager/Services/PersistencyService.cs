using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LyricsManager.Models;

namespace LyricsManager.Services
{
    static class PersistencyService
    {
        private const string FileName = "Songlist.xml";

        public static async Task<List<Song>> LoadLyricsAsync()
        {
            await Task.Delay(0);

            try
            {
                using (var fileStream = File.Open(FileName, FileMode.Open))
                {
                    
                    var serializer = new XmlSerializer(typeof(List<Song>));
                    var obj = serializer.Deserialize(fileStream);
                    var songs = obj as List<Song>;
                    
                    return songs;
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine($"Fehler beim Laden: {e.Message}");
                Console.Out.WriteLine(e.StackTrace);
                return new List<Song>();
            }
        }
        public static async Task SaveLyricsAsync(IEnumerable<Song> songs)
        {
            await Task.Delay(0);
            try
            {
                using (var fileStream = File.Open(FileName, FileMode.Create))
                {
                    var serializer = new XmlSerializer(typeof(List<Song>));
                    serializer.Serialize(fileStream, songs.ToList());
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine($"Fehler beim Speichern: {e.Message}");
            }
        }
    }
    }

