﻿using System;
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
using System.Windows.Shapes;
using LyricsManager.ViewModels;
using MahApps.Metro.Controls;

namespace LyricsManager
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class SearchWindow : MetroWindow
    {
        public SearchWindow()
        {
            InitializeComponent();
        }

        private void AcceptButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as SearchWindowViewModel;
            context.SearchCommand.Execute(null);
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as SearchWindowViewModel;
            context.ApplyCommand.Execute(null);
        }

        private void ManualButton_OnClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as SearchWindowViewModel;
            context.ManualCommand.Execute(null);
        }
    }
}
