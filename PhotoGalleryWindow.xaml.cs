using HGC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HGC
{
    public partial class PhotoGalleryWindow : Window
    {
        private List<CollectionPhoto> photos;
        private CollectionPhoto selectedPhoto;
        private readonly string currentLanguage;

        public List<CollectionPhoto> Photos => photos;

        public PhotoGalleryWindow(
        List<CollectionPhoto> photos,
        string language = "pl",
        CollectionPhoto startPhoto = null)
        {
            InitializeComponent();

            currentLanguage = language;
            this.photos = photos ?? new List<CollectionPhoto>();

            selectedPhoto = startPhoto;

            ApplyLanguage();
            RefreshGallery();
        }
        private void ApplyLanguage()
        {
            GalleryTitle.Text =
                currentLanguage == "pl" ? "Galeria zdjęć" : "Photo gallery";

            NoPhotoTitle.Text =
                currentLanguage == "pl" ? "Brak zdjęć" : "No photos";

            AddPhotoButton.Content =
                currentLanguage == "pl" ? "Dodaj zdjęcie" : "Add photo";

            DeletePhotoButton.Content =
                currentLanguage == "pl" ? "Usuń zdjęcie" : "Delete photo";

            MovePhotoUpButton.Content =
                currentLanguage == "pl" ? "Przesuń wyżej" : "Move up";

            MovePhotoDownButton.Content =
                currentLanguage == "pl" ? "Przesuń niżej" : "Move down";

            SetMainPhotoButton.Content =
                currentLanguage == "pl" ? "Ustaw jako główne" : "Set as main";

            CancelButton.Content =
                currentLanguage == "pl" ? "Anuluj" : "Cancel";

            FinishButton.Content =
                currentLanguage == "pl" ? "Zakończ" : "Finish";
        }

        private void RefreshGallery()
        {
            ThumbnailsPanel.Children.Clear();

            if (photos.Count == 0)
            {
                MainPreviewImage.Source = null;
                MainPreviewImage.Visibility = Visibility.Collapsed;
                NoPhotoPanel.Visibility = Visibility.Visible;

                selectedPhoto = null;
                return;
            }

            NoPhotoPanel.Visibility = Visibility.Collapsed;
            MainPreviewImage.Visibility = Visibility.Visible;

            if (selectedPhoto == null || !photos.Contains(selectedPhoto))
            {
                selectedPhoto =
                    photos.FirstOrDefault(p => p.IsMain)
                    ?? photos.First();
            }

            if (selectedPhoto != null &&
                File.Exists(selectedPhoto.OriginalPath))
            {
                MainPreviewImage.Source =
                    LoadImageWithoutLock(selectedPhoto.OriginalPath);
            }

            foreach (CollectionPhoto photo in photos)
            {
                Image thumbnail = new Image
                {
                    Width = 110,
                    Height = 110,
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(4)
                };

                string imagePath =
                    File.Exists(photo.ThumbnailPath)
                        ? photo.ThumbnailPath
                        : photo.OriginalPath;

                if (File.Exists(imagePath))
                {
                    thumbnail.Source =
                        LoadImageWithoutLock(imagePath, 110);
                }

                Grid thumbnailGrid = new Grid();

                thumbnailGrid.Children.Add(thumbnail);

                if (photo.IsMain)
                {
                    Border mainBadge = new Border
                    {
                        Background = Brushes.Gold,
                        CornerRadius = new CornerRadius(4),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(4),
                        Padding = new Thickness(6, 2, 6, 2)
                    };

                    mainBadge.Child = new TextBlock
                    {
                        Text = "★",
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.Black
                    };

                    thumbnailGrid.Children.Add(mainBadge);
                }

                Border border = new Border
                {
                    BorderThickness = new Thickness(photo == selectedPhoto ? 3 : 1),
                    BorderBrush = photo == selectedPhoto
                        ? Brushes.DodgerBlue
                        : Brushes.Gray,
                    Child = thumbnailGrid,
                    Margin = new Thickness(4),
                    Cursor = Cursors.Hand
                };

                border.MouseLeftButtonUp += (s, e) =>
                {
                    selectedPhoto = photo;
                    RefreshGallery();
                };

                ThumbnailsPanel.Children.Add(border);
            }
        }
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Wybierz zdjęcie",
                Filter = "Obrazy|*.bmp;*.gif;*.jpg;*.jpeg;*.png"
            };

            if (dialog.ShowDialog() != true)
                return;

            string photosFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "HGC",
                "Photos");

            string thumbnailsFolder = Path.Combine(
                photosFolder,
                "Thumbnails");

            Directory.CreateDirectory(photosFolder);
            Directory.CreateDirectory(thumbnailsFolder);

            string extension = Path.GetExtension(dialog.FileName);
            string fileName = $"{Guid.NewGuid()}{extension}";

            string targetPath = Path.Combine(photosFolder, fileName);
            string thumbnailPath = Path.Combine(
                thumbnailsFolder,
                $"{Path.GetFileNameWithoutExtension(fileName)}.png");

            File.Copy(dialog.FileName, targetPath, true);
            CreateThumbnail(targetPath, thumbnailPath);

            CollectionPhoto photo = new CollectionPhoto
            {
                OriginalPath = targetPath,
                ThumbnailPath = thumbnailPath,
                IsMain = photos.Count == 0
            };

            photos.Add(photo);
            selectedPhoto = photo;

            RefreshGallery();
        }
        private void CreateThumbnail(string sourcePath, string thumbnailPath)
        {
            BitmapImage image = new BitmapImage();

            image.BeginInit();
            image.UriSource = new Uri(sourcePath);
            image.DecodePixelWidth = 240;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using FileStream stream = new FileStream(thumbnailPath, FileMode.Create);
            encoder.Save(stream);
        }

        private void DeletePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPhoto == null)
                return;

            bool confirm = HgcMessageBox.ShowYesNo(
                this,
                "Czy na pewno usunąć wybrane zdjęcie?",
                "HGC",
                HgcMessageBoxType.Warning,
                "pl");

            if (!confirm)
                return;

            bool wasMain = selectedPhoto.IsMain;

            if (File.Exists(selectedPhoto.OriginalPath))
                File.Delete(selectedPhoto.OriginalPath);

            if (File.Exists(selectedPhoto.ThumbnailPath))
                File.Delete(selectedPhoto.ThumbnailPath);

            photos.Remove(selectedPhoto);

            if (wasMain && photos.Count > 0)
                photos[0].IsMain = true;

            selectedPhoto = photos.FirstOrDefault();

            RefreshGallery();
        }
        private BitmapImage LoadImageWithoutLock(string path, int decodePixelWidth = 0)
        {
            BitmapImage image = new BitmapImage();

            image.BeginInit();
            image.UriSource = new Uri(path);
            image.CacheOption = BitmapCacheOption.OnLoad;

            if (decodePixelWidth > 0)
                image.DecodePixelWidth = decodePixelWidth;

            image.EndInit();
            image.Freeze();

            return image;
        }

        private void MovePhotoUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPhoto == null)
                return;

            int index = photos.IndexOf(selectedPhoto);

            if (index <= 0)
                return;

            photos.RemoveAt(index);
            photos.Insert(index - 1, selectedPhoto);

            RefreshGallery();
        }
        private void MovePhotoDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPhoto == null)
                return;

            int index = photos.IndexOf(selectedPhoto);

            if (index < 0 || index >= photos.Count - 1)
                return;

            photos.RemoveAt(index);
            photos.Insert(index + 1, selectedPhoto);

            RefreshGallery();
        }

        private void SetMainPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPhoto == null)
                return;

            foreach (CollectionPhoto photo in photos)
                photo.IsMain = false;

            selectedPhoto.IsMain = true;

            RefreshGallery();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}