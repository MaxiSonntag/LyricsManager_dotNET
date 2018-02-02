using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Navigation;
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

        /// <summary>
        ///     Öffnet die Webansicht des selektierten Songs
        /// </summary>
        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        /// <summary>
        ///     Speichert die aktuelle Songliste vor dem Schließen des MainWindow
        /// </summary>
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            var context = DataContext as MainWindowViewModel;
            context?.SaveCommand.Execute(null);
        }
    }
}