using System.Windows;

namespace LyricsManager
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class EditWindow
    {
        public EditWindow()
        {
            InitializeComponent();
        }

        private void DoneButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
    }
}
