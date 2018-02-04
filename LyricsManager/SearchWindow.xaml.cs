using System.Windows;
using System.Windows.Input;

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

        /// <summary>
        ///     Schließt das SearchWindow beim Betätigen des Cancel-Buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Ermöglicht die Ausführung der Suche beim Betätigen der Enter-Taste
        /// </summary>
        private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (SearchButton.IsEnabled)
                {
                    SearchButton.Command.Execute(null);
                }
            }
        }

        /// <summary>
        ///     Ermöglicht die Ausführung der Suche beim Betätigen der Enter-Taste (Lyric-Tab)
        /// </summary>
        private void TextBoxLyric_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (SearchButtonLyric.IsEnabled)
                {
                    SearchButtonLyric.Command.Execute(null);
                }
            }
        }
    }
}
