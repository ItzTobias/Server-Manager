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
            get => (bool)GetValue(IsMaximizingProperty);
            set => SetValue(IsMaximizingProperty, value);
        }

        public MaximizeRestoreButton()
        {
            InitializeComponent();
        }

        public void ChangeState(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = MainWindow.GetMainWindow;

            switch (mainWindow.WindowState)
            {
                case WindowState.Normal:
                    mainWindow.WindowState = WindowState.Maximized;
                    IsMaximizing = true;
                    break;
                case WindowState.Maximized:
                    mainWindow.WindowState = WindowState.Normal;
                    IsMaximizing = false;
                    break;
                default:
                    break;
            }
        }
    }
}
