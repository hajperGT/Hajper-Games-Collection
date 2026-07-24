using System.Windows;

namespace HGC
{
    public enum PdfPhotoSize
    {
        Small,
        Medium,
        Large
    }

    public partial class PdfExportOptionsWindow : Window
    {
        private readonly string currentLanguage;

        public PdfExportOptionsWindow(string currentLanguage)
        {
            InitializeComponent();

            this.currentLanguage = currentLanguage;

            ApplyLanguage();
        }

        public bool IncludeTitlePage { get; private set; } = true;
        public bool IncludeStatistics { get; private set; } = true;
        public bool IncludeGames { get; private set; } = true;
        public bool IncludeHardware { get; private set; } = true;
        public bool IncludePc { get; private set; } = true;
        public bool IncludeAccessories { get; private set; } = true;
        public bool IncludeService { get; private set; } = true;
        public bool IncludeCurrencyRates { get; private set; } = true;
        public bool IncludePhotos { get; private set; } = true;
        public bool StartSectionsOnNewPage { get; private set; } = true;
        public bool NumberPages { get; private set; } = true;

        public PdfPhotoSize PhotoSize { get; private set; } = PdfPhotoSize.Small;

        private void ApplyLanguage()
        {
            Title = currentLanguage == "pl" ? "Eksport PDF" : "PDF export";

            TitleText.Text = currentLanguage == "pl" ? "Eksport PDF" : "PDF export";
            SubtitleText.Text = currentLanguage == "pl"
                ? "Wybierz elementy raportu"
                : "Choose report elements";

            IncludeTitlePageCheckBox.Content = currentLanguage == "pl" ? "Strona tytułowa" : "Title page";
            IncludeStatisticsCheckBox.Content = currentLanguage == "pl" ? "Statystyki" : "Statistics";
            IncludeGamesCheckBox.Content = currentLanguage == "pl" ? "Gry" : "Games";
            IncludeHardwareCheckBox.Content = currentLanguage == "pl" ? "Sprzęt" : "Hardware";
            IncludePcCheckBox.Content = currentLanguage == "pl" ? "Komputery" : "Computers";
            IncludeAccessoriesCheckBox.Content = currentLanguage == "pl" ? "Akcesoria" : "Accessories";
            IncludeServiceCheckBox.Content = currentLanguage == "pl" ? "Historia serwisu" : "Service history";
            IncludeCurrencyRatesCheckBox.Content = currentLanguage == "pl" ? "Kursy walut" : "Currency rates";

            IncludePhotosCheckBox.Content = currentLanguage == "pl" ? "Dodaj zdjęcia główne" : "Include main photos";
           
            StartSectionsOnNewPageCheckBox.Content = currentLanguage == "pl"
                ? "Rozpoczynaj każdą sekcję od nowej strony"
                : "Start each section on a new page";

            NumberPagesCheckBox.Content = currentLanguage == "pl"
                ? "Numeruj strony"
                : "Number pages";

            GenerateButton.Content = currentLanguage == "pl" ? "Generuj" : "Generate";
            CancelButton.Content = currentLanguage == "pl" ? "Anuluj" : "Cancel";
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            IncludeTitlePage = IncludeTitlePageCheckBox.IsChecked == true;
            IncludeStatistics = IncludeStatisticsCheckBox.IsChecked == true;
            IncludeGames = IncludeGamesCheckBox.IsChecked == true;
            IncludeHardware = IncludeHardwareCheckBox.IsChecked == true;
            IncludePc = IncludePcCheckBox.IsChecked == true;
            IncludeAccessories = IncludeAccessoriesCheckBox.IsChecked == true;
            IncludeService = IncludeServiceCheckBox.IsChecked == true;
            IncludeCurrencyRates = IncludeCurrencyRatesCheckBox.IsChecked == true;
            IncludePhotos = IncludePhotosCheckBox.IsChecked == true;
            StartSectionsOnNewPage = StartSectionsOnNewPageCheckBox.IsChecked == true;
            NumberPages = NumberPagesCheckBox.IsChecked == true;

            PhotoSize = PdfPhotoSize.Small;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}