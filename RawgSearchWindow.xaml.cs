using HGC.Services;
using System.Windows;
using System.Windows.Input;

namespace HGC
{
    public partial class RawgSearchWindow : Window
    {
        private readonly string currentLanguage;

        public RawgGameResult? SelectedGame { get; private set; }

        public RawgSearchWindow(string initialSearchText, string language = "pl")
        {
            InitializeComponent();

            currentLanguage = language;

            SearchTextBox.Text = initialSearchText;
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            WindowTitleText.Text =
                currentLanguage == "pl"
                    ? "Pobierz okładkę z RAWG"
                    : "Download cover from RAWG";

            SearchButton.Content =
                currentLanguage == "pl"
                    ? "Szukaj"
                    : "Search";

            CancelButton.Content =
                currentLanguage == "pl"
                    ? "Anuluj"
                    : "Cancel";

            SelectButton.Content =
                currentLanguage == "pl"
                    ? "Wybierz"
                    : "Select";
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResultsListBox.SelectedItem is not RawgGameResult selectedGame)
                return;

            SelectedGame = selectedGame;
            DialogResult = true;
            Close();
        }
        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            SearchButton_Click(SearchButton, new RoutedEventArgs());
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ResultsListBox.Items.Clear();

            var results = await RawgService.SearchGamesAsync(SearchTextBox.Text);

            foreach (var game in results)
            {
                ResultsListBox.Items.Add(game);
            }
        }
        private void ResultsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectButton_Click(sender, e);
        }
    }
}