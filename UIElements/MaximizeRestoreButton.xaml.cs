using System.Windows;
using System.Windows.Controls;

namespace Server_Manager.UIElements
{
    /// <summary>
    /// Interaction logic for MaximizeRestoreButton.xaml
    /// </summary>
    public partial class MaximizeRestoreButton : Button
    {
        public static readonly DependencyProperty IsMaximizingProperty =
        DependencyProperty.Register(
                nameof(IsMaximizing),
                typeof(bool),
                typeof(MaximizeRestoreButton));

        public bool IsMaximizing
        {
            get
            {
                return (bool)GetValue(IsMaximizingProperty);
            }
            set
            {
                SetValue(IsMaximizingProperty, value);
            }
        }

        public MaximizeRestoreButton()
        {
            InitializeComponent();

            Click += Maximize;
        }

        public void Maximize(object sender, RoutedEventArgs e)
        {
            switch (MainWindow.GetMainWindow.WindowState)
            {
                case WindowState.Normal:
                    MainWindow.GetMainWindow.BorderThickness = new Thickness(5);
                    MainWindow.GetMainWindow.WindowState = WindowState.Maximized;
                    IsMaximizing = true;
                    break;
                case WindowState.Maximized:
                    MainWindow.GetMainWindow.BorderThickness = new Thickness(0);
                    MainWindow.GetMainWindow.WindowState = WindowState.Normal;
                    IsMaximizing = false;
                    break;
                default:
                    break;
            }
        }
    }
}
