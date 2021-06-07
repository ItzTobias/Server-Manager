using Server_Manager.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Server_Manager.UIElements
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorMessage : Grid
    {
        public string Text { get; init; }

        public Tuple<string, RoutedEventHandler> GreenButton
        {
            init
            {
                _GreenButton.Content = value.Item1;
                _GreenButton.Click += value.Item2;
                _GreenButton.Visibility = Visibility.Visible;
            }
        }
        public Tuple<string, RoutedEventHandler> GrayButton
        {
            init
            {
                _GrayButton.Content = value.Item1;
                _GrayButton.Click += value.Item2;
                _GrayButton.Visibility = Visibility.Visible;
            }
        }
        public Tuple<string, RoutedEventHandler> RedButton
        {
            init
            {
                _RedButton.Content = value.Item1;
                _RedButton.Click += value.Item2;
                _RedButton.Visibility = Visibility.Visible;
            }
        }

        public IEnumerable<ErrorItem> ErrorItems
        {
            init
            {
                _ListBox.ItemsSource = value;
            }
        }

        public ErrorMessage()
        {
            InitializeComponent();
        }
    }
}
