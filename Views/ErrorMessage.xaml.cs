using Server_Manager.Scripts;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Server_Manager.Views
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorMessage : Grid
    {
        public string Text { get; init; }

        public Tuple<string, RoutedEventHandler> RedButton { get; init; }
        public Tuple<string, RoutedEventHandler> GrayButton { get; init; }
        public Tuple<string, RoutedEventHandler> GreenButton { get; init; }

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

        public void Initialize()
        {
            List<Tuple<Tuple<string, RoutedEventHandler>, ButtonType>> usedButtons = new();

            if (RedButton != null)
            {
                usedButtons.Add(new(RedButton, ButtonType.Red));
            }
            if (GrayButton != null)
            {
                usedButtons.Add(new(GrayButton, ButtonType.Neutral));
            }
            if (GreenButton != null)
            {
                usedButtons.Add(new(GreenButton, ButtonType.Green));
            }

            if (usedButtons.Count == 3)
            {
                Button1.Content = usedButtons[0].Item1.Item1;
                Button1.Style = GetButtonStyle(usedButtons[0].Item2);
                Button1.Click += usedButtons[0].Item1.Item2;
                Button1.Visibility = Visibility.Visible;

                Button2.Content = usedButtons[1].Item1.Item1;
                Button2.Style = GetButtonStyle(usedButtons[1].Item2);
                Button2.Click += usedButtons[1].Item1.Item2;
                Button2.Visibility = Visibility.Visible;

                Button3.Content = usedButtons[2].Item1.Item1;
                Button3.Style = GetButtonStyle(usedButtons[2].Item2);
                Button3.Click += usedButtons[2].Item1.Item2;
                Button3.Visibility = Visibility.Visible;
            }
            else if (usedButtons.Count == 2)
            {
                Button1.Content = usedButtons[0].Item1.Item1;
                Button1.Style = GetButtonStyle(usedButtons[0].Item2);
                Button1.Click += usedButtons[0].Item1.Item2;
                Button1.Visibility = Visibility.Visible;

                Button3.Content = usedButtons[1].Item1.Item1;
                Button3.Style = GetButtonStyle(usedButtons[1].Item2);
                Button3.Click += usedButtons[1].Item1.Item2;
                Button3.Visibility = Visibility.Visible;
            }
            else if (usedButtons.Count == 1)
            {
                Button2.Content = usedButtons[0].Item1.Item1;
                Button2.Style = GetButtonStyle(usedButtons[0].Item2);
                Button2.Click += usedButtons[0].Item1.Item2;
                Button2.Visibility = Visibility.Visible;
            }
        }

        private Style GetButtonStyle(ButtonType buttonType)
        {
            return buttonType switch
            {
                ButtonType.Red => Resources["RedButton"] as Style,
                ButtonType.Neutral => Resources["GrayButton"] as Style,
                ButtonType.Green => Resources["GreenButton"] as Style,
                _ => null
            };
        }

        private enum ButtonType
        {
            Red,
            Neutral,
            Green
        }
    }
}
