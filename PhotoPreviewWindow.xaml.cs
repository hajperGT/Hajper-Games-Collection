using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HGC
{
    public partial class PhotoPreviewWindow : Window
    {
        private readonly string imagePath;
        private readonly string currentLanguage;

        public PhotoPreviewWindow(
            string imagePath,
            string language = "pl")
        {
            InitializeComponent();

            this.imagePath = imagePath;
            currentLanguage = language;

            ApplyLanguage();
            LoadImage();
        }

        private void ApplyLanguage()
        {
            PreviewTitle.Text =
                currentLanguage == "pl"
                    ? "Podgląd zdjęcia"
                    : "Photo preview";
        }

        private void LoadImage()
        {
            if (string.IsNullOrWhiteSpace(imagePath) ||
                !File.Exists(imagePath))
                return;

            PreviewImage.Source =
                LoadImageWithoutLock(imagePath);
        }

        private BitmapImage LoadImageWithoutLock(string path)
        {
            BitmapImage image = new BitmapImage();

            image.BeginInit();
            image.UriSource = new Uri(path);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();

            return image;
        }
        private void TitleBar_MouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Button)
                return;

            if (e.ClickCount == 2)
            {
                ToggleMaximize();
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }

        private void ToggleMaximize()
        {
            WindowState =
                WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}