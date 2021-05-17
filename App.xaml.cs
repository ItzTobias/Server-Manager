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
        /// <summary>
        /// Path for resources inside the project folder
        /// </summary>
        public static string DataPath => "pack://application:,,,/";
        /// <summary>
        /// Path to the folder where the Application is installed in
        /// </summary>
        public static string AppDirectory => AppDomain.CurrentDomain.BaseDirectory;

        public static readonly Color white = new() { R = 255, G = 255, B = 255, A = 255 };

        public static readonly Color red = new() { R = 212, G = 68, B = 16, A = 255 };
        public static readonly Color redHover = new() { R = 231, G = 100, B = 58, A = 255 };

        public static readonly Color green = new() { R = 0, G = 133, B = 64, A = 255 };
        public static readonly Color greenHover = new() { R = 13, G = 209, B = 100, A = 255 };

        public static readonly Color input = new() { R = 18, G = 18, B = 18, A = 255 };
        public static readonly Color inputHover = new() { R = 14, G = 14, B = 14, A = 255 };
        public static readonly Color darkGray = new() { R = 30, G = 30, B = 30, A = 255 };
        public static readonly Color gray = new() { R = 40, G = 40, B = 40, A = 255 };
        public static readonly Color lightGray = new() { R = 48, G = 48, B = 48, A = 255 };
        public static readonly Color fontColor = new() { R = 204, G = 204, B = 204, A = 255 };
        public static readonly Color grayHover = new() { R = 69, G = 69, B = 69, A = 255 };
        public static readonly Color line = new() { R = 55, G = 55, B = 55, A = 255 };
        public static readonly Color scrollBarThumbHover = new() { R = 99, G = 99, B = 99, A = 255 };

        public static readonly Color brightness1 = new() { R = 78, G = 78, B = 78, A = 255 };
        public static readonly Color brightness2 = new() { R = 89, G = 89, B = 89, A = 255 };
        public static readonly Color brightness3 = new() { R = 111, G = 111, B = 111, A = 255 };
        public static readonly Color brightness4 = new() { R = 114, G = 114, B = 114, A = 255 };
        public static readonly Color brightness5 = new() { R = 138, G = 138, B = 138, A = 255 };
        public static readonly Color brightness6 = new() { R = 209, G = 209, B = 209, A = 255 };

        public static readonly SolidColorBrush whiteBrush = new(white);

        public static readonly SolidColorBrush redBrush = new(red);
        public static readonly SolidColorBrush redHoverBrush = new(redHover);

        public static readonly SolidColorBrush greenBrush = new(green);
        public static readonly SolidColorBrush greenHoverBrush = new(greenHover);

        public static readonly SolidColorBrush inputBrush = new(input);
        public static readonly SolidColorBrush inputHoverBrush = new(inputHover);
        public static readonly SolidColorBrush darkGrayBrush = new(darkGray);
        public static readonly SolidColorBrush grayBrush = new(gray);
        public static readonly SolidColorBrush lightGrayBrush = new(lightGray);
        public static readonly SolidColorBrush fontColorBrush = new(fontColor);
        public static readonly SolidColorBrush grayHoverBrush = new(grayHover);
        public static readonly SolidColorBrush lineBrush = new(line);
        public static readonly SolidColorBrush scrollBarThumbHoverBrush = new(scrollBarThumbHover);

        public static readonly SolidColorBrush brightness1Brush = new(brightness1);
        public static readonly SolidColorBrush brightness2Brush = new(brightness2);
        public static readonly SolidColorBrush brightness3Brush = new(brightness3);
        public static readonly SolidColorBrush brightness4Brush = new(brightness4);
        public static readonly SolidColorBrush brightness5Brush = new(brightness5);
        public static readonly SolidColorBrush brightness6Brush = new(brightness6);
    }
}
