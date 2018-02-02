using System.Diagnostics;
using LyricsManager.MVVM;

namespace LyricsManager.ViewModels
{
    /// <summary>
    ///     View Model eines YouTube-Suchergebnisses
    /// </summary>
    internal class YoutubeSearchResultViewModel
    {
        /// <summary>
        ///     Link zum Preview-Bild eines YouTube-Videos
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        ///     Die Id des Videos
        /// </summary>
        private string VideoId { get; }
        /// <summary>
        ///     Der Titel des Videos
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        ///     Die Beschreibung eines Videos
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        ///     Die Url zum Video
        /// </summary>
        private string WatchUrl { get; }

        /// <summary>
        ///     Command zum Öffnen des Videos im Browser
        /// </summary>
        public DelegateCommand VideoPressedCommand { get; set; }

        public YoutubeSearchResultViewModel(YoutubeSearchResultViewModel model)
        {
            ImageUrl = model.ImageUrl;
            VideoId = model.VideoId;
            Title = model.Title;
            Description = model.Description;
            WatchUrl = "https://www.youtube.com/watch?v=" + VideoId;
            VideoPressedCommand = new DelegateCommand(VideoPressedCommandExecute);
        }

        /// <summary>
        ///     Öffnet die YouTube-Website des entsprechenden Videos im Browser
        /// </summary>
        private void VideoPressedCommandExecute(object obj)
        {
            Process.Start(new ProcessStartInfo(WatchUrl));
        }

        public YoutubeSearchResultViewModel(string url, string id, string title, string desc)
        {
            ImageUrl = url;
            VideoId = id;
            Title = title;
            Description = desc;
            WatchUrl = "https://www.youtube.com/watch=" + VideoId;
        }
    }
}
