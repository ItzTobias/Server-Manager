using System;
using System.Windows;
using System.Windows.Media;

namespace Server_Manager
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Path for resources inside the project folder
        /// </summary>
        public static string DataPath => "pack://application:,,,/";
        /// <summary>
        /// Path to the folder where the Application is installed in
        /// </summary>
        public static string AppDirectory => AppDomain.CurrentDomain.BaseDirectory;

        public static Color Transparent => new();
        public static Color Hover => new() { R = 255, G = 255, B = 255, A = 31 };
        public static Color White => new() { R = 255, G = 255, B = 255, A = 255 };
        public static Color Red => new() { R = 212, G = 68, B = 16, A = 255 };
        public static Color RedHover => new() { R = 231, G = 100, B = 58, A = 255 };
        public static Color Stopping => new() { R = 188, G = 45, B = 0, A = 255 };
        public static Color Green => new() { R = 0, G = 133, B = 64, A = 255 };
        public static Color GreenHover => new() { R = 13, G = 209, B = 100, A = 255 };
        public static Color Starting => new() { R = 0, G = 107, B = 53, A = 255 };
        public static Color Input => new() { R = 18, G = 18, B = 18, A = 255 };
        public static Color InputHover => new() { R = 14, G = 14, B = 14, A = 255 };
        public static Color DarkGray => new() { R = 30, G = 30, B = 30, A = 255 };
        public static Color Gray => new() { R = 40, G = 40, B = 40, A = 255 };
        public static Color LightGray => new() { R = 48, G = 48, B = 48, A = 255 };
        public static Color FontColor => new() { R = 204, G = 204, B = 204, A = 255 };
        public static Color GrayHover => new() { R = 69, G = 69, B = 69, A = 255 };
        public static Color Line => new() { R = 55, G = 55, B = 55, A = 255 };
        public static Color ScrollBarThumbHover => new() { R = 99, G = 99, B = 99, A = 255 };
        public static Color Brightness1 => new() { R = 78, G = 78, B = 78, A = 255 };
        public static Color Brightness2 => new() { R = 89, G = 89, B = 89, A = 255 };
        public static Color Brightness3 => new() { R = 111, G = 111, B = 111, A = 255 };
        public static Color Brightness4 => new() { R = 114, G = 114, B = 114, A = 255 };
        public static Color Brightness5 => new() { R = 138, G = 138, B = 138, A = 255 };
        public static Color Brightness6 => new() { R = 209, G = 209, B = 209, A = 255 };

        public static SolidColorBrush TransparentBrush => new(Transparent);
        public static SolidColorBrush HoverBrush => new(Hover);
        public static SolidColorBrush WhiteBrush => new(White);
        public static SolidColorBrush RedBrush => new(Red);
        public static SolidColorBrush RedHoverBrush => new(RedHover);
        public static SolidColorBrush StoppingBrush => new(Stopping);
        public static SolidColorBrush GreenBrush => new(Green);
        public static SolidColorBrush GreenHoverBrush => new(GreenHover);
        public static SolidColorBrush StartingBrush => new(Starting);
        public static SolidColorBrush InputBrush => new(Input);
        public static SolidColorBrush InputHoverBrush => new(InputHover);
        public static SolidColorBrush DarkGrayBrush => new(DarkGray);
        public static SolidColorBrush GrayBrush => new(Gray);
        public static SolidColorBrush LightGrayBrush => new(LightGray);
        public static SolidColorBrush FontColorBrush => new(FontColor);
        public static SolidColorBrush GrayHoverBrush => new(GrayHover);
        public static SolidColorBrush LineBrush => new(Line);
        public static SolidColorBrush ScrollBarThumbHoverBrush => new(ScrollBarThumbHover);
        public static SolidColorBrush Brightness1Brush => new(Brightness1);
        public static SolidColorBrush Brightness2Brush => new(Brightness2);
        public static SolidColorBrush Brightness3Brush => new(Brightness3);
        public static SolidColorBrush Brightness4Brush => new(Brightness4);
        public static SolidColorBrush Brightness5Brush => new(Brightness5);
        public static SolidColorBrush Brightness6Brush => new(Brightness6);
    }
}
