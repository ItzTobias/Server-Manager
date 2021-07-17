using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Server_Manager.UIElements
{
    /// <summary>
    /// Interaction logic for ImageButton.xaml
    /// </summary>
    public partial class ImageButton : Button
    {
        private readonly Path defaultIcon;

        public ImageButton()
        {
            InitializeComponent();

            defaultIcon = Resources["DefaultIcon"] as Path;
        }

        public void SetImage(string imagePath)
        {
            if (!System.IO.File.Exists(imagePath))
            {
                Default();
            }
            else
            {
                Custom(imagePath);
            }
        }

        private void Default()
        {
            Content = defaultIcon;
        }
        private void Custom(string imagePath)
        {
            BitmapImage image = new();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(imagePath);
            image.EndInit();

            Content = new Image()
            {
                Source = image
            };
        }
    }
}
