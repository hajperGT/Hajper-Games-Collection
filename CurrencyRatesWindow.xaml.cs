using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HGC.Models;

namespace HGC
{
    public partial class CurrencyRatesWindow : Window
    {
        private readonly CollectionDatabase collection;
        private readonly string defaultCurrency;
        private readonly string currentLanguage;

        private readonly Dictionary<string, TextBox> rateTextBoxes = new();

        private readonly string[] supportedCurrencies =
        {
            "PLN",
            "EUR",
            "USD",
            "GBP",
            "CHF",
            "HUF",
            "CZK",
            "JPY",
            "CNY"
        };

        internal CurrencyRatesWindow(
            CollectionDatabase collection,
            string defaultCurrency,
            string currentLanguage)
        {
            InitializeComponent();

            this.collection = collection;
            this.defaultCurrency = defaultCurrency;
            this.currentLanguage = currentLanguage;

            ApplyLanguage();
            BuildRateRows();
        }

        private void ApplyLanguage()
        {
            TitleText.Text = currentLanguage == "pl"
                ? "Kursy walut"
                : "Currency rates";

            HeaderText.Text = currentLanguage == "pl"
                ? "Kursy walut"
                : "Currency rates";

            SubtitleText.Text = currentLanguage == "pl"
                ? $"Kursy względem waluty domyślnej: {defaultCurrency}"
                : $"Rates relative to default currency: {defaultCurrency}";

            SaveButton.Content = currentLanguage == "pl"
                ? "Zapisz"
                : "Save";

            CloseButton.Content = currentLanguage == "pl"
                ? "Zamknij"
                : "Close";
        }

        private void BuildRateRows()
        {
            RatesPanel.Children.Clear();
            rateTextBoxes.Clear();

            foreach (string currency in supportedCurrencies)
            {
                Grid row = new()
                {
                    Margin = new Thickness(0, 0, 0, 8)
                };

                row.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(70)
                });

                row.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(140)
                });

                TextBlock currencyText = new()
                {
                    Text = currency,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = (Brush)FindResource("AppTextBrush")
                };

                TextBox rateTextBox = new()
                {
                    Width = 130,
                    Height = 30,
                    Padding = new Thickness(8, 3, 8, 3),
                    Text = GetRateText(currency),
                    IsEnabled = currency != defaultCurrency,
                    Foreground = (Brush)FindResource("AppTextBrush"),
                    Background = (Brush)FindResource("ContentBackgroundBrush"),
                    BorderBrush = (Brush)FindResource("AppBorderBrush"),
                    BorderThickness = new Thickness(1),
                    VerticalContentAlignment = VerticalAlignment.Center
                };

                Grid.SetColumn(currencyText, 0);
                Grid.SetColumn(rateTextBox, 1);

                row.Children.Add(currencyText);
                row.Children.Add(rateTextBox);

                RatesPanel.Children.Add(row);
                rateTextBoxes[currency] = rateTextBox;
            }
        }

        private string GetRateText(string currency)
        {
            if (currency == defaultCurrency)
                return "1.00";

            CurrencyRate? rate = collection.CurrencyRates
                .FirstOrDefault(r => r.Currency == currency);

            return rate != null
                ? rate.Rate.ToString("0.####", CultureInfo.InvariantCulture)
                : "";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var pair in rateTextBoxes)
            {
                string currency = pair.Key;
                TextBox textBox = pair.Value;

                if (currency == defaultCurrency)
                    continue;

                if (!decimal.TryParse(
                        textBox.Text.Replace(",", "."),
                        NumberStyles.Number,
                        CultureInfo.InvariantCulture,
                        out decimal rate))
                {
                    HgcMessageBox.Show(
                        this,
                        currentLanguage == "pl"
                            ? $"Nieprawidłowy kurs dla waluty {currency}."
                            : $"Invalid exchange rate for currency {currency}.",
                        "HGC",
                        HgcMessageBoxType.Warning,
                        currentLanguage);

                    return;
                }

                CurrencyRate? existingRate = collection.CurrencyRates
                    .FirstOrDefault(r => r.Currency == currency);

                if (existingRate == null)
                {
                    collection.CurrencyRates.Add(new CurrencyRate
                    {
                        Currency = currency,
                        Rate = rate
                    });
                }
                else
                {
                    existingRate.Rate = rate;
                }
            }

            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}