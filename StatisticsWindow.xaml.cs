using Hajper_Game_Collection.Data;
using HGC.Models;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading.Tasks;

namespace HGC
{
    public partial class StatisticsWindow : Window
    {
        private readonly CollectionDatabase collection;
        private readonly string currentLanguage;
        private readonly string defaultCurrency;

        private enum GameChartMode
        {
            Count,
            Cost
        }

        private GameChartMode currentGameChartMode = GameChartMode.Count;

        private sealed class ChartSlice
        {
            public string Name { get; set; } = "";
            public decimal Value { get; set; }
            public Color Color { get; set; }
        }

        internal StatisticsWindow(
        CollectionDatabase collection,
        string currentLanguage,
        string defaultCurrency)
        {
            InitializeComponent();

            this.collection = collection;
            this.currentLanguage = currentLanguage;
            this.defaultCurrency = defaultCurrency;

            ApplyLanguage();
            BuildStats();
        }

        private void ApplyLanguage()
        {
            Title = currentLanguage == "pl"
                ? "Statystyki"
                : "Statistics";

            TitleText.Text = currentLanguage == "pl"
                ? "Statystyki kolekcji"
                : "Collection statistics";

            ExportTxtButton.Content = currentLanguage == "pl"
                ? "Eksport TXT"
                : "TXT export";

            ExportPdfButton.Content = currentLanguage == "pl"
                ? "Eksport PDF"
                : "PDF export";

            CloseButton.Content = currentLanguage == "pl"
                ? "Zamknij"
                : "Close";

            HeaderText.Text = currentLanguage == "pl"
                ? "Statystyki Twojej kolekcji"
                : "Your Collection Statistics";

            SubtitleText.Text = currentLanguage == "pl"
                ? "Podsumowanie danych zapisanych w Hajper Games Collection"
                : "Summary of data stored in Hajper Games Collection";

            CurrencyRatesButton.Content = currentLanguage == "pl"
                ? "Kursy walut"
                : "Currency rates";
        }
        private string T(string pl, string en)
        {
            return currentLanguage == "pl" ? pl : en;

        }
        private void AddStatisticsGroup(
         List<StatLine> lines,
         string title,
         Dictionary<string, decimal> values,
         string totalLabel,
         bool translateServiceTypes = false)
        {
            lines.Add(new StatLine(title, true));

            foreach (var item in values
                .Where(x => x.Value > 0)
                .OrderBy(x => x.Key))
            {
                string name = translateServiceTypes
                    ? ServiceTypeDatabase.Translate(item.Key, currentLanguage)
                    : item.Key;

                lines.Add(new StatLine(
                    $"{name}: {item.Value:N2} {defaultCurrency}"));
            }

            decimal total = values.Sum(x => x.Value);

            lines.Add(new StatLine(
                $"{totalLabel}: {total:N2} {defaultCurrency}",
                true));
        }
        private decimal ConvertCurrency(
        decimal amount,
        string fromCurrency,
        string toCurrency)
        {
            if (string.IsNullOrWhiteSpace(fromCurrency))
                fromCurrency = defaultCurrency;

            if (string.IsNullOrWhiteSpace(toCurrency))
                toCurrency = defaultCurrency;

            fromCurrency = fromCurrency.Trim().ToUpper();
            toCurrency = toCurrency.Trim().ToUpper();

            if (fromCurrency == toCurrency)
                return amount;

            decimal fromRate = GetCurrencyRate(fromCurrency);
            decimal toRate = GetCurrencyRate(toCurrency);

            decimal amountInBase = amount * fromRate;

            return amountInBase / toRate;
        }
        private decimal GetTotalValue<T>(
        IEnumerable<T> items,
        Func<T, decimal?> priceSelector,
        Func<T, string> currencySelector)
        {
            decimal total = 0m;

            foreach (T item in items)
            {
                decimal? price = priceSelector(item);

                if (!price.HasValue)
                    continue;

                total += ConvertCurrency(
                    price.Value,
                    currencySelector(item),
                    defaultCurrency);
            }

            return total;
        }
        private Dictionary<string, decimal> GetGroupedStatistics<T>(
        IEnumerable<T> items,
        Func<T, string?> groupSelector,
        Func<T, decimal?> valueSelector,
        Func<T, string> currencySelector,
        string emptyGroupName)
        {
            Dictionary<string, decimal> totals = new();

            foreach (T item in items)
            {
                decimal? value = valueSelector(item);

                if (!value.HasValue)
                    continue;

                string groupName = groupSelector(item) ?? "";

                string key = string.IsNullOrWhiteSpace(groupName)
                    ? emptyGroupName
                    : groupName.Trim();

                decimal convertedValue = ConvertCurrency(
                    value.Value,
                    currencySelector(item),
                    defaultCurrency);

                if (!totals.ContainsKey(key))
                    totals[key] = 0m;

                totals[key] += convertedValue;
            }

            return totals;
        }

        private decimal GetCurrencyRate(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                return 1m;

            currency = currency.Trim().ToUpper();

            if (currency == defaultCurrency)
                return 1m;

            CurrencyRate? rate = collection.CurrencyRates
                .FirstOrDefault(r => r.Currency == currency);

            return rate?.Rate ?? 1m;
        }
        private void BuildStats()
        {
            LeftStatsPanel.Children.Clear();
            RightStatsPanel.Children.Clear();
            GameChartsPanel.Children.Clear();

            int gamesCount = collection.Games.Count;
            int pcCount = collection.PcSystems.Count;
            int hardwareCount = collection.HardwareItems.Count;
            int accessoryCount = collection.Accessories.Count;

            decimal gamesTotalValue =
                GetTotalValue(
                    collection.Games,
                    g => g.PurchasePrice,
                    g => g.PurchaseCurrency);

            decimal hardwareTotalValue =
                GetTotalValue(
                    collection.HardwareItems,
                    h => h.PurchasePrice,
                    h => h.PurchaseCurrency);

            decimal pcTotalValue =
                GetTotalValue(
                    collection.PcSystems,
                    p => p.PurchasePrice,
                    p => p.PurchaseCurrency);

            decimal accessoriesTotalValue =
                GetTotalValue(
                    collection.Accessories,
                    a => a.PurchasePrice,
                    a => a.PurchaseCurrency);

            decimal collectionTotalValue =
                gamesTotalValue +
                hardwareTotalValue +
                pcTotalValue +
                accessoriesTotalValue;

            Dictionary<string, decimal> manufacturerTotals =
                BuildManufacturerTotals();

            Dictionary<string, decimal> serviceTotals =
                BuildServiceTotals();

            decimal serviceTotalValue =
                serviceTotals.Sum(x => x.Value);

            decimal totalCollectionValue =
                collectionTotalValue + serviceTotalValue;

            Dictionary<string, decimal> originalCurrencyTotals =
                GetOriginalCurrencyTotals();

            BuildBasicStatistics(
                gamesCount,
                pcCount,
                hardwareCount,
                accessoryCount);

            BuildGameStatistics(gamesCount);

            BuildHardwareStatistics(
                hardwareCount,
                pcCount,
                accessoryCount);

            BuildCostStatistics(
                gamesTotalValue,
                hardwareTotalValue,
                pcTotalValue,
                accessoriesTotalValue,
                collectionTotalValue,
                manufacturerTotals,
                serviceTotals);

            // Stary panel wartości kolekcji chwilowo pomijamy.
            // BuildCollectionValueStatistics(
            //     totalCollectionValue,
            //     originalCurrencyTotals);

            BuildGameChartsPanel();

            BuildCollectionValuePanel(totalCollectionValue);
        }

