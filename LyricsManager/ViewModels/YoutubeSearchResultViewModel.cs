using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyricsManager.MVVM;

namespace LyricsManager.ViewModels
{
    class YoutubeSearchResultViewModel
    {
        public string ImageUrl { get; set; }
        private string VideoId { get; }
        public string Title { get; set; }
        public string Description { get; set; }
        private string EmbedUrl { get; }
        public DelegateCommand VideoPressedCommand { get; set; }

        public YoutubeSearchResultViewModel(YoutubeSearchResultViewModel model)
        {
            ImageUrl = model.ImageUrl;
            VideoId = model.VideoId;
            Title = model.Title;
            Description = model.Description;
            EmbedUrl = "https://www.youtube.com/watch?v=" + VideoId;
            VideoPressedCommand = new DelegateCommand(VideoPressedCommandExecute);
        }

        private void VideoPressedCommandExecute(object obj)
        {
            Process.Start(new ProcessStartInfo(EmbedUrl));
        }

        public YoutubeSearchResultViewModel(string url, string id, string title, string desc)
        {
            ImageUrl = url;
            VideoId = id;
            Title = title;
            Description = desc;
            EmbedUrl = "https://www.youtube.com/watch=" + VideoId;
        }
    }
}
