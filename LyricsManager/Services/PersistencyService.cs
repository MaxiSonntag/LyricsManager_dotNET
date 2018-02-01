using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LyricsManager.Models;

namespace LyricsManager.Services
{
    /// <summary>
    ///     Klasse zum persistenten Speichern und Laden ´von Daten
    /// </summary>
    static class PersistencyService
    {
        private const string FileName = "Songlist.xml";

        /// <summary>
        ///     Lädt (falls vorhanden) gespeicherte Songs vom Dateisystem.
        /// </summary>
        /// <returns>Liste der gespeicherten Songs</returns>
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
                //Console.Out.WriteLine(e.StackTrace);
                return new List<Song>();
            }
        }

        /// <summary>
        ///     Speichert Songs auf dem Dateisystem (XML-Format)
        /// </summary>
        /// <param name="songs">Liste der zu speichernden Songs</param>
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