        private void BuildGameChartsPanel()
        {
            Color statsColor =
                ((SolidColorBrush)FindResource("StatsBackgroundBrush")).Color;

            Border sectionBorder = new()
            {
                Margin = new Thickness(0, 0, 0, 22),
                Padding = new Thickness(14),
                CornerRadius = new CornerRadius(10),

                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 315,
                    ShadowDepth = 7,
                    BlurRadius = 18,
                    Opacity = 0.48
                },

                Background = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1),
                    GradientStops =
            {
                new GradientStop(LightenColor(statsColor, 0.08), 0.0),
                new GradientStop(statsColor, 0.55),
                new GradientStop(DarkenColor(statsColor, 0.14), 1.0)
            }
                },

                BorderBrush = (Brush)FindResource("AppBorderBrush"),
                BorderThickness = new Thickness(1)
            };

            StackPanel panel = new();

            Color accentColor =
                ((SolidColorBrush)FindResource("AppAccentBrush")).Color;

            Border headerBorder = new()
            {
                Padding = new Thickness(10, 6, 10, 6),
                Margin = new Thickness(-14, -14, -14, 12),
                Background = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1),
                    GradientStops =
            {
                new GradientStop(LightenColor(accentColor, 0.25), 0.0),
                new GradientStop(accentColor, 0.55),
                new GradientStop(DarkenColor(accentColor, 0.30), 1.0)
            }
                },
                CornerRadius = new CornerRadius(10, 10, 0, 0),

                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 10,
                    ShadowDepth = 2,
                    Opacity = 0.35
                },
            };

            headerBorder.Child = new TextBlock
            {
                Text = T("Analiza gier", "Game analysis").ToUpper(),
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)FindResource("ButtonTextBrush")
            };


            StackPanel modePanel = new()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 4)
            };

            modePanel.Children.Add(new TextBlock
            {
                Text = T("Sortuj według:", "Sort by:"),
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 12.5,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)FindResource("AppTextBrush"),
                Margin = new Thickness(0, 0, 8, 0)
            });

            ComboBox modeCombo = new()
            {
                Width = 110,
                Height = 24,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };

            modeCombo.Items.Add(T("Ilość", "Quantity"));
            modeCombo.Items.Add(T("Koszt", "Cost"));
            modeCombo.SelectedIndex =
                currentGameChartMode == GameChartMode.Cost
                    ? 1
                    : 0;

            modeCombo.SelectionChanged += (s, e) =>
            {
                currentGameChartMode =
                    modeCombo.SelectedIndex == 1
                        ? GameChartMode.Cost
                        : GameChartMode.Count;

                BuildStats();
            };

            modePanel.Children.Add(modeCombo);

            panel.Children.Add(headerBorder);

            Grid chartsGrid = new()
            {
                Margin = new Thickness(0, 0, 0, 12)
            };

            chartsGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1.0, GridUnitType.Star)
            });

            chartsGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(16)
            });

            chartsGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1.18, GridUnitType.Star)
            });

            StackPanel leftChartsPanel = new();

            leftChartsPanel.Children.Add(CreateGameChartBlock(
                T("Gry według producenta", "Games by manufacturer"),
                BuildGameManufacturerChartSlices()));

            leftChartsPanel.Children.Add(CreateGameChartBlock(
                T("Gry według gatunku", "Games by genre"),
                BuildGameGenreChartSlices()));

            StackPanel platformChartPanel = new()
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            platformChartPanel.Children.Add(CreateGameChartBlock(
                T("Gry według platform", "Games by platform"),
                BuildGamePlatformChartSlices(),
                230));

            Grid.SetColumn(leftChartsPanel, 0);
            Grid.SetColumn(platformChartPanel, 2);

            chartsGrid.Children.Add(leftChartsPanel);
            chartsGrid.Children.Add(platformChartPanel);

            panel.Children.Add(chartsGrid);
            panel.Children.Add(modePanel);


            sectionBorder.Child = panel;
            GameChartsPanel.Children.Add(sectionBorder);
        }
        private Border CreateGameChartCard(string title, string placeholderText)
        {
            Border card = new()
            {
                Margin = new Thickness(0, 0, 0, 14),
                Padding = new Thickness(12),
                CornerRadius = new CornerRadius(9),
                Background = (Brush)FindResource("PreviewBoxBrush"),
                BorderBrush = (Brush)FindResource("AppBorderBrush"),
                BorderThickness = new Thickness(1),
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 315,
                    ShadowDepth = 4,
                    BlurRadius = 12,
                    Opacity = 0.35
                }
            };

            StackPanel root = new();

            root.Children.Add(new TextBlock
            {
                Text = title.ToUpper(),
                FontSize = 12.5,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)FindResource("AppTextBrush"),
                Margin = new Thickness(0, 0, 0, 10)
            });

            Grid chartGrid = new();

            chartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
            chartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12) });
            chartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            Canvas chartCanvas = new()
            {
                Width = 150,
                Height = 150,
                Background = Brushes.Transparent
            };

            TextBlock legendPlaceholder = new()
            {
                Text = placeholderText,
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = (Brush)FindResource("AppTextBrush")
            };

            Grid.SetColumn(chartCanvas, 0);
            Grid.SetColumn(legendPlaceholder, 2);

            chartGrid.Children.Add(chartCanvas);
            chartGrid.Children.Add(legendPlaceholder);

            root.Children.Add(chartGrid);

            card.Child = root;

            return card;
        }
        private StackPanel CreateGameChartBlock(
        string title,
        List<ChartSlice> slices)
        {
            StackPanel root = new()
            {
                Margin = new Thickness(0, 0, 0, 18)
            };

            root.Children.Add(new TextBlock
            {
                Text = title.ToUpper(),
                FontSize = 12.5,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)FindResource("AppTextBrush"),
                Margin = new Thickness(0, 0, 0, 8)
            });

            Grid chartGrid = new();

            chartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(190) });
            chartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12) });
            chartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            Canvas chartCanvas = new()
            {
                Width = 190,
                Height = 190,
                Background = Brushes.Transparent
            };

            chartCanvas.RenderTransformOrigin = new Point(0.5, 0.5);

            ScaleTransform scale = new ScaleTransform(0.90, 0.90);
            chartCanvas.RenderTransform = scale;

            chartCanvas.Opacity = 0;

            StackPanel legendPanel = new()
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetColumn(chartCanvas, 0);
            Grid.SetColumn(legendPanel, 2);

            chartGrid.Children.Add(chartCanvas);
            chartGrid.Children.Add(legendPanel);

            root.Children.Add(chartGrid);

            DrawDonutChart(
            chartCanvas,
            legendPanel,
            EnsureChartHasData(slices));

            AnimateChart(chartCanvas);

            return root;
        }
        private StackPanel CreateGameChartBlock(
        string title,
        List<ChartSlice> slices,
        double chartSize)
        {
            StackPanel root = new()
            {
                Margin = new Thickness(0, 0, 0, 18)
            };

            root.Children.Add(new TextBlock
            {
                Text = title.ToUpper(),
                FontSize = 12.5,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)FindResource("AppTextBrush"),
                Margin = new Thickness(0, 0, 0, 8)
            });

            Grid chartGrid = new();

            chartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(chartSize) });
            chartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12) });
            chartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            Canvas chartCanvas = new()
            {
                Width = chartSize,
                Height = chartSize,
                Background = Brushes.Transparent
            };

            chartCanvas.RenderTransformOrigin = new Point(0.5, 0.5);

            ScaleTransform scale = new ScaleTransform(0.90, 0.90);
            chartCanvas.RenderTransform = scale;

            chartCanvas.Opacity = 0;

            StackPanel legendPanel = new()
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetColumn(chartCanvas, 0);
            Grid.SetColumn(legendPanel, 2);

            chartGrid.Children.Add(chartCanvas);
            chartGrid.Children.Add(legendPanel);

            root.Children.Add(chartGrid);

            DrawDonutChart(chartCanvas, legendPanel, EnsureChartHasData(slices));

            AnimateChart(chartCanvas);

            return root;
        }
        private List<ChartSlice> BuildGamePlatformChartSlices()
        {
            Dictionary<string, decimal> values = new();

            foreach (Game game in collection.Games)
            {
                string platform = string.IsNullOrWhiteSpace(game.Platform)
                    ? T("Niesklasyfikowane", "Unclassified")
                    : game.Platform.Trim();

                decimal value;

                if (currentGameChartMode == GameChartMode.Count)
                {
                    value = 1m;
                }
                else
                {
                    if (!game.PurchasePrice.HasValue || game.PurchasePrice.Value <= 0)
                        continue;

                    value = ConvertCurrency(
                        game.PurchasePrice.Value,
                        game.PurchaseCurrency,
                        defaultCurrency);
                }

                if (!values.ContainsKey(platform))
                    values[platform] = 0m;

                values[platform] += value;
            }

            return BuildChartSlicesAll(values);
        }
        private List<ChartSlice> BuildChartSlicesAll(
        Dictionary<string, decimal> values)
        {
            var ordered = values
                .Where(x => x.Value > 0)
                .OrderByDescending(x => x.Value)
                .ThenBy(x => x.Key)
                .ToList();

            if (ordered.Count == 0)
                return new List<ChartSlice>();

            List<Color> colors = BuildAccentPalette(ordered.Count);

            return ordered
                .Select((x, index) => new ChartSlice
                {
                    Name = x.Key,
                    Value = x.Value,
                    Color = colors[index]
                })
                .ToList();
        }
        private Dictionary<string, decimal> BuildManufacturerTotals()
        {
            Dictionary<string, decimal> manufacturerTotals = new();

            foreach (var group in new[]
            {
            GetGroupedStatistics(
                collection.Games,
                g => g.Manufacturer,
                g => g.PurchasePrice,
                g => g.PurchaseCurrency,
                T("Niesklasyfikowane", "Unclassified")),

            GetGroupedStatistics(
                collection.HardwareItems,
                h => h.Manufacturer,
                h => h.PurchasePrice,
                h => h.PurchaseCurrency,
                T("Niesklasyfikowane", "Unclassified")),

            GetGroupedStatistics(
                collection.PcSystems,
                p => p.Manufacturer,
                p => p.PurchasePrice,
                p => p.PurchaseCurrency,
                T("Niesklasyfikowane", "Unclassified")),

            GetGroupedStatistics(
                collection.Accessories,
                a => a.Manufacturer,
                a => a.PurchasePrice,
                a => a.PurchaseCurrency,
                T("Niesklasyfikowane", "Unclassified"))
        })
            {
                foreach (var item in group)
                {
                    if (!manufacturerTotals.ContainsKey(item.Key))
                        manufacturerTotals[item.Key] = 0m;

                    manufacturerTotals[item.Key] += item.Value;
                }
            }

            return manufacturerTotals;
        }
        private Dictionary<string, decimal> BuildServiceTotals()
        {
            var allServiceEntries = collection.HardwareItems
                .SelectMany(h => h.ServiceHistory)
                .Concat(collection.PcSystems.SelectMany(p => p.ServiceHistory));

            return GetGroupedStatistics(
                allServiceEntries,
                s => s.ServiceType,
                s => s.Cost,
                s => s.CostCurrency,
                T("Inne", "Other"));
        }
        private void BuildCollectionValueStatistics(
        decimal totalCollectionValue,
        Dictionary<string, decimal> originalCurrencyTotals)
        {
            bool hasMultipleCurrencies = originalCurrencyTotals.Count > 1;

            var totalValueLines = new List<StatLine>
        {
        new(T(
            hasMultipleCurrencies
                ? $"Łącznie: {totalCollectionValue:N2} {defaultCurrency}, w tym:"
                : $"Łącznie: {totalCollectionValue:N2} {defaultCurrency}",
            hasMultipleCurrencies
                ? $"Total: {totalCollectionValue:N2} {defaultCurrency}, including:"
                : $"Total: {totalCollectionValue:N2} {defaultCurrency}"),
            true)
        };

            if (hasMultipleCurrencies)
            {
                foreach (var currency in originalCurrencyTotals.OrderBy(x => x.Key))
                {
                    totalValueLines.Add(new StatLine(
                        $"{currency.Key}: {currency.Value:N2}"));
                }
            }

            AddSection(
                T("Łączna wartość kolekcji (z serwisem)",
                 "Total collection value (including service)"),
                totalValueLines,
                true,
                true);
        }
        private void BuildCostStatistics(
        decimal gamesTotalValue,
        decimal hardwareTotalValue,
        decimal pcTotalValue,
        decimal accessoriesTotalValue,
        decimal collectionTotalValue,
        Dictionary<string, decimal> manufacturerTotals,
        Dictionary<string, decimal> serviceTotals)
        {
            var costLines = new List<StatLine>
        {
        new(T("Statystyki według kosztów gier i sprzętu", "Statistics by games and hardware cost"), true),

        new(T(
            $"Gry: {gamesTotalValue:N2} {defaultCurrency}",
            $"Games: {gamesTotalValue:N2} {defaultCurrency}")),

        new(T(
            $"Konsole: {hardwareTotalValue:N2} {defaultCurrency}",
            $"Consoles: {hardwareTotalValue:N2} {defaultCurrency}")),

        new(T(
            $"Komputery: {pcTotalValue:N2} {defaultCurrency}",
            $"Computers: {pcTotalValue:N2} {defaultCurrency}")),

        new(T(
            $"Akcesoria: {accessoriesTotalValue:N2} {defaultCurrency}",
            $"Accessories: {accessoriesTotalValue:N2} {defaultCurrency}")),

        new(T(
            $"Razem: {collectionTotalValue:N2} {defaultCurrency}",
            $"Total: {collectionTotalValue:N2} {defaultCurrency}"),
            true),

        new("---")
        };

            AddStatisticsGroup(
                costLines,
                T("Statystyki według producentów", "Statistics by manufacturers"),
                manufacturerTotals,
                T("Razem", "Total"));

            costLines.Add(new StatLine("---"));

            AddStatisticsGroup(
                costLines,
                T("Serwis (niewliczony powyżej)", "Service (not included above)"),
                serviceTotals,
                T("Razem serwis", "Total service"),
                true);

            AddSection(
                 T("Koszt kolekcji", "Collection cost"),
                 costLines,
                 true);
        }
        private void BuildHardwareStatistics(
        int hardwareCount,
        int pcCount,
        int accessoryCount)
        {
            AddSection(
                T("Sprzęt", "Hardware"),
                new List<StatLine>
                {
            new(T(
                $"Konsole / sprzęt: {hardwareCount}",
                $"Consoles / hardware: {hardwareCount}")),

            new(T(
                $"Komputery: {pcCount}",
                $"Computers: {pcCount}")),

            new(T(
                $"Akcesoria: {accessoryCount}",
                $"Accessories: {accessoryCount}")),

            new(T(
                $"Zarchiwizowany sprzęt: {collection.ArchivedHardwareItems.Count}",
                $"Archived hardware: {collection.ArchivedHardwareItems.Count}")),

            new(T(
                $"Zarchiwizowane komputery: {collection.ArchivedPcSystems.Count}",
                $"Archived computers: {collection.ArchivedPcSystems.Count}")),

            new(T(
                $"Zarchiwizowane akcesoria: {collection.ArchivedAccessories.Count}",
                $"Archived accessories: {collection.ArchivedAccessories.Count}"))
                });
        }
        private void BuildGameStatistics(int gamesCount)
        {
            Dictionary<string, int> genreCounts = collection.Games
                .GroupBy(g =>
                    string.IsNullOrWhiteSpace(g.Genre)
                        ? T("Niesklasyfikowane", "Unclassified")
                        : g.Genre.Trim())
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Count());

            int favoriteCount = collection.Games.Count(g => g.IsFavorite);
            int completedCount = collection.Games.Count(g => g.IsCompleted);
            int plannedCount = collection.Games.Count(g => g.IsPlanned);

            var gameLines = new List<StatLine>
        {
        new(T($"Wszystkie gry: {gamesCount}", $"All games: {gamesCount}"), true)
        };

            foreach (var genre in genreCounts)
            {
                gameLines.Add(new StatLine(
                    $"{GenreDatabase.TranslateGenre(genre.Key, currentLanguage)}: {genre.Value}"));
            }

            gameLines.Add(new StatLine(
                T($"Ulubione: {favoriteCount}", $"Favorites: {favoriteCount}"),
                true));

            gameLines.Add(new StatLine(
                T($"Ukończone: {completedCount}", $"Completed: {completedCount}"),
                true));

            gameLines.Add(new StatLine(
                T($"Planowane: {plannedCount}", $"Planned: {plannedCount}"),
                true));

            AddSection(
                T("Gry", "Games"),
                gameLines);
        }
        private List<ChartSlice> EnsureChartHasData(List<ChartSlice> slices)
        {
            if (slices != null && slices.Any(x => x.Value > 0))
                return slices;

            return new List<ChartSlice>
    {
        new ChartSlice
        {
            Name = T("Brak danych", "No data"),
            Value = 1,
            Color = Color.FromArgb(90, 255, 255, 255)
        }
    };
        }
        private void BuildBasicStatistics(
        int gamesCount,
        int pcCount,
        int hardwareCount,
        int accessoryCount)
        {
            AddSection(
                T("Podstawowe", "Basic"),
                new List<StatLine>
                {
                    new(T($"Gry: {gamesCount}", $"Games: {gamesCount}")),
                    new(T($"Komputery: {pcCount}", $"Computers: {pcCount}")),
                    new(T($"Sprzęt / konsole: {hardwareCount}", $"Hardware / consoles: {hardwareCount}")),
                    new(T($"Akcesoria: {accessoryCount}", $"Accessories: {accessoryCount}")),

                    new StatLine(
                        T(
                            $"Razem: {gamesCount + pcCount + hardwareCount + accessoryCount}",
                            $"Total: {gamesCount + pcCount + hardwareCount + accessoryCount}"
                        ),
                        true)
                });
        }

        private void AddOriginalCurrencyValue(
        Dictionary<string, decimal> totals,
        decimal? price,
        string currency)
        {
            if (!price.HasValue)
                return;

            string key = string.IsNullOrWhiteSpace(currency)
                ? defaultCurrency
                : currency.Trim().ToUpper();

            if (!totals.ContainsKey(key))
                totals[key] = 0m;

            totals[key] += price.Value;
        }
        private Dictionary<string, decimal> GetOriginalCurrencyTotals()
        {
            Dictionary<string, decimal> totals = new();

            foreach (Game game in collection.Games)
                AddOriginalCurrencyValue(totals, game.PurchasePrice, game.PurchaseCurrency);

            foreach (Hardware hardware in collection.HardwareItems)
                AddOriginalCurrencyValue(totals, hardware.PurchasePrice, hardware.PurchaseCurrency);

            foreach (PcSystem pc in collection.PcSystems)
                AddOriginalCurrencyValue(totals, pc.PurchasePrice, pc.PurchaseCurrency);

            foreach (Accessory accessory in collection.Accessories)
                AddOriginalCurrencyValue(totals, accessory.PurchasePrice, accessory.PurchaseCurrency);

            foreach (Hardware hardware in collection.HardwareItems)
            {
                foreach (ServiceEntry service in hardware.ServiceHistory)
                    AddOriginalCurrencyValue(totals, service.Cost, service.CostCurrency);
            }

            foreach (PcSystem pc in collection.PcSystems)
            {
                foreach (ServiceEntry service in pc.ServiceHistory)
                    AddOriginalCurrencyValue(totals, service.Cost, service.CostCurrency);
            }

            return totals;
        }
        private void AddSection(
        string title,
        IEnumerable<StatLine> lines,
        bool rightColumn = false,
        bool addCurrencyRatesGrid = false)
        {
            Color statsColor =
        ((SolidColorBrush)FindResource("StatsBackgroundBrush")).Color;

            Border sectionBorder = new()
            {
                Margin = new Thickness(0, 0, 0, 22),
                Padding = new Thickness(14),
                CornerRadius = new CornerRadius(10),

                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 315,
                    ShadowDepth = 5,
                    BlurRadius = 14,
                    Opacity = 0.40
                },

                Background = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1),
                    GradientStops =
            {
            new GradientStop(LightenColor(statsColor, 0.08), 0.0),
            new GradientStop(statsColor, 0.55),
            new GradientStop(DarkenColor(statsColor, 0.14), 1.0)
             }
                },

                BorderBrush = (Brush)FindResource("AppBorderBrush"),
                BorderThickness = new Thickness(1)
            };

            StackPanel panel = new();

            Color accentColor =
            ((SolidColorBrush)FindResource("AppAccentBrush")).Color;

            Border headerBorder = new()
            {
                Padding = new Thickness(10, 6, 10, 6),
                Margin = new Thickness(-14, -14, -14, 10),
                Background = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1),
                    GradientStops =
                    {
                        new GradientStop(LightenColor(accentColor, 0.25), 0.0),
                        new GradientStop(accentColor, 0.55),
                        new GradientStop(DarkenColor(accentColor, 0.30), 1.0)
                    }
                },
                CornerRadius = new CornerRadius(10, 10, 0, 0),

                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 10,
                    ShadowDepth = 2,
                    Opacity = 0.35
                },
            };

            headerBorder.Child = new TextBlock
            {
                Text = title.ToUpper(),
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)FindResource("ButtonTextBrush")
            };

            panel.Children.Add(headerBorder);

            foreach (StatLine line in lines)
            {
                if (line.Text == "---")
                {
                    panel.Children.Add(new Border
                    {
                        Height = 1,
                        Width = 260,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0, 12, 0, 12),
                        Opacity = 1.05,
                        Background = (Brush)FindResource("AppBorderBrush")
                    });

                    continue;
                }

                panel.Children.Add(new TextBlock
                {
                    Text = line.Text,
                    FontSize = 12.5,
                    Margin = new Thickness(0),
                    FontWeight = line.IsBold
                        ? FontWeights.Bold
                        : FontWeights.Normal,
                    Foreground = (Brush)FindResource("AppTextBrush")
                });
            }

            if (addCurrencyRatesGrid)
                AddCurrencyRatesGrid(panel);

            sectionBorder.Child = panel;

            if (rightColumn)
                RightStatsPanel.Children.Add(sectionBorder);
            else
                LeftStatsPanel.Children.Add(sectionBorder);
        }

        private void AddCurrencyRatesGrid(StackPanel panel)
        {
            List<CurrencyRate> rates = collection.CurrencyRates
                .Where(r => r.Currency != defaultCurrency)
                .OrderBy(r => r.Currency)
                .ToList();

            if (rates.Count == 0)
                return;

            panel.Children.Add(new TextBlock
            {
                Text = T("Przeliczniki walut", "Exchange rates"),
                FontSize = 12.5,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 16, 0, 6),
                Foreground = (Brush)FindResource("AppTextBrush")
            });

            UniformGrid grid = new()
            {
                Columns = 2,
                Margin = new Thickness(0, -8, 0, 0)
            };

            foreach (CurrencyRate rate in rates)
            {
                grid.Children.Add(new TextBlock
                {
                    Text = $"{rate.Currency} → {rate.Rate:0.####} {defaultCurrency}",
                    FontSize = 12.3,
                    Margin = new Thickness(0, 0, 4, 0),
                    Foreground = (Brush)FindResource("AppTextBrush")
                });
            }

            panel.Children.Add(grid);
        }
        private void BuildCollectionValuePanel(decimal totalValueWithService)
        {
            CollectionValuePanel.Visibility = Visibility.Visible;

            Color statsColor =
                ((SolidColorBrush)FindResource("StatsBackgroundBrush")).Color;

            Color accentColor =
                ((SolidColorBrush)FindResource("AppAccentBrush")).Color;

            CollectionValuePanel.Padding = new Thickness(0);
            CollectionValuePanel.CornerRadius = new CornerRadius(10);
            CollectionValuePanel.BorderBrush = (Brush)FindResource("AppBorderBrush");
            CollectionValuePanel.BorderThickness = new Thickness(1);

            CollectionValuePanel.Background = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
                GradientStops =
        {
            new GradientStop(LightenColor(statsColor, 0.08), 0.0),
            new GradientStop(statsColor, 0.55),
            new GradientStop(DarkenColor(statsColor, 0.14), 1.0)
        }
            };

            StackPanel root = new StackPanel();

            Border headerBorder = new()
            {
                Padding = new Thickness(10, 6, 10, 6),
                Background = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1),
                    GradientStops =
            {
                new GradientStop(LightenColor(accentColor, 0.25), 0.0),
                new GradientStop(accentColor, 0.55),
                new GradientStop(DarkenColor(accentColor, 0.30), 1.0)
            }
                },
                CornerRadius = new CornerRadius(10, 10, 0, 0)
            };

            headerBorder.Child = new TextBlock
            {
                Text = T(
                    "Łączna wartość kolekcji (z serwisem)",
                    "Total collection value (including service)").ToUpper(),
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)FindResource("ButtonTextBrush")
            };

            root.Children.Add(headerBorder);

            Grid contentGrid = new Grid
            {
                Margin = new Thickness(14, -10, 14, 6)
            };

            contentGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(0.42, GridUnitType.Star)
            });

            contentGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(0.58, GridUnitType.Star)
            });

            TextBlock totalText = new TextBlock
            {
                Text = T(
                    $"RAZEM: {totalValueWithService:N2} {defaultCurrency}",
                    $"TOTAL: {totalValueWithService:N2} {defaultCurrency}"),
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(-12, 10, 0, 0),
                Foreground = (Brush)FindResource("AppTextBrush")
            };

            Grid.SetColumn(totalText, 0);
            contentGrid.Children.Add(totalText);

            StackPanel ratesPanel = new StackPanel
            {
                Margin = new Thickness(8, 0, 0, 0)
            };

            AddCurrencyRatesGrid(ratesPanel);

            Grid.SetColumn(ratesPanel, 1);
            contentGrid.Children.Add(ratesPanel);

            root.Children.Add(contentGrid);

            CollectionValuePanel.Child = root;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private Color LightenColor(Color color, double amount)
        {
            return Color.FromRgb(
                (byte)Math.Min(255, color.R + (255 - color.R) * amount),
                (byte)Math.Min(255, color.G + (255 - color.G) * amount),
                (byte)Math.Min(255, color.B + (255 - color.B) * amount));
        }

        private Color DarkenColor(Color color, double amount)
        {
            return Color.FromRgb(
                (byte)(color.R * (1 - amount)),
                (byte)(color.G * (1 - amount)),
                (byte)(color.B * (1 - amount)));
        }
        private List<Color> BuildAccentPalette(int count)
        {
            Color accentColor =
                ((SolidColorBrush)FindResource("AppAccentBrush")).Color;

            List<Color> colors = new();

            if (count <= 0)
                return colors;

            double[] steps =
            {
        0.32, 0.18, 0.00, 0.12, 0.24, 0.36, 0.48, 0.60
        };

            for (int i = 0; i < count; i++)
            {
                double step = steps[i % steps.Length];

                Color color = i < 2
                    ? DarkenColor(accentColor, step)
                    : LightenColor(accentColor, step);

                colors.Add(color);
            }

            return colors;
        }
        private Brush GetContrastBrush(Color backgroundColor)
        {
            double luminance =
                (0.299 * backgroundColor.R +
                 0.587 * backgroundColor.G +
                 0.114 * backgroundColor.B) / 255.0;

            return luminance > 0.60
                ? Brushes.Black
                : Brushes.White;
        }

        private Color GetBrushColor(Brush brush)
        {
            if (brush is SolidColorBrush solidBrush)
                return solidBrush.Color;

            return Colors.Black;
        }
        private Brush BuildSliceBrush(Color baseColor)
        {
            Color light = LightenColor(baseColor, 0.28);
            Color dark = DarkenColor(baseColor, 0.22);

            return new RadialGradientBrush
            {
                Center = new Point(0.35, 0.30),
                GradientOrigin = new Point(0.25, 0.20),
                RadiusX = 0.85,
                RadiusY = 0.85,
                GradientStops =
        {
            new GradientStop(light, 0.0),
            new GradientStop(baseColor, 0.58),
            new GradientStop(dark, 1.0)
        }
            };
        }
        private string FormatChartValue(decimal value)
        {
            if (currentGameChartMode == GameChartMode.Count)
                return $"{value:N0}";

            return $"{value:N2} {defaultCurrency}";
        }
        private List<ChartSlice> BuildGameManufacturerChartSlices()
        {
            Dictionary<string, decimal> values = new();

            foreach (Game game in collection.Games)
            {
                string manufacturer = string.IsNullOrWhiteSpace(game.Manufacturer)
                    ? T("Niesklasyfikowane", "Unclassified")
                    : game.Manufacturer.Trim();

                decimal value;

                if (currentGameChartMode == GameChartMode.Count)
                {
                    value = 1m;
                }
                else
                {
                    if (!game.PurchasePrice.HasValue || game.PurchasePrice.Value <= 0)
                        continue;

                    value = ConvertCurrency(
                        game.PurchasePrice.Value,
                        game.PurchaseCurrency,
                        defaultCurrency);
                }

                if (!values.ContainsKey(manufacturer))
                    values[manufacturer] = 0m;

                values[manufacturer] += value;
            }

            return BuildChartSlices(values);
        }
        private List<ChartSlice> BuildGameGenreChartSlices()
        {
            Dictionary<string, decimal> values = new();

            foreach (Game game in collection.Games)
            {
                string genre = string.IsNullOrWhiteSpace(game.Genre)
                    ? T("Niesklasyfikowane", "Unclassified")
                    : GenreDatabase.TranslateGenre(game.Genre.Trim(), currentLanguage);

                decimal value;

                if (currentGameChartMode == GameChartMode.Count)
                {
                    value = 1m;
                }
                else
                {
                    if (!game.PurchasePrice.HasValue || game.PurchasePrice.Value <= 0)
                        continue;

                    value = ConvertCurrency(
                        game.PurchasePrice.Value,
                        game.PurchaseCurrency,
                        defaultCurrency);
                }

                if (!values.ContainsKey(genre))
                    values[genre] = 0m;

                values[genre] += value;
            }

            return BuildChartSlices(values);
        }
        private List<ChartSlice> BuildChartSlices(
    Dictionary<string, decimal> values)
        {
            var ordered = values
                .Where(x => x.Value > 0)
                .OrderByDescending(x => x.Value)
                .ToList();

            if (ordered.Count == 0)
                return new List<ChartSlice>();

            int maxVisible = 8;

            var visible = ordered
                .Take(maxVisible)
                .ToList();

            decimal otherValue = ordered
                .Skip(maxVisible)
                .Sum(x => x.Value);

            if (otherValue > 0)
            {
                visible.Add(new KeyValuePair<string, decimal>(
                    T("Pozostałe", "Other"),
                    otherValue));
            }

            List<Color> colors = BuildAccentPalette(visible.Count);

            return visible
                .Select((x, index) => new ChartSlice
                {
                    Name = x.Key,
                    Value = x.Value,
                    Color = colors[index]
                })
                .ToList();
        }
        private void DrawDonutChart(
        Canvas canvas,
        StackPanel legendPanel,
        List<ChartSlice> slices)
        {
            canvas.Children.Clear();
            legendPanel.Children.Clear();

            if (slices.Count == 0 || slices.Sum(x => x.Value) <= 0)
            {
                legendPanel.Children.Add(new TextBlock
                {
                    Text = T("Brak danych.", "No data."),
                    FontSize = 12,
                    Foreground = (Brush)FindResource("AppTextBrush")
                });

                return;
            }

            double size = Math.Min(canvas.Width, canvas.Height);
            double center = size / 2;
            double outerRadius = size / 2 - 4;
            double innerRadius = outerRadius * 0.58;

            System.Windows.Shapes.Ellipse shadow = new()
            {
                Width = outerRadius * 2,
                Height = outerRadius * 2,
                Fill = new SolidColorBrush(Color.FromArgb(95, 0, 0, 0)),
                Effect = new System.Windows.Media.Effects.BlurEffect
                {
                    Radius = 6
                }
            };

            Canvas.SetLeft(shadow, center - outerRadius + 3);
            Canvas.SetTop(shadow, center - outerRadius + 5);
            canvas.Children.Add(shadow);

            System.Windows.Shapes.Ellipse shadowHole = new()
            {
                Width = innerRadius * 2,
                Height = innerRadius * 2,
                Fill = (Brush)FindResource("StatsBackgroundBrush")
            };

            Canvas.SetLeft(shadowHole, center - innerRadius + 3);
            Canvas.SetTop(shadowHole, center - innerRadius + 5);
            canvas.Children.Add(shadowHole);

            decimal total = slices.Sum(x => x.Value);
            double startAngle = -90;

            if (slices.Count == 1)
            {
                ChartSlice slice = slices[0];

                canvas.Children.Add(new Ellipse
                {
                    Width = outerRadius * 2,
                    Height = outerRadius * 2,
                    Fill = new SolidColorBrush(slice.Color)
                });

                Canvas.SetLeft(canvas.Children[^1], center - outerRadius);
                Canvas.SetTop(canvas.Children[^1], center - outerRadius);

                canvas.Children.Add(new Ellipse
                {
                    Width = innerRadius * 2,
                    Height = innerRadius * 2,
                    Fill = (Brush)FindResource("StatsBackgroundBrush")
                });

                Canvas.SetLeft(canvas.Children[^1], center - innerRadius);
                Canvas.SetTop(canvas.Children[^1], center - innerRadius);

                AddChartLegendRows(legendPanel, slices);
                return;
            }

            foreach (ChartSlice slice in slices)
            {
                double sweepAngle =
                    (double)(slice.Value / total) * 360.0;

                System.Windows.Shapes.Path path = CreateDonutSlicePath(
                    center,
                    center,
                    outerRadius,
                    innerRadius,
                    startAngle,
                    sweepAngle);

                path.Fill = new SolidColorBrush(slice.Color);
                path.Stroke = (Brush)FindResource("ContentBackgroundBrush");
                path.StrokeThickness = 1;

                canvas.Children.Add(path);

                startAngle += sweepAngle;
            }

            // Delikatny połysk na całym wykresie
            System.Windows.Shapes.Ellipse gloss = new()
            {
                Width = outerRadius * 2 - 4,
                Height = outerRadius * 2 - 4,
                IsHitTestVisible = false,
                Fill = new LinearGradientBrush
                {
                    StartPoint = new Point(0.5, 0),
                    EndPoint = new Point(0.5, 1),
                    GradientStops =
            {
                new GradientStop(Color.FromArgb(55, 255, 255, 255), 0.00),
                new GradientStop(Color.FromArgb(18, 255, 255, 255), 0.35),
                new GradientStop(Color.FromArgb(0, 255, 255, 255), 0.65)
            }
                },
                Opacity = 0.20
            };

            Canvas.SetLeft(gloss, center - outerRadius + 2);
            Canvas.SetTop(gloss, center - outerRadius + 2);

            canvas.Children.Add(gloss);

            foreach (ChartSlice slice in slices)
            {
                StackPanel row = new()
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 0, 0, 5)
                };

                row.Children.Add(new Border
                {
                    Width = 10,
                    Height = 10,
                    CornerRadius = new CornerRadius(2),
                    Background = new SolidColorBrush(slice.Color),
                    Margin = new Thickness(0, 3, 7, 0)
                });

                row.Children.Add(new TextBlock
                {
                    Text = slice.Name == T("Brak danych", "No data")
                    ? slice.Name
                    : $"{slice.Name}: {FormatChartValue(slice.Value)}",
                    FontSize = 11.5,
                    Foreground = (Brush)FindResource("AppTextBrush")
                });
                              

                legendPanel.Children.Add(row);
            }
        }
        private void AnimateChart(Canvas canvas)
        {
            if (canvas.RenderTransform is not ScaleTransform scale)
                return;

            DoubleAnimation scaleAnimation = new DoubleAnimation
            {
                From = 0.90,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(280),
                EasingFunction = new CubicEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(220)
            };

            scale.BeginAnimation(
                ScaleTransform.ScaleXProperty,
                scaleAnimation);

            scale.BeginAnimation(
                ScaleTransform.ScaleYProperty,
                scaleAnimation);

            canvas.BeginAnimation(
                UIElement.OpacityProperty,
                opacityAnimation);
        }
        private void AddChartLegendRows(
        StackPanel legendPanel,
        List<ChartSlice> slices)
        {
            foreach (ChartSlice slice in slices)
            {
                StackPanel row = new()
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 0, 0, 5)
                };

                row.Children.Add(new Border
                {
                    Width = 10,
                    Height = 10,
                    CornerRadius = new CornerRadius(2),
                    Background = new SolidColorBrush(slice.Color),
                    Margin = new Thickness(0, 3, 7, 0)
                });

                row.Children.Add(new TextBlock
                {
                    Text = slice.Name == T("Brak danych", "No data")
                    ? slice.Name
                    : $"{slice.Name}: {FormatChartValue(slice.Value)}",
                    FontSize = 11.5,
                    Foreground = (Brush)FindResource("AppTextBrush")
                });

                legendPanel.Children.Add(row);
            }
        }
        private System.Windows.Shapes.Path CreateDonutSlicePath(
            double centerX,
            double centerY,
            double outerRadius,
            double innerRadius,
            double startAngle,
            double sweepAngle)
        {
            double endAngle = startAngle + sweepAngle;

            Point outerStart = PointOnCircle(centerX, centerY, outerRadius, startAngle);
            Point outerEnd = PointOnCircle(centerX, centerY, outerRadius, endAngle);

            Point innerStart = PointOnCircle(centerX, centerY, innerRadius, endAngle);
            Point innerEnd = PointOnCircle(centerX, centerY, innerRadius, startAngle);

            bool isLargeArc = sweepAngle > 180;

            PathFigure figure = new()
            {
                StartPoint = outerStart,
                IsClosed = true
            };

            figure.Segments.Add(new ArcSegment(
                outerEnd,
                new Size(outerRadius, outerRadius),
                0,
                isLargeArc,
                SweepDirection.Clockwise,
                true));

            figure.Segments.Add(new LineSegment(innerStart, true));

            figure.Segments.Add(new ArcSegment(
                innerEnd,
                new Size(innerRadius, innerRadius),
                0,
                isLargeArc,
                SweepDirection.Counterclockwise,
                true));

            figure.Segments.Add(new LineSegment(outerStart, true));

            return new System.Windows.Shapes.Path
            {
                Data = new PathGeometry(new[] { figure })
            };
        }

        private Point PointOnCircle(
            double centerX,
            double centerY,
            double radius,
            double angleDegrees)
        {
            double angleRadians = angleDegrees * Math.PI / 180.0;

            return new Point(
                centerX + radius * Math.Cos(angleRadians),
                centerY + radius * Math.Sin(angleRadians));
        }
        private void CurrencyRatesButton_Click(
        object sender,
        RoutedEventArgs e)
        {
            CurrencyRatesWindow window = new(
                collection,
                defaultCurrency,
                currentLanguage);

            window.Owner = this;
            window.ShowDialog();

            BuildStats();
        }

        private void ExportTxtButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = currentLanguage == "pl"
                    ? "Plik tekstowy (*.txt)|*.txt"
                    : "Text file (*.txt)|*.txt",
                FileName = $"HGC_Statistics_{DateTime.Now:yyyy-MM-dd_HH-mm}.txt"
            };

            if (dialog.ShowDialog() != true)
                return;

            string report = BuildTxtStatisticsReport();

            File.WriteAllText(dialog.FileName, report, Encoding.UTF8);

            HgcMessageBox.Show(
                this,
                currentLanguage == "pl"
                    ? "Raport TXT został zapisany."
                    : "TXT report has been saved.",
                "HGC",
                HgcMessageBoxType.Information,
                currentLanguage);
        }

        private string BuildTxtStatisticsReport()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Hajper Games Collection");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Wygenerowano: {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
                : $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            sb.AppendLine();

            sb.AppendLine("==================================================");
            sb.AppendLine();

            sb.AppendLine(currentLanguage == "pl"
                ? "PODSTAWOWE"
                : "BASIC");

            sb.AppendLine("----------------------------------------");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Gry: {collection.Games.Count}"
                : $"Games: {collection.Games.Count}");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Komputery: {collection.PcSystems.Count}"
                : $"Computers: {collection.PcSystems.Count}");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Sprzęt / Konsole: {collection.HardwareItems.Count}"
                : $"Hardware / Consoles: {collection.HardwareItems.Count}");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Akcesoria: {collection.Accessories.Count}"
                : $"Accessories: {collection.Accessories.Count}");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Łącznie rekordów: {collection.Games.Count + collection.PcSystems.Count + collection.HardwareItems.Count + collection.Accessories.Count}"
                : $"Total records: {collection.Games.Count + collection.PcSystems.Count + collection.HardwareItems.Count + collection.Accessories.Count}");

            sb.AppendLine();
            sb.AppendLine(currentLanguage == "pl" ? "GRY" : "GAMES");
            sb.AppendLine("----------------------------------------");

            Dictionary<string, int> genreCounts = collection.Games
                .GroupBy(g =>
                    string.IsNullOrWhiteSpace(g.Genre)
                        ? T("Niesklasyfikowane", "Unclassified")
                        : g.Genre.Trim())
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Count());

            sb.AppendLine(currentLanguage == "pl"
                ? $"Wszystkie gry: {collection.Games.Count}"
                : $"All games: {collection.Games.Count}");

            foreach (var genre in genreCounts)
            {
                sb.AppendLine(
                    $"{GenreDatabase.TranslateGenre(genre.Key, currentLanguage)}: {genre.Value}");
            }

            sb.AppendLine(currentLanguage == "pl"
                ? $"Ukończone: {collection.Games.Count(g => g.IsCompleted)}"
                : $"Completed: {collection.Games.Count(g => g.IsCompleted)}");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Planowane: {collection.Games.Count(g => g.IsPlanned)}"
                : $"Planned: {collection.Games.Count(g => g.IsPlanned)}");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Ulubione: {collection.Games.Count(g => g.IsFavorite)}"
                : $"Favorites: {collection.Games.Count(g => g.IsFavorite)}");

            sb.AppendLine();
            sb.AppendLine(currentLanguage == "pl" ? "SPRZĘT" : "HARDWARE");
            sb.AppendLine("----------------------------------------");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Konsole / sprzęt: {collection.HardwareItems.Count}"
                : $"Consoles / hardware: {collection.HardwareItems.Count}");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Komputery: {collection.PcSystems.Count}"
                : $"Computers: {collection.PcSystems.Count}");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Akcesoria: {collection.Accessories.Count}"
                : $"Accessories: {collection.Accessories.Count}");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Zarchiwizowany sprzęt: {collection.ArchivedHardwareItems.Count}"
                : $"Archived hardware: {collection.ArchivedHardwareItems.Count}");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Zarchiwizowane komputery: {collection.ArchivedPcSystems.Count}"
                : $"Archived computers: {collection.ArchivedPcSystems.Count}");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Zarchiwizowane akcesoria: {collection.ArchivedAccessories.Count}"
                : $"Archived accessories: {collection.ArchivedAccessories.Count}");

            sb.AppendLine();
            sb.AppendLine(currentLanguage == "pl"
                ? "KOSZT GIER I SPRZĘTU"
                : "GAMES AND HARDWARE COST");

            sb.AppendLine("----------------------------------------");

            decimal gamesTotalValue = GetTotalValue(
                collection.Games,
                g => g.PurchasePrice,
                g => g.PurchaseCurrency);

            decimal hardwareTotalValue = GetTotalValue(
                collection.HardwareItems,
                h => h.PurchasePrice,
                h => h.PurchaseCurrency);

            decimal pcTotalValue = GetTotalValue(
                collection.PcSystems,
                p => p.PurchasePrice,
                p => p.PurchaseCurrency);

            decimal accessoriesTotalValue = GetTotalValue(
                collection.Accessories,
                a => a.PurchasePrice,
                a => a.PurchaseCurrency);

            decimal collectionTotalValue =
                gamesTotalValue +
                hardwareTotalValue +
                pcTotalValue +
                accessoriesTotalValue;

            sb.AppendLine(currentLanguage == "pl"
                ? $"Gry: {gamesTotalValue:N2} {defaultCurrency}"
                : $"Games: {gamesTotalValue:N2} {defaultCurrency}");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Konsole: {hardwareTotalValue:N2} {defaultCurrency}"
                : $"Consoles: {hardwareTotalValue:N2} {defaultCurrency}");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Komputery: {pcTotalValue:N2} {defaultCurrency}"
                : $"Computers: {pcTotalValue:N2} {defaultCurrency}");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Akcesoria: {accessoriesTotalValue:N2} {defaultCurrency}"
                : $"Accessories: {accessoriesTotalValue:N2} {defaultCurrency}");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Razem: {collectionTotalValue:N2} {defaultCurrency}"
                : $"Total: {collectionTotalValue:N2} {defaultCurrency}");

            sb.AppendLine();
            sb.AppendLine(currentLanguage == "pl"
                ? "STATYSTYKI WEDŁUG PRODUCENTÓW"
                : "STATISTICS BY MANUFACTURERS");

            sb.AppendLine("----------------------------------------");

            Dictionary<string, decimal> manufacturerTotals = BuildManufacturerTotals();

            foreach (var item in manufacturerTotals
                .Where(x => x.Value > 0)
                .OrderBy(x => x.Key))
            {
                sb.AppendLine($"{item.Key}: {item.Value:N2} {defaultCurrency}");
            }

            sb.AppendLine(currentLanguage == "pl"
                ? $"Razem: {manufacturerTotals.Sum(x => x.Value):N2} {defaultCurrency}"
                : $"Total: {manufacturerTotals.Sum(x => x.Value):N2} {defaultCurrency}");

            sb.AppendLine();
            sb.AppendLine(currentLanguage == "pl"
                ? "SERWIS"
                : "SERVICE");

            sb.AppendLine("----------------------------------------");

            Dictionary<string, decimal> serviceTotals = BuildServiceTotals();

            if (serviceTotals.Count == 0 || serviceTotals.All(x => x.Value <= 0))
            {
                sb.AppendLine(currentLanguage == "pl"
                    ? "Brak kosztów serwisowych."
                    : "No service costs.");
            }
            else
            {
                foreach (var item in serviceTotals
                    .Where(x => x.Value > 0)
                    .OrderBy(x => x.Key))
                {
                    sb.AppendLine(
                        $"{ServiceTypeDatabase.Translate(item.Key, currentLanguage)}: {item.Value:N2} {defaultCurrency}");
                }

                sb.AppendLine(currentLanguage == "pl"
                    ? $"Razem serwis: {serviceTotals.Sum(x => x.Value):N2} {defaultCurrency}"
                    : $"Total service: {serviceTotals.Sum(x => x.Value):N2} {defaultCurrency}");
            }

            sb.AppendLine();
            sb.AppendLine(currentLanguage == "pl"
                ? "ŁĄCZNA WARTOŚĆ KOLEKCJI"
                : "TOTAL COLLECTION VALUE");

            sb.AppendLine("----------------------------------------");

            decimal serviceTotalValue = serviceTotals.Sum(x => x.Value);

            decimal totalCollectionValue =
                collectionTotalValue +
                serviceTotalValue;

            Dictionary<string, decimal> originalCurrencyTotals =
                GetOriginalCurrencyTotals();

            sb.AppendLine(currentLanguage == "pl"
                ? $"Łącznie: {totalCollectionValue:N2} {defaultCurrency}"
                : $"Total: {totalCollectionValue:N2} {defaultCurrency}");

            if (originalCurrencyTotals.Count > 1)
            {
                sb.AppendLine(currentLanguage == "pl"
                    ? "W tym według oryginalnych walut:"
                    : "Including original currencies:");

                foreach (var currency in originalCurrencyTotals.OrderBy(x => x.Key))
                {
                    sb.AppendLine($"{currency.Key}: {currency.Value:N2}");
                }
            }

            sb.AppendLine();
            sb.AppendLine(currentLanguage == "pl"
                ? "PRZELICZNIKI WALUT"
                : "EXCHANGE RATES");

            sb.AppendLine("----------------------------------------");

            sb.AppendLine(currentLanguage == "pl"
                ? $"Waluta domyślna: {defaultCurrency}"
                : $"Default currency: {defaultCurrency}");

            foreach (CurrencyRate rate in collection.CurrencyRates
                .Where(r => r.Currency != defaultCurrency)
                .OrderBy(r => r.Currency))
            {
                sb.AppendLine($"1 {rate.Currency} = {rate.Rate:0.####} {defaultCurrency}");
            }

            return sb.ToString();
        }
        private async void ExportPdfButton_Click(object sender, RoutedEventArgs e)
        {
            PdfExportOptionsWindow optionsWindow =
                new PdfExportOptionsWindow(currentLanguage);

            optionsWindow.Owner = this;

            if (optionsWindow.ShowDialog() != true)
                return;

            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = currentLanguage == "pl"
                    ? "Plik PDF (*.pdf)|*.pdf"
                    : "PDF file (*.pdf)|*.pdf",
                FileName = $"HGC_Report_{DateTime.Now:yyyy-MM-dd_HH-mm}.pdf"
            };

            if (dialog.ShowDialog() != true)
                return;

            PdfGeneratingWindow generatingWindow =
     new PdfGeneratingWindow(currentLanguage)
     {
         Owner = this
     };

            generatingWindow.Show();

            try
            {
                await Task.Run(() =>
                {
                    PdfReportGenerator.Generate(
                        collection,
                        defaultCurrency,
                        currentLanguage,
                        optionsWindow,
                        dialog.FileName);
                });

                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? "Raport PDF został zapisany."
                        : "PDF report has been saved.",
                    "HGC",
                    HgcMessageBoxType.Information,
                    currentLanguage);
            }
            catch (Exception ex)
            {
                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? $"Nie udało się wygenerować PDF.\n\n{ex.Message}"
                        : $"PDF generation failed.\n\n{ex.Message}",
                    "HGC",
                    HgcMessageBoxType.Error,
                    currentLanguage);
            }
            finally
            {
                generatingWindow.Close();
            }
        }

    }
    public class StatLine
    {
        public string Text { get; set; } = "";
        public bool IsBold { get; set; }

        public StatLine(string text, bool isBold = false)
        {
            Text = text;
            IsBold = isBold;
        }
    }   

}