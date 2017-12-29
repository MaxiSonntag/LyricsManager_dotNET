using System.Windows;
using LyricsManager.ViewModels;
using MahApps.Metro.Controls;

namespace LyricsManager
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NewButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as MainWindowViewModel;
            context.NewCommand.Execute(null);
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as MainWindowViewModel;
            context.DeleteCommand.Execute(null);
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as MainWindowViewModel;
            context.SaveCommand.Execute(null);
        }

        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as MainWindowViewModel;
            context.EditCommand.Execute(null);
        }
    }
}