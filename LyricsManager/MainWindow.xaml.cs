using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using LyricsManager.Services;
using LyricsManager.ViewModels;

namespace LyricsManager
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /*private void NewButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as MainWindowViewModel;
            context?.NewCommand.Execute(null);
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as MainWindowViewModel;
            context?.DeleteCommand.Execute(null);
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as MainWindowViewModel;
            context?.SaveCommand.Execute(null);
        }

        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as MainWindowViewModel;
            context?.EditCommand.Execute(null);
        }*/


        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }


        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            var context = DataContext as MainWindowViewModel;
            context?.SaveCommand.Execute(null);
        }

        /*private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as MainWindowViewModel;
            context.SearchAndPlaySpotifyCommand.Execute(null);
        }

        private void ConnectLocalSpotify_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as MainWindowViewModel;
            context.ConnectLocalSpotifyCommand.Execute(null);
        }

        private void ConnectWebSpotify_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as MainWindowViewModel;
            context.ConnectWebSpotifyCommand.Execute(null);
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as MainWindowViewModel;
            context.PauseSpotifyCommand.Execute(null);
        }*/
    }
}