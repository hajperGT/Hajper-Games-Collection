using HGC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Threading.Tasks;

namespace HGC
{
    public partial class GamePhotoGalleryWindow : Window
    {
        private List<CollectionPhoto> photos;
        private CollectionPhoto selectedPhoto;
        private readonly string currentLanguage;

        private CollectionPhoto frontCoverPhoto;
        private CollectionPhoto backCoverPhoto;


        private bool isSearchActive = false;

        public List<CollectionPhoto> Photos => photos;

        public CollectionPhoto FrontCoverPhoto => frontCoverPhoto;
        public CollectionPhoto BackCoverPhoto => backCoverPhoto;

        public GamePhotoGalleryWindow(
            List<CollectionPhoto> photos,
            CollectionPhoto frontCover,
            CollectionPhoto backCover,
            string language = "pl")
        {
            InitializeComponent();

            currentLanguage = language;
            this.photos = photos ?? new List<CollectionPhoto>();

            frontCoverPhoto = frontCover;
            backCoverPhoto = backCover;

            ApplyLanguage();
            RefreshGallery();
        }
        private CollectionPhoto GetMainGamePhoto()
        {
            List<CollectionPhoto> allPhotos = new();

            if (frontCoverPhoto != null)
                allPhotos.Add(frontCoverPhoto);

            if (backCoverPhoto != null)
                allPhotos.Add(backCoverPhoto);

            allPhotos.AddRange(photos);

            CollectionPhoto mainPhoto =
                allPhotos.FirstOrDefault(x => x.IsMain)
                ?? frontCoverPhoto
                ?? backCoverPhoto
                ?? photos.FirstOrDefault();

            if (mainPhoto != null &&
                !allPhotos.Any(x => x.IsMain))
            {
                mainPhoto.IsMain = true;
            }

            return mainPhoto;
        }
        private void ApplyLanguage()
        {
            GalleryTitle.Text =
            currentLanguage == "pl"
                ? "Zdjęcia i okładki gry"
                : "Game photos and covers";

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

            List<CollectionPhoto> displayPhotos = new();

            if (frontCoverPhoto != null &&
            !File.Exists(frontCoverPhoto.OriginalPath))
            {
                frontCoverPhoto = null;
            }

            if (backCoverPhoto != null &&
                !File.Exists(backCoverPhoto.OriginalPath))
            {
                backCoverPhoto = null;
            }

            if (frontCoverPhoto != null)
                displayPhotos.Add(frontCoverPhoto);

            if (backCoverPhoto != null)
                displayPhotos.Add(backCoverPhoto);

            displayPhotos.AddRange(photos);

            if (displayPhotos.Count == 0)
            {
                MainPreviewImage.Source = null;
                MainPreviewImage.Visibility = Visibility.Collapsed;
                NoPhotoPanel.Visibility = Visibility.Visible;

                selectedPhoto = null;
                return;
            }

            NoPhotoPanel.Visibility = Visibility.Collapsed;
            MainPreviewImage.Visibility = Visibility.Visible;

            CollectionPhoto mainPhoto = GetMainGamePhoto();

            if (selectedPhoto == null || !displayPhotos.Contains(selectedPhoto))
                selectedPhoto = mainPhoto ?? displayPhotos.First();

            if (selectedPhoto != null &&
                File.Exists(selectedPhoto.OriginalPath))
            {
                MainPreviewImage.Source =
                    LoadImageWithoutLock(selectedPhoto.OriginalPath);
            }

            foreach (CollectionPhoto photo in displayPhotos)
            {
                bool isMainPhoto = photo == mainPhoto;

                Image thumbnail = new Image
                {
                    Width = 110,
                    Height = 110,
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(0)
                };

                string imagePath =
                    File.Exists(photo.ThumbnailPath)
                        ? photo.ThumbnailPath
                        : photo.OriginalPath;

                if (File.Exists(imagePath))
                    thumbnail.Source = LoadImageWithoutLock(imagePath, 110);

                Grid thumbnailGrid = new Grid
                {
                    Width = 118,
                    Height = 118
                };

                Border imageContainer = new Border
                {
                    Width = 110,
                    Height = 110,
                    CornerRadius = new CornerRadius(8),
                    ClipToBounds = true,
                    Child = thumbnail
                };

                thumbnailGrid.Children.Add(imageContainer);

                string badgeText = "";

                if (photo == frontCoverPhoto)
                    badgeText = currentLanguage == "pl" ? "PRZÓD" : "FRONT";
                else if (photo == backCoverPhoto)
                    badgeText = currentLanguage == "pl" ? "TYŁ" : "BACK";

                if (!string.IsNullOrWhiteSpace(badgeText))
                {
                    Border badge = new Border
                    {
                        CornerRadius = new CornerRadius(4),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(6),
                        Padding = new Thickness(6, 2, 6, 2)
                    };

                    badge.SetResourceReference(Border.BackgroundProperty, "AppAccentBrush");

                    TextBlock badgeTextBlock = new TextBlock
                    {
                        Text = badgeText,
                        FontSize = 10,
                        FontWeight = FontWeights.Bold
                    };

                    badgeTextBlock.SetResourceReference(TextBlock.ForegroundProperty, "ButtonTextBrush");

                    badge.Child = badgeTextBlock;
                    thumbnailGrid.Children.Add(badge);
                }

                if (isMainPhoto)
                {
                    Border mainBadge = new Border
                    {
                        Background = Brushes.Gold,
                        CornerRadius = new CornerRadius(4),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(6),
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
                    BorderThickness = new Thickness(isMainPhoto ? 3 : photo == selectedPhoto ? 2 : 1),
                    BorderBrush = isMainPhoto
                        ? Brushes.Gold
                        : photo == selectedPhoto
                            ? (Brush)FindResource("AppAccentBrush")
                            : (Brush)FindResource("AppBorderBrush"),
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
        private Brush GetCurrentThemeAccentBrush()
        {
            if (Owner is FrameworkElement owner &&
                owner.TryFindResource("AppAccentBrush") is Brush ownerBrush)
            {
                return ownerBrush;
            }

            if (TryFindResource("AppAccentBrush") is Brush localBrush)
                return localBrush;

            return Brushes.Orange;
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
        private void AddFrontCoverButton_Click(object sender, RoutedEventArgs e)
        {
            FrontCoverPopup.IsOpen = true;
        }
        private bool HasAnyMainGamePhoto()
        {
            if (frontCoverPhoto != null && frontCoverPhoto.IsMain)
                return true;

            if (backCoverPhoto != null && backCoverPhoto.IsMain)
                return true;

            return photos.Any(x => x.IsMain);
        }

        private void AddBackCoverButton_Click(object sender, RoutedEventArgs e)
        {
            BackCoverPopup.IsOpen = true;
        }
        private void AddFrontCoverManualMenu_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog =
                new Microsoft.Win32.OpenFileDialog
                {
                    Title = currentLanguage == "pl"
                        ? "Wybierz okładkę przód"
                        : "Select front cover",

                    Filter = "Obrazy|*.bmp;*.gif;*.jpg;*.jpeg;*.png"
                };

            if (dialog.ShowDialog() != true)
                return;

            frontCoverPhoto = CreatePhotoFromFile(dialog.FileName);

            if (!HasAnyMainGamePhoto())
                frontCoverPhoto.IsMain = true;

            RefreshGallery();
        }

        private async void AddFrontCoverRawgMenu_Click(object sender, RoutedEventArgs e)
        {
            string gameTitle = "";

            if (Owner is MainWindow mainWindow)
                gameTitle = mainWindow.GameTitleTextBox.Text.Trim();

            RawgSearchWindow window = new RawgSearchWindow(gameTitle, currentLanguage);
            window.Owner = this;

            if (window.ShowDialog() != true)
                return;

            if (window.SelectedGame == null)
                return;

            if (string.IsNullOrWhiteSpace(window.SelectedGame.BackgroundImage))
                return;

            frontCoverPhoto = await CreatePhotoFromUrlAsync(window.SelectedGame.BackgroundImage);

            if (!HasAnyMainGamePhoto())
                frontCoverPhoto.IsMain = true;

            selectedPhoto = frontCoverPhoto;

            RefreshGallery();
        }
        private async Task<CollectionPhoto> CreatePhotoFromUrlAsync(string imageUrl)
        {
            string photosFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "HGC",
                "Photos");

            string thumbnailsFolder = Path.Combine(
                photosFolder,
                "Thumbnails");

            Directory.CreateDirectory(photosFolder);
            Directory.CreateDirectory(thumbnailsFolder);

            string fileName = $"{Guid.NewGuid()}.jpg";

            string targetPath = Path.Combine(photosFolder, fileName);
            string thumbnailPath = Path.Combine(
                thumbnailsFolder,
                $"{Path.GetFileNameWithoutExtension(fileName)}.png");

            using HttpClient client = new HttpClient();
            byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);

            await File.WriteAllBytesAsync(targetPath, imageBytes);

            CreateThumbnail(targetPath, thumbnailPath);

            return new CollectionPhoto
            {
                OriginalPath = targetPath,
                ThumbnailPath = thumbnailPath
            };
        }
        private void AddBackCoverManualMenu_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog =
                new Microsoft.Win32.OpenFileDialog
                {
                    Title = currentLanguage == "pl"
                        ? "Wybierz okładkę tył"
                        : "Select back cover",

                    Filter = "Obrazy|*.bmp;*.gif;*.jpg;*.jpeg;*.png"
                };

            if (dialog.ShowDialog() != true)
                return;

            backCoverPhoto = CreatePhotoFromFile(dialog.FileName);

            RefreshGallery();
        }
        private async void AddBackCoverRawgMenu_Click(object sender, RoutedEventArgs e)
        {
            string gameTitle = "";

            if (Owner is MainWindow mainWindow)
                gameTitle = mainWindow.GameTitleTextBox.Text.Trim();

            RawgSearchWindow window = new RawgSearchWindow(gameTitle, currentLanguage);
            window.Owner = this;

            if (window.ShowDialog() != true)
                return;

            if (window.SelectedGame == null)
                return;

            if (string.IsNullOrWhiteSpace(window.SelectedGame.BackgroundImage))
                return;

            backCoverPhoto = await CreatePhotoFromUrlAsync(window.SelectedGame.BackgroundImage);

            selectedPhoto = backCoverPhoto;

            RefreshGallery();
        }
        private CollectionPhoto CreatePhotoFromFile(string sourceFile)
        {
            string photosFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "HGC",
                "Photos");

            string thumbnailsFolder = Path.Combine(
                photosFolder,
                "Thumbnails");

            Directory.CreateDirectory(photosFolder);
            Directory.CreateDirectory(thumbnailsFolder);

            string extension = Path.GetExtension(sourceFile);
            string fileName = $"{Guid.NewGuid()}{extension}";

            string targetPath =
                Path.Combine(photosFolder, fileName);

            string thumbnailPath =
                Path.Combine(thumbnailsFolder, fileName);

            File.Copy(sourceFile, targetPath, true);

            CreateThumbnail(targetPath, thumbnailPath);

            return new CollectionPhoto
            {
                OriginalPath = targetPath,
                ThumbnailPath = thumbnailPath
            };
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

            CollectionPhoto removedPhoto = selectedPhoto;

            photos.Remove(removedPhoto);

            if (frontCoverPhoto == removedPhoto)
                frontCoverPhoto = null;

            if (backCoverPhoto == removedPhoto)
                backCoverPhoto = null;

            if (wasMain && photos.Count > 0)
                photos[0].IsMain = true;

            selectedPhoto = null;

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
            if (frontCoverPhoto != null)
                frontCoverPhoto.IsMain = false;

            if (backCoverPhoto != null)
                backCoverPhoto.IsMain = false;

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