using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LyricsManager.Models;
using LyricsManager.Services;
using LyricsManager.ViewModels;

namespace LyricsManager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
    }
}
