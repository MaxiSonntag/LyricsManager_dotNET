using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyricsManager.MVVM;

namespace LyricsManager.ViewModels
{
    /// <summary>
    ///     View Model eines YouTube-Suchergebnisses
    /// </summary>
    internal class YoutubeSearchResultViewModel
    {
        public string ImageUrl { get; set; }
        private string VideoId { get; }
        public string Title { get; set; }
        public string Description { get; set; }
        private string WatchUrl { get; }
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
        ///     Öffnet die YouTube-Website des entsprechenden Liedes
        /// </summary>
        /// <param name="obj"></param>
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
