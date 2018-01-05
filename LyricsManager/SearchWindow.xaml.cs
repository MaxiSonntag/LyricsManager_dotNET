using System.Windows;
using LyricsManager.ViewModels;

namespace LyricsManager
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class SearchWindow
    {
        public SearchWindow()
        {
            InitializeComponent();
        }

        private void AcceptButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as SearchWindowViewModel;
            context?.SearchCommand.Execute(null);
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as SearchWindowViewModel;
            context?.ApplyCommand.Execute(null);
        }

        private void ManualButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as SearchWindowViewModel;
            context?.ManualCommand.Execute(null);
        }
    }
}
