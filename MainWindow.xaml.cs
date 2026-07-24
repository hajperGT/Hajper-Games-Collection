using Hajper_Game_Collection.Data;
using HGC.Data;
using HGC.Models;
using HGC.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace HGC
{
    public partial class MainWindow : Window
    {
        private PcSystem? currentPcDraft;
        private HgcSettings settings = new();
        private string currentLanguage = "pl";
        private string currentTheme = "Default";
        private string currentGamesTitle = "";
        private readonly List<CollectionPhoto> testHardwarePhotos = new();     
        private int currentOpacity = 100;
        private string currentPlatformGroup = "Default";
        private bool isAddChoicePanelOpen = false;
        private TreeViewItem? archiveRootItem;
        private Button? RestoreRecordButton;
        private WrapPanel? currentCollectionTilesWrap;
        private List<CollectionPhoto> currentHardwarePhotos = new();
        private List<CollectionPhoto> currentAccessoryPhotos = new();
        private bool showRecentlyAddedPanel = true;
        private bool isClearingTreeSelection = false;
        private string collectionCategoryOrder = "Hardware,Accessories,Games";
        private Game? recentlyAddedGame;
        private TreeNodeData? lastCollectionTilesNode = null;
        private bool lastCollectionTilesShowRecentlyAdded = false;
        private readonly Dictionary<string, ImageSource> tileImageCache = new Dictionary<string, ImageSource>();
        private bool showHardwareCategory = true;
        private bool showAccessoriesCategory = true;
        private bool showGamesCategory = true;

        private bool isSearchActive = false;

        private int collectionCategoryLines = 1;
        private bool collectionCategoryLinesAuto = true;

        private string collectionTileSize = "Medium";

        private string collectionSortMode = "AZ";

        private string defaultCurrency = "PLN";

        private int collectionItemsPerPage = 40;
        private string collectionItemsPerPageMode = "Auto";

        private int collectionCurrentPage = 1;
        private int collectionTotalPages = 1;

        private ServiceEntry? editingServiceEntry;

        private bool isEditingService;

        private readonly Dictionary<string, string> categoryOrders = new()
            {
                { "HAG", "Hardware,Accessories,Games" },
                { "HGA", "Hardware,Games,Accessories" },

                { "AHG", "Accessories,Hardware,Games" },
                { "AGH", "Accessories,Games,Hardware" },

                { "GHA", "Games,Hardware,Accessories" },
                { "GAH", "Games,Accessories,Hardware" }
            };

        private readonly CollectionDatabase collection =
        new CollectionDatabase();
        private string GetHardwareCategoryName()
        {
            return currentLanguage == "pl" ? "Sprzęt" : "Hardware";
        }
        private string GetModelVariantName(HardwareModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Revision))
                return model.DisplayName;

            if (model.DisplayName.Contains(model.Revision, StringComparison.OrdinalIgnoreCase))
                return model.DisplayName;

            return $"{model.DisplayName} {model.Revision}";
        }
        private void CollectionCategoryOrderMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem clickedItem)
                return;

            if (clickedItem.Tag is not string orderKey)
                return;

            if (clickedItem.Parent is MenuItem parentMenu)
            {
                foreach (object item in parentMenu.Items)
                {
                    if (item is MenuItem menuItem)
                        menuItem.IsChecked = false;
                }
            }

            clickedItem.IsChecked = true;

            collectionCurrentPage = 1;

            SetCollectionCategoryOrder(orderKey);

        }
        private List<GameTreeNode> BuildGameTreeNodes()
        {
            List<GameTreeNode> nodes = new();

            GameTreeNode allGamesNode = new()
            {
                Text = currentLanguage == "pl" ? "Wszystkie gry" : "All games",
                Count = collection.Games.Count,
                FilterType = GameFilterType.All
            };

            var genreGroups = collection.Games
                .GroupBy(g =>
                    string.IsNullOrWhiteSpace(g.Genre)
                        ? (currentLanguage == "pl" ? "Niesklasyfikowane" : "Unclassified")
                        : g.Genre.Trim())
                .OrderBy(g => g.Key);

            foreach (var genreGroup in genreGroups)
            {
                allGamesNode.Children.Add(new GameTreeNode
                {
                    Text = genreGroup.Key,
                    Count = genreGroup.Count(),
                    FilterType = GameFilterType.Genre,
                    Value = genreGroup.Key
                });
            }

            nodes.Add(allGamesNode);

            nodes.Add(new GameTreeNode
            {
                Text = currentLanguage == "pl" ? "Ulubione" : "Favorites",
                Count = collection.Games.Count(g => g.IsFavorite),
                FilterType = GameFilterType.Favorites
            });

            nodes.Add(new GameTreeNode
            {
                Text = currentLanguage == "pl" ? "Ukończone" : "Completed",
                Count = collection.Games.Count(g => g.IsCompleted),
                FilterType = GameFilterType.Completed
            });

            nodes.Add(new GameTreeNode
            {
                Text = currentLanguage == "pl" ? "Nieukończone" : "Uncompleted",
                Count = collection.Games.Count(g => !g.IsCompleted),
                FilterType = GameFilterType.Uncompleted
            });

            nodes.Add(new GameTreeNode
            {
                Text = currentLanguage == "pl" ? "Planowane" : "Planned",
                Count = collection.Games.Count(g => g.IsPlanned),
                FilterType = GameFilterType.Planned
            });

            return nodes;
        }
        private void CollectionCategoryVisibilityMenu_Click(
        object sender,
        RoutedEventArgs e)
        {
            if (sender is not MenuItem item)
                return;

            if (item.Tag is not string category)
                return;

            switch (category)
            {
                case "Hardware":
                    showHardwareCategory = item.IsChecked;
                    break;

                case "Accessories":
                    showAccessoriesCategory = item.IsChecked;
                    break;

                case "Games":
                    showGamesCategory = item.IsChecked;
                    break;
            }

            settings.ShowHardwareCategory = showHardwareCategory;
            settings.ShowAccessoriesCategory = showAccessoriesCategory;
            settings.ShowGamesCategory = showGamesCategory;
            SaveSettingsChanges();

            collectionCurrentPage = 1;

            ShowDefaultRightPanel();
        }
        private void FillCurrencyCombo(ComboBox combo)
        {
            combo.ItemsSource = SupportedCurrencies;
            combo.SelectedItem = defaultCurrency;
        }
        private void CollectionCategoryLinesMenu_Click(
        object sender,
        RoutedEventArgs e)
        {
            if (sender is not MenuItem clickedItem)
                return;

            if (clickedItem.Tag is not string value)
                return;

            if (clickedItem.Parent is MenuItem parentMenu)
            {
                foreach (object item in parentMenu.Items)
                {
                    if (item is MenuItem menuItem)
                        menuItem.IsChecked = false;
                }
            }

            clickedItem.IsChecked = true;

            if (value == "Auto")
            {
                collectionCategoryLinesAuto = true;
                collectionCategoryLines = 1;
            }
            else if (int.TryParse(value, out int lines))
            {
                collectionCategoryLinesAuto = false;
                collectionCategoryLines = lines;
            }

            settings.CollectionCategoryLinesAuto = collectionCategoryLinesAuto;
            settings.CollectionCategoryLines = collectionCategoryLines;
            SaveSettingsChanges();

            collectionCurrentPage = 1;

            ShowDefaultRightPanel();
        }
        private void CollectionTileSizeMenu_Click(
        object sender,
        RoutedEventArgs e)
        {
            if (sender is not MenuItem clickedItem)
                return;

            if (clickedItem.Tag is not string size)
                return;

            if (clickedItem.Parent is MenuItem parentMenu)
            {
                foreach (object item in parentMenu.Items)
                {
                    if (item is MenuItem menuItem)
                        menuItem.IsChecked = false;
                }
            }

            clickedItem.IsChecked = true;

            collectionTileSize = size;

            settings.CollectionTileSize = collectionTileSize;
            SaveSettingsChanges();

            collectionCurrentPage = 1;

            if (CollectionTilesPanel.Visibility == Visibility.Visible)
            {
                ShowCollectionTiles();
            }
            else
            {
                ShowDefaultRightPanel();
            }
        }

        private void CollectionSortMenu_Click(
        object sender,
        RoutedEventArgs e)
        {
            if (sender is not MenuItem clickedItem)
                return;

            if (clickedItem.Tag is not string sortMode)
                return;

            if (clickedItem.Parent is MenuItem parentMenu)
            {
                foreach (object item in parentMenu.Items)
                {
                    if (item is MenuItem menuItem)
                        menuItem.IsChecked = false;
                }
            }

            clickedItem.IsChecked = true;

            collectionSortMode = sortMode;

            settings.CollectionSortMode = collectionSortMode;
            SaveSettingsChanges();

            collectionCurrentPage = 1;

            ShowDefaultRightPanel();
        }
        private void CollectionItemsPerPageMenu_Click(
        object sender,
        RoutedEventArgs e)
        {
            if (sender is not MenuItem clickedItem)
                return;

            if (clickedItem.Tag is not string value)
                return;

            if (clickedItem.Parent is MenuItem parentMenu)
            {
                foreach (object item in parentMenu.Items)
                {
                    if (item is MenuItem menuItem)
                        menuItem.IsChecked = false;
                }
            }

            clickedItem.IsChecked = true;

            collectionItemsPerPageMode = value;

            collectionItemsPerPage = value == "Auto"
                ? 40
                : int.Parse(value);

            settings.CollectionItemsPerPageMode = collectionItemsPerPageMode;
            settings.CollectionItemsPerPage = collectionItemsPerPage;
            SaveSettingsChanges();

            collectionCurrentPage = 1;

            ShowDefaultRightPanel();
        }
        private void BuildCollectionPagination(int totalItems)
        {
            CollectionPaginationPanel.Children.Clear();

            collectionTotalPages = Math.Max(
                1,
                (int)Math.Ceiling(totalItems / (double)collectionItemsPerPage));

            if (collectionCurrentPage > collectionTotalPages)
                collectionCurrentPage = collectionTotalPages;

            CollectionPaginationBar.Visibility =
                collectionTotalPages > 1
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (collectionTotalPages <= 1)
                return;

            Button prevButton = new Button
            {
                Content = "<",
                Width = 34,
                Height = 28,
                Margin = new Thickness(0, 0, 6, 0),
                IsEnabled = collectionCurrentPage > 1
            };

            prevButton.Click += (s, e) =>
            {
                if (collectionCurrentPage > 1)
                {
                    collectionCurrentPage--;
                    ShowCollectionTiles(
                        lastCollectionTilesNode,
                        lastCollectionTilesShowRecentlyAdded);
                    CollectionTilesScrollViewer.ScrollToTop();
                }
            };

            CollectionPaginationPanel.Children.Add(prevButton);

            for (int i = 1; i <= collectionTotalPages; i++)
            {
                Button pageButton = new Button
                {
                    Content = i.ToString(),
                    Width = 34,
                    Height = 28,
                    Margin = new Thickness(0, 0, 6, 0),
                    Opacity = i == collectionCurrentPage ? 0.65 : 1.0,
                    FontWeight = i == collectionCurrentPage
                        ? FontWeights.Bold
                        : FontWeights.Normal
                };

                int page = i;

                pageButton.Click += (s, e) =>
                {
                    collectionCurrentPage = page;
                    ShowCollectionTiles(
                        lastCollectionTilesNode,
                        lastCollectionTilesShowRecentlyAdded);
                    CollectionTilesScrollViewer.ScrollToTop();
                };

                CollectionPaginationPanel.Children.Add(pageButton);
            }

            Button nextButton = new Button
            {
                Content = ">",
                Width = 34,
                Height = 28,
                IsEnabled = collectionCurrentPage < collectionTotalPages
            };

            nextButton.Click += (s, e) =>
            {
                if (collectionCurrentPage < collectionTotalPages)
                {
                    collectionCurrentPage++;
                    ShowCollectionTiles(
                        lastCollectionTilesNode,
                        lastCollectionTilesShowRecentlyAdded);
                    CollectionTilesScrollViewer.ScrollToTop();
                }
            };

            CollectionPaginationPanel.Children.Add(nextButton);
        }
        private void SetCollectionCategoryOrder(string orderKey)
        {
            if (!categoryOrders.TryGetValue(orderKey, out string order))
                return;

            collectionCategoryOrder = order;

            settings.CollectionCategoryOrder = collectionCategoryOrder;
            SaveSettingsChanges();

            ShowDefaultRightPanel();
        }
        private string GetGamesCategoryName()
        {
            return currentLanguage == "pl" ? "Gry" : "Games";
        }
        private Hardware? selectedHardwareItem = null;
        private Game? selectedGameItem;
        private PcSystem? selectedPcItem = null;
        private Accessory? selectedAccessoryItem = null;

        private List<CollectionPhoto> currentGamePhotos = new();

        private CollectionPhoto currentGameFrontCover = null;

        private CollectionPhoto currentGameBackCover = null;

        private readonly HashSet<object> expandedPcDetails = new();

        private bool isEditingPc = false;
        private PcSystem? editingPcItem = null;
        private List<CollectionPhoto>? currentPcPhotos = null;
        private TreeViewItem? editingPcTreeItem = null;
        private class TreeNodeData
        {
            public string NodeType { get; set; } = "";
            public string BackgroundGroup { get; set; } = "Default";
            public Game? GameItem { get; set; }
            public string Manufacturer { get; set; } = "";
            public string Family { get; set; } = "";
            public string Platform { get; set; } = "";
            public string Category { get; set; } = "";
            public Hardware? HardwareItem { get; set; }
            public PcSystem? PcItem { get; set; }
            public Accessory? AccessoryItem { get; set; }
        }
        private void TopSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchLabel.Visibility =
                string.IsNullOrWhiteSpace(TopSearchTextBox.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            ClearSearchButton.Visibility =
                string.IsNullOrWhiteSpace(TopSearchTextBox.Text)
                    ? Visibility.Collapsed
                    : Visibility.Visible;

            if (!isSearchActive)
                return;

            if (!string.IsNullOrWhiteSpace(TopSearchTextBox.Text))
                return;

            isSearchActive = false;

            ShowDefaultRightPanel();
        }

        private void SearchLabel_MouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            TopSearchTextBox.Focus();
        }
        private bool UsesDirectCategories(
         string manufacturer,
         string family,
         string platform)
        {
            // CD-i
            if (family == "CD-i")
                return true;

            // 3DO
            if (family == "3DO")
                return true;

            // Neo Geo
            if (family == "Neo Geo")
                return true;

            // Neo Geo Pocket
            if (family == "Neo Geo Pocket")
                return true;

            // WonderSwan
            if (family == "WonderSwan")
                return true;

            // N-Gage
            if (family == "N-Gage")
                return true;

            // GameCube + Panasonic Q
            if (platform == "GameCube" ||
                platform == "Panasonic Q")
                return true;

            return false;
        }

        private string NormalizePlatformForForm(string platform)
        {
            return platform switch
            {
                "PlayStation 4 Slim" => "PlayStation 4",
                "PlayStation 4 Pro" => "PlayStation 4",

                "PlayStation 5 Digital" => "PlayStation 5",
                "PlayStation 5 Slim" => "PlayStation 5",
                "PlayStation 5 Pro" => "PlayStation 5",

                "Xbox One S" => "Xbox One",
                "Xbox One X" => "Xbox One",

                "Nintendo Switch Lite" => "Nintendo Switch",
                "Nintendo Switch OLED" => "Nintendo Switch",

                "Wii Mini" => "Wii",

                _ => platform
            };
        }
        private List<HardwareModel> currentHardwareModels = new();
        public MainWindow()
        {
            InitializeComponent();
            collection = HgcStorageService.LoadCollection() ?? new CollectionDatabase();
            settings = HgcSettingsService.LoadSettings();
            currentOpacity = settings.WindowOpacity;
            defaultCurrency = settings.DefaultCurrency;
            collectionCategoryOrder = settings.CollectionCategoryOrder;
            showHardwareCategory = settings.ShowHardwareCategory;
            showAccessoriesCategory = settings.ShowAccessoriesCategory;
            showGamesCategory = settings.ShowGamesCategory;
            collectionCategoryLines = settings.CollectionCategoryLines;
            collectionCategoryLinesAuto = settings.CollectionCategoryLinesAuto;
            collectionTileSize = settings.CollectionTileSize;
            collectionSortMode = settings.CollectionSortMode;
            collectionItemsPerPage = settings.CollectionItemsPerPage;
            collectionItemsPerPageMode = settings.CollectionItemsPerPageMode;
            showRecentlyAddedPanel = settings.ShowRecentlyAddedPanel;
            FillCurrencyCombo(GamePurchaseCurrencyCombo);
            FillCurrencyCombo(HardwarePurchaseCurrencyCombo);
            FillCurrencyCombo(PcPurchaseCurrencyCombo);
            FillCurrencyCombo(AddAccessoryPurchaseCurrencyCombo);
            BuildDefaultCurrencyMenu();
            InitializeDefaultCurrencyRates();
            EnsureRestoreRecordButton();
            SizeChanged += MainWindow_SizeChanged;

            StateChanged += MainWindow_StateChanged;
            PreviewKeyDown += MainWindow_PreviewKeyDown;

            ApplyTheme(settings.Theme);
            SetOpacityValue(settings.WindowOpacity);
            ApplyLanguage(settings.Language);
            RefreshSortingMenuChecks();

            BuildGamesPanel();
            RebuildCollectionTree();
            RefreshStats();
            ShowDefaultRightPanel();            
        }
        private bool isEditingHardware = false;
        private Hardware? editingHardwareItem = null;
        private TreeViewItem? editingHardwareTreeItem = null;
        private bool isEditingGame = false;
        private Game? editingGameItem = null;
        private TreeViewItem? editingGameTreeItem = null;
        private bool isEditingAccessory = false;
        private Accessory? editingAccessoryItem = null;
        // =========================
        // LANGUAGE
        // =========================

        private readonly Dictionary<string, Dictionary<string, string>> Texts = new()
        {
            ["pl"] = new Dictionary<string, string>
            {
                ["AppName"] = "Hajper Games Collection",
                ["AppShortName"] = "HGC",

                ["File"] = "Plik",
                //["Search"] = "Szukaj",
                ["Stats"] = "Statystyki",
                ["Settings"] = "Ustawienia",
                ["Sorting"] = "Sortowanie",
                ["Info"] = "Informacje",

                ["Language"] = "Język",
                ["Polish"] = "Polski",
                ["English"] = "English",
                ["Theme"] = "Motyw",
                ["Opacity"] = "Przezroczystość",

                ["Add"] = "+ Dodaj grę / sprzęt",
                ["Collection"] = "Kolekcja",
                ["Games"] = "Ilość gier",
                ["Hardware"] = "Ilość sprzętu",


                ["WelcomeTitle"] = "Hajper Games Collection",
                ["WelcomeDescription"] = "",
                ["PreviewTitle"] = "Brak elementów w kolekcji",
                ["PreviewText"] = "",

                ["AddChoiceTitle"] = "Co chcesz dodać?",
                ["AddHardwareAction"] = "Dodaj sprzęt",
                ["AddGameAction"] = "Dodaj grę",

                ["ExampleSony"] = "Sony",
                ["ExamplePS2"] = "PlayStation 2",
                ["ExamplePS3"] = "PlayStation 3",
                ["AddConsole"] = "Dodaj konsolę",
                ["AddAccessories"] = "Dodaj akcesoria",
                ["AddComputer"] = "Dodaj komputer",
                ["RecentlyAddedUpper"] = "OSTATNIO DODANA GRA",
                ["Platform"] = "Platforma",
                ["Genre"] = "Gatunek",
                ["Publisher"] = "Wydawca",
                ["GamePreview"] = "Podgląd gry",
                ["Small"] = "Mały",
                ["Medium"] = "Średni",
                ["Large"] = "Duży",
                ["DateAdded"] = "Data dodania",
                ["SearchPlaceholder"] = "Szukaj...",
                ["AddedDemo"] = "Dodano przykładowe elementy testowe."
            },

            ["en"] = new Dictionary<string, string>
            {
                ["AppName"] = "Hajper Games Collection",
                ["AppShortName"] = "HGC",

                ["File"] = "File",
                //["Search"] = "Search",
                ["Stats"] = "Statistics",
                ["Settings"] = "Settings",
                ["Sorting"] = "Sorting",
                ["Info"] = "Info",

                ["Language"] = "Language",
                ["Polish"] = "Polski",
                ["English"] = "English",
                ["Theme"] = "Theme",
                ["Opacity"] = "Transparency",

                ["Add"] = "+ Add game / hardware",
                ["Collection"] = "Collection",
                ["Games"] = "Games",
                ["Hardware"] = "Hardware",

                ["WelcomeTitle"] = "Hajper Games Collection",
                ["WelcomeDescription"] = "",
                ["PreviewTitle"] = "No items in collection",
                ["PreviewText"] = "",

                ["AddChoiceTitle"] = "What do you want to add?",
                ["AddHardwareAction"] = "Add hardware",
                ["AddGameAction"] = "Add game",

                ["ExampleSony"] = "Sony",
                ["ExamplePS2"] = "PlayStation 2",
                ["ExamplePS3"] = "PlayStation 3",
                ["AddConsole"] = "Add console",
                ["AddAccessories"] = "Add accessories",
                ["AddComputer"] = "Add computer",
                ["RecentlyAddedUpper"] = "LATEST GAME",
                ["Platform"] = "Platform",
                ["Genre"] = "Genre",
                ["Publisher"] = "Publisher",
                ["GamePreview"] = "Game preview",
                ["Small"] = "Small",
                ["Medium"] = "Medium",
                ["Large"] = "Large",
                ["DateAdded"] = "Date added",
                ["SearchPlaceholder"] = "Search...",
                ["AddedDemo"] = "Demo test items added."
            }
        };
        private void MainWindow_StateChanged(
        object? sender,
        EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MaxHeight = SystemParameters.WorkArea.Height + 14;
            }
            else
            {
                MaxHeight = double.PositiveInfinity;
            }
        }
        private void MainWindow_PreviewKeyDown(
        object sender,
        KeyEventArgs e)
        {
            if (e.Key != Key.Escape)
                return;

            if (AddChoicePanel.Visibility == Visibility.Visible)
            {
                AddChoicePanel.Visibility = Visibility.Collapsed;
                AddHardwareSubChoicePanel.Visibility = Visibility.Collapsed;

                isAddChoicePanelOpen = false;

                ShowDefaultRightPanel();

                e.Handled = true;
            }
        }
        private void BuildGamesPanel()
        {
            GamesTree.Items.Clear();
            GamesFiltersPanel.Children.Clear();

            bool hasGames = collection.Games.Any();

            bool hasCollectionTreeItems =
                collection.HardwareItems.Any() ||
                collection.Games.Any() ||
                collection.PcSystems.Any() ||
                collection.Accessories.Any() ||
                collection.ArchivedHardwareItems.Any() ||
                collection.ArchivedGames.Any() ||
                collection.ArchivedPcSystems.Any() ||
                collection.ArchivedAccessories.Any();

            GamesSectionPanel.Visibility = hasGames
                ? Visibility.Visible
                : Visibility.Collapsed;

            PlatformsTreeHeader.Visibility = hasCollectionTreeItems
                ? Visibility.Visible
                : Visibility.Collapsed;

            CollectionTree.Visibility = hasCollectionTreeItems
                ? Visibility.Visible
                : Visibility.Collapsed;

            GamesPlatformsSeparator.Visibility =
                hasGames && hasCollectionTreeItems
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (!hasGames)
                return;

            TreeViewItem allGamesItem = CreateGameTreeItem(
                currentLanguage == "pl" ? "Wszystkie gry" : "All games",
                collection.Games.Count,
                GameFilterMode.All);

            string unclassifiedGenre = currentLanguage == "pl"
                 ? "Niesklasyfikowane"
                 : "Unclassified";

            var genreGroups = collection.Games
                .GroupBy(g =>
                    string.IsNullOrWhiteSpace(g.Genre)
                        ? unclassifiedGenre
                        : g.Genre.Trim())
                .OrderBy(g => g.Key == unclassifiedGenre ? 1 : 0)
                .ThenBy(g => g.Key);

            foreach (var genreGroup in genreGroups)
            {
                allGamesItem.Items.Add(CreateGameTreeItem(
                    GenreDatabase.TranslateGenre(
                        genreGroup.Key,
                        currentLanguage),
                    genreGroup.Count(),
                    GameFilterMode.Genre,
                    genreGroup.Key,
                    false));
            }

            GamesTree.Items.Add(allGamesItem);

            int favoritesCount = collection.Games.Count(g => g.IsFavorite);
            int completedCount = collection.Games.Count(g => g.IsCompleted);
            int uncompletedCount = collection.Games.Count(g => !g.IsCompleted);
            int plannedCount = collection.Games.Count(g => g.IsPlanned);

            if (favoritesCount > 0)
            {
                GamesTree.Items.Add(CreateGameTreeItem(
                    currentLanguage == "pl" ? "Ulubione" : "Favorites",
                    favoritesCount,
                    GameFilterMode.Favorites));
            }

            if (completedCount > 0)
            {
                GamesTree.Items.Add(CreateGameTreeItem(
                    currentLanguage == "pl" ? "Ukończone" : "Completed",
                    completedCount,
                    GameFilterMode.Completed));
            }

            if (uncompletedCount > 0)
            {
                GamesTree.Items.Add(CreateGameTreeItem(
                    currentLanguage == "pl" ? "Nieukończone" : "Uncompleted",
                    uncompletedCount,
                    GameFilterMode.Uncompleted));
            }

            if (plannedCount > 0)
            {
                GamesTree.Items.Add(CreateGameTreeItem(
                    currentLanguage == "pl" ? "Planowane" : "Planned",
                    plannedCount,
                    GameFilterMode.Planned));
            }
        }
        private List<Hardware> SortHardwareItems(IEnumerable<Hardware> items)
        {
            return collectionSortMode switch
            {
                "ZA" => items.OrderByDescending(x => GetHardwareDisplayName(x)).ToList(),
                "Added" => items.OrderByDescending(x => x.DateAdded).ToList(),
                _ => items.OrderBy(x => GetHardwareDisplayName(x)).ToList()
            };
        }

        private List<Game> SortGameItems(IEnumerable<Game> items)
        {
            return collectionSortMode switch
            {
                "ZA" => items.OrderByDescending(x => x.Title).ToList(),
                "Added" => items.OrderByDescending(x => x.DateAdded).ToList(),
                _ => items.OrderBy(x => x.Title).ToList()
            };
        }

        private List<Accessory> SortAccessoryItems(IEnumerable<Accessory> items)
        {
            return collectionSortMode switch
            {
                "ZA" => items.OrderByDescending(x => GetAccessoryDisplayName(x)).ToList(),
                "Added" => items.OrderByDescending(x => x.DateAdded).ToList(),
                _ => items.OrderBy(x => GetAccessoryDisplayName(x)).ToList()
            };
        }

        private List<PcSystem> SortPcItems(IEnumerable<PcSystem> items)
        {
            return collectionSortMode switch
            {
                "ZA" => items.OrderByDescending(x => x.Name).ToList(),
                "Added" => items.OrderByDescending(x => x.DateAdded).ToList(),
                _ => items.OrderBy(x => x.Name).ToList()
            };
        }
        private bool HasSelectedRecordBackground()
        {
            if (selectedGameItem != null)
                return !string.IsNullOrWhiteSpace(selectedGameItem.CustomBackgroundPath);

            if (selectedHardwareItem != null)
                return !string.IsNullOrWhiteSpace(selectedHardwareItem.CustomBackgroundPath);

            if (selectedPcItem != null)
                return !string.IsNullOrWhiteSpace(selectedPcItem.CustomBackgroundPath);

            return false;
        }

        private string T(string key)
        {
            return Texts[currentLanguage].TryGetValue(key, out string value) ? value : key;
        }
        private bool HasCollectionTreeItems()
        {
            return collection.HardwareItems.Any() ||
                   collection.Games.Any() ||
                   collection.PcSystems.Any() ||
                   collection.Accessories.Any();
        }

        private void ApplyLanguage(string language)
        {
            currentLanguage = language;

            Title = T("AppName");

            PlatformsTreeHeader.Text =
                currentLanguage == "pl"
                    ? "PLATFORMY"
                    : "PLATFORMS";

            MenuFile.Header = CreateMenuHeader(
                 "Assets/Icons/menu_file.ico",
                 T("File"));

            MenuStats.Header = CreateMenuHeader(
                "Assets/Icons/menu_stats.ico",
                T("Stats"));

            MenuSettings.Header = CreateMenuHeader(
                "Assets/Icons/menu_settings.ico",
                T("Settings"));

            MenuSorting.Header = CreateMenuHeader(
                "Assets/Icons/menu_sortowanie.ico",
                T("Sorting"));

            MenuInfo.Header = CreateMenuHeader(
                "Assets/Icons/menu_info.ico",
                T("Info"));

            MenuCreateBackup.Header =
                currentLanguage == "pl"
                    ? "Utwórz kopię zapasową"
                    : "Create backup";

            MenuRestoreBackup.Header =
                currentLanguage == "pl"
                    ? "Przywróć kopię zapasową"
                    : "Restore backup";

            RecentlyAddedDeleteButton.ToolTip =
                currentLanguage == "pl" ? "Usuń" : "Delete";

            RecentlyAddedEditButton.ToolTip =
                currentLanguage == "pl" ? "Edytuj" : "Edit";

            RecentlyAddedPreviewButton.ToolTip =
                currentLanguage == "pl" ? "Podgląd gry" : "Game preview";

            RecentlyAddedFavoriteButton.ToolTip =
                recentlyAddedGame != null && recentlyAddedGame.IsFavorite
                    ? (currentLanguage == "pl" ? "Usuń z ulubionych" : "Remove from favorites")
                    : (currentLanguage == "pl" ? "Dodaj do ulubionych" : "Add to favorites");

            MenuInfoAbout.Header =
                currentLanguage == "pl" ? "O programie" : "About";

            MenuInfoRawg.Header =
                "RAWG";

            MenuInfoCopyright.Header =
                currentLanguage == "pl" ? "Prawa autorskie" : "Copyright";

            MenuInfoSupport.Header =
                currentLanguage == "pl" ? "Postaw kawę" : "Buy me a coffee";

            SearchLabel.Text =
                currentLanguage == "pl"
                    ? "Szukaj..."
                    : "Search...";

            MenuClearCollection.Header =
                currentLanguage == "pl"
                    ? "Usuń kolekcję"
                    : "Delete collection";

            MenuRestartApplication.Header =
                currentLanguage == "pl"
                    ? "Restart aplikacji"
                    : "Restart application";

            MenuExitApplication.Header =
                currentLanguage == "pl"
                    ? "Zamknij aplikację"
                    : "Exit";

            MenuCategoryOrder.Header = currentLanguage == "pl"
                ? "Kolejność kategorii"
                : "Category order";

            MenuCategoryOrderHAG.Header = currentLanguage == "pl"
                ? "Sprzęt / Akcesoria / Gry"
                : "Hardware / Accessories / Games";

            MenuCategoryOrderHGA.Header = currentLanguage == "pl"
                ? "Sprzęt / Gry / Akcesoria"
                : "Hardware / Games / Accessories";

            MenuCategoryOrderAHG.Header = currentLanguage == "pl"
                ? "Akcesoria / Sprzęt / Gry"
                : "Accessories / Hardware / Games";

            MenuCategoryOrderAGH.Header = currentLanguage == "pl"
                ? "Akcesoria / Gry / Sprzęt"
                : "Accessories / Games / Hardware";

            MenuCategoryOrderGHA.Header = currentLanguage == "pl"
                ? "Gry / Sprzęt / Akcesoria"
                : "Games / Hardware / Accessories";

            MenuCategoryOrderGAH.Header = currentLanguage == "pl"
                ? "Gry / Akcesoria / Sprzęt"
                : "Games / Accessories / Hardware";

            MenuRecentlyAdded.Header =
                currentLanguage == "pl"
                    ? "Ostatnio dodane"
                    : "Recently added";

            MenuRecentlyAddedShow.Header =
                currentLanguage == "pl"
                    ? "Pokaż"
                    : "Show";

            MenuRecentlyAddedHide.Header =
                currentLanguage == "pl"
                    ? "Ukryj"
                    : "Hide";

            MenuCategoryVisibility.Header = currentLanguage == "pl"
                ? "Wyświetlanie kategorii"
                : "Category visibility";

            MenuShowHardwareCategory.Header = currentLanguage == "pl"
                ? "Sprzęt"
                : "Hardware";

            MenuShowAccessoriesCategory.Header = currentLanguage == "pl"
                ? "Akcesoria"
                : "Accessories";

            MenuShowGamesCategory.Header = currentLanguage == "pl"
                ? "Gry"
                : "Games";

            MenuCategoryLines.Header = currentLanguage == "pl"
                ? "Linie kategorii"
                : "Category rows";
            MenuCategoryLinesAuto.Header = "Auto";
            MenuCategoryLines1.Header = currentLanguage == "pl"
                ? "1 linia"
                : "1 row";
            MenuCategoryLines2.Header = currentLanguage == "pl"
                ? "2 linie"
                : "2 rows";
            MenuCategoryLines3.Header = currentLanguage == "pl"
                ? "3 linie"
                : "3 rows";
            MenuCategoryLines4.Header = currentLanguage == "pl"
                ? "4 linie"
                : "4 rows";
            MenuTileSize.Header = currentLanguage == "pl"
                ? "Rozmiar kafelków"
                : "Tile size";
            MenuRecordSorting.Header = currentLanguage == "pl"
                ? "Sortowanie rekordów"
                : "Record sorting";
            MenuSortAZ.Header = "A-Z";
            MenuSortZA.Header = "Z-A";
            MenuSortAdded.Header = currentLanguage == "pl"
                ? "Data dodania"
                : "Date added";
            MenuItemsPerPage.Header = currentLanguage == "pl"
                ? "Pozycji na stronę"
                : "Items per page";
            MenuDefaultCurrency.Header =
                currentLanguage == "pl"
                    ? "Waluta domyślna"
                    : "Default currency";
            MenuItemsPerPageAuto.Header = "Auto (40)";
            MenuItemsPerPage50.Header = "50";
            MenuItemsPerPage60.Header = "60";
            MenuItemsPerPage75.Header = "75";
            MenuItemsPerPage100.Header = "100";
            MenuTileSizeSmall.Header = T("Small");
            MenuTileSizeMedium.Header = T("Medium");
            MenuTileSizeLarge.Header = T("Large");
            MenuLanguage.Header = T("Language");
            MenuPolish.Header = T("Polish");
            MenuEnglish.Header = T("English");
            MenuTheme.Header = T("Theme");
            MenuOpacity.Header = T("Opacity");
            AddButton.Content = T("Add");
            AddConsoleText.Text = T("AddConsole");
            AddAccessoryText.Text = T("AddAccessories");
            AddPcText.Text = T("AddComputer");
            RecentlyAddedHeaderText.Text = T("RecentlyAddedUpper");
            RecentlyAddedPlatformHeader.Text = T("Platform");
            RecentlyAddedGenreHeader.Text = T("Genre");
            RecentlyAddedPublisherHeader.Text = T("Publisher");
            RecentlyAddedDateHeader.Text = T("DateAdded");
            RecentlyAddedPreviewButton.ToolTip = T("GamePreview");

            PcCardOpacityButton.Content =
                currentLanguage == "pl"
                    ? "Przezroczystość ▼"
                    : "Opacity ▼";

            PcAddPageBackgroundButton.Content =
                currentLanguage == "pl"
                    ? "Tło"
                    : "Background";

            PcCardOpacityButton.Content = T("Opacity") + " ▼";
            PcAddPageBackgroundButton.Content = currentLanguage == "pl" ? "Tło" : "Background";
            AccessoryPhotosPlaceholderText.Text = currentLanguage == "pl" ? "Zdjęcia akcesorium" : "Accessory photos";
            AccessoryActionsTitle.Text = currentLanguage == "pl" ? "Zarządzanie" : "Management";
            EditAccessoryButton.Content = currentLanguage == "pl" ? "Edytuj" : "Edit";
            AccessoryCardOpacityButton.Content = T("Opacity") + " 100% ▼";
            DeleteAccessoryButton.Content = currentLanguage == "pl" ? "Usuń" : "Delete";
            AccessoryExtraDetailsTitle.Text = currentLanguage == "pl" ? "Szczegóły" : "Details";

            MainTitle.Visibility = Visibility.Collapsed;

            MainDescription.Text = "";
            MainDescription.Visibility = Visibility.Collapsed;

            PreviewTitle.Text = T("PreviewTitle");

            PreviewText.Text = "";
            PreviewText.Visibility = Visibility.Collapsed;

            AddChoiceTitle.Text = T("AddChoiceTitle");
            AddHardwareText.Text = T("AddHardwareAction");
            AddGameText.Text = T("AddGameAction");

            GamesTreeHeader.Text = currentLanguage == "pl" ? "GRY" : "GAMES";

            ApplyRecordActionsLanguage();
            ApplyAddAccessoryFormLanguage();
            RefreshStats();

            if (PreviewPlaceholder.Visibility == Visibility.Visible)
            {
                PreviewTitle.Text =
                    currentLanguage == "pl"
                        ? "Rozpocznij budowę swojej kolekcji"
                        : "Start building your collection";

                PreviewSubtitle.Text =
                    currentLanguage == "pl"
                        ? "Dodaj pierwszą grę, konsolę lub komputer."
                        : "Add your first game, console or computer.";
            }

            UpdateLanguageMenuChecks();

            settings.Language = language;
            SaveSettingsChanges();
        }
        private void TopSearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ShowSearchResults(TopSearchTextBox.Text);
                e.Handled = true;
            }
        }
        private bool TextContains(string source, string query)
        {
            return !string.IsNullOrWhiteSpace(source) &&
                   source.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool HardwareMatchesSearch(Hardware item, string query)
        {
            return TextContains(GetHardwareDisplayName(item), query) ||
                   TextContains(item.Manufacturer, query) ||
                   TextContains(item.Model, query) ||
                   TextContains(item.SerialNumber, query) ||
                   TextContains(item.Notes, query);
        }

        private bool GameMatchesSearch(Game item, string query)
        {
            return TextContains(item.Title, query) ||
                   TextContains(item.ShortDescription, query) ||
                   TextContains(item.Manufacturer, query) ||
                   TextContains(item.Family, query) ||
                   TextContains(item.Platform, query) ||
                   TextContains(item.OperatingSystem, query) ||
                   TextContains(item.Publisher, query) ||
                   TextContains(item.Genre, query) ||
                   TextContains(item.Region, query) ||
                   TextContains(item.MediaType, query) ||
                   TextContains(item.Notes, query);
        }

        private bool PcMatchesSearch(PcSystem item, string query)
        {
            return TextContains(item.Name, query) ||
                   TextContains(item.Manufacturer, query) ||
                   TextContains(item.Model, query) ||
                   TextContains(item.SerialNumber, query) ||
                   TextContains(item.OperatingSystem, query) ||
                   TextContains(item.Notes, query);
        }

        private bool AccessoryMatchesSearch(Accessory item, string query)
        {
            return TextContains(GetAccessoryDisplayName(item), query) ||
                   TextContains(item.AccessoryType, query) ||
                   TextContains(item.Manufacturer, query) ||
                   TextContains(item.Model, query) ||
                   TextContains(item.AssignedManufacturer, query) ||
                   TextContains(item.AssignedFamily, query) ||
                   TextContains(item.AssignedPlatform, query) ||
                   TextContains(item.ControllerManufacturer, query) ||
                   TextContains(item.ControllerModel, query) ||
                   TextContains(item.Condition, query) ||
                   TextContains(item.SerialNumber, query) ||
                   TextContains(item.Notes, query);
        }
        private void ShowSearchResults(string query)
        {
            query = query.Trim();

            isSearchActive = true;

            if (string.IsNullOrWhiteSpace(query))
            {
                ShowDefaultRightPanel();
                return;
            }

            HideRightPanels();

            RecentlyAddedBorder.Visibility = Visibility.Collapsed;

            CollectionTilesPanel.Visibility = Visibility.Visible;
            CollectionTilesSectionsPanel.Children.Clear();
            currentCollectionTilesWrap = null;

            CollectionTilesTitle.Text =
            currentLanguage == "pl"
                ? "WYNIKI WYSZUKIWANIA"
                : "SEARCH RESULTS";

            CollectionTilesTitle.FontSize = 14;
            CollectionTilesTitle.FontWeight = FontWeights.SemiBold;
            CollectionTilesTitle.Opacity = 0.75;
            CollectionTilesTitle.Effect = null;

            CollectionTilesTitle.Visibility = Visibility.Visible;

            var hardwareResults = collection.HardwareItems
                .Where(x => HardwareMatchesSearch(x, query))
                .ToList();

            var accessoryResults = collection.Accessories
                .Where(x => AccessoryMatchesSearch(x, query))
                .ToList();

            var gameResults = collection.Games
                .Where(x => GameMatchesSearch(x, query))
                .ToList();

            var pcResults = collection.PcSystems
                .Where(x => PcMatchesSearch(x, query))
                .ToList();

            if (hardwareResults.Count > 0)
            {
                AddTilesSectionTitle(currentLanguage == "pl" ? "Sprzęt" : "Hardware");

                foreach (var item in hardwareResults)
                    AddHardwareTile(item);
            }

            if (accessoryResults.Count > 0)
            {
                AddTilesSectionTitle(currentLanguage == "pl" ? "Akcesoria" : "Accessories");

                foreach (var item in accessoryResults)
                    AddAccessoryTile(item);
            }

            if (gameResults.Count > 0)
            {
                AddTilesSectionTitle(currentLanguage == "pl" ? "Gry" : "Games");

                foreach (var item in gameResults)
                    AddGameTile(item);
            }

            if (pcResults.Count > 0)
            {
                AddTilesSectionTitle("PC");

                foreach (var item in pcResults)
                    AddPcTile(item);
            }

            if (hardwareResults.Count == 0 &&
                accessoryResults.Count == 0 &&
                gameResults.Count == 0 &&
                pcResults.Count == 0)
            {
                TextBlock emptyText = new TextBlock
                {
                    Text = currentLanguage == "pl"
                        ? "BRAK WYNIKÓW"
                        : "NO RESULTS",
                    FontSize = 13,
                    Opacity = 0.65,
                    Margin = new Thickness(0, 12, 0, 0)
                };

                CollectionTilesSectionsPanel.Children.Add(emptyText);
            }

        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            TopSearchTextBox.Clear();
            TopSearchTextBox.Focus();
        }
        private void MenuPolish_Click(object sender, RoutedEventArgs e)
        {
            if (currentLanguage == "pl")
                return;

            bool restart = HgcMessageBox.ShowYesNo(
                this,
                "Zmiana języka wymaga ponownego uruchomienia programu.\n\n" +
                "Zapisz bieżące operacje i uruchom program ponownie.",
                "HGC",
                HgcMessageBoxType.Question,
                "pl");

            if (!restart)
            {
                UpdateLanguageMenuChecks();
                return;
            }

            ApplyLanguage("pl");

            Process.Start(new ProcessStartInfo
            {
                FileName = Environment.ProcessPath!,
                Arguments = "--no-splash",
                UseShellExecute = true
            });

            Application.Current.Shutdown();
        }

        private void MenuEnglish_Click(object sender, RoutedEventArgs e)
        {
            if (currentLanguage == "en")
                return;

            bool restart = HgcMessageBox.ShowYesNo(
                this,
                "Changing the language requires restarting the application.\n\n" +
                "Save your current work and restart the application.",
                "HGC",
                HgcMessageBoxType.Question,
                "en");

            if (!restart)
            {
                UpdateLanguageMenuChecks();
                return;
            }

            ApplyLanguage("en");

            Process.Start(new ProcessStartInfo
            {
                FileName = Environment.ProcessPath!,
                Arguments = "--no-splash",
                UseShellExecute = true
            });

            Application.Current.Shutdown();
        }

        // =========================
        // THEMES
        // =========================

        private void ApplyTheme(string theme)
        {
            currentTheme = theme;

            switch (theme)
            {
                case "PlayStation":
                    SetTheme("#08111F", "#101B2E", "#14243D", "#1E3F75", "#2D7DFF", "#174C9A", "#EAF2FF", "#FFFFFF", "#284B80", "#0F1A2A");
                    break;

                case "Xbox":
                    SetTheme("#071007", "#0E1A0E", "#122512", "#183818", "#36D145", "#1F7A2A", "#EFFFF0", "#071007", "#2F7036", "#102010");
                    break;

                case "SEGA":
                    SetTheme("#071226", "#0D1D3D", "#102852", "#143A7A", "#1E7BFF", "#1756B8", "#F2F7FF", "#FFFFFF", "#2E62B8", "#0D1F42");
                    break;

                case "Nintendo":
                    SetTheme("#1A0606", "#260B0B", "#351010", "#4A1515", "#E60012", "#9D0010", "#FFF2F2", "#FFFFFF", "#8A2028", "#2A0A0A");
                    break;

                case "Atari":
                    SetTheme("#4A0000", "#6B0000", "#850000", "#A00000", "#151515", "#151515", "#FFFFFF", "#FFFFFF", "#FF8080", "#650000");
                    break;

                case "Commodore":
                    SetTheme("#1A1A1A", "#103A8C", "#1449B0", "#1852C5", "#FFFFFF", "#D92A2A", "#FFFFFF", "#0A2F73", "#5A8DFF", "#123B90");
                    break;

                case "Modern":
                    SetTheme(
                        "#03070C", // tło aplikacji - ciemniejsze
                        "#07121E", // title/menu
                        "#0B1826", // panele
                        "#102235", // content
                        "#148CAD", // accent / przyciski - ciemniejszy cyan
                        "#123241", // hover menu
                        "#F2FAFF", // tekst
                        "#F2FAFF", // tekst przycisków
                        "#24566A", // border
                        "#081522"  // preview/stats
                    );
                    break;

                default:
                    SetTheme("#0C0D0F", "#121417", "#181B1F", "#20242A", "#B85F0D", "#5F3000", "#ECECEC", "#ECECEC", "#353A42", "#14171A");
                    break;

            }

            Opacity = currentOpacity / 100.0;

            RefreshThemeChecks();
            UpdateThemeFont();
            UpdateAddChoiceIcons();
            UpdateEmptyCollectionIcon();
            UpdateBackgroundForCurrentState();

            if (AddPcFormPanel.Visibility == Visibility.Visible &&
            editingPcItem != null)
            {
                BuildPcEditComponentsPanel(editingPcItem);
            }

            if (selectedGameItem != null &&
             HardwareDetailsViewPanel.Visibility == Visibility.Visible)
            {
                LoadGamePhotosPreview(selectedGameItem);
            }

            if (selectedHardwareItem != null &&
                HardwareDetailsViewPanel.Visibility == Visibility.Visible)
            {
                LoadHardwarePhotosPreview(selectedHardwareItem);
            }

            if (selectedPcItem != null &&
                PcDetailsPanel.Visibility == Visibility.Visible)
            {
                LoadPcPhotosPreview(selectedPcItem);
            }

            settings.Theme = theme;
            SaveSettingsChanges();
            UpdateRecentlyAddedIcons();

        }
        private void ApplyDynamicPcPanelStyle(Border border)
        {
            border.SetResourceReference(Border.BackgroundProperty, "PanelBackgroundBrush");
            border.SetResourceReference(Border.BorderBrushProperty, "AppBorderBrush");
        }

        private void SetTheme(
            string appBg,
            string menuBg,
            string panelBg,
            string contentBg,
            string accent,
            string hover,
            string text,
            string buttonText,
            string border,
            string previewBox)
        {
            SetThemeResource("AppBackgroundBrush", appBg);
            SetThemeResource("TitleBarBrush", appBg);
            SetMenuBarGradient(menuBg);
            SetThemeResource("MenuPopupBackgroundBrush", menuBg);
            SetThemeResource("MenuPopupBorderBrush", accent);
            SetThemeResource("MenuHoverBrush", hover);

            SetThemeResource("PanelBackgroundBrush", panelBg);
            SetLeftPanelGradient(panelBg);
            SetThemeResource("ContentBackgroundBrush", contentBg);
            SetThemeResource("AppAccentBrush", accent);
            SetButtonGradient(accent);
            SetThemeResource("AppTextBrush", text);
            SetThemeResource("ButtonTextBrush", buttonText);
            SetThemeResource("AppBorderBrush", border);
            SetThemeResource("StatsBackgroundBrush", previewBox);
            SetThemeResource("PreviewBoxBrush", previewBox);
            SetPreviewBoxGradient(previewBox);
        }
        private void ChangeTheme(string theme)
        {
            if (currentTheme == theme)
                return;

            bool restart = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "Zmiana motywu wymaga ponownego uruchomienia aplikacji.\n\nZapisz bieżącą pracę i uruchom aplikację ponownie."
                    : "Changing the theme requires restarting the application.\n\nSave your current work and restart the application.",
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!restart)
                return;

            ApplyTheme(theme);

            Process.Start(new ProcessStartInfo
            {
                FileName = Environment.ProcessPath!,
                Arguments = "--no-splash",
                UseShellExecute = true
            });

            Application.Current.Shutdown();
        }
        private void SetPreviewBoxGradient(string baseHex)
        {
            Color c = (Color)ColorConverter.ConvertFromString(baseHex);

            LinearGradientBrush brush = new()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };

            brush.GradientStops.Add(new GradientStop(Shift(c, -0.04), 0.00));
            brush.GradientStops.Add(new GradientStop(Shift(c, +0.02), 0.50));
            brush.GradientStops.Add(new GradientStop(Shift(c, -0.04), 1.00));

            Resources["PreviewBoxGradientBrush"] = brush;
            Application.Current.Resources["PreviewBoxGradientBrush"] = brush;
        }
        private void SetButtonGradient(string accentHex)
        {
            Color c = (Color)ColorConverter.ConvertFromString(accentHex);

            LinearGradientBrush brush = new()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };

            brush.GradientStops.Add(new GradientStop(Shift(c, +0.15), 0.00));
            brush.GradientStops.Add(new GradientStop(c, 0.50));
            brush.GradientStops.Add(new GradientStop(Shift(c, -0.15), 1.00));

            Resources["ButtonGradientBrush"] = brush;
            Application.Current.Resources["ButtonGradientBrush"] = brush;
        }
        private void SetThemeResource(string key, string hex)
        {
            SolidColorBrush brush = Brush(hex);

            Resources[key] = brush;
            Application.Current.Resources[key] = brush;
        }
        private void SetMenuBarGradient(string baseHex)
        {
            Color c = (Color)ColorConverter.ConvertFromString(baseHex);

            LinearGradientBrush brush = new()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };

            brush.GradientStops.Add(new GradientStop(Shift(c, +0.01), 0.00));
            brush.GradientStops.Add(new GradientStop(c, 0.50));
            brush.GradientStops.Add(new GradientStop(Shift(c, -0.01), 1.00));

            Resources["MenuBackgroundBrush"] = brush;
            Application.Current.Resources["MenuBackgroundBrush"] = brush;
        }
        private StackPanel CreateMenuHeader(string iconPath, string text)
        {
            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            Image icon = new Image
            {
                Source = new BitmapImage(new Uri(iconPath, UriKind.Relative)),
                Width = 20,
                Height = 20,
                Margin = new Thickness(0, 0, 2, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Opacity = 0.90
            };

            TextBlock label = new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center
            };

            panel.Children.Add(icon);
            panel.Children.Add(label);

            return panel;
        }

        private void SetLeftPanelGradient(string baseHex)
        {
            Color c = (Color)ColorConverter.ConvertFromString(baseHex);

            LinearGradientBrush brush = new()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };

            brush.GradientStops.Add(
                new GradientStop(Shift(c, -0.10), 0.00));

            brush.GradientStops.Add(
                new GradientStop(Shift(c, +0.05), 0.50));

            brush.GradientStops.Add(
                new GradientStop(Shift(c, -0.10), 1.00));

            Resources["LeftPanelBackgroundBrush"] = brush;
            Application.Current.Resources["LeftPanelBackgroundBrush"] = brush;
        }

        private Color Shift(Color color, double amount)
        {
            if (amount > 0)
            {
                byte r = (byte)(color.R + (255 - color.R) * amount);
                byte g = (byte)(color.G + (255 - color.G) * amount);
                byte b = (byte)(color.B + (255 - color.B) * amount);

                return Color.FromRgb(r, g, b);
            }

            amount = Math.Abs(amount);

            byte dr = (byte)(color.R * (1 - amount));
            byte dg = (byte)(color.G * (1 - amount));
            byte db = (byte)(color.B * (1 - amount));

            return Color.FromRgb(dr, dg, db);
        }

        private Color Lighten(Color color, double amount)
        {
            byte r = (byte)(color.R + (255 - color.R) * amount);
            byte g = (byte)(color.G + (255 - color.G) * amount);
            byte b = (byte)(color.B + (255 - color.B) * amount);

            return Color.FromRgb(r, g, b);
        }

        private Color Darken(Color color, double amount)
        {
            byte r = (byte)(color.R * (1 - amount));
            byte g = (byte)(color.G * (1 - amount));
            byte b = (byte)(color.B * (1 - amount));

            return Color.FromRgb(r, g, b);
        }

        private SolidColorBrush Brush(string hex)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }

        private void RefreshThemeChecks()
        {
            SetThemeHeader(MenuThemeDefault, "Default");
            SetThemeHeader(MenuThemeModern, "Modern");
            SetThemeHeader(MenuThemePlayStation, "PlayStation");
            SetThemeHeader(MenuThemeXbox, "Xbox");
            SetThemeHeader(MenuThemeSega, "SEGA");
            SetThemeHeader(MenuThemeNintendo, "Nintendo");
            SetThemeHeader(MenuThemeAtari, "Atari");
            SetThemeHeader(MenuThemeCommodore, "Commodore");
        }

        private void SetThemeHeader(MenuItem item, string themeName)
        {
            string displayName = themeName switch
            {
                "Default" => "Classic",
                "Modern" => "Modern",
                "PlayStation" => "PS",
                "Nintendo" => "The Big N",
                "Xbox" => "Redmond",
                "SEGA" => "Blue Team",
                "Commodore" => "Breadbox",
                "Atari" => "Vader",
                _ => themeName
            };

            item.Header =
                currentTheme == themeName
                    ? $"✓ {displayName}"
                    : displayName;
        }

        private void UpdateThemeFont()
        {
            string fontName = currentTheme switch
            {
                "PlayStation" => "Segoe UI Semibold",
                "Xbox" => "Segoe UI Black",
                "SEGA" => "Arial Black",
                "Nintendo" => "Trebuchet MS",
                "Atari" => "Consolas",
                "Commodore" => "Courier New",
                _ => "Segoe UI"
            };

            Resources["AppFontFamily"] = new FontFamily(fontName);
        }

        private void UpdateAddChoiceIcons()
        {
            string prefix = currentTheme switch
            {
                "PlayStation" => "ps",
                "Xbox" => "xbox",
                "SEGA" => "sega",
                "Nintendo" => "nintendo",
                "Atari" => "atari",
                "Commodore" => "commodore",
                "Modern" => "modern",
                _ => "default"
            };

            AddHardwareIcon.Source = LoadIcon($"{prefix}_hardware.png");
            AddGameIcon.Source = LoadIcon($"{prefix}_game.png");

            AddPcIcon.Source = LoadIcon($"{prefix}_addcomputer.png");
            AddAccessoryIcon.Source = LoadIcon($"{prefix}_addaccessories.png");
            AddConsoleIcon.Source = LoadIcon($"{prefix}_addconsole.png");
        }
        private void UpdateEmptyCollectionIcon()
        {
            string prefix = currentTheme switch
            {
                "Modern" => "modern",
                "PlayStation" => "ps",
                "Xbox" => "xbox",
                "SEGA" => "sega",
                "Nintendo" => "nintendo",
                "Atari" => "atari",
                "Commodore" => "commodore",
                _ => "default"
            };

            EmptyCollectionIcon.Source =
                LoadIcon($"{prefix}_emptycollection.png");
        }
        private BitmapImage LoadIcon(string fileName)
        {
            BitmapImage image = new BitmapImage();

            image.BeginInit();
            image.UriSource = new Uri($"Assets/Icons/{fileName}", UriKind.Relative);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.EndInit();
            image.Freeze();

            return image;
        }

        private void ThemeDefault_Click(object sender, RoutedEventArgs e) => ChangeTheme("Default");
        private void ThemeModern_Click(object sender, RoutedEventArgs e) => ChangeTheme("Modern");
        private void ThemePlayStation_Click(object sender, RoutedEventArgs e) => ChangeTheme("PlayStation");
        private void ThemeXbox_Click(object sender, RoutedEventArgs e) => ChangeTheme("Xbox");
        private void ThemeSega_Click(object sender, RoutedEventArgs e) => ChangeTheme("SEGA");
        private void ThemeNintendo_Click(object sender, RoutedEventArgs e) => ChangeTheme("Nintendo");
        private void ThemeAtari_Click(object sender, RoutedEventArgs e) => ChangeTheme("Atari");
        private void ThemeCommodore_Click(object sender, RoutedEventArgs e) => ChangeTheme("Commodore");

        // =========================
        // OPACITY
        // =========================

        private void SetOpacityValue(int value)
        {
            if (value < 50)
                value = 50;

            currentOpacity = value;

            Opacity = currentOpacity / 100.0;

            settings.WindowOpacity = currentOpacity;
            SaveSettingsChanges();

            RefreshOpacityChecks();
        }

        private void RefreshOpacityChecks()
        {
            SetOpacityHeader(MenuOpacity100, 100);
            SetOpacityHeader(MenuOpacity90, 90);
            SetOpacityHeader(MenuOpacity80, 80);
            SetOpacityHeader(MenuOpacity70, 70);
            SetOpacityHeader(MenuOpacity60, 60);
            SetOpacityHeader(MenuOpacity50, 50);
        }

        private void SetOpacityHeader(MenuItem item, int value)
        {
            item.Header = currentOpacity == value ? $"✓ {value}%" : $"{value}%";
        }

        private void Opacity100_Click(object sender, RoutedEventArgs e) => SetOpacityValue(100);
        private void Opacity90_Click(object sender, RoutedEventArgs e) => SetOpacityValue(90);
        private void Opacity80_Click(object sender, RoutedEventArgs e) => SetOpacityValue(80);
        private void Opacity70_Click(object sender, RoutedEventArgs e) => SetOpacityValue(70);
        private void Opacity60_Click(object sender, RoutedEventArgs e) => SetOpacityValue(60);
        private void Opacity50_Click(object sender, RoutedEventArgs e) => SetOpacityValue(50);

        // =========================
        // EMPTY COLLECTION / ADD CHOICE
        // =========================

        private void RefreshEmptyCollectionView()
        {
            CollectionTree.Items.Clear();

            currentPlatformGroup = "Default";
            UpdateBackgroundForCurrentState();

            PreviewPlaceholder.Visibility = Visibility.Visible;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if 
            (
                AddHardwareFormPanel.Visibility == Visibility.Visible ||
                AddGameFormPanel.Visibility == Visibility.Visible ||
                AddPcFormPanel.Visibility == Visibility.Visible ||
                AddAccessoryFormPanel.Visibility == Visibility.Visible ||
                AddServicePanel.Visibility == Visibility.Visible)
            {
                bool result = HgcMessageBox.ShowYesNo(
         this,
         currentLanguage == "pl"
             ? "Czy chcesz przerwać obecną operację i dodać nowy rekord?"
             : "Do you want to cancel the current operation and add a new record?",
         "HGC",
         HgcMessageBoxType.Question,
         currentLanguage);

                if (!result)
                    return;

         AddAccessoryFormPanel.Visibility = Visibility.Collapsed;
         AddAccessoryDetailsColumn.Visibility = Visibility.Collapsed;

            }

            ClearSelectedRecordForNonRecordView();

            HideRightPanels();

            RecordActionsPanel.Visibility = Visibility.Collapsed;
            HardwareRightDetailsColumn.Visibility = Visibility.Collapsed;
            GameRightDetailsColumn.Visibility = Visibility.Collapsed;
            PcRightDetailsColumn.Visibility = Visibility.Collapsed;

            AddChoicePanel.Visibility = Visibility.Visible;
            isAddChoicePanelOpen = true;

            UpdateAddChoiceIcons();
        }
        private void AddHardwareButton_Click(object sender, RoutedEventArgs e)
        {
            AddHardwareSubChoicePanel.Visibility =
                AddHardwareSubChoicePanel.Visibility == Visibility.Visible
                    ? Visibility.Collapsed
                    : Visibility.Visible;
        }
        private void AddConsoleButton_Click(object sender, RoutedEventArgs e)
        {
            AddChoicePanel.Visibility = Visibility.Collapsed;
            AddHardwareSubChoicePanel.Visibility = Visibility.Collapsed;
            isAddChoicePanelOpen = false;

            AddHardwareFormTitle.Text =
                currentLanguage == "pl"
                    ? "Dodaj konsolę"
                    : "Add console";

            ShowAddHardwareForm();
        }
        private void ShowAddAccessoryForm()
        {
            currentPlatformGroup = "Default";
            UpdateBackgroundForCurrentState();

            PreviewPlaceholder.Visibility = Visibility.Collapsed;
            AddAccessoryFormPanel.Visibility = Visibility.Visible;
            AddAccessoryDetailsColumn.Visibility = Visibility.Collapsed;

            ToggleAccessoryDetailsButton.Content =
            currentLanguage == "pl"
                ? "Więcej szczegółów"
                : "More details";

            LoadAddAccessoryForm();

            HardwareDetailsViewPanel.Visibility = Visibility.Collapsed;
            RecordActionsPanel.Visibility = Visibility.Collapsed;
        }
        private void AddAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            AddChoicePanel.Visibility = Visibility.Collapsed;
            AddHardwareSubChoicePanel.Visibility = Visibility.Collapsed;
            isAddChoicePanelOpen = false;

            ShowAddAccessoryForm();
        }
        private bool addAccessoryDetailsVisible = false;
        private void LoadAccessoryRefreshRateOptions()
        {
            string selectText = currentLanguage == "pl" ? "Wybierz" : "Select";

            string[] refreshRates =
            {
        "50 Hz",
        "60 Hz",
        "75 Hz",
        "100 Hz",
        "120 Hz",
        "144 Hz",
        "165 Hz",
        "180 Hz",
        "200 Hz",
        "240 Hz",
        "360 Hz",
        "480 Hz"
    };

            AddAccessoryRefreshRateCombo.Items.Clear();
            AddAccessoryRefreshRateCombo.Items.Add(selectText);

            AddAccessoryTvRefreshRateCombo.Items.Clear();
            AddAccessoryTvRefreshRateCombo.Items.Add(selectText);

            foreach (string rate in refreshRates)
            {
                AddAccessoryRefreshRateCombo.Items.Add(rate);
                AddAccessoryTvRefreshRateCombo.Items.Add(rate);
            }

            AddAccessoryRefreshRateCombo.SelectedIndex = 0;
            AddAccessoryTvRefreshRateCombo.SelectedIndex = 0;
        }
        private void LoadAddAccessoryForm()
        {
            ClearAccessoryForm();

            addAccessoryDetailsVisible = false;

            ToggleAccessoryDetailsButton.Content =
                currentLanguage == "pl" ? "Więcej szczegółów" : "More details";

            AddAccessoryDetailsColumn.Visibility = Visibility.Collapsed;

            LoadAddAccessoryTypes();
            LoadAddAccessoryConditions();
            LoadAddAccessoryAssignmentCombos();
            LoadControllerManufacturers();
            LoadDisplaySpecificOptions();
            LoadAccessoryResolutionOptions();
            LoadAccessoryRefreshRateOptions();
            LoadDisplayPanelTypes();
            LoadDisplayBacklightTypes();
        }
        private void ClearAccessoryForm()
        {
            AddAccessoryManufacturerTextBox.Clear();
            AddAccessoryModelTextBox.Clear();
            AddAccessorySerialNumberTextBox.Clear();
            AddAccessoryNotesTextBox.Clear();
            AddAccessoryCustomControllerModelTextBox.Clear();
            AddAccessoryDisplaySizeTextBox.Clear();
            AddAccessoryResponseTimeTextBox.Clear();
            AddAccessoryAnsiLumensTextBox.Clear();
            AddAccessoryLampLifeTextBox.Clear();
            AddAccessoryTvDisplaySizeTextBox.Clear();
            AddAccessoryAnsiLumensTextBox.Clear();
            AddAccessoryLampLifeTextBox.Clear();
            AddAccessoryPurchasePriceTextBox.Clear();
            AddAccessoryPurchaseCurrencyCombo.SelectedItem = defaultCurrency;
            AddAccessoryPurchaseDatePicker.SelectedDate = null;
            AddAccessorySmartTvCheckBox.IsChecked = false;
            AddAccessoryTvTunerCheckBox.IsChecked = false;
            AddAccessoryTvHdrCheckBox.IsChecked = false;
            AddAccessoryHdrCheckBox.IsChecked = false;       
            AddAccessorySmartTvCheckBox.IsChecked = false;
            AddAccessoryTvTunerCheckBox.IsChecked = false;
            AddAccessoryControllerPanel.Visibility = Visibility.Collapsed;
            AddAccessoryMonitorPanel.Visibility = Visibility.Collapsed;
            AddAccessoryTelevisionPanel.Visibility = Visibility.Collapsed;
            AddAccessoryProjectorPanel.Visibility = Visibility.Collapsed;
            AddAccessoryCustomControllerModelLabel.Visibility = Visibility.Collapsed;
            AddAccessoryCustomControllerModelTextBox.Visibility = Visibility.Collapsed;
            currentAccessoryPhotos = new List<CollectionPhoto>();
        }
        private void LoadAccessoryFamiliesForSelectedManufacturer()
        {
            if (AddAccessoryAssignedFamilyCombo == null)
                return;

            AddAccessoryAssignedFamilyCombo.Items.Clear();
            AddAccessoryAssignedPlatformCombo.Items.Clear();

            string manufacturer =
                AddAccessoryAssignedManufacturerCombo.SelectedItem?.ToString() ?? "";

            string selectManufacturerText =
                currentLanguage == "pl" ? "Wybierz" : "Select";

            if (string.IsNullOrWhiteSpace(manufacturer) ||
                manufacturer == selectManufacturerText)
            {
                AddAccessoryAssignedFamilyCombo.Items.Add(
                    currentLanguage == "pl"
                        ? "Wybierz producenta/grupę"
                        : "Select manufacturer/group");

                AddAccessoryAssignedPlatformCombo.Items.Add(
                    currentLanguage == "pl"
                        ? "Wybierz rodzinę"
                        : "Select family");

                AddAccessoryAssignedFamilyCombo.SelectedIndex = 0;
                AddAccessoryAssignedPlatformCombo.SelectedIndex = 0;
                return;
            }

            AddAccessoryAssignedFamilyCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            AddAccessoryAssignedPlatformCombo.Items.Add(
                currentLanguage == "pl"
                    ? "Wybierz rodzinę"
                    : "Select family");

            if (manufacturer == "PC")
            {
                AddAccessoryAssignedFamilyCombo.Items.Add("PC");
                AddAccessoryAssignedFamilyCombo.SelectedIndex = 0;

                LoadAccessoryPlatformsForSelectedFamily();
                return;
            }

            if (manufacturer == "Apple")
            {
                AddAccessoryAssignedFamilyCombo.Items.Add("Mac");
                AddAccessoryAssignedFamilyCombo.SelectedIndex = 0;
                return;
            }

            var families = ConsolePlatformDatabase
                .GetFamilies()
                .Where(x => x.Manufacturer == manufacturer)
                .Select(x => x.Name)
                .ToList();

            var customFamilies = collection.HardwareItems
                .Where(x => x.Manufacturer == manufacturer)
                .Select(x => x.Family)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            families = families
                .Concat(customFamilies)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            foreach (string family in families)
                AddAccessoryAssignedFamilyCombo.Items.Add(family);

            AddAccessoryAssignedFamilyCombo.SelectedIndex = 0;
        }
        private void LoadAccessoryPlatformsForSelectedFamily()
        {
            if (AddAccessoryAssignedPlatformCombo == null)
                return;

            AddAccessoryAssignedPlatformCombo.Items.Clear();

            string manufacturer =
                AddAccessoryAssignedManufacturerCombo.SelectedItem?.ToString() ?? "";

            string family =
                AddAccessoryAssignedFamilyCombo.SelectedItem?.ToString() ?? "";

            string familyPlaceholder =
                currentLanguage == "pl"
                    ? "Wybierz producenta/grupę"
                    : "Select manufacturer/group";

            if (string.IsNullOrWhiteSpace(family) ||
                family == familyPlaceholder)
            {
                AddAccessoryAssignedPlatformCombo.Items.Add(
                    currentLanguage == "pl"
                        ? "Wybierz rodzinę"
                        : "Select family");

                AddAccessoryAssignedPlatformCombo.SelectedIndex = 0;
                return;
            }

            AddAccessoryAssignedPlatformCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            if (manufacturer == "PC")
            {
                AddAccessoryAssignedPlatformCombo.Items.Add("Windows");
                AddAccessoryAssignedPlatformCombo.Items.Add("DOS");
                AddAccessoryAssignedPlatformCombo.Items.Add("Linux");
                AddAccessoryAssignedPlatformCombo.Items.Add(
                    currentLanguage == "pl" ? "Inne" : "Other");

                AddAccessoryAssignedPlatformCombo.SelectedIndex = 0;
                return;
            }

            if (manufacturer == "Apple")
            {
                AddAccessoryAssignedPlatformCombo.Items.Add("Classic Mac OS");
                AddAccessoryAssignedPlatformCombo.Items.Add("Mac OS X");
                AddAccessoryAssignedPlatformCombo.Items.Add("macOS");
                AddAccessoryAssignedPlatformCombo.Items.Add(
                    currentLanguage == "pl" ? "Inne" : "Other");

                AddAccessoryAssignedPlatformCombo.SelectedIndex = 0;
                return;
            }

            var platforms = ConsolePlatformDatabase
                .GetPlatforms()
                .Where(x =>
                    x.Manufacturer == manufacturer &&
                    x.Family == family)
                .Select(x => NormalizePlatformForForm(x.Name))
                .ToList();

            var customPlatforms = collection.HardwareItems
                .Where(x =>
                    x.Manufacturer == manufacturer &&
                    x.Family == family)
                .Select(x => x.Platform)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            platforms = platforms
                .Concat(customPlatforms)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            foreach (string platform in platforms)
                AddAccessoryAssignedPlatformCombo.Items.Add(platform);

            AddAccessoryAssignedPlatformCombo.SelectedIndex = 0;
        }
        private void AddAccessoryAssignedManufacturerCombo_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
        {
            LoadAccessoryFamiliesForSelectedManufacturer();
        }

        private void AddAccessoryAssignedFamilyCombo_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            LoadAccessoryPlatformsForSelectedFamily();
        }
        private void LoadAddAccessoryTypes()
        {
            AddAccessoryTypeCombo.Items.Clear();

            string[] types = currentLanguage == "pl"
    ? new[]
    {
        "Adapter",
        "Akcesorium kolekcjonerskie",
        "Akumulator",
        "Arcade Stick",
        "Czytnik kart",
        "Dysk zewnętrzny",
        "Gitara",
        "Głośniki",
        "Gogle VR",
        "Hub USB",
        "Joystick",
        "Kamera",
        "Kamera internetowa",
        "Karta dźwiękowa",
        "Karta pamięci",
        "Kierownica",
        "Klawiatura",
        "Kontroler VR",
        "Ładowarka",
        "Mata taneczna",
        "Mikrofon",
        "Monitor",
        "Mysz",
        "Pad",
        "Pendrive",
        "Perkusja",
        "Pilot",
        "Pistolet świetlny",
        "Power Bank",
        "Projektor",
        "Przejściówka",
        "Słuchawki",
        "Stacja dokująca",
        "Telewizor",
        "Tracker VR",
        "Zasilacz",
        "Inne"
    }
                   : new[]
    {
        "Adapter",
        "Arcade Stick",
        "Battery Pack",
        "Camera",
        "Card Reader",
        "Charger",
        "Collectible Accessory",
        "Controller",
        "Converter",
        "Dance Mat",
        "Docking Station",
        "Drum Controller",
        "External Drive",
        "Guitar Controller",
        "Headphones",
        "Joystick",
        "Keyboard",
        "Light Gun",
        "Memory Card",
        "Microphone",
        "Monitor",
        "Mouse",
        "Power Bank",
        "Power Supply",
        "Projector",
        "Racing Wheel",
        "Remote Controller",
        "Sound Card",
        "Speakers",
        "Television",
        "USB Flash Drive",
        "USB Hub",
        "VR Controller",
        "VR Headset",
        "VR Tracker",
        "Webcam",
        "Other"
    };

            foreach (string type in types)
                AddAccessoryTypeCombo.Items.Add(type);

            AddAccessoryTypeCombo.SelectedIndex = 0;
        }

        private void LoadAddAccessoryConditions()
        {
            AddAccessoryConditionCombo.Items.Clear();

            AddAccessoryConditionCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            string[] conditions = currentLanguage == "pl"
                ? new[]
                {
            "Nowy",
            "Bardzo dobry",
            "Dobry",
            "Dostateczny",
            "Uszkodzony"
                }
                : new[]
                {
            "New",
            "Very good",
            "Good",
            "Fair",
            "Damaged"
                };

            foreach (string condition in conditions)
                AddAccessoryConditionCombo.Items.Add(condition);

            AddAccessoryConditionCombo.SelectedIndex = 0;
        }

        private void LoadAddAccessoryAssignmentCombos()
        {
            AddAccessoryAssignedManufacturerCombo.Items.Clear();
            AddAccessoryAssignedFamilyCombo.Items.Clear();
            AddAccessoryAssignedPlatformCombo.Items.Clear();

            string selectText = currentLanguage == "pl" ? "Wybierz" : "Select";

            AddAccessoryAssignedManufacturerCombo.Items.Add(selectText);
            AddAccessoryAssignedFamilyCombo.Items.Add(selectText);
            AddAccessoryAssignedPlatformCombo.Items.Add(selectText);

            HashSet<string> manufacturers = new(
            ConsolePlatformDatabase
        .   GetManufacturers()
        .   Select(x => x.Name));

            manufacturers.Add("PC");
            manufacturers.Add("Apple");

            foreach (var hardware in collection.HardwareItems)
            {
                if (!string.IsNullOrWhiteSpace(hardware.Manufacturer))
                    manufacturers.Add(hardware.Manufacturer);
            }

            foreach (string manufacturer in manufacturers.OrderBy(x => x))
                AddAccessoryAssignedManufacturerCombo.Items.Add(manufacturer);

            AddAccessoryAssignedManufacturerCombo.SelectedIndex = 0;
            AddAccessoryAssignedFamilyCombo.SelectedIndex = 0;
            AddAccessoryAssignedPlatformCombo.SelectedIndex = 0;
        }        
        private void LoadControllerManufacturers()
        {
            AddAccessoryControllerManufacturerCombo.Items.Clear();

            AddAccessoryControllerManufacturerCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            AddAccessoryControllerManufacturerCombo.Items.Add("Sony");
            AddAccessoryControllerManufacturerCombo.Items.Add("Microsoft");
            AddAccessoryControllerManufacturerCombo.Items.Add("Nintendo");
            AddAccessoryControllerManufacturerCombo.Items.Add("Sega");
            AddAccessoryControllerManufacturerCombo.Items.Add("Atari");

            AddAccessoryControllerManufacturerCombo.Items.Add(
                currentLanguage == "pl" ? "Inny" : "Other");

            AddAccessoryControllerManufacturerCombo.SelectedIndex = 0;
        }
        private int GetCollectionTilesPerRow()
        {
            double availableWidth = CollectionTilesPanel.ActualWidth;

            if (availableWidth <= 0)
                availableWidth = CollectionTilesSectionsPanel.ActualWidth;

            if (availableWidth <= 0)
                availableWidth = 900;

            // kafelek 150 + prawy margines 16
            int tileFullWidth = (int)GetCollectionTileWidth() + 16;

            int count = (int)Math.Floor((availableWidth - 40) / tileFullWidth);

            return Math.Max(1, count);
        }
        private double GetCollectionTileWidth()
        {
            return collectionTileSize switch
            {
                "Small" => 110,
                "Large" => 200,
                _ => 150
            };
        }
        private double GetCollectionHardwareTileHeight()
        {
            return collectionTileSize switch
            {
                "Small" => 120,
                "Large" => 190,
                _ => 150
            };
        }

        private double GetCollectionGameTileHeight()
        {
            return collectionTileSize switch
            {
                "Small" => 165,
                "Large" => 270,
                _ => 205
            };
        }

        private double GetCollectionTileImageSize()
        {
            return collectionTileSize switch
            {
                "Small" => 90,
                "Large" => 175,
                _ => 130
            };
        }

        private double GetCollectionHardwareImageHeight()
        {
            return collectionTileSize switch
            {
                "Small" => 75,
                "Large" => 135,
                _ => 100
            };
        }
        private int GetCollectionCategoryTileLimit()
        {
            int lines = collectionCategoryLinesAuto
                ? 1
                : collectionCategoryLines;

            return GetCollectionTilesPerRow() * lines;
        }
        private void AddAccessoryControllerModelCombo_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
        {
            string model =
                AddAccessoryControllerModelCombo.SelectedItem?.ToString() ?? "";

            bool showCustom =
                model == "Inny" ||
                model == "Other";

            AddAccessoryCustomControllerModelLabel.Visibility =
                showCustom
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            AddAccessoryCustomControllerModelTextBox.Visibility =
                showCustom
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (!showCustom)
                AddAccessoryCustomControllerModelTextBox.Text = "";
        }

        private void AddAccessoryTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string type = AddAccessoryTypeCombo.SelectedItem?.ToString() ?? "";

            bool isController =
                type == "Pad" ||
                type == "Controller";

            AddAccessoryControllerPanel.Visibility =
                isController ? Visibility.Visible : Visibility.Collapsed;

            AddAccessoryMonitorPanel.Visibility = Visibility.Collapsed;
            AddAccessoryTelevisionPanel.Visibility = Visibility.Collapsed;
            AddAccessoryProjectorPanel.Visibility = Visibility.Collapsed;

            if (type == "Monitor")
            {
                AddAccessoryMonitorPanel.Visibility = Visibility.Visible;
            }
            else if (type == "Telewizor" || type == "Television")
            {
                AddAccessoryTelevisionPanel.Visibility = Visibility.Visible;
            }
            else if (type == "Projektor" || type == "Projector")
            {
                AddAccessoryProjectorPanel.Visibility = Visibility.Visible;
            }
        }
        private void LoadAccessoryResolutionOptions()
        {
            string selectText = currentLanguage == "pl" ? "Wybierz" : "Select";

            string[] monitorResolutions =
            {
        "320x200",
        "320x240",
        "640x200",
        "640x350",
        "640x400",
        "640x480",
        "800x600",
        "1024x768",
        "1152x864",
        "1280x720",
        "1280x768",
        "1280x800",
        "1280x960",
        "1280x1024",
        "1360x768",
        "1366x768",
        "1400x1050",
        "1440x900",
        "1600x900",
        "1600x1200",
        "1680x1050",
        "1920x1080",
        "1920x1200",
        "2048x1152",
        "2560x1080",
        "2560x1440",
        "2560x1600",
        "3440x1440",
        "3840x1600",
        "3840x2160",
        "4096x2160",
        "5120x1440",
        "5120x2160",
        "5120x2880",
        "6016x3384",
        "6144x3456",
        "7680x2160",
        "7680x4320"
        };

            string[] tvResolutions =
            {
        "320x200",
        "320x240",
        "512x384",
        "640x200",
        "640x240",
        "640x350",
        "640x400",
        "640x480",
        "720x480",
        "720x576",
        "768x576",
        "800x600",
        "852x480",
        "1024x576",
        "1024x768",
        "1280x720",
        "1366x768",
        "1920x1080",
        "2560x1440",
        "3840x2160",
        "7680x4320"
        };

            string[] projectorResolutions =
            {
        "640x480",
        "800x600",
        "1024x768",
        "1280x720",
        "1280x800",
        "1366x768",
        "1920x1080",
        "1920x1200",
        "2560x1440",
        "3840x2160",
        "4096x2160"
        };

            AddAccessoryDisplayResolutionCombo.Items.Clear();
            AddAccessoryDisplayResolutionCombo.Items.Add(selectText);
            foreach (string resolution in monitorResolutions)
                AddAccessoryDisplayResolutionCombo.Items.Add(resolution);
            AddAccessoryDisplayResolutionCombo.SelectedIndex = 0;

            AddAccessoryTvResolutionCombo.Items.Clear();
            AddAccessoryTvResolutionCombo.Items.Add(selectText);
            foreach (string resolution in tvResolutions)
                AddAccessoryTvResolutionCombo.Items.Add(resolution);
            AddAccessoryTvResolutionCombo.SelectedIndex = 0;

            AddAccessoryProjectorResolutionCombo.Items.Clear();
            AddAccessoryProjectorResolutionCombo.Items.Add(selectText);
            foreach (string resolution in projectorResolutions)
                AddAccessoryProjectorResolutionCombo.Items.Add(resolution);
            AddAccessoryProjectorResolutionCombo.SelectedIndex = 0;
        }
        private void LoadDisplaySpecificOptions()
        {
            AddAccessoryAspectRatioCombo.Items.Clear();

            AddAccessoryAspectRatioCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            AddAccessoryAspectRatioCombo.Items.Add("16:9");
            AddAccessoryAspectRatioCombo.Items.Add("16:10");
            AddAccessoryAspectRatioCombo.Items.Add("21:9");
            AddAccessoryAspectRatioCombo.Items.Add("32:9");
            AddAccessoryAspectRatioCombo.Items.Add("4:3");
            AddAccessoryAspectRatioCombo.Items.Add("5:4");

            AddAccessoryAspectRatioCombo.SelectedIndex = 0;

            AddAccessorySyncTechnologyCombo.Items.Clear();

            AddAccessorySyncTechnologyCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            AddAccessorySyncTechnologyCombo.Items.Add(
                currentLanguage == "pl" ? "Brak" : "None");

            AddAccessorySyncTechnologyCombo.Items.Add("G-Sync");
            AddAccessorySyncTechnologyCombo.Items.Add("FreeSync");
            AddAccessorySyncTechnologyCombo.Items.Add("G-Sync Compatible");

            AddAccessorySyncTechnologyCombo.SelectedIndex = 0;

            AddAccessoryTvSystemCombo.Items.Clear();

            AddAccessoryTvSystemCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            AddAccessoryTvSystemCombo.Items.Add("Android TV");
            AddAccessoryTvSystemCombo.Items.Add("Google TV");
            AddAccessoryTvSystemCombo.Items.Add("Tizen");
            AddAccessoryTvSystemCombo.Items.Add("webOS");
            AddAccessoryTvSystemCombo.Items.Add("Fire TV");
            AddAccessoryTvSystemCombo.Items.Add("Roku");

            AddAccessoryTvSystemCombo.SelectedIndex = 0;

            AddAccessoryProjectionTechnologyCombo.Items.Clear();

            AddAccessoryProjectionTechnologyCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            AddAccessoryProjectionTechnologyCombo.Items.Add("LCD");
            AddAccessoryProjectionTechnologyCombo.Items.Add("DLP");
            AddAccessoryProjectionTechnologyCombo.Items.Add("LCoS");
            AddAccessoryProjectionTechnologyCombo.Items.Add("Laser");

            AddAccessoryProjectionTechnologyCombo.SelectedIndex = 0;
        }
        private void LoadDisplayOptions()
        {
            AddAccessoryDisplayResolutionCombo.Items.Clear();
            AddAccessoryDisplayResolutionCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");

            AddAccessoryDisplayResolutionCombo.Items.Add("640x480");
            AddAccessoryDisplayResolutionCombo.Items.Add("800x600");
            AddAccessoryDisplayResolutionCombo.Items.Add("1024x768");
            AddAccessoryDisplayResolutionCombo.Items.Add("1280x720");
            AddAccessoryDisplayResolutionCombo.Items.Add("1366x768");
            AddAccessoryDisplayResolutionCombo.Items.Add("1600x900");
            AddAccessoryDisplayResolutionCombo.Items.Add("1920x1080");
            AddAccessoryDisplayResolutionCombo.Items.Add("2560x1440");
            AddAccessoryDisplayResolutionCombo.Items.Add("3440x1440");
            AddAccessoryDisplayResolutionCombo.Items.Add("3840x2160");
            AddAccessoryDisplayResolutionCombo.Items.Add("5120x1440");
            AddAccessoryDisplayResolutionCombo.Items.Add("7680x4320");

            AddAccessoryDisplayResolutionCombo.SelectedIndex = 0;
                        
            AddAccessoryRefreshRateCombo.Items.Clear();

            AddAccessoryRefreshRateCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            AddAccessoryRefreshRateCombo.Items.Add("50 Hz");
            AddAccessoryRefreshRateCombo.Items.Add("60 Hz");
            AddAccessoryRefreshRateCombo.Items.Add("75 Hz");
            AddAccessoryRefreshRateCombo.Items.Add("100 Hz");
            AddAccessoryRefreshRateCombo.Items.Add("120 Hz");
            AddAccessoryRefreshRateCombo.Items.Add("144 Hz");
            AddAccessoryRefreshRateCombo.Items.Add("165 Hz");
            AddAccessoryRefreshRateCombo.Items.Add("180 Hz");
            AddAccessoryRefreshRateCombo.Items.Add("200 Hz");
            AddAccessoryRefreshRateCombo.Items.Add("240 Hz");
            AddAccessoryRefreshRateCombo.Items.Add("360 Hz");
            AddAccessoryRefreshRateCombo.Items.Add("480 Hz");

            AddAccessoryRefreshRateCombo.SelectedIndex = 0;
        }
        private void InitializeDefaultCurrencyRates()
        {
            if (collection.CurrencyRates.Count > 0)
                return;

            collection.CurrencyRates.Add(new CurrencyRate { Currency = "PLN", Rate = 1.00m });
            collection.CurrencyRates.Add(new CurrencyRate { Currency = "EUR", Rate = 4.30m });
            collection.CurrencyRates.Add(new CurrencyRate { Currency = "USD", Rate = 4.00m });
            collection.CurrencyRates.Add(new CurrencyRate { Currency = "GBP", Rate = 5.00m });
            collection.CurrencyRates.Add(new CurrencyRate { Currency = "CHF", Rate = 4.60m });
            collection.CurrencyRates.Add(new CurrencyRate { Currency = "HUF", Rate = 0.011m });
            collection.CurrencyRates.Add(new CurrencyRate { Currency = "CZK", Rate = 0.17m });
            collection.CurrencyRates.Add(new CurrencyRate { Currency = "JPY", Rate = 0.027m });
            collection.CurrencyRates.Add(new CurrencyRate { Currency = "CNY", Rate = 0.55m });
        }
        private void AddAccessoryControllerManufacturerCombo_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
        {
            LoadControllerModels();
        }
        private void LoadControllerModels()
        {
            AddAccessoryControllerModelCombo.Items.Clear();

            string manufacturer =
                AddAccessoryControllerManufacturerCombo.SelectedItem?.ToString() ?? "";

            if (manufacturer == "Wybierz" ||
                manufacturer == "Select" ||
                string.IsNullOrWhiteSpace(manufacturer))
            {
                AddAccessoryControllerModelCombo.Items.Add(
                    currentLanguage == "pl"
                        ? "Wybierz producenta pada"
                        : "Select controller manufacturer");
            }
            else
            {
                AddAccessoryControllerModelCombo.Items.Add(
                    currentLanguage == "pl"
                        ? "Wybierz"
                        : "Select");
            }

            if (manufacturer == "Sony")
            {
                AddAccessoryControllerModelCombo.Items.Add("Dual Analog Controller");
                AddAccessoryControllerModelCombo.Items.Add("DualShock");
                AddAccessoryControllerModelCombo.Items.Add("DualShock 2");
                AddAccessoryControllerModelCombo.Items.Add("DualShock 3");
                AddAccessoryControllerModelCombo.Items.Add("DualShock 4");
                AddAccessoryControllerModelCombo.Items.Add("DualSense");
            }
            else if (manufacturer == "Microsoft")
            {
                AddAccessoryControllerModelCombo.Items.Add("Xbox Controller");
                AddAccessoryControllerModelCombo.Items.Add("Xbox 360 Controller");
                AddAccessoryControllerModelCombo.Items.Add("Xbox One Controller");
                AddAccessoryControllerModelCombo.Items.Add("Xbox Series Controller");
            }
            else if (manufacturer == "Nintendo")
            {
                AddAccessoryControllerModelCombo.Items.Add("NES Controller");
                AddAccessoryControllerModelCombo.Items.Add("SNES Controller");
                AddAccessoryControllerModelCombo.Items.Add("Nintendo 64 Controller");
                AddAccessoryControllerModelCombo.Items.Add("GameCube Controller");
                AddAccessoryControllerModelCombo.Items.Add("Wii Remote");
                AddAccessoryControllerModelCombo.Items.Add("Wii U Pro Controller");
                AddAccessoryControllerModelCombo.Items.Add("Joy-Con");
                AddAccessoryControllerModelCombo.Items.Add("Switch Pro Controller");
            }
            else if (manufacturer == "Sega")
            {
                AddAccessoryControllerModelCombo.Items.Add("Master System Controller");
                AddAccessoryControllerModelCombo.Items.Add("Mega Drive Controller");
                AddAccessoryControllerModelCombo.Items.Add("Saturn Controller");
                AddAccessoryControllerModelCombo.Items.Add("Dreamcast Controller");
            }
            else if (manufacturer == "Atari")
            {
                AddAccessoryControllerModelCombo.Items.Add("Atari 2600 Joystick");
                AddAccessoryControllerModelCombo.Items.Add("Atari Jaguar Controller");
            }

            if (!string.IsNullOrWhiteSpace(manufacturer) &&
                manufacturer != "Wybierz" &&
                manufacturer != "Select")
            {
                AddAccessoryControllerModelCombo.Items.Add(
                    currentLanguage == "pl" ? "Inny" : "Other");
            }

            if (AddAccessoryControllerModelCombo.Items.Count > 0)
                AddAccessoryControllerModelCombo.SelectedIndex = 0;

            AddAccessoryControllerModelCombo.SelectedIndex = 0;
        }
        private void ToggleAccessoryDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            addAccessoryDetailsVisible = !addAccessoryDetailsVisible;

            AddAccessoryDetailsColumn.Visibility =
                addAccessoryDetailsVisible ? Visibility.Visible : Visibility.Collapsed;

            ToggleAccessoryDetailsButton.Content =
                addAccessoryDetailsVisible
                    ? (currentLanguage == "pl" ? "Mniej szczegółów" : "Less details")
                    : (currentLanguage == "pl" ? "Więcej szczegółów" : "More details");
        }

        private void CancelAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            bool result = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? (isEditingAccessory
                        ? "Czy na pewno chcesz anulować edytowanie akcesorium?\n\nWprowadzone zmiany zostaną utracone."
                        : "Czy na pewno chcesz anulować dodawanie akcesorium?\n\nWprowadzone dane zostaną utracone.")
                    : (isEditingAccessory
                        ? "Are you sure you want to cancel editing the accessory?\n\nEntered changes will be lost."
                        : "Are you sure you want to cancel adding the accessory?\n\nEntered data will be lost."),
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!result)
                return;

            ClearAccessoryForm();

            AddAccessoryFormPanel.Visibility = Visibility.Collapsed;

            ShowDefaultRightPanel(false);
        }
        private void UpdateRecentlyAddedPanel()
        {
            if (!showRecentlyAddedPanel)
            {
                recentlyAddedGame = null;
                RecentlyAddedBorder.Visibility = Visibility.Collapsed;
                return;
            }

            Game? latestGame = collection.Games
                .Where(x => !x.IsArchived)
                .OrderByDescending(x => x.DateAdded)
                .FirstOrDefault();

            if (latestGame == null)
            {
                recentlyAddedGame = null;
                RecentlyAddedBorder.Visibility = Visibility.Collapsed;
                return;
            }

            RecentlyAddedBorder.Visibility = Visibility.Visible;
            recentlyAddedGame = latestGame;
            UpdateRecentlyAddedIcons();

            RecentlyAddedFavoriteButton.ToolTip =
                latestGame.IsFavorite
                    ? (currentLanguage == "pl" ? "Usuń z ulubionych" : "Remove from favorites")
                    : (currentLanguage == "pl" ? "Dodaj do ulubionych" : "Add to favorites");

            RecentlyAddedTitle.Text = latestGame.Title;

            CollectionPhoto? mainPhoto = null;

            if (latestGame.FrontCoverPhoto != null && latestGame.FrontCoverPhoto.IsMain)
                mainPhoto = latestGame.FrontCoverPhoto;

            if (mainPhoto == null &&
                latestGame.BackCoverPhoto != null &&
                latestGame.BackCoverPhoto.IsMain)
            {
                mainPhoto = latestGame.BackCoverPhoto;
            }

            if (mainPhoto == null)
                mainPhoto = latestGame.Photos?.FirstOrDefault(x => x.IsMain);

            if (mainPhoto == null)
                mainPhoto = latestGame.FrontCoverPhoto;

            if (mainPhoto == null)
                mainPhoto = latestGame.BackCoverPhoto;

            if (mainPhoto == null)
                mainPhoto = latestGame.Photos?.FirstOrDefault();

            if (mainPhoto != null && File.Exists(mainPhoto.OriginalPath))
            {
                RecentlyAddedImage.Source =
                    LoadImageWithoutLock(mainPhoto.OriginalPath, 420);
            }
            else
            {
                RecentlyAddedImage.Source =
                    new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/nophoto.png"));
            }

            RecentlyAddedImage.OpacityMask = new VisualBrush
            {
                Visual = new Border
                {
                    Width = 120,
                    Height = 170,
                    CornerRadius = new CornerRadius(7),
                    Background = Brushes.Black
                }
            };

            RecentlyAddedImageContainer.Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 10,
                ShadowDepth = 6,
                Opacity = 0.45
            };

            RecentlyAddedPlatformText.Text =
                latestGame.Manufacturer == "PC"
                    ? "PC"
                    : latestGame.Platform;

            BuildRecentlyAddedRating(latestGame.UserRating);

            RecentlyAddedStatusText.Text =
                latestGame.IsCompleted
                    ? (currentLanguage == "pl" ? "Ukończona" : "Completed")
                    : (currentLanguage == "pl" ? "Nieukończona" : "Not completed");

            RecentlyAddedGenreText.Text =
                GenreDatabase.TranslateGenre(latestGame.Genre, currentLanguage);

            RecentlyAddedPublisherText.Text =
                latestGame.Publisher;

            RecentlyAddedStatusHeader.Text =
                currentLanguage == "pl" ? "Status" : "Status";


            RecentlyAddedDateText.Text =
                latestGame.DateAdded.ToString("yyyy-MM-dd");

            RecentlyAddedDescription.Text =
                latestGame.ShortDescription;
        }       
        private void MenuStats_Click(object sender, RoutedEventArgs e)
        {
            StatisticsWindow window = new StatisticsWindow(
                collection,
                currentLanguage,
                defaultCurrency);

            window.Owner = this;

            window.ShowDialog();
        }
        private void RecentlyAddedPreviewButton_Click(
        object sender,
        RoutedEventArgs e)
        {
            if (recentlyAddedGame == null)
                return;

            ShowGameDetails(recentlyAddedGame);
        }
        private void BuildRecentlyAddedRating(double? rating)
        {
            RecentlyAddedRatingPanel.Children.Clear();

            if (!rating.HasValue)
                return;

            double value = rating.Value;

            Brush filledBrush = Brushes.Gold;
            Brush emptyBrush = new SolidColorBrush(Color.FromArgb(70, 255, 255, 255));

            for (int i = 1; i <= 10; i++)
            {
                double fillAmount;

                if (value >= i)
                    fillAmount = 1.0;
                else if (value >= i - 0.5)
                    fillAmount = 0.42;
                else
                    fillAmount = 0.0;

                Grid starGrid = new Grid
                {
                    Width = 18,
                    Height = 20,
                    Margin = new Thickness(0, 0, 2, 0)
                };

                TextBlock emptyStar = new TextBlock
                {
                    Text = "★",
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Foreground = emptyBrush
                };

                TextBlock filledStar = new TextBlock
                {
                    Text = "★",
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Foreground = filledBrush
                };

                if (fillAmount < 1.0)
                {
                    filledStar.Clip = new RectangleGeometry(
                        new Rect(0, 0, 14 * fillAmount, 20));
                }

                starGrid.Children.Add(emptyStar);

                if (fillAmount > 0)
                    starGrid.Children.Add(filledStar);

                RecentlyAddedRatingPanel.Children.Add(starGrid);
            }

            TextBlock ratingText = new TextBlock
            {
                Text = $"{value:0.0}/10",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Opacity = 0.75,
                Margin = new Thickness(8, 1, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            RecentlyAddedRatingPanel.Children.Add(ratingText);
        }

        private void SaveAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            bool wasEditing = isEditingAccessory;

            Accessory accessory;

            if (wasEditing && editingAccessoryItem != null)
                accessory = editingAccessoryItem;
            else
                accessory = new Accessory();

            accessory.AccessoryType = AddAccessoryTypeCombo.Text;
            accessory.Manufacturer = AddAccessoryManufacturerTextBox.Text.Trim();
            accessory.Model = AddAccessoryModelTextBox.Text.Trim();

            if (!ValidateNameLength(GetAccessoryDisplayName(accessory)))
                return;

            accessory.Photos = currentAccessoryPhotos;
            accessory.HdrSupport =
                AddAccessoryHdrCheckBox.IsChecked == true ||
                AddAccessoryTvHdrCheckBox.IsChecked == true;

            currentAccessoryPhotos = accessory.Photos != null
                    ? accessory.Photos
                    : new List<CollectionPhoto>();

            accessory.AssignedManufacturer =
                AddAccessoryAssignedManufacturerCombo.Text.StartsWith("Wybierz") ||
                AddAccessoryAssignedManufacturerCombo.Text.StartsWith("Select")
                    ? ""
                    : AddAccessoryAssignedManufacturerCombo.Text;

            accessory.AssignedFamily =
                AddAccessoryAssignedFamilyCombo.Text.StartsWith("Wybierz") ||
                AddAccessoryAssignedFamilyCombo.Text.StartsWith("Select")
                    ? ""
                    : AddAccessoryAssignedFamilyCombo.Text;

            accessory.AssignedPlatform =
                AddAccessoryAssignedPlatformCombo.Text.StartsWith("Wybierz") ||
                AddAccessoryAssignedPlatformCombo.Text.StartsWith("Select")
                    ? ""
                    : AddAccessoryAssignedPlatformCombo.Text;

            accessory.Condition =
                AddAccessoryConditionCombo.Text.StartsWith("Wybierz") ||
                AddAccessoryConditionCombo.Text.StartsWith("Select")
                    ? ""
                    : NormalizeConditionForSave(AddAccessoryConditionCombo.Text);

            accessory.SerialNumber = AddAccessorySerialNumberTextBox.Text.Trim();
            accessory.Notes = AddAccessoryNotesTextBox.Text.Trim();

            string controllerModel = AddAccessoryControllerModelCombo.Text;

            if (controllerModel == "Inny" || controllerModel == "Other")
                controllerModel = AddAccessoryCustomControllerModelTextBox.Text.Trim();

            accessory.ControllerManufacturer =
                AddAccessoryControllerManufacturerCombo.Text.StartsWith("Wybierz") ||
                AddAccessoryControllerManufacturerCombo.Text.StartsWith("Select")
                    ? ""
                    : AddAccessoryControllerManufacturerCombo.Text;

            accessory.ControllerModel =
                controllerModel.StartsWith("Wybierz") ||
                controllerModel.StartsWith("Select")
                    ? ""
                    : controllerModel;

            if (accessory.AccessoryType == "Monitor")
            {
                accessory.DisplaySize =
                    AddAccessoryDisplaySizeTextBox.Text.Trim();

                accessory.DisplayResolution =
                    AddAccessoryDisplayResolutionCombo.Text.StartsWith("Wybierz") ||
                    AddAccessoryDisplayResolutionCombo.Text.StartsWith("Select")
                        ? ""
                        : AddAccessoryDisplayResolutionCombo.Text;

                accessory.DisplayPanelType =
                    AddAccessoryDisplayPanelTypeCombo.Text.StartsWith("Wybierz") ||
                    AddAccessoryDisplayPanelTypeCombo.Text.StartsWith("Select")
                        ? ""
                        : AddAccessoryDisplayPanelTypeCombo.Text;

                accessory.DisplayBacklightType =
                    AddAccessoryDisplayBacklightTypeCombo.Text.StartsWith("Wybierz") ||
                    AddAccessoryDisplayBacklightTypeCombo.Text.StartsWith("Select")
                        ? ""
                        : AddAccessoryDisplayBacklightTypeCombo.Text;

                accessory.RefreshRate =
                    AddAccessoryRefreshRateCombo.Text.StartsWith("Wybierz") ||
                    AddAccessoryRefreshRateCombo.Text.StartsWith("Select")
                        ? ""
                        : AddAccessoryRefreshRateCombo.Text;

                accessory.AspectRatio =
                    AddAccessoryAspectRatioCombo.Text.StartsWith("Wybierz") ||
                    AddAccessoryAspectRatioCombo.Text.StartsWith("Select")
                        ? ""
                        : AddAccessoryAspectRatioCombo.Text;

                accessory.ResponseTime =
                    AddAccessoryResponseTimeTextBox.Text.Trim();

                accessory.SyncTechnology =
                    AddAccessorySyncTechnologyCombo.Text.StartsWith("Wybierz") ||
                    AddAccessorySyncTechnologyCombo.Text.StartsWith("Select")
                        ? ""
                        : AddAccessorySyncTechnologyCombo.Text;
            }
            else if (accessory.AccessoryType == "Telewizor")
            {
                accessory.DisplaySize =
                    AddAccessoryTvDisplaySizeTextBox.Text.Trim();

                accessory.DisplayResolution =
                    AddAccessoryTvResolutionCombo.Text.StartsWith("Wybierz") ||
                    AddAccessoryTvResolutionCombo.Text.StartsWith("Select")
                        ? ""
                        : AddAccessoryTvResolutionCombo.Text;

                accessory.DisplayPanelType =
                    AddAccessoryTvPanelTypeCombo.Text.StartsWith("Wybierz") ||
                    AddAccessoryTvPanelTypeCombo.Text.StartsWith("Select")
                        ? ""
                        : AddAccessoryTvPanelTypeCombo.Text;

                accessory.DisplayBacklightType =
                    AddAccessoryTvBacklightTypeCombo.Text.StartsWith("Wybierz") ||
                    AddAccessoryTvBacklightTypeCombo.Text.StartsWith("Select")
                        ? ""
                        : AddAccessoryTvBacklightTypeCombo.Text;

                accessory.RefreshRate =
                    AddAccessoryTvRefreshRateCombo.Text.StartsWith("Wybierz") ||
                    AddAccessoryTvRefreshRateCombo.Text.StartsWith("Select")
                        ? ""
                        : AddAccessoryTvRefreshRateCombo.Text;
            }
            else if (accessory.AccessoryType == "Projektor")
            {
                accessory.DisplayResolution =
                    AddAccessoryProjectorResolutionCombo.Text.StartsWith("Wybierz") ||
                    AddAccessoryProjectorResolutionCombo.Text.StartsWith("Select")
                        ? ""
                        : AddAccessoryProjectorResolutionCombo.Text;
            }

            accessory.SmartTv = AddAccessorySmartTvCheckBox.IsChecked == true;

            accessory.TvSystem =
                AddAccessoryTvSystemCombo.Text.StartsWith("Wybierz") ||
                AddAccessoryTvSystemCombo.Text.StartsWith("Select")
                    ? ""
                    : AddAccessoryTvSystemCombo.Text;

            accessory.TvTuner = AddAccessoryTvTunerCheckBox.IsChecked == true;

            accessory.ProjectionTechnology =
                AddAccessoryProjectionTechnologyCombo.Text.StartsWith("Wybierz") ||
                AddAccessoryProjectionTechnologyCombo.Text.StartsWith("Select")
                    ? ""
                    : AddAccessoryProjectionTechnologyCombo.Text;

            accessory.AnsiLumens = AddAccessoryAnsiLumensTextBox.Text.Trim();
            accessory.LampLife = AddAccessoryLampLifeTextBox.Text.Trim();

            if (decimal.TryParse(AddAccessoryPurchasePriceTextBox.Text.Trim(), out decimal accessoryPrice))
                accessory.PurchasePrice = accessoryPrice;
            else
                accessory.PurchasePrice = 0;

            accessory.PurchaseCurrency =
                AddAccessoryPurchaseCurrencyCombo.SelectedItem?.ToString()
                ?? defaultCurrency;

            if (!wasEditing)
            {
                collection.Accessories.Add(accessory);
                AddAccessoryToTree(accessory);
            }

            isEditingAccessory = false;
            editingAccessoryItem = null;

            HgcMessageBox.Show(
                this,
                currentLanguage == "pl"
                    ? (wasEditing
                        ? "Akcesorium zostało zaktualizowane."
                        : "Akcesorium zostało dodane.")
                    : (wasEditing
                        ? "Accessory updated."
                        : "Accessory added."),
                "HGC",
                HgcMessageBoxType.Information,
                currentLanguage);

            AddAccessoryFormPanel.Visibility = Visibility.Collapsed;
            AddAccessoryDetailsColumn.Visibility = Visibility.Collapsed;

            RefreshStats();

            SaveCollectionChanges();

            ShowDefaultRightPanel();

        }
        private void EditAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccessoryItem == null)
                return;

            isEditingAccessory = true;
            editingAccessoryItem = selectedAccessoryItem;

            AddAccessoryFormTitle.Text =
                currentLanguage == "pl"
                    ? "Edytuj akcesorium"
                    : "Edit accessory";

            HideRightPanels();

            AddAccessoryFormPanel.Visibility = Visibility.Visible;
            AddAccessoryDetailsColumn.Visibility = Visibility.Collapsed;

            LoadAddAccessoryForm();

            FillAccessoryForm(editingAccessoryItem);

            currentAccessoryPhotos =
                editingAccessoryItem.Photos != null
                    ? new List<CollectionPhoto>(editingAccessoryItem.Photos)
                    : new List<CollectionPhoto>();

            ToggleAccessoryDetailsButton.Content =
                currentLanguage == "pl" ? "Więcej szczegółów" : "More details";

            addAccessoryDetailsVisible = false;
        }
        private void FillAccessoryForm(Accessory accessory)
        {
            AddAccessoryTypeCombo.Text =
                accessory.AccessoryType == "Pad" && currentLanguage != "pl"
                    ? "Controller"
                    : accessory.AccessoryType == "Controller" && currentLanguage == "pl"
                        ? "Pad"
                        : accessory.AccessoryType;

            AddAccessoryTypeCombo_SelectionChanged(AddAccessoryTypeCombo, null);

            AddAccessoryManufacturerTextBox.Text = accessory.Manufacturer;
            AddAccessoryModelTextBox.Text = accessory.Model;

            AddAccessoryHdrCheckBox.IsChecked = accessory.HdrSupport;

            AddAccessoryTvHdrCheckBox.IsChecked = accessory.HdrSupport;

            AddAccessoryAssignedManufacturerCombo.Text =
                string.IsNullOrWhiteSpace(accessory.AssignedManufacturer)
                    ? (currentLanguage == "pl" ? "Wybierz" : "Select")
                    : accessory.AssignedManufacturer;

            LoadAccessoryFamiliesForSelectedManufacturer();

            AddAccessoryAssignedFamilyCombo.Text =
                string.IsNullOrWhiteSpace(accessory.AssignedFamily)
                    ? (currentLanguage == "pl" ? "Wybierz producenta/grupę" : "Select manufacturer/group")
                    : accessory.AssignedFamily;

            LoadAccessoryPlatformsForSelectedFamily();

            AddAccessoryAssignedPlatformCombo.Text =
                string.IsNullOrWhiteSpace(accessory.AssignedPlatform)
                    ? (currentLanguage == "pl" ? "Wybierz rodzinę" : "Select family")
                    : accessory.AssignedPlatform;

            AddAccessoryConditionCombo.Text =
                 string.IsNullOrWhiteSpace(accessory.Condition)
                     ? (currentLanguage == "pl" ? "Wybierz" : "Select")
                     : ConditionDatabase.Translate(accessory.Condition, currentLanguage);

            AddAccessorySerialNumberTextBox.Text = accessory.SerialNumber;
            AddAccessoryNotesTextBox.Text = accessory.Notes;

            AddAccessoryControllerManufacturerCombo.Text =
                string.IsNullOrWhiteSpace(accessory.ControllerManufacturer)
                    ? (currentLanguage == "pl" ? "Wybierz" : "Select")
                    : accessory.ControllerManufacturer;

            LoadControllerModels();

            AddAccessoryControllerModelCombo.Text =
                string.IsNullOrWhiteSpace(accessory.ControllerModel)
                    ? (currentLanguage == "pl" ? "Wybierz producenta pada" : "Select controller manufacturer")
                    : accessory.ControllerModel;

            AddAccessoryDisplaySizeTextBox.Text = accessory.DisplaySize;

            AddAccessoryDisplayResolutionCombo.Text =
                string.IsNullOrWhiteSpace(accessory.DisplayResolution)
                    ? (currentLanguage == "pl" ? "Wybierz" : "Select")
                    : accessory.DisplayResolution;

            AddAccessoryDisplayPanelTypeCombo.Text =
                 string.IsNullOrWhiteSpace(accessory.DisplayPanelType)
                     ? (currentLanguage == "pl" ? "Wybierz" : "Select")
                     : accessory.DisplayPanelType;

            AddAccessoryDisplayBacklightTypeCombo.Text =
                string.IsNullOrWhiteSpace(accessory.DisplayBacklightType)
                    ? (currentLanguage == "pl" ? "Wybierz" : "Select")
                    : accessory.DisplayBacklightType;

            AddAccessoryRefreshRateCombo.Text =
                string.IsNullOrWhiteSpace(accessory.RefreshRate)
                    ? (currentLanguage == "pl" ? "Wybierz" : "Select")
                    : accessory.RefreshRate;

            AddAccessoryAspectRatioCombo.Text =
                string.IsNullOrWhiteSpace(accessory.AspectRatio)
                    ? (currentLanguage == "pl" ? "Wybierz" : "Select")
                    : accessory.AspectRatio;

            AddAccessoryResponseTimeTextBox.Text = accessory.ResponseTime;

            AddAccessorySyncTechnologyCombo.Text =
                string.IsNullOrWhiteSpace(accessory.SyncTechnology)
                    ? (currentLanguage == "pl" ? "Wybierz" : "Select")
                    : accessory.SyncTechnology;

            AddAccessorySmartTvCheckBox.IsChecked = accessory.SmartTv;

            AddAccessoryTvSystemCombo.Text =
                string.IsNullOrWhiteSpace(accessory.TvSystem)
                    ? (currentLanguage == "pl" ? "Wybierz" : "Select")
                    : accessory.TvSystem;

            AddAccessoryTvTunerCheckBox.IsChecked = accessory.TvTuner;

            AddAccessoryProjectionTechnologyCombo.Text =
                string.IsNullOrWhiteSpace(accessory.ProjectionTechnology)
                    ? (currentLanguage == "pl" ? "Wybierz" : "Select")
                    : accessory.ProjectionTechnology;

            AddAccessoryAnsiLumensTextBox.Text = accessory.AnsiLumens;
            AddAccessoryLampLifeTextBox.Text = accessory.LampLife;

            AddAccessoryPurchaseDatePicker.SelectedDate =
                accessory.PurchaseDate;

            AddAccessoryPurchasePriceTextBox.Text =
                accessory.PurchasePrice.HasValue
                    ? accessory.PurchasePrice.Value.ToString("0.00")
                    : "";

            AddAccessoryPurchaseCurrencyCombo.SelectedItem =
                string.IsNullOrWhiteSpace(accessory.PurchaseCurrency)
                    ? defaultCurrency
                    : accessory.PurchaseCurrency;
        }
        private void AddAccessoryToTree(Accessory accessory)
        {
            if (accessory == null)
                return;

            string type = accessory.AccessoryType;

            if (string.IsNullOrWhiteSpace(accessory.AssignedManufacturer) &&
                string.IsNullOrWhiteSpace(accessory.AssignedFamily) &&
                string.IsNullOrWhiteSpace(accessory.AssignedPlatform))
            {
                TreeViewItem accessoriesRoot = GetOrCreateTreeItem(
                    CollectionTree.Items,
                    currentLanguage == "pl" ? "Akcesoria" : "Accessories",
                    "Default");

                accessoriesRoot.Tag = new TreeNodeData
                {
                    NodeType = "AccessoriesRoot",
                    BackgroundGroup = "Default"
                };

                TreeViewItem typeNode = GetOrCreateTreeItem(
                    accessoriesRoot.Items,
                    type,
                    "Default");

                typeNode.Tag = new TreeNodeData
                {
                    NodeType = "AccessoryType",
                    Category = type,
                    BackgroundGroup = "Default"
                };

                typeNode.Items.Add(new TreeViewItem
                {
                    Header = GetAccessoryDisplayName(accessory),
                    Tag = new TreeNodeData
                    {
                        NodeType = "AccessoryRecord",
                        BackgroundGroup = "Default",
                        AccessoryItem = accessory
                    }
                });

                BuildGamesPanel();
                return;

            }

            TreeViewItem manufacturerNode = GetOrCreateTreeItem(
                CollectionTree.Items,
                accessory.AssignedManufacturer,
                GetBackgroundGroupForManufacturer(accessory.AssignedManufacturer));

            manufacturerNode.Tag = new TreeNodeData
            {
                NodeType = "Manufacturer",
                Manufacturer = accessory.AssignedManufacturer,
                Family = "",
                Platform = "",
                Category = "",
                BackgroundGroup = GetBackgroundGroupForManufacturer(accessory.AssignedManufacturer)
            };

            TreeViewItem accessoriesCategory = GetOrCreateTreeItem(
                manufacturerNode.Items,
                currentLanguage == "pl" ? "Akcesoria" : "Accessories",
                GetBackgroundGroupForManufacturer(accessory.AssignedManufacturer));

            accessoriesCategory.Tag = new TreeNodeData
            {
                NodeType = "AccessoryCategory",
                BackgroundGroup = GetBackgroundGroupForManufacturer(accessory.AssignedManufacturer),
                Manufacturer = accessory.AssignedManufacturer,
                Family = "",
                Platform = "",
                Category = currentLanguage == "pl" ? "Akcesoria" : "Accessories"
            };

            accessoriesCategory.Items.Add(new TreeViewItem
            {
                Header = GetAccessoryDisplayName(accessory),
                Tag = new TreeNodeData
                {
                    NodeType = "AccessoryRecord",
                    BackgroundGroup = GetBackgroundGroupForManufacturer(accessory.AssignedManufacturer),
                    AccessoryItem = accessory
                }
            });
        }
        private TreeViewItem CreateGameTreeItem(
        string title,
        int count,
        GameFilterMode filterMode,
        string genre = "",
        bool isMainGameNode = true)
        {
            Grid grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });

            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });

            TextBlock titleBlock = new TextBlock
            {
                Text = title,
                FontSize = isMainGameNode ? 14 : 12,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock countBlock = new TextBlock
            {
                Text = count.ToString(),
                FontSize = isMainGameNode ? 14 : 12,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Opacity = 0.75
            };

            Grid.SetColumn(titleBlock, 0);
            Grid.SetColumn(countBlock, 1);

            grid.Children.Add(titleBlock);
            grid.Children.Add(countBlock);

            TreeViewItem item = new()
            {
                Header = grid
            };


            titleBlock.SetBinding(TextBlock.ForegroundProperty,
                new Binding("Foreground") { Source = item });

            countBlock.SetBinding(TextBlock.ForegroundProperty,
                new Binding("Foreground") { Source = item });


            item.Selected += (s, e) =>
            {
                e.Handled = true;

                if (isClearingTreeSelection)
                    return;

                SetGameFilter(filterMode, genre);
            };

            return item;
        }
        private string GetAccessoryDisplayName(Accessory accessory)
        {
            if (!string.IsNullOrWhiteSpace(accessory.ControllerModel))
                return accessory.ControllerModel;

            if (!string.IsNullOrWhiteSpace(accessory.Manufacturer) &&
                !string.IsNullOrWhiteSpace(accessory.Model))
                return $"{accessory.Manufacturer} {accessory.Model}";

            if (!string.IsNullOrWhiteSpace(accessory.Model))
                return accessory.Model;

            return accessory.AccessoryType;
        }
        private void CardOpacityValue_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            string text = button.Content?.ToString()?.Replace("%", "") ?? "100";

            if (!int.TryParse(text, out int opacity))
                return;

            double opacityValue = opacity / 100.0;

            if (selectedHardwareItem != null)
            {
                selectedHardwareItem.CardOpacity = opacity;
            }
            else if (selectedGameItem != null)
            {
                selectedGameItem.CardOpacity = opacity;
            }
            else if (selectedAccessoryItem != null)
            {
                selectedAccessoryItem.CardOpacity = opacity;
                ApplyAccessoryCardOpacity(selectedAccessoryItem);
            }
            else
            {
                return;
            }

            HardwareDetailsCardBorder.Opacity = opacityValue;
            RecordActionsPanel.Opacity = opacityValue;

            string buttonText =
                $"{(currentLanguage == "pl" ? "Przezroczystość" : "Opacity")} {opacity}% ▼";

            CardOpacityButton.Content = buttonText;

            if (AccessoryCardOpacityButton != null)
                AccessoryCardOpacityButton.Content = buttonText;

            SaveCollectionChanges();
            CardOpacityPopup.IsOpen = false;
        }
        private void AccessoryCardOpacityButton_Click(object sender, RoutedEventArgs e)
        {
            AccessoryCardOpacityPopup.IsOpen =
                !AccessoryCardOpacityPopup.IsOpen;
        }

        private void AccessoryCardOpacityValue_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            string text = button.Content?.ToString()?.Replace("%", "") ?? "100";

            if (!int.TryParse(text, out int opacity))
                return;

            if (selectedAccessoryItem == null)
                return;

            selectedAccessoryItem.CardOpacity = opacity;

            ApplyAccessoryCardOpacity(selectedAccessoryItem);

            SaveCollectionChanges();

            AccessoryCardOpacityButton.Content =
                $"{(currentLanguage == "pl" ? "Przezroczystość" : "Opacity")} {opacity}% ▼";

            AccessoryCardOpacityPopup.Visibility = Visibility.Collapsed;

            AccessoryCardOpacityPopup.IsOpen = false;
        }
        private void PcCardOpacityValue_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            string text = button.Content?.ToString()?.Replace("%", "") ?? "100";

            if (!int.TryParse(text, out int opacity))
                return;

            if (selectedPcItem == null)
                return;

            selectedPcItem.CardOpacity = opacity;

            ApplyPcCardOpacity(selectedPcItem);

            SaveCollectionChanges();

            PcCardOpacityPopup.IsOpen = false;
        }
        private void PcCardOpacityButton_Click(object sender, RoutedEventArgs e)
        {
            PcCardOpacityPopup.IsOpen = !PcCardOpacityPopup.IsOpen;
        }
        private void ApplyPcCardOpacity(PcSystem pc)
        {
            double opacity = pc.CardOpacity / 100.0;

            PcPhotosBorder.Opacity = opacity;
            PcDetailsContentBorder.Opacity = opacity;
            PcActionsBorder.Opacity = opacity;
            PcComponentsBorder.Opacity = opacity;
        }
        private void ApplyCardOpacity(Hardware hardware)
        {
            double opacity = hardware.CardOpacity / 100.0;

            HardwareDetailsCardBorder.Opacity = opacity;
            RecordActionsPanel.Opacity = opacity;
        }
        private void ApplyGameCardOpacity(Game game)
        {
            double opacity = game.CardOpacity / 100.0;

            HardwareDetailsCardBorder.Opacity = opacity;
            RecordActionsPanel.Opacity = opacity;
        }
        private void CardOpacityButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            CardOpacityPopup.IsOpen = !CardOpacityPopup.IsOpen;
        }
        private void CancelHardwareFormButton_Click(object sender, RoutedEventArgs e)
        {
            bool cancel = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? (isEditingHardware
                        ? "Anulować edytowanie sprzętu?\n\nWprowadzone zmiany zostaną utracone."
                        : "Anulować dodawanie sprzętu?\n\nWprowadzone dane zostaną utracone.")
                    : (isEditingHardware
                        ? "Cancel editing hardware?\n\nEntered changes will be lost."
                        : "Cancel adding hardware?\n\nEntered data will be lost."),
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!cancel)
                return;

            CloseAddHardwareForm();
        }

        private void CloseAddHardwareForm()
        {
            AddHardwareFormPanel.Visibility = Visibility.Collapsed;
            HardwareRightDetailsColumn.Visibility = Visibility.Collapsed;
            HardwareDetailsViewPanel.Visibility = Visibility.Collapsed;
            RecordActionsPanel.Visibility = Visibility.Collapsed;

            PreviewPlaceholder.Visibility = Visibility.Visible;

            HardwareCustomNameTextBox.Clear();

            selectedHardwareItem = null;
            selectedGameItem = null;
            selectedPcItem = null;
        }
        private void AddPcButton_Click(object sender, RoutedEventArgs e)
        {
            AddChoicePanel.Visibility = Visibility.Collapsed;
            AddHardwareSubChoicePanel.Visibility = Visibility.Collapsed;
            isAddChoicePanelOpen = false;

            ShowAddPcForm();
        }
        private void ShowAddPcForm()
        {
            currentPlatformGroup = "Default";
            UpdateBackgroundForCurrentState(); 
            
            HideRightPanels();

            selectedHardwareItem = null;
            selectedGameItem = null;
            selectedPcItem = null;

            isEditingPc = false;
            editingPcItem = null;
            editingPcTreeItem = null;
            currentPcDraft = new PcSystem();

            AddPcFormPanel.Visibility = Visibility.Visible;
            PcRightDetailsColumn.Visibility = Visibility.Collapsed;

            ApplyPcFormLanguage();
            LoadPcFormDefaults();
            ClearPcForm();
        }
        private void ClearPcForm()
        {
            PcNameTextBox.Text = "";

            PcAddModelSerialCheckBox.IsChecked = false;
            PcModelSerialPanel.Visibility = Visibility.Collapsed;

            PcModelTextBox.Text = "";
            PcSerialTextBox.Text = "";

            PcOperatingSystemEditionLabel.Visibility = Visibility.Collapsed;
            PcOperatingSystemEditionCombo.Visibility = Visibility.Collapsed;
            PcOperatingSystemRevisionLabel.Visibility = Visibility.Collapsed;
            PcOperatingSystemRevisionTextBox.Visibility = Visibility.Collapsed;

            PcOperatingSystemRevisionTextBox.Text = "";

            PcPurchaseDatePicker.SelectedDate = null;
            PcPurchasePriceTextBox.Text = "";
            PcPurchaseCurrencyCombo.SelectedItem = defaultCurrency;

            PcRightDetailsColumn.Visibility = Visibility.Collapsed;
        }
        private void CancelPcFormButton_Click(object sender, RoutedEventArgs e)
        {
            bool cancel = HgcMessageBox.ShowYesNo(
                this,
                isEditingPc
                    ? (currentLanguage == "pl"
                        ? "Anulować edytowanie komputera?\n\nWprowadzone zmiany zostaną utracone."
                        : "Cancel editing computer?\n\nChanges will be lost.")
                    : (currentLanguage == "pl"
                        ? "Anulować dodawanie komputera?\n\nWprowadzone dane zostaną utracone."
                        : "Cancel adding computer?\n\nEntered data will be lost."),
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!cancel)
                return;

            CloseAddPcForm();
        }

        private void CloseAddPcForm()
        {
            AddPcFormPanel.Visibility = Visibility.Collapsed;
            PcRightDetailsColumn.Visibility = Visibility.Collapsed;

            isEditingPc = false;
            editingPcItem = null;
            editingPcTreeItem = null;
            currentPcDraft = null;

            ClearPcForm();

            selectedPcItem = null;
            selectedHardwareItem = null;
            selectedGameItem = null;

            ShowDefaultRightPanel();
        }
        private void ApplyPcFormLanguage()
        {
            AddPcFormTitle.Text =
                currentLanguage == "pl" ? "Dodaj komputer" : "Add PC";

            PcNameLabel.Text =
                currentLanguage == "pl" ? "Nazwa komputera" : "PC name";

            PcBasicSectionTitle.Text =
                currentLanguage == "pl" ? "Podstawowe informacje" : "Basic information";

            PcTypeLabel.Text =
                currentLanguage == "pl" ? "Typ komputera" : "Computer type";

            PcPortableTypeLabel.Text =
                currentLanguage == "pl" ? "Typ komputera przenośnego" : "Portable computer type";

            PcManufacturerLabel.Text =
                currentLanguage == "pl" ? "Producent" : "Manufacturer";

            PcAddModelSerialCheckBox.Content =
                currentLanguage == "pl" ? "Dodaj model i numer seryjny" : "Add model and serial number";

            PcModelLabel.Text =
                currentLanguage == "pl" ? "Model" : "Model";

            PcSerialLabel.Text =
                currentLanguage == "pl" ? "Numer seryjny" : "Serial number";

            PcSystemSectionTitle.Text =
                currentLanguage == "pl" ? "System operacyjny" : "Operating system";

            PcOperatingSystemLabel.Text =
                currentLanguage == "pl" ? "System operacyjny" : "Operating system";

            PcOperatingSystemEditionLabel.Text =
                currentLanguage == "pl" ? "Edycja systemu" : "System edition";

            PcOperatingSystemRevisionLabel.Text =
                currentLanguage == "pl" ? "Rewizja / build" : "Revision / build";

            PcPurchaseSectionTitle.Text =
                currentLanguage == "pl" ? "Zakup" : "Purchase";

            PcPurchaseDateLabel.Text =
                currentLanguage == "pl" ? "Data zakupu" : "Purchase date";

            PcPurchasePriceLabel.Text =
                currentLanguage == "pl" ? "Cena zakupu" : "Purchase price";

            CancelPcButton.Content =
                currentLanguage == "pl" ? "Anuluj" : "Cancel";

            TogglePcDetailsButton.Content =
                currentLanguage == "pl" ? "Dodaj szczegóły" : "Add details";

            PcPhotoGalleryButton.Content =
                currentLanguage == "pl"
                    ? "Dodaj zdjęcia"
                    : "Add photos";

            SavePcButton.Content =
                currentLanguage == "pl" ? "Zapisz" : "Save";

            PcDetailsSectionTitle.Text =
                currentLanguage == "pl" ? "Szczegóły komputera" : "PC details";

            PcDetailsPlaceholderText.Text =
                currentLanguage == "pl"
                    ? "Szczegółowe podzespoły dodamy w następnym kroku."
                    : "Detailed components will be added in the next step.";
        }
        private void LoadPcFormDefaults()
        {
            LoadPcTypes();
            LoadPcPortableTypes();
            LoadPcManufacturers();
            LoadPcOperatingSystems();
            LoadPcOperatingSystemEditions();

            UpdatePcTypeFields();
            UpdatePcManufacturerFields();
            UpdatePcOperatingSystemFields();
        }
        private void UpdateGamesCollectionTitle()
        {
            CollectionTilesTitle.Text =
                string.IsNullOrWhiteSpace(currentGamesTitle)
                    ? (currentLanguage == "pl" ? "Gry" : "Games")
                    : currentGamesTitle;
        }
        private void LoadPcTypes()
        {
            PcTypeCombo.Items.Clear();

            PcTypeCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");
            PcTypeCombo.Items.Add(currentLanguage == "pl" ? "Komputer stacjonarny" : "Desktop");
            PcTypeCombo.Items.Add(currentLanguage == "pl" ? "Komputer przenośny" : "Portable computer");
            PcTypeCombo.Items.Add("All-in-One");
            PcTypeCombo.Items.Add("Mini PC");
            PcTypeCombo.Items.Add("HTPC");
            PcTypeCombo.Items.Add("Workstation");
            PcTypeCombo.Items.Add(currentLanguage == "pl" ? "Serwer" : "Server");
            PcTypeCombo.Items.Add("Thin Client");
            PcTypeCombo.Items.Add("Handheld PC");
            PcTypeCombo.Items.Add("Retro PC");
            PcTypeCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");

            PcTypeCombo.SelectedIndex = 0;
        }

        private void LoadPcManufacturers()
        {
            PcManufacturerCombo.Items.Clear();

            PcManufacturerCombo.Items.Add("Custom Build");
            PcManufacturerCombo.Items.Add("Dell");
            PcManufacturerCombo.Items.Add("HP");
            PcManufacturerCombo.Items.Add("Lenovo");
            PcManufacturerCombo.Items.Add("Acer");
            PcManufacturerCombo.Items.Add("ASUS");
            PcManufacturerCombo.Items.Add("MSI");
            PcManufacturerCombo.Items.Add("Apple");
            PcManufacturerCombo.Items.Add("IBM");
            PcManufacturerCombo.Items.Add("Compaq");
            PcManufacturerCombo.Items.Add("Packard Bell");
            PcManufacturerCombo.Items.Add("Fujitsu");
            PcManufacturerCombo.Items.Add("Toshiba");
            PcManufacturerCombo.Items.Add("Alienware");
            PcManufacturerCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");

            PcManufacturerCombo.SelectedIndex = 0;
        }

        private void LoadPcOperatingSystems()
        {
            PcOperatingSystemCombo.Items.Clear();

            PcOperatingSystemCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");

            PcOperatingSystemCombo.Items.Add("MS-DOS");
            PcOperatingSystemCombo.Items.Add("PC DOS");
            PcOperatingSystemCombo.Items.Add("FreeDOS");

            PcOperatingSystemCombo.Items.Add("Windows 3.1");
            PcOperatingSystemCombo.Items.Add("Windows 3.11");
            PcOperatingSystemCombo.Items.Add("Windows 95");
            PcOperatingSystemCombo.Items.Add("Windows 98");
            PcOperatingSystemCombo.Items.Add("Windows 98 SE");
            PcOperatingSystemCombo.Items.Add("Windows ME");
            PcOperatingSystemCombo.Items.Add("Windows NT 4.0");
            PcOperatingSystemCombo.Items.Add("Windows 2000");
            PcOperatingSystemCombo.Items.Add("Windows XP");
            PcOperatingSystemCombo.Items.Add("Windows Vista");
            PcOperatingSystemCombo.Items.Add("Windows 7");
            PcOperatingSystemCombo.Items.Add("Windows 8");
            PcOperatingSystemCombo.Items.Add("Windows 8.1");
            PcOperatingSystemCombo.Items.Add("Windows 10");
            PcOperatingSystemCombo.Items.Add("Windows 11");

            PcOperatingSystemCombo.Items.Add("Linux");
            PcOperatingSystemCombo.Items.Add("macOS");
            PcOperatingSystemCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");

            PcOperatingSystemCombo.SelectedIndex = 0;
        }

        private void LoadPcOperatingSystemEditions()
        {
            PcOperatingSystemEditionCombo.Items.Clear();

            PcOperatingSystemEditionCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");
            PcOperatingSystemEditionCombo.Items.Add("Home");
            PcOperatingSystemEditionCombo.Items.Add("Pro");
            PcOperatingSystemEditionCombo.Items.Add("Professional");
            PcOperatingSystemEditionCombo.Items.Add("Ultimate");
            PcOperatingSystemEditionCombo.Items.Add("Enterprise");
            PcOperatingSystemEditionCombo.Items.Add("Education");
            PcOperatingSystemEditionCombo.Items.Add("Business");
            PcOperatingSystemEditionCombo.Items.Add("Starter");
            PcOperatingSystemEditionCombo.Items.Add("Media Center Edition");
            PcOperatingSystemEditionCombo.Items.Add("Embedded");
            PcOperatingSystemEditionCombo.Items.Add(currentLanguage == "pl" ? "Inna" : "Other");

            PcOperatingSystemEditionCombo.SelectedIndex = 0;
        }
        private void AddPcComponentPreviewSection(
        string title,
        IEnumerable<string> items)
        {
            List<string> values = items
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            if (values.Count == 0)
                return;

            Border sectionBorder = new Border
            {
                Padding = new Thickness(14),
                CornerRadius = new CornerRadius(10),
                Background = (Brush)FindResource("PanelBackgroundBrush"),
                BorderBrush = (Brush)FindResource("AppBorderBrush"),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 12)
            };

            StackPanel sectionPanel = new StackPanel();

            sectionPanel.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            foreach (string value in values)
            {
                sectionPanel.Children.Add(new TextBlock
                {
                    Text = value,
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 6),
                    TextWrapping = TextWrapping.Wrap
                });
            }

            sectionBorder.Child = sectionPanel;
            PcComponentsPanel.Children.Add(sectionBorder);
        }
        private void AddPcComponentExpandableSection<T>(
        string title,
        IEnumerable<T> items,
        Func<T, string> summarySelector,
        Func<T, string> detailsSelector)
        {
            List<T> values = items.ToList();

            if (values.Count == 0)
                return;

            Border sectionBorder = new Border
            {
                Padding = new Thickness(14),
                CornerRadius = new CornerRadius(10),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 12)
            };

            sectionBorder.SetResourceReference(
                Border.BackgroundProperty,
                "PanelBackgroundBrush");

            sectionBorder.SetResourceReference(
                Border.BorderBrushProperty,
                "AppBorderBrush");

            StackPanel sectionPanel = new StackPanel();

            sectionPanel.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Opacity = 0.85,
                Margin = new Thickness(0, 0, 0, 10)
            });

            foreach (T item in values)
            {
                bool isExpanded = expandedPcDetails.Contains(item!);

                StackPanel itemPanel = new StackPanel
                {
                    Margin = new Thickness(0, 0, 0, 10)
                };

                itemPanel.Children.Add(new TextBlock
                {
                    Text = summarySelector(item),
                    FontSize = 12,
                    Opacity = 0.85,
                    FontWeight = FontWeights.Bold,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 6)
                });

                string detailsText = detailsSelector(item);

                if (isExpanded && !string.IsNullOrWhiteSpace(detailsText))
                {
                    itemPanel.Children.Add(new TextBlock
                    {
                        Text = detailsText,
                        FontSize = 12,
                        Opacity = 0.80,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0, 0, 8)
                    });
                }

                Button toggleButton = new Button
                {
                    Content = isExpanded
                        ? (currentLanguage == "pl" ? "Ukryj szczegóły" : "Hide details")
                        : (currentLanguage == "pl" ? "Pokaż szczegóły" : "Show details"),
                    Tag = item,
                    Height = 30,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    MinWidth = 130
                };

                toggleButton.Click += TogglePcComponentDetails_Click;

                if (!string.IsNullOrWhiteSpace(detailsText))
                    itemPanel.Children.Add(toggleButton);

                sectionPanel.Children.Add(itemPanel);
            }

            sectionBorder.Child = sectionPanel;
            PcComponentsPanel.Children.Add(sectionBorder);
        }
        private void TogglePcComponentDetails_Click(
    object sender,
    RoutedEventArgs e)
        {
            if (sender is not Button button ||
                button.Tag == null ||
                selectedPcItem == null)
                return;

            object item = button.Tag;

            if (expandedPcDetails.Contains(item))
                expandedPcDetails.Remove(item);
            else
                expandedPcDetails.Add(item);

            ShowPcDetails(selectedPcItem);
        }
        private void UpdatePcTypeFields()
        {
            string selectedType =
                PcTypeCombo.SelectedItem?.ToString() ?? "";

            bool isPortable =
                selectedType == "Komputer przenośny" ||
                selectedType == "Portable computer";

            PcPortableTypeLabel.Visibility =
                isPortable ? Visibility.Visible : Visibility.Collapsed;

            PcPortableTypeCombo.Visibility =
                isPortable ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdatePcManufacturerFields()
        {
            string manufacturer =
                PcManufacturerCombo.SelectedItem?.ToString() ?? "";

            bool isCustomBuild =
                manufacturer == "Custom Build";

            PcAddModelSerialCheckBox.Visibility =
                isCustomBuild
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (isCustomBuild)
            {
                PcModelSerialPanel.Visibility =
                    PcAddModelSerialCheckBox.IsChecked == true
                        ? Visibility.Visible
                        : Visibility.Collapsed;
            }
            else
            {
                PcModelSerialPanel.Visibility = Visibility.Visible;
            }
        }

        private void UpdatePcOperatingSystemFields()
        {
            string os =
                PcOperatingSystemCombo.SelectedItem?.ToString() ?? "";

            bool isWindows =
                os.StartsWith("Windows");

            PcOperatingSystemEditionLabel.Visibility =
                isWindows ? Visibility.Visible : Visibility.Collapsed;

            PcOperatingSystemEditionCombo.Visibility =
                isWindows ? Visibility.Visible : Visibility.Collapsed;

            PcOperatingSystemRevisionLabel.Visibility =
                isWindows ? Visibility.Visible : Visibility.Collapsed;

            PcOperatingSystemRevisionTextBox.Visibility =
                isWindows ? Visibility.Visible : Visibility.Collapsed;
        }
        private void LoadPcPortableTypes()
        {
            PcPortableTypeCombo.Items.Clear();

            PcPortableTypeCombo.Items.Add(
                currentLanguage == "pl"
                    ? "Wybierz"
                    : "Select");

            PcPortableTypeCombo.Items.Add("Notebook");
            PcPortableTypeCombo.Items.Add("Gaming Laptop");
            PcPortableTypeCombo.Items.Add("Ultrabook");
            PcPortableTypeCombo.Items.Add("Netbook");
            PcPortableTypeCombo.Items.Add("Subnotebook");

            PcPortableTypeCombo.Items.Add(
                currentLanguage == "pl"
                    ? "Mobilna stacja robocza"
                    : "Mobile Workstation");

            PcPortableTypeCombo.Items.Add("2-in-1");

            PcPortableTypeCombo.Items.Add(
                currentLanguage == "pl"
                    ? "Inny"
                    : "Other");

            PcPortableTypeCombo.SelectedIndex = 0;
        }
        private void CancelPcButton_Click(object sender, RoutedEventArgs e)
        {
            CancelPcFormButton_Click(sender, e);
        }

       private void TogglePcDetailsButton_Click(object sender, RoutedEventArgs e)
{
    bool showDetails = PcRightDetailsColumn.Visibility != Visibility.Visible;

    PcRightDetailsColumn.Visibility =
        showDetails
            ? Visibility.Visible
            : Visibility.Collapsed;

    TogglePcDetailsButton.Content =
        currentLanguage == "pl"
            ? (showDetails ? "Ukryj szczegóły" : "Dodaj szczegóły")
            : (showDetails ? "Hide details" : "Add details");

    if (showDetails)
    {
        PcSystem? targetPc =
            isEditingPc
                ? selectedPcItem
                : currentPcDraft;

        if (targetPc != null)
            BuildPcEditComponentsPanel(targetPc);
    }
}
        private void RefreshPcComponentsPanel(PcSystem pc)
        {
            if (isEditingPc)
                ShowPcDetailsForEdit(pc);
            else
                BuildPcEditComponentsPanel(pc);
        }

        private void SavePcButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PcNameTextBox.Text))
            {
                HgcMessageBox.Show(
                 this,
                 currentLanguage == "pl"
                     ? "Podaj nazwę komputera."
                     : "Enter PC name.",
                 "HGC",
                 HgcMessageBoxType.Information,
                 currentLanguage);

                return;
            }

            if (!ValidateNameLength(PcNameTextBox.Text))
                return;

            PcSystem pc = isEditingPc && editingPcItem != null
            ? editingPcItem
            : currentPcDraft ?? new PcSystem();

            pc.Photos = currentPcPhotos ?? new List<CollectionPhoto>();

            bool wasEditingPc = isEditingPc;

            pc.Name = PcNameTextBox.Text.Trim();

            pc.ComputerType =
                IsSelectedValue(PcTypeCombo.Text)
                    ? PcTypeCombo.Text
                    : "";

            pc.PortableType =
                PcPortableTypeCombo.Visibility == Visibility.Visible &&
                IsSelectedValue(PcPortableTypeCombo.Text)
                    ? PcPortableTypeCombo.Text
                    : "";

            pc.Manufacturer =
                IsSelectedValue(PcManufacturerCombo.Text)
                    ? PcManufacturerCombo.Text
                    : "";

            pc.AddModelAndSerialForCustomBuild =
                PcAddModelSerialCheckBox.IsChecked == true;

            pc.Model =
                PcModelSerialPanel.Visibility == Visibility.Visible
                    ? PcModelTextBox.Text.Trim()
                    : "";

            pc.SerialNumber =
                PcModelSerialPanel.Visibility == Visibility.Visible
                    ? PcSerialTextBox.Text.Trim()
                    : "";

            pc.OperatingSystem =
                IsSelectedValue(PcOperatingSystemCombo.Text)
                    ? PcOperatingSystemCombo.Text
                    : "";

            pc.OperatingSystemEdition =
                PcOperatingSystemEditionCombo.Visibility == Visibility.Visible &&
                IsSelectedValue(PcOperatingSystemEditionCombo.Text)
                    ? PcOperatingSystemEditionCombo.Text
                    : "";

            pc.OperatingSystemRevision =
                PcOperatingSystemRevisionTextBox.Visibility == Visibility.Visible
                    ? PcOperatingSystemRevisionTextBox.Text.Trim()
                    : "";

            pc.PurchaseDate = PcPurchaseDatePicker.SelectedDate;

            if (decimal.TryParse(PcPurchasePriceTextBox.Text, out decimal purchasePrice))
                pc.PurchasePrice = purchasePrice;
            else
                pc.PurchasePrice = null;

            pc.PurchaseCurrency =
                PcPurchaseCurrencyCombo.SelectedItem?.ToString()
                ?? defaultCurrency;

            if (isEditingPc)
            {
                if (editingPcTreeItem != null)
                    editingPcTreeItem.Header = pc.Name;

                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? $"{pc.Name} — zapisano zmiany."
                        : $"{pc.Name} — changes saved.",
                    "HGC",
                    HgcMessageBoxType.Information,
                    currentLanguage);
            }
            else
            {
                collection.PcSystems.Add(pc);
                currentPcDraft = null;
                AddPcRecordToTree(pc);

                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? $"{pc.Name} został dodany."
                        : $"{pc.Name} has been added.",
                    "HGC",
                    HgcMessageBoxType.Information,
                    currentLanguage);
            }

            isEditingPc = false;
            editingPcItem = null;
            editingPcTreeItem = null;

            currentPcPhotos = new List<CollectionPhoto>();
            currentPcDraft = null;

            PcPhotoGalleryButton.Content =
                currentLanguage == "pl"
                    ? "Dodaj zdjęcia"
                    : "Add photos";

            AddPcFormPanel.Visibility = Visibility.Collapsed;

            ClearPcForm();

            if (wasEditingPc)
                ShowPcDetails(pc);
            else
                ShowDefaultRightPanel();

            RefreshStats();
            SaveCollectionChanges();
        }

        private void AddPcRecordToTree(PcSystem pc)
        {
            if (pc == null)
                return;

            TreeViewItem pcRoot = GetOrCreateTreeItem(
                CollectionTree.Items,
                "PC",
                "Default");

            if (pcRoot.Tag is not TreeNodeData existingNode ||
            existingNode.NodeType != "PcRoot")
            {
                pcRoot.Tag = new TreeNodeData
                {
                    NodeType = "PcRoot",
                    BackgroundGroup = "Default"
                };
            }

            TreeViewItem computersNode = GetOrCreateTreeItem(
                pcRoot.Items,
                currentLanguage == "pl" ? "Komputery" : "Computers",
                "Default");

            computersNode.Tag = new TreeNodeData
            {
                NodeType = "PcCategory",
                Category = currentLanguage == "pl" ? "Komputery" : "Computers",
                BackgroundGroup = "Default"
            };

            string pcType =
                string.IsNullOrWhiteSpace(pc.ComputerType)
                    ? (currentLanguage == "pl" ? "Nieokreślony typ" : "Unspecified type")
                    : pc.ComputerType;

            TreeViewItem typeNode = GetOrCreateTreeItem(
                computersNode.Items,
                pcType,
                "Default");

            typeNode.Tag = new TreeNodeData
            {
                NodeType = "PcType",
                Category = pcType,
                BackgroundGroup = "Default"
            };

            TreeViewItem pcItem = new TreeViewItem
            {
                Header = pc.Name,
                Tag = new TreeNodeData
                {
                    NodeType = "PcRecord",
                    BackgroundGroup = "Default",
                    PcItem = pc
                }
            };

            typeNode.Items.Add(pcItem);
            BuildGamesPanel();
        }
        private void PcTypeCombo_SelectionChanged(
    object sender,
    SelectionChangedEventArgs e)
        {
            UpdatePcTypeFields();
        }

        private void PcManufacturerCombo_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            UpdatePcManufacturerFields();
        }
        private void PcAddModelSerialCheckBox_Checked(
        object sender,
        RoutedEventArgs e)
        {
            PcModelSerialPanel.Visibility =
                PcAddModelSerialCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
        private void PcOperatingSystemCombo_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            UpdatePcOperatingSystemFields();
        }
        private void ShowAddHardwareForm()
        {
            ClearSelectedRecordForNonRecordView();

            currentPlatformGroup = "Default";
            UpdateBackgroundForCurrentState();

            PreviewPlaceholder.Visibility = Visibility.Collapsed;
            AddHardwareFormPanel.Visibility = Visibility.Visible;

            HardwareRightDetailsColumn.Visibility = Visibility.Collapsed;

            ApplyAddHardwareFormLanguage();
            LoadHardwareFormDefaults();

            HardwareCustomNameTextBox.Clear();

            HardwareDetailsViewPanel.Visibility = Visibility.Collapsed;
            RecordActionsPanel.Visibility = Visibility.Collapsed;
        }
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged)
                return;

            if (CollectionTilesPanel.Visibility != Visibility.Visible)
                return;

            if (lastCollectionTilesNode == null)
                return;

            ShowCollectionTiles(
                lastCollectionTilesNode,
                lastCollectionTilesShowRecentlyAdded);
        }
        private void ToggleHardwareDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            bool show = HardwareRightDetailsColumn.Visibility != Visibility.Visible;

            HardwareRightDetailsColumn.Visibility = show
                ? Visibility.Visible
                : Visibility.Collapsed;

            ToggleHardwareDetailsButton.Content = show
                ? (currentLanguage == "pl" ? "Ukryj szczegóły" : "Hide details")
                : (currentLanguage == "pl" ? "Dodaj szczegóły" : "Add details");
        }

        private void ApplyAddHardwareFormLanguage()
        {
            AddConsoleText.Text =
                currentLanguage == "pl"
                    ? "Dodaj konsolę"
                    : "Add console";
            AddAccessoryText.Text =
                currentLanguage == "pl"
                    ? "Dodaj akcesoria"
                    : "Add accessories";
            AddPcText.Text =
                currentLanguage == "pl"
                    ? "Dodaj komputer"
                    : "Add PC";
            HardwareBasicSectionTitle.Text = currentLanguage == "pl" ? "Podstawowe informacje" : "Basic information";
            HardwareManufacturerLabel.Text = currentLanguage == "pl" ? "Producent" : "Manufacturer";
            HardwareTypeLabel.Text = currentLanguage == "pl" ? "Typ" : "Type";
            HardwarePlatformLabel.Text = currentLanguage == "pl" ? "Platforma" : "Platform";
            HardwareCustomNameLabel.Text =
            currentLanguage == "pl"
            ? "Nazwa własna sprzętu"
            : "Custom hardware name";
            SaveHardwareButton.Content = currentLanguage == "pl" ? "Zapisz" : "Save";
            ToggleHardwareDetailsButton.Content = currentLanguage == "pl" ? "Dodaj szczegóły" : "Add details";
            HardwareBoxSectionTitle.Text = currentLanguage == "pl" ? "Zawartość zestawu" : "Set contents";
            HardwareHasBoxCheckBox.Content = currentLanguage == "pl" ? "Oryginalne pudełko" : "Original box";
            HardwareHasManualCheckBox.Content = currentLanguage == "pl" ? "Instrukcja" : "Manual";
            HardwareHasInsertsCheckBox.Content = currentLanguage == "pl" ? "Wkładki / ulotki" : "Inserts / leaflets";
            HardwareHasPowerSupplyCheckBox.Content = currentLanguage == "pl" ? "Zasilacz" : "Power supply";
            HardwareHasCableCheckBox.Content = currentLanguage == "pl" ? "Kabel AV / HDMI / RF" : "AV / HDMI / RF cable";
            HardwareHasControllerCheckBox.Content =
            currentLanguage == "pl"
            ? "Pad / kontroler z zestawu"
            : "Bundled pad / controller";
            HardwareControllerHintText.Text =
            currentLanguage == "pl"
            ? "(aby dodać lub edytować, sprawdź akcesoria)"
            : "(to add or edit it, check accessories)";
            HardwareHasMemoryCardCheckBox.Content = currentLanguage == "pl" ? "Karta pamięci" : "Memory card";
            HardwarePurchaseSectionTitle.Text = currentLanguage == "pl" ? "Zakup" : "Purchase";
            HardwarePurchaseDateLabel.Text = currentLanguage == "pl" ? "Data zakupu" : "Purchase date";
            HardwarePurchasePriceLabel.Text = currentLanguage == "pl" ? "Cena zakupu" : "Purchase price";
            HardwareNotesSectionTitle.Text = currentLanguage == "pl" ? "Notatki" : "Notes";
            HardwareDetailsSectionTitle.Text = currentLanguage == "pl" ? "Szczegóły" : "Details";
            HardwareModelLabel.Text = currentLanguage == "pl" ? "Model / wariant" : "Model / variant";
            HardwareRevisionLabel.Text = currentLanguage == "pl" ? "Rewizja" : "Revision";
            HardwareBoardRevisionLabel.Text = currentLanguage == "pl" ? "Rewizja płyty" : "Board revision";
            HardwareRegionLabel.Text = currentLanguage == "pl" ? "Region" : "Region";
            HardwareColorLabel.Text = currentLanguage == "pl" ? "Kolor" : "Color";
            HardwareEditionLabel.Text = currentLanguage == "pl" ? "Edycja specjalna" : "Special edition";
            HardwareSerialLabel.Text = currentLanguage == "pl" ? "Numer seryjny" : "Serial number";
            HardwareConditionSectionTitle.Text = currentLanguage == "pl" ? "Stan" : "Condition";
            AddHardwarePhotoButton.Content = currentLanguage == "pl" ? "Dodaj zdjęcie" : "Add photo";
        }
        private void ApplyAddAccessoryFormLanguage()
        {
            AddAccessoryFormTitle.Text =
                currentLanguage == "pl" ? "Dodaj akcesorium" : "Add accessory";

            AccessoryMainSectionTitle.Text =
                currentLanguage == "pl" ? "Akcesorium" : "Accessory";

            AccessoryTypeLabel.Text =
                currentLanguage == "pl" ? "Typ akcesorium" : "Accessory type";

            AccessoryManufacturerLabel.Text =
                currentLanguage == "pl" ? "Producent" : "Manufacturer";

            AccessoryModelLabel.Text =
                currentLanguage == "pl" ? "Model" : "Model";

            AccessoryAssignSectionTitle.Text =
                currentLanguage == "pl" ? "Przypisanie" : "Assignment";

            AccessoryAssignedManufacturerLabel.Text =
                currentLanguage == "pl" ? "Producent / grupa" : "Manufacturer / group";

            AccessoryAssignedFamilyLabel.Text =
                currentLanguage == "pl" ? "Rodzina" : "Family";

            AccessoryAssignedPlatformLabel.Text =
                currentLanguage == "pl" ? "Platforma" : "Platform";

            CancelAccessoryButton.Content =
                currentLanguage == "pl" ? "Anuluj" : "Cancel";

            AccessoryPhotosButton.Content =
                currentLanguage == "pl" ? "Dodaj zdjęcia" : "Add photos";

            ToggleAccessoryDetailsButton.Content =
                currentLanguage == "pl" ? "Dodaj szczegóły" : "Add details";

            SaveAccessoryButton.Content =
                currentLanguage == "pl" ? "Zapisz" : "Save";

            AccessoryControllerSectionTitle.Text =
                currentLanguage == "pl" ? "Pad" : "Controller";

            AccessoryControllerManufacturerLabel.Text =
                currentLanguage == "pl" ? "Producent pada" : "Controller manufacturer";

            AccessoryControllerModelLabel.Text =
                currentLanguage == "pl" ? "Model pada" : "Controller model";

            AddAccessoryCustomControllerModelLabel.Text =
                currentLanguage == "pl" ? "Własna nazwa modelu" : "Custom model name";

            AccessoryMonitorSectionTitle.Text =
                currentLanguage == "pl" ? "Monitor" : "Monitor";

            AccessoryMonitorSizeLabel.Text =
                currentLanguage == "pl" ? "Przekątna (cale)" : "Screen size (inches)";

            AccessoryMonitorResolutionLabel.Text =
                currentLanguage == "pl" ? "Rozdzielczość" : "Resolution";

            AccessoryMonitorPanelTypeLabel.Text =
                currentLanguage == "pl" ? "Typ matrycy" : "Panel type";

            AccessoryMonitorBacklightLabel.Text =
                currentLanguage == "pl" ? "Podświetlenie" : "Backlight";

            AccessoryMonitorRefreshRateLabel.Text =
                currentLanguage == "pl" ? "Odświeżanie" : "Refresh rate";

            AccessoryMonitorAspectRatioLabel.Text =
                currentLanguage == "pl" ? "Format obrazu" : "Aspect ratio";

            AccessoryMonitorResponseTimeLabel.Text =
                currentLanguage == "pl" ? "Czas reakcji" : "Response time";

            AccessoryMonitorSyncLabel.Text =
                currentLanguage == "pl" ? "Synchronizacja" : "Sync technology";

            AccessoryTvSectionTitle.Text =
                currentLanguage == "pl" ? "Telewizor" : "Television";

            AccessoryTvSizeLabel.Text =
                currentLanguage == "pl" ? "Przekątna (cale)" : "Screen size (inches)";

            AccessoryTvResolutionLabel.Text =
                currentLanguage == "pl" ? "Rozdzielczość" : "Resolution";

            AccessoryTvRefreshRateLabel.Text =
                currentLanguage == "pl" ? "Odświeżanie" : "Refresh rate";

            AccessoryTvPanelTypeLabel.Text =
                currentLanguage == "pl" ? "Typ matrycy" : "Panel type";

            AccessoryTvBacklightLabel.Text =
                currentLanguage == "pl" ? "Podświetlenie" : "Backlight";

            AccessoryTvSystemLabel.Text =
                currentLanguage == "pl" ? "System" : "System";

            AddAccessoryTvTunerCheckBox.Content =
                currentLanguage == "pl" ? "Tuner TV" : "TV tuner";

            AccessoryProjectorSectionTitle.Text =
                currentLanguage == "pl" ? "Projektor" : "Projector";

            AccessoryProjectorResolutionLabel.Text =
                currentLanguage == "pl" ? "Rozdzielczość" : "Resolution";

            AccessoryProjectorTechnologyLabel.Text =
                currentLanguage == "pl" ? "Technologia projekcji" : "Projection technology";

            AccessoryProjectorBrightnessLabel.Text =
                currentLanguage == "pl" ? "Jasność ANSI" : "ANSI brightness";

            AccessoryProjectorLampLifeLabel.Text =
                currentLanguage == "pl" ? "Żywotność lampy" : "Lamp life";

            AccessoryFormDetailsSectionTitle.Text =
                currentLanguage == "pl" ? "Szczegóły" : "Details";

            AccessoryFormConditionLabel.Text =
                currentLanguage == "pl" ? "Stan" : "Condition";

            AccessoryFormSerialNumberLabel.Text =
                currentLanguage == "pl" ? "Numer seryjny" : "Serial number";

            AccessoryFormPurchaseSectionTitle.Text =
                currentLanguage == "pl" ? "Zakup" : "Purchase";

            AccessoryFormPurchaseDateLabel.Text =
                currentLanguage == "pl" ? "Data zakupu" : "Purchase date";

            AccessoryFormPurchasePriceLabel.Text =
                currentLanguage == "pl" ? "Cena zakupu" : "Purchase price";

            AccessoryFormNotesSectionTitle.Text =
                currentLanguage == "pl" ? "Notatki" : "Notes";
        }
        private void ApplyServiceLanguage()
        {
            ServiceTitle.Text =
                currentLanguage == "pl" ? "Dodaj serwis" : "Add service";

            ServiceDateLabel.Text =
                currentLanguage == "pl" ? "Data serwisu" : "Service date";

            ServiceTypeLabel.Text =
                currentLanguage == "pl" ? "Typ serwisu" : "Service type";

            ServiceCostLabel.Text =
                currentLanguage == "pl" ? "Koszt serwisu" : "Service cost";

            ServiceNotesLabel.Text =
                currentLanguage == "pl" ? "Notatki serwisowe" : "Service notes";

            SaveServiceButton.Content =
                currentLanguage == "pl" ? "Zapisz" : "Save";

            CancelServiceButton.Content =
                currentLanguage == "pl" ? "Anuluj" : "Cancel";

            ServiceHistoryTitle.Text =
                currentLanguage == "pl"
                    ? "Historia serwisowa"
                    : "Service history";

            DeleteServiceEntryButton.Content =
                currentLanguage == "pl"
                    ? "Usuń wpis"
                    : "Delete entry";
        }
        private void LoadServiceTypes()
        {
            ServiceTypeCombo.Items.Clear();

            if (currentLanguage == "pl")
            {
                ServiceTypeCombo.Items.Add("Czyszczenie");
                ServiceTypeCombo.Items.Add("Naprawa");
                ServiceTypeCombo.Items.Add("Modyfikacja");
                ServiceTypeCombo.Items.Add("Wymiana dysku");
                ServiceTypeCombo.Items.Add("Wymiana napędu");
                ServiceTypeCombo.Items.Add("Wymiana baterii");
                ServiceTypeCombo.Items.Add("Inne");
            }
            else
            {
                ServiceTypeCombo.Items.Add("Cleaning");
                ServiceTypeCombo.Items.Add("Repair");
                ServiceTypeCombo.Items.Add("Modification");
                ServiceTypeCombo.Items.Add("Drive replacement");
                ServiceTypeCombo.Items.Add("Optical drive replacement");
                ServiceTypeCombo.Items.Add("Battery replacement");
                ServiceTypeCombo.Items.Add("Other");
            }

            ServiceTypeCombo.SelectedIndex = 0;
        }
        private void LoadHardwareFormDefaults()
        {
            HardwareManufacturerCombo.Items.Clear();

            HardwareManufacturerCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            var preferredOrder = new[]
            {
        "Sony",
        "Microsoft",
        "Nintendo",
        "SEGA",
        "Atari",
        "Commodore",
        "Amiga (Commodore)"
        };

            var manufacturers = ConsolePlatformDatabase
                .GetManufacturers()
                .Select(x => x.Name)
                .Where(x => x != "Other")
                .Where(x => x != "PC")
                .ToList();

            foreach (string name in preferredOrder)
            {
                if (manufacturers.Contains(name))
                    HardwareManufacturerCombo.Items.Add(name);
            }

            foreach (string name in manufacturers
                .Where(x => !preferredOrder.Contains(x))
                .OrderBy(x => x))
            {
                HardwareManufacturerCombo.Items.Add(name);
            }

            HardwareManufacturerCombo.Items.Add(
                currentLanguage == "pl" ? "Inny" : "Other");

            HardwareManufacturerCombo.SelectedIndex = 0;

            HardwareTypeCombo.Items.Clear();
            HardwareTypeCombo.Items.Add(
                currentLanguage == "pl"
                    ? "Wybierz producenta"
                    : "Select manufacturer");
            HardwareTypeCombo.SelectedIndex = 0;

            HardwarePlatformCombo.Items.Clear();
            HardwarePlatformCombo.Items.Add(
                currentLanguage == "pl"
                    ? "Wybierz typ"
                    : "Select type");
            HardwarePlatformCombo.SelectedIndex = 0;

            LoadHardwareConditionDefaults();
            LoadBoxConditionDefaults();
            ClearHardwareDetailsTextBoxes();
            ResetHardwarePackageCheckboxes();

            HardwarePurchaseDatePicker.SelectedDate = null;
            HardwarePurchasePriceTextBox.Text = "";
            HardwarePurchaseCurrencyCombo.SelectedItem = defaultCurrency;
            HardwareNotesTextBox.Text = "";
        }
        private void ResetHardwarePackageCheckboxes()
        {
            HardwareHasBoxCheckBox.IsChecked = false;
            HardwareHasManualCheckBox.IsChecked = false;
            HardwareHasInsertsCheckBox.IsChecked = false;
            HardwareHasPowerSupplyCheckBox.IsChecked = false;
            HardwareHasCableCheckBox.IsChecked = false;
            HardwareHasControllerCheckBox.IsChecked = false;
            HardwareHasMemoryCardCheckBox.IsChecked = false;
        }
        private void EnsureRestoreRecordButton()
        {
            if (RestoreRecordButton != null)
                return;

            if (ArchiveRecordButton?.Parent is not Panel parentPanel)
                return;

            RestoreRecordButton = new Button
            {
                Content = currentLanguage == "pl" ? "Przywróć" : "Restore",
                Visibility = Visibility.Collapsed,
                Margin = ArchiveRecordButton.Margin,
                Padding = ArchiveRecordButton.Padding,
                MinWidth = ArchiveRecordButton.MinWidth,
                MinHeight = ArchiveRecordButton.MinHeight,
                Width = ArchiveRecordButton.Width,
                Height = ArchiveRecordButton.Height,
                HorizontalAlignment = ArchiveRecordButton.HorizontalAlignment,
                VerticalAlignment = ArchiveRecordButton.VerticalAlignment,
                Style = ArchiveRecordButton.Style
            };

            RestoreRecordButton.Click += RestoreRecordButton_Click;

            int archiveButtonIndex = parentPanel.Children.IndexOf(ArchiveRecordButton);

            if (archiveButtonIndex >= 0)
                parentPanel.Children.Insert(archiveButtonIndex + 1, RestoreRecordButton);
            else
                parentPanel.Children.Add(RestoreRecordButton);
        }

        private void ApplyRecordActionsLanguage()
        {
            RecordActionsTitle.Text =
                currentLanguage == "pl"
                    ? "Zarządzanie"
                    : "Management";

            EditRecordButton.Content =
                currentLanguage == "pl"
                    ? "Edytuj"
                    : "Edit";

            AddServiceButton.Content =
                currentLanguage == "pl"
                    ? "Dodaj serwis"
                    : "Add service";

            ArchiveRecordButton.Content =
                currentLanguage == "pl"
                    ? "Przenieś do archiwum"
                    : "Move to archive";

            EnsureRestoreRecordButton();

            if (RestoreRecordButton != null)
            {
                RestoreRecordButton.Content =
                    currentLanguage == "pl"
                        ? "Przywróć"
                        : "Restore";
            }

            DeleteRecordButton.Content =
                currentLanguage == "pl"
                    ? "Usuń"
                    : "Delete";

            AddPageBackgroundButton.Content =
                 currentLanguage == "pl"
                    ? "Dodaj tło podstrony"
                    : "Add Page Background";
        }
        private void AddPageBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            if (HasSelectedRecordBackground())
            {
                string oldBackgroundPath = "";

                if (selectedGameItem != null)
                    oldBackgroundPath = selectedGameItem.CustomBackgroundPath;

                if (selectedHardwareItem != null)
                    oldBackgroundPath = selectedHardwareItem.CustomBackgroundPath;

                if (selectedPcItem != null)
                    oldBackgroundPath = selectedPcItem.CustomBackgroundPath;

                if (!string.IsNullOrWhiteSpace(oldBackgroundPath) &&
                    File.Exists(oldBackgroundPath))
                {
                    File.Delete(oldBackgroundPath);
                }

                if (selectedGameItem != null)
                    selectedGameItem.CustomBackgroundPath = "";

                if (selectedHardwareItem != null)
                    selectedHardwareItem.CustomBackgroundPath = "";

                if (selectedPcItem != null)
                    selectedPcItem.CustomBackgroundPath = "";

                ApplyRecordBackground();
                UpdatePageBackgroundButtonText();

                SaveCollectionChanges();
                return;
            }

            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Title = currentLanguage == "pl"
                    ? "Wybierz tło podstrony"
                    : "Select page background"
            };

            if (dialog.ShowDialog() != true)
                return;

            string backgroundsFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "HGC",
                "Backgrounds");

            Directory.CreateDirectory(backgroundsFolder);

            string extension = Path.GetExtension(dialog.FileName);
            string fileName = $"{Guid.NewGuid()}{extension}";
            string targetPath = Path.Combine(backgroundsFolder, fileName);

            File.Copy(dialog.FileName, targetPath, true);

            bool fitToPage = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "Czy dopasować tło do strony?\n\nTak = obraz zostanie rozciągnięty do całego tła.\nNie = obraz zachowa proporcje i może zostać przycięty."
                    : "Fit the background to the page?\n\nYes = the image will be stretched to the full background.\nNo = the image will keep its proportions and may be cropped.",
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (selectedGameItem != null)
            {
                selectedGameItem.CustomBackgroundPath = targetPath;
                selectedGameItem.CustomBackgroundOpacity = 30;
                selectedGameItem.CustomBackgroundFitToPage = fitToPage;
            }
            else if (selectedHardwareItem != null)
            {
                selectedHardwareItem.CustomBackgroundPath = targetPath;
                selectedHardwareItem.CustomBackgroundOpacity = 30;
                selectedHardwareItem.CustomBackgroundFitToPage = fitToPage;
            }
            else if (selectedPcItem != null)
            {
                selectedPcItem.CustomBackgroundPath = targetPath;
                selectedPcItem.CustomBackgroundOpacity = 30;
                selectedPcItem.CustomBackgroundFitToPage = fitToPage;
            }

            ApplyRecordBackground();
            UpdatePageBackgroundButtonText();

            SaveCollectionChanges();
        }
        private void ApplyRecordBackground()
        {
            string path = "";
            int opacity = 30;
            bool fitToPage = true;

            if (selectedGameItem != null)
            {
                path = selectedGameItem.CustomBackgroundPath;
                opacity = selectedGameItem.CustomBackgroundOpacity;
                fitToPage = selectedGameItem.CustomBackgroundFitToPage;
            }
            else if (selectedHardwareItem != null)
            {
                path = selectedHardwareItem.CustomBackgroundPath;
                opacity = selectedHardwareItem.CustomBackgroundOpacity;
                fitToPage = selectedHardwareItem.CustomBackgroundFitToPage;
            }
            else if (selectedPcItem != null)
            {
                path = selectedPcItem.CustomBackgroundPath;
                opacity = selectedPcItem.CustomBackgroundOpacity;
                fitToPage = selectedPcItem.CustomBackgroundFitToPage;
            }

            if (!string.IsNullOrWhiteSpace(path) &&
                File.Exists(path))
            {
                ThemeBackground.Source =
                    LoadBackgroundImageWithoutLock(path);

                ThemeBackground.Stretch =
                    fitToPage ? Stretch.Fill : Stretch.UniformToFill;

                ThemeBackground.Opacity =
                    opacity / 100.0;

                return;
            }

            UpdateBackgroundForCurrentState();
        }
        private void AddGameButton_Click(object sender, RoutedEventArgs e)
        {
            ClearSelectedRecordForNonRecordView();

            currentPlatformGroup = "Default";
            UpdateBackgroundForCurrentState();

            AddChoicePanel.Visibility = Visibility.Collapsed;
            AddHardwareFormPanel.Visibility = Visibility.Collapsed;
            HardwareDetailsViewPanel.Visibility = Visibility.Collapsed;
            AddServicePanel.Visibility = Visibility.Collapsed;
            CollectionTilesPanel.Visibility = Visibility.Collapsed;

            AddGameFormPanel.Visibility = Visibility.Visible;
            GameRightDetailsColumn.Visibility = Visibility.Collapsed;

            LoadGameFormDefaults();
            ApplyGameFormLanguage();
            ClearGameForm();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                AddGameFormPanel.ScrollToVerticalOffset(0);
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }
        private void CancelGameFormButton_Click(object sender, RoutedEventArgs e)
        {
            bool cancel = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? (isEditingGame
                        ? "Anulować edytowanie gry?\n\nWprowadzone zmiany zostaną utracone."
                        : "Anulować dodawanie gry?\n\nWprowadzone dane zostaną utracone.")
                    : (isEditingGame
                        ? "Cancel editing game?\n\nEntered changes will be lost."
                        : "Cancel adding game?\n\nEntered data will be lost."),
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!cancel)
                return;

            CloseAddGameForm();
        }
        private void MenuInfoAbout_Click(object sender, RoutedEventArgs e)
        {
            new InfoWindow(InfoPage.About, currentLanguage)
            {
                Owner = this
            }.ShowDialog();
        }

        private void MenuInfoRawg_Click(object sender, RoutedEventArgs e)
        {
            new InfoWindow(InfoPage.Rawg, currentLanguage)
            {
                Owner = this
            }.ShowDialog();
        }

        private void MenuInfoCopyright_Click(object sender, RoutedEventArgs e)
        {
            new InfoWindow(InfoPage.Copyright, currentLanguage)
            {
                Owner = this
            }.ShowDialog();
        }

        private void MenuInfoSupport_Click(object sender, RoutedEventArgs e)
        {
            new InfoWindow(InfoPage.Support, currentLanguage)
            {
                Owner = this
            }.ShowDialog();
        }
        private void CloseAddGameForm()
        {
            AddGameFormPanel.Visibility = Visibility.Collapsed;
            GameRightDetailsColumn.Visibility = Visibility.Collapsed;

            CollectionTilesPanel.Visibility = Visibility.Visible;

            isEditingGame = false;
            editingGameItem = null;
            editingGameTreeItem = null;

            selectedGameItem = null;
            selectedHardwareItem = null;
            selectedPcItem = null;

            ClearGameForm();

            ShowDefaultRightPanel();
        }
        private void GameHasBoxCheckBox_Checked(
        object sender,
        RoutedEventArgs e)
        {
            GameBoxDetailsPanel.Visibility =
                GameHasBoxCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
        private void LoadGameBoxConditions()
        {
            GameBoxConditionCombo.Items.Clear();

            GameBoxConditionCombo.Items.Add(
            currentLanguage == "pl" ? "Wybierz" : "Select");

            if (currentLanguage == "pl")
            {
                GameBoxConditionCombo.Items.Add("Idealny");
                GameBoxConditionCombo.Items.Add("Bardzo dobry");
                GameBoxConditionCombo.Items.Add("Dobry");
                GameBoxConditionCombo.Items.Add("Akceptowalny");
                GameBoxConditionCombo.Items.Add("Uszkodzony");
            }
            else
            {
                GameBoxConditionCombo.Items.Add("Mint");
                GameBoxConditionCombo.Items.Add("Very Good");
                GameBoxConditionCombo.Items.Add("Good");
                GameBoxConditionCombo.Items.Add("Acceptable");
                GameBoxConditionCombo.Items.Add("Damaged");
            }

            GameBoxConditionCombo.SelectedIndex = 0;
        }
        private string TranslateGameBoxConditionForCurrentLanguage(string condition)
        {
            if (currentLanguage == "pl")
            {
                return condition switch
                {
                    "Mint" => "Idealny",
                    "Very Good" => "Bardzo dobry",
                    "Good" => "Dobry",
                    "Acceptable" => "Akceptowalny",
                    "Damaged" => "Uszkodzony",
                    _ => condition
                };
            }

            return condition switch
            {
                "Idealny" => "Mint",
                "Bardzo dobry" => "Very Good",
                "Dobry" => "Good",
                "Akceptowalny" => "Acceptable",
                "Uszkodzony" => "Damaged",
                _ => condition
            };
        }
        private void ApplyGameFormLanguage()
        {

            GameIsFavoriteCheckBox.Content =
                currentLanguage == "pl" ? "Ulubiona" : "Favorite";

            GameIsCompletedCheckBox.Content =
                currentLanguage == "pl" ? "Ukończona" : "Completed";

            GameShortDescriptionLabel.Text =
                 currentLanguage == "pl"
                    ? "Krótki opis"
                    : "Short description";

            GameShortDescriptionHintText.Text =
                currentLanguage == "pl"
                    ? "Opcjonalny opis gry (do 350 znaków)"
                    : "Optional game description (up to 350 characters)";

            GameIsPlannedCheckBox.Content =
                currentLanguage == "pl" ? "Planowana" : "Planned";

            AddGameFormTitle.Text =
                currentLanguage == "pl" ? "Dodaj grę" : "Add game";

            GamePublisherLabel.Text =
                currentLanguage == "pl"
                    ? "Wydawca gry"
                    : "Game publisher";

            GameReleaseSectionTitle.Text =
                currentLanguage == "pl"
                    ? "Wydanie"
                    : "Release";

            ToggleGameDetailsButton.Content =
                currentLanguage == "pl" ? "Dodaj szczegóły" : "Add details";

            GamePlatformSectionTitle.Text =
                currentLanguage == "pl" ? "Platforma" : "Platform";

            GameBoxConditionLabel.Text =
            currentLanguage == "pl"
                ? "Stan pudełka"
                : "Box Condition";

            GameManufacturerLabel.Text =
                currentLanguage == "pl" ? "System" : "System";

            GamePlatformHintPart1.Text =
                currentLanguage == "pl"
                ? "Nie widzisz swojego producenta/systemu? "
                : "Can't find your manufacturer/system? ";

            GamePlatformHintPart2.Text =
                currentLanguage == "pl"
                ? "Możesz go dodać tutaj"
                : "You can add it here";

            GameFamilyLabel.Text =
                currentLanguage == "pl" ? "Rodzina" : "Family";

            GamePlatformLabel.Text =
                currentLanguage == "pl" ? "Platforma" : "Platform";

            GameOperatingSystemLabel.Text =
                currentLanguage == "pl" ? "System operacyjny" : "Operating system";

            GameTitleLabel.Text =
                currentLanguage == "pl" ? "Tytuł gry" : "Game title";

            GameReleaseTypeLabel.Text =
                currentLanguage == "pl" ? "Typ wydania" : "Release type";

            GameDigitalPlatformLabel.Text =
                currentLanguage == "pl" ? "Platforma cyfrowa" : "Digital platform";

            GameRegionLabel.Text =
                currentLanguage == "pl" ? "Region" : "Region";

            GameMediaTypeLabel.Text =
                currentLanguage == "pl" ? "Nośnik" : "Media type";

            GameBoxTypeLabel.Text =
                currentLanguage == "pl" ? "Rodzaj pudełka" : "Box type";

            SaveGameButton.Content =
                currentLanguage == "pl" ? "Zapisz" : "Save";

            CancelGameButton.Content =
                currentLanguage == "pl" ? "Anuluj" : "Cancel";

            GameInformationSectionTitle.Text =
                currentLanguage == "pl" ? "Informacje o grze" : "Game Information";

            GameUserRatingLabel.Text =
                currentLanguage == "pl"
                    ? "Moja ocena"
                    : "My rating";

            GameGenreLabel.Text =
                currentLanguage == "pl" ? "Gatunek" : "Genre";

            GameConditionLabel.Text =
                currentLanguage == "pl"
                    ? "Stan nośnika"
                    : "Media Condition";

            GameContentsSectionTitle.Text =
                currentLanguage == "pl" ? "Zawartość pudełka" : "Box Contents";

            GameHasBoxCheckBox.Content =
                currentLanguage == "pl" ? "Posiadam pudełko" : "Has box";

            GameHasManualCheckBox.Content =
                currentLanguage == "pl"
                    ? "Instrukcja"
                    : "Manual";

            GameHasAdvertisementsCheckBox.Content =
                currentLanguage == "pl"
                    ? "Wkładki reklamowe"
                    : "Advertisements";

            GameHasMapCheckBox.Content =
                currentLanguage == "pl"
                    ? "Mapa"
                    : "Map";

            GameHasPosterCheckBox.Content =
                currentLanguage == "pl"
                    ? "Plakat"
                    : "Poster";

            GameHasSoundtrackCheckBox.Content =
                currentLanguage == "pl"
                    ? "Soundtrack CD"
                    : "Soundtrack CD";

            GameHasArtbookCheckBox.Content =
                currentLanguage == "pl"
                    ? "Artbook"
                    : "Artbook";

            GameHasFigureCheckBox.Content =
                currentLanguage == "pl"
                    ? "Figurka"
                    : "Figure";

            GameHasCertificateCheckBox.Content =
                currentLanguage == "pl"
                    ? "Certyfikat"
                    : "Certificate";

            GameHasOtherExtrasCheckBox.Content =
                currentLanguage == "pl"
                    ? "Inne dodatki"
                    : "Other Extras";

            GamePurchaseSectionTitle.Text =
                currentLanguage == "pl" ? "Zakup" : "Purchase";

            GamePurchaseDateLabel.Text =
                currentLanguage == "pl" ? "Data zakupu" : "Purchase date";

            GamePurchasePriceLabel.Text =
                currentLanguage == "pl" ? "Cena zakupu" : "Purchase price";

            GameNotesSectionTitle.Text =
                currentLanguage == "pl" ? "Notatki" : "Notes";
        }
        private void GamePlatformHint_Click(
    object sender,
    System.Windows.Input.MouseButtonEventArgs e)
        {
            AddGameFormPanel.Visibility = Visibility.Collapsed;

            AddChoicePanel.Visibility = Visibility.Visible;
            AddHardwareSubChoicePanel.Visibility = Visibility.Visible;

            e.Handled = true;
        }
        private void LoadGameGenres()
        {
            GameUserRatingCombo.Items.Clear();

            GameUserRatingCombo.Items.Add(
                currentLanguage == "pl" ? "Brak" : "None");

            for (double rating = 0.0; rating <= 10.0; rating += 0.5)
            {
                GameUserRatingCombo.Items.Add(rating.ToString("0.0"));
            }

            GameUserRatingCombo.SelectedIndex = 0;

            GameGenreCombo.Items.Clear();

            GameGenreCombo.Items.Add(
            currentLanguage == "pl" ? "Wybierz" : "Select");

            if (currentLanguage == "pl")
            {
                GameGenreCombo.Items.Add("Akcja");
                GameGenreCombo.Items.Add("Przygodowa");
                GameGenreCombo.Items.Add("RPG");
                GameGenreCombo.Items.Add("Action RPG");
                GameGenreCombo.Items.Add("JRPG");
                GameGenreCombo.Items.Add("MMORPG");
                GameGenreCombo.Items.Add("FPS");
                GameGenreCombo.Items.Add("TPS");
                GameGenreCombo.Items.Add("Shoot'em Up");
                GameGenreCombo.Items.Add("Beat'em Up");
                GameGenreCombo.Items.Add("RTS");
                GameGenreCombo.Items.Add("Strategiczna");
                GameGenreCombo.Items.Add("Taktyczna");
                GameGenreCombo.Items.Add("MOBA");
                GameGenreCombo.Items.Add("Symulacja");
                GameGenreCombo.Items.Add("Survival");
                GameGenreCombo.Items.Add("Sandbox");
                GameGenreCombo.Items.Add("Wyścigi");
                GameGenreCombo.Items.Add("Sportowa");
                GameGenreCombo.Items.Add("Bijatyka");
                GameGenreCombo.Items.Add("Platformowa");
                GameGenreCombo.Items.Add("Logiczna");
                GameGenreCombo.Items.Add("Horror");
                GameGenreCombo.Items.Add("Rytmiczna");
                GameGenreCombo.Items.Add("Edukacyjna");
                GameGenreCombo.Items.Add("Visual Novel");
                GameGenreCombo.Items.Add("Imprezowa");
                GameGenreCombo.Items.Add("Inna");
            }
            else
            {
                GameGenreCombo.Items.Add("Action");
                GameGenreCombo.Items.Add("Adventure");
                GameGenreCombo.Items.Add("RPG");
                GameGenreCombo.Items.Add("Action RPG");
                GameGenreCombo.Items.Add("JRPG");
                GameGenreCombo.Items.Add("MMORPG");
                GameGenreCombo.Items.Add("FPS");
                GameGenreCombo.Items.Add("TPS");
                GameGenreCombo.Items.Add("Shoot'em Up");
                GameGenreCombo.Items.Add("Beat'em Up");
                GameGenreCombo.Items.Add("RTS");
                GameGenreCombo.Items.Add("Strategy");
                GameGenreCombo.Items.Add("Tactical");
                GameGenreCombo.Items.Add("MOBA");
                GameGenreCombo.Items.Add("Simulation");
                GameGenreCombo.Items.Add("Survival");
                GameGenreCombo.Items.Add("Sandbox");
                GameGenreCombo.Items.Add("Racing");
                GameGenreCombo.Items.Add("Sports");
                GameGenreCombo.Items.Add("Fighting");
                GameGenreCombo.Items.Add("Platform");
                GameGenreCombo.Items.Add("Puzzle");
                GameGenreCombo.Items.Add("Horror");
                GameGenreCombo.Items.Add("Rhythm");
                GameGenreCombo.Items.Add("Educational");
                GameGenreCombo.Items.Add("Visual Novel");
                GameGenreCombo.Items.Add("Party");
                GameGenreCombo.Items.Add("Other");
            }

            GameGenreCombo.SelectedIndex = 0;
        }
        
        private void RecentlyAddedMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem item)
                return;

            showRecentlyAddedPanel = item.IsChecked;

            ShowDefaultRightPanel();
        }
        private void LoadGameConditions()
        {
            GameConditionCombo.Items.Clear();

            GameConditionCombo.Items.Add(
            currentLanguage == "pl" ? "Wybierz" : "Select");

            GameConditionCombo.Items.Add(
                currentLanguage == "pl" ? "Nowa" : "New");

            GameConditionCombo.Items.Add(
                currentLanguage == "pl" ? "Idealna" : "Mint");

            GameConditionCombo.Items.Add(
                currentLanguage == "pl" ? "Bardzo dobra" : "Very Good");

            GameConditionCombo.Items.Add(
                currentLanguage == "pl" ? "Dobra" : "Good");

            GameConditionCombo.Items.Add(
                currentLanguage == "pl" ? "Przeciętna" : "Fair");

            GameConditionCombo.Items.Add(
                currentLanguage == "pl" ? "Uszkodzona" : "Damaged");

            GameConditionCombo.SelectedIndex = 0;
        }

        private void LoadGameFormDefaults()
        {
            GameManufacturerCombo.Items.Clear();

            GameManufacturerCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            string[] manufacturers =
            {
        "PC",

        "Sony",
        "Microsoft",
        "Nintendo",
        "SEGA",
        "Atari",
        "Commodore",
        "Apple",

        "SNK",
        "NEC",
        "Panasonic",
        "Philips",
        "Bandai",
        "Coleco",
        "Mattel",
        "Magnavox",
        "Fairchild",
        "RCA",
        "Bally / Midway",
        "GCE / Vectrex",
        "Nokia",
        "Tiger",
        "Watara",
        "GamePark",
        "Zeebo",
        "Epoch",
        "Casio",
        "Fujitsu",
        "Interton",
        "APF",
        "Emerson",
        "Evercade",
        "Analogue",
        "OUYA",
        "NVIDIA",
        "Sinclair",
        "Developer Hardware"
    };

            foreach (string manufacturer in manufacturers)
                GameManufacturerCombo.Items.Add(manufacturer);

            var customManufacturers = collection.HardwareItems
                .Select(x => x.Manufacturer)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            foreach (string manufacturer in customManufacturers)
            {
                if (!GameManufacturerCombo.Items.Contains(manufacturer))
                    GameManufacturerCombo.Items.Add(manufacturer);
            }

            GameManufacturerCombo.SelectedIndex = 0;

            GameFamilyCombo.Items.Clear();
            GameFamilyCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz system" : "Select system");
            GameFamilyCombo.SelectedIndex = 0;

            GamePlatformCombo.Items.Clear();
            GamePlatformCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz rodzinę" : "Select family");
            GamePlatformCombo.SelectedIndex = 0;

            LoadGameReleaseTypes();
            LoadGameRegions();
            LoadGameMediaTypes(false);
            LoadGameBoxTypes(false);
            LoadGameBoxConditions();
            LoadGameGenres();
            LoadGameConditions();
        }
        private string TranslateGameConditionForCurrentLanguage(string condition)
        {
            if (currentLanguage == "pl")
            {
                return condition switch
                {
                    "New" => "Nowa",
                    "Mint" => "Idealna",
                    "Very Good" => "Bardzo dobra",
                    "Good" => "Dobra",
                    "Fair" => "Przeciętna",
                    "Damaged" => "Uszkodzona",
                    _ => condition
                };
            }

            return condition switch
            {
                "Nowa" => "New",
                "Idealna" => "Mint",
                "Bardzo dobra" => "Very Good",
                "Dobra" => "Good",
                "Przeciętna" => "Fair",
                "Uszkodzona" => "Damaged",
                _ => condition
            };
        }
        private enum GameFilterType
        {
            All,
            Genre,
            Favorites,
            Completed,
            Uncompleted,
            Planned
        }
        private enum GameFilterMode
        {
            All,
            Favorites,
            Completed,
            Uncompleted,
            Planned,
            Genre
        }

        private GameFilterMode currentGameFilter = GameFilterMode.All;
        private string currentGameGenre = "";
        private bool isGamesViewActive = false;

        private sealed class GameTreeNode
        {
            public string Text { get; set; } = "";
            public int Count { get; set; }
            public GameFilterType FilterType { get; set; }
            public string Value { get; set; } = "";
            public List<GameTreeNode> Children { get; } = new();
        }
        private void LoadGameReleaseTypes()
        {
            GameReleaseTypeCombo.Items.Clear();

            GameReleaseTypeCombo.Items.Add(
                currentLanguage == "pl"
                    ? "Nośnik fizyczny"
                    : "Physical");

            GameReleaseTypeCombo.Items.Add(
                currentLanguage == "pl"
                    ? "Edycja cyfrowa"
                    : "Digital");

            GameReleaseTypeCombo.SelectedIndex = 0;
        }
        private void LoadGameRegions()
        {
            GameRegionCombo.Items.Clear();

            GameRegionCombo.Items.Add(
            currentLanguage == "pl" ? "Wybierz" : "Select");

            GameRegionCombo.Items.Add("PAL");
            GameRegionCombo.Items.Add("NTSC-U");
            GameRegionCombo.Items.Add("NTSC-J");
            GameRegionCombo.Items.Add("Region Free");

            GameRegionCombo.Items.Add(
                currentLanguage == "pl"
                    ? "Inny"
                    : "Other");

            GameRegionCombo.SelectedIndex = 0;
        }

        private void LoadGameMediaTypes(bool isPc)
        {
            GameMediaTypeCombo.Items.Clear();

            GameMediaTypeCombo.Items.Add(
            currentLanguage == "pl" ? "Wybierz" : "Select");

            if (isPc)
            {
                GameMediaTypeCombo.Items.Add("3.5\" Floppy Disk");
                GameMediaTypeCombo.Items.Add("5.25\" Floppy Disk");

                GameMediaTypeCombo.Items.Add("CD");
                GameMediaTypeCombo.Items.Add("DVD");
                GameMediaTypeCombo.Items.Add("Blu-ray");

                GameMediaTypeCombo.Items.Add("Cassette Tape");

                GameMediaTypeCombo.Items.Add("Digital Download");
            }
            else
            {
                GameMediaTypeCombo.Items.Add("CD");
                GameMediaTypeCombo.Items.Add("DVD");
                GameMediaTypeCombo.Items.Add("Blu-ray");

                GameMediaTypeCombo.Items.Add("GD-ROM");
                GameMediaTypeCombo.Items.Add("UMD");
                GameMediaTypeCombo.Items.Add("MiniDVD");

                GameMediaTypeCombo.Items.Add("Cartridge");
                GameMediaTypeCombo.Items.Add("HuCard");

                GameMediaTypeCombo.Items.Add("Nintendo DS Card");
                GameMediaTypeCombo.Items.Add("Nintendo 3DS Card");
                GameMediaTypeCombo.Items.Add("Nintendo Switch Game Card");

                GameMediaTypeCombo.Items.Add("PlayStation Vita Card");

                GameMediaTypeCombo.Items.Add("Cassette Tape");

                GameMediaTypeCombo.Items.Add("Digital Download");
            }

            GameMediaTypeCombo.SelectedIndex = 0;
        }
        private void AddHardwarePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditingHardware && editingHardwareItem != null)
            {
                PhotoGalleryWindow window = new PhotoGalleryWindow(
                    editingHardwareItem.Photos,
                    currentLanguage)
                {
                    Owner = this
                };

                window.ShowDialog();
                return;
            }

            if (currentHardwarePhotos == null)
                currentHardwarePhotos = new List<CollectionPhoto>();

            PhotoGalleryWindow addWindow = new PhotoGalleryWindow(
                currentHardwarePhotos,
                currentLanguage)
            {
                Owner = this
            };

            addWindow.ShowDialog();
        }
        private void LoadGameBoxTypes(bool isPc)
        {
            GameBoxTypeCombo.Items.Clear();

            GameBoxTypeCombo.Items.Add(
            currentLanguage == "pl" ? "Wybierz" : "Select");

            if (isPc)
            {
                GameBoxTypeCombo.Items.Add(
                    currentLanguage == "pl"
                        ? "Plastikowe pudełko"
                        : "Plastic Case");

                GameBoxTypeCombo.Items.Add(
                    currentLanguage == "pl"
                        ? "Kartonowe pudełko"
                        : "Cardboard Box");

                GameBoxTypeCombo.Items.Add("Big Box");
                GameBoxTypeCombo.Items.Add("Small Box");

                GameBoxTypeCombo.Items.Add("Jewel Case");
                GameBoxTypeCombo.Items.Add("DVD Box");

                GameBoxTypeCombo.Items.Add("Digipak");
                GameBoxTypeCombo.Items.Add("Cardboard Sleeve");

                GameBoxTypeCombo.Items.Add("Steelbook");
            }
            else
            {
                GameBoxTypeCombo.Items.Add(
                    currentLanguage == "pl"
                        ? "Plastikowe pudełko"
                        : "Plastic Case");

                GameBoxTypeCombo.Items.Add(
                    currentLanguage == "pl"
                        ? "Kartonowe pudełko"
                        : "Cardboard Box");

                GameBoxTypeCombo.Items.Add("Steelbook");

                GameBoxTypeCombo.Items.Add("DVD Box");
                GameBoxTypeCombo.Items.Add("Blu-ray Box");

                GameBoxTypeCombo.Items.Add("Jewel Case");
                GameBoxTypeCombo.Items.Add("Long Box");
                GameBoxTypeCombo.Items.Add("Snap Case");
                GameBoxTypeCombo.Items.Add("Clamshell");

                GameBoxTypeCombo.Items.Add("Boxed Cartridge");

            }

            GameBoxTypeCombo.SelectedIndex = 0;
        }

        private void GameManufacturerCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadGameFamiliesForSelectedManufacturer();

            if (GameReleaseTypeCombo.SelectedIndex == 1)
                LoadDigitalPlatforms(GameManufacturerCombo.Text);
        }

        private void GameFamilyCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadGamePlatformsForSelectedFamily();
        }

        private void GamePlatformCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGamePlatformSpecificFields();

            if (GameReleaseTypeCombo.SelectedIndex == 1)
                LoadDigitalPlatforms(GameManufacturerCombo.Text);
        }
        private void UpdateGamePlatformSpecificFields()
        {
            string manufacturer = GameManufacturerCombo.SelectedItem?.ToString() ?? "";
            string platform = GamePlatformCombo.SelectedItem?.ToString() ?? "";

            bool isPcWindows =
                manufacturer == "PC" &&
                platform == "Windows";

            GameOperatingSystemLabel.Visibility =
                isPcWindows ? Visibility.Visible : Visibility.Collapsed;

            GameOperatingSystemCombo.Visibility =
                isPcWindows ? Visibility.Visible : Visibility.Collapsed;

            if (isPcWindows)
                LoadWindowsVersions();

            bool isPc =
                manufacturer == "PC";

            LoadGameMediaTypes(isPc);
            LoadGameBoxTypes(isPc);
        }
        private void LoadWindowsVersions()
        {
            GameOperatingSystemCombo.Items.Clear();

            GameOperatingSystemCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            GameOperatingSystemCombo.Items.Add("Windows 3.1");
            GameOperatingSystemCombo.Items.Add("Windows 3.11");
            GameOperatingSystemCombo.Items.Add("Windows 95");
            GameOperatingSystemCombo.Items.Add("Windows 98");
            GameOperatingSystemCombo.Items.Add("Windows ME");
            GameOperatingSystemCombo.Items.Add("Windows 2000");
            GameOperatingSystemCombo.Items.Add("Windows XP");
            GameOperatingSystemCombo.Items.Add("Windows Vista");
            GameOperatingSystemCombo.Items.Add("Windows 7");
            GameOperatingSystemCombo.Items.Add("Windows 8");
            GameOperatingSystemCombo.Items.Add("Windows 8.1");
            GameOperatingSystemCombo.Items.Add("Windows 10");
            GameOperatingSystemCombo.Items.Add("Windows 11");
            GameOperatingSystemCombo.Items.Add(currentLanguage == "pl" ? "Inne" : "Other");

            GameOperatingSystemCombo.SelectedIndex = 0;
        }
        private void GameReleaseTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGameReleaseFields();
        }
        private bool IsSelectedValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return value != "Wybierz" &&
                   value != "Select" &&
                   value != "Inny" &&
                   value != "Other";
        }
        private void UpdateGameReleaseFields()
        {
            string manufacturer =
                GameManufacturerCombo.SelectedItem?.ToString() ?? "";

            bool isDigital =
                GameReleaseTypeCombo.SelectedIndex == 1;

            GameDigitalPlatformLabel.Visibility =
                isDigital ? Visibility.Visible : Visibility.Collapsed;

            GameDigitalPlatformCombo.Visibility =
                isDigital ? Visibility.Visible : Visibility.Collapsed;

            if (isDigital)
                LoadDigitalPlatforms(manufacturer);

            GameMediaTypeLabel.Visibility =
                isDigital ? Visibility.Collapsed : Visibility.Visible;

            GameMediaTypeCombo.Visibility =
                isDigital ? Visibility.Collapsed : Visibility.Visible;

            GameConditionLabel.Visibility =
                isDigital ? Visibility.Collapsed : Visibility.Visible;

            GameConditionCombo.Visibility =
                isDigital ? Visibility.Collapsed : Visibility.Visible;

            // Region zostaje widoczny zawsze.
            GameRegionLabel.Visibility = Visibility.Visible;
            GameRegionCombo.Visibility = Visibility.Visible;
        }
        private void LoadDigitalPlatforms(string manufacturer)
        {
            GameDigitalPlatformCombo.Items.Clear();

            GameDigitalPlatformCombo.Items.Add(
            currentLanguage == "pl" ? "Wybierz" : "Select");

            if (manufacturer == "PC")
            {
                GameDigitalPlatformCombo.Items.Add("Steam");
                GameDigitalPlatformCombo.Items.Add("GOG");
                GameDigitalPlatformCombo.Items.Add("Epic Games Store");
                GameDigitalPlatformCombo.Items.Add("EA App");
                GameDigitalPlatformCombo.Items.Add("Ubisoft Connect");
                GameDigitalPlatformCombo.Items.Add("Battle.net");
                GameDigitalPlatformCombo.Items.Add("Microsoft Store");
                GameDigitalPlatformCombo.Items.Add("Amazon Games");
                GameDigitalPlatformCombo.Items.Add("itch.io");
                GameDigitalPlatformCombo.Items.Add("Rockstar Games Launcher");
                GameDigitalPlatformCombo.Items.Add(currentLanguage == "pl" ? "Inne" : "Other");
            }
            else if (manufacturer == "Sony")
            {
                GameDigitalPlatformCombo.Items.Add("PlayStation Store");
            }
            else if (manufacturer == "Microsoft")
            {
                GameDigitalPlatformCombo.Items.Add("Xbox Store");
                GameDigitalPlatformCombo.Items.Add("Microsoft Store");
            }
            else if (manufacturer == "Nintendo")
            {
                GameDigitalPlatformCombo.Items.Add("Nintendo eShop");
            }
            else if (manufacturer == "Apple")
            {
                GameDigitalPlatformCombo.Items.Add("Mac App Store");
                GameDigitalPlatformCombo.Items.Add("Steam");
                GameDigitalPlatformCombo.Items.Add("GOG");
                GameDigitalPlatformCombo.Items.Add(currentLanguage == "pl" ? "Inne" : "Other");
            }
            else
            {
                GameDigitalPlatformCombo.Items.Add(currentLanguage == "pl" ? "Inne" : "Other");
            }

            GameDigitalPlatformCombo.SelectedIndex = 0;
        }

        private void SaveGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(GameTitleTextBox.Text))
            {
                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? "Podaj tytuł gry."
                        : "Enter game title.",
                    "HGC",
                    HgcMessageBoxType.Information,
                    currentLanguage);

                return;
            }

            if (!ValidateNameLength(GameTitleTextBox.Text))
                return;

            Game game = isEditingGame && editingGameItem != null
                ? editingGameItem
                : new Game();

            bool wasEditingGame = isEditingGame;

            if (!IsSelectedValue(GameManufacturerCombo.Text))
            {
                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? "Wybierz producenta przed zapisaniem gry."
                        : "Select manufacturer before saving the game.",
                    "HGC",
                    HgcMessageBoxType.Warning,
                    currentLanguage);

                return;
            }

            if (!IsSelectedValue(GamePlatformCombo.Text))
            {
                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? "Wybierz platformę przed zapisaniem gry."
                        : "Select platform before saving the game.",
                    "HGC",
                    HgcMessageBoxType.Warning,
                    currentLanguage);

                return;
            }

            game.Title = GameTitleTextBox.Text.Trim();
            game.ShortDescription = GameShortDescriptionTextBox.Text.Trim();

            game.Photos = currentGamePhotos ?? new List<CollectionPhoto>();

            if (currentGameFrontCover != null &&
            !File.Exists(currentGameFrontCover.OriginalPath))
            {
                currentGameFrontCover = null;
            }

            if (currentGameBackCover != null &&
                !File.Exists(currentGameBackCover.OriginalPath))
            {
                currentGameBackCover = null;
            }

            game.FrontCoverPhoto = currentGameFrontCover;
            game.BackCoverPhoto = currentGameBackCover;

            game.Manufacturer = GameManufacturerCombo.Text;
            game.Family = GameFamilyCombo.Text;
            game.Platform = GamePlatformCombo.Text;

            game.OperatingSystem =
                GameOperatingSystemCombo.Visibility == Visibility.Visible &&
                IsSelectedValue(GameOperatingSystemCombo.Text)
                    ? GameOperatingSystemCombo.Text
                    : "";

            game.ReleaseType = GameReleaseTypeCombo.Text;
            game.Publisher = GamePublisherTextBox.Text.Trim();

            game.DigitalPlatform =
                GameDigitalPlatformCombo.Visibility == Visibility.Visible &&
                IsSelectedValue(GameDigitalPlatformCombo.Text)
                    ? GameDigitalPlatformCombo.Text
                    : "";

            game.Region =
                GameRegionCombo.Visibility == Visibility.Visible &&
                IsSelectedValue(GameRegionCombo.Text)
                    ? GameRegionCombo.Text
                    : "";

            game.MediaType =
                GameMediaTypeCombo.Visibility == Visibility.Visible &&
                IsSelectedValue(GameMediaTypeCombo.Text)
                    ? GameMediaTypeCombo.Text
                    : "";

            game.Condition =
                 GameConditionCombo.Visibility == Visibility.Visible &&
                 IsSelectedValue(GameConditionCombo.Text)
                     ? NormalizeConditionForSave(GameConditionCombo.Text)
                     : "";

            game.Genre =
                IsSelectedValue(GameGenreCombo.Text)
                    ? NormalizeGenreForSave(GameGenreCombo.Text)
                    : "";

            if (GameUserRatingCombo.SelectedIndex > 0 &&
            double.TryParse(GameUserRatingCombo.SelectedItem?.ToString(), out double rating))
            {
                game.UserRating = rating;
            }
            else
            {
                game.UserRating = null;
            }

            game.IsFavorite = GameIsFavoriteCheckBox.IsChecked == true;
            game.IsCompleted = GameIsCompletedCheckBox.IsChecked == true;
            game.IsPlanned = GameIsPlannedCheckBox.IsChecked == true;
            game.HasBox = GameHasBoxCheckBox.IsChecked == true;
            game.HasManual = GameHasManualCheckBox.IsChecked == true;
            game.HasAdvertisements =
                GameHasAdvertisementsCheckBox.IsChecked == true;
            game.HasMap =
                GameHasMapCheckBox.IsChecked == true;
            game.HasPoster =
                GameHasPosterCheckBox.IsChecked == true;
            game.HasSoundtrack =
                GameHasSoundtrackCheckBox.IsChecked == true;
            game.HasArtbook =
                GameHasArtbookCheckBox.IsChecked == true;
            game.HasFigure =
                GameHasFigureCheckBox.IsChecked == true;
            game.HasCertificate =
                GameHasCertificateCheckBox.IsChecked == true;
            game.HasOtherExtras =
                GameHasOtherExtrasCheckBox.IsChecked == true;

            game.BoxType =
                game.HasBox && IsSelectedValue(GameBoxTypeCombo.Text)
                    ? GameBoxTypeCombo.Text
                    : "";

            game.BoxCondition =
                 game.HasBox && IsSelectedValue(GameBoxConditionCombo.Text)
                     ? NormalizeConditionForSave(GameBoxConditionCombo.Text)
                     : "";

            game.PurchaseDate = GamePurchaseDatePicker.SelectedDate;
            game.Notes = GameNotesTextBox.Text.Trim();

            if (decimal.TryParse(GamePurchasePriceTextBox.Text, out decimal purchasePrice))
                game.PurchasePrice = purchasePrice;
            else
                game.PurchasePrice = null;

            game.PurchaseCurrency =
                GamePurchaseCurrencyCombo.SelectedItem?.ToString()
                ?? defaultCurrency;

            if (isEditingGame)
            {
                if (editingGameTreeItem != null)
                    editingGameTreeItem.Header = game.Title;

                HgcMessageBox.Show(
                 this,
                 currentLanguage == "pl"
                     ? $"{game.Title} — zapisano zmiany."
                     : $"{game.Title} — changes saved.",
                 "HGC",
                 HgcMessageBoxType.Information,
                 currentLanguage);
            }
            else
            {
                collection.Games.Add(game);
                AddGameRecordToTree(game);

                HgcMessageBox.Show(
                this,
                currentLanguage == "pl"
                    ? $"{game.Title} została dodana."
                    : $"{game.Title} has been added.",
                "HGC",
                HgcMessageBoxType.Information,
                currentLanguage);
            }

            isEditingGame = false;
            editingGameItem = null;
            editingGameTreeItem = null;

            SaveGameButton.Content =
                currentLanguage == "pl" ? "Zapisz" : "Save";

            ClearGameForm();

            RebuildCollectionTree();
            BuildGamesPanel();
            RefreshStats();

            if (wasEditingGame)
                ShowGameDetails(game);
            else
                ShowDefaultRightPanel();

            SaveCollectionChanges();

        }
        private Border CreateGameFilterRow(
     string text,
     int count,
     GameFilterMode filterMode)
        {
            Grid grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });

            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });

            TextBlock title = new()
            {
                Text = text,
                FontSize = 12,
                Foreground = (Brush)FindResource("AppTextBrush"),
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock value = new()
            {
                Text = count.ToString(),
                FontSize = 12,
                Foreground = (Brush)FindResource("AppTextBrush"),
                Opacity = 0.75,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Grid.SetColumn(title, 0);
            Grid.SetColumn(value, 1);

            grid.Children.Add(title);
            grid.Children.Add(value);

            Border row = new Border
            {
                Child = grid,
                Padding = new Thickness(2, 2, 2, 2),
                Margin = new Thickness(0, 2, 0, 2),
                Background = Brushes.Transparent,
                Cursor = Cursors.Hand
            };

            row.MouseLeftButtonUp += (s, e) =>
            {
                SetGameFilter(filterMode);
            };

            return row;
        }
        private void AddGameRecordToTree(Game game)
        {
            if (game == null)
                return;

            string backgroundGroup =
                GetBackgroundGroupForManufacturer(game.Manufacturer);

            TreeViewItem manufacturerItem = GetOrCreateTreeItem(
                CollectionTree.Items,
                game.Manufacturer,
                backgroundGroup);

            manufacturerItem.Tag = new TreeNodeData
            {
                NodeType = "Manufacturer",
                BackgroundGroup = backgroundGroup,
                Manufacturer = game.Manufacturer
            };

            if (game.Manufacturer == "PC")
            {
                TreeViewItem pcGamesCategoryItem = GetOrCreateTreeItem(
                    manufacturerItem.Items,
                    GetGamesCategoryName(),
                    backgroundGroup);

                pcGamesCategoryItem.Tag = new TreeNodeData
                {
                    NodeType = "Category",
                    BackgroundGroup = backgroundGroup,
                    Manufacturer = game.Manufacturer,
                    Category = GetGamesCategoryName()
                };

                TreeViewItem pcPlatformItem = GetOrCreateTreeItem(
                    pcGamesCategoryItem.Items,
                    game.Platform,
                    backgroundGroup);

                pcPlatformItem.Tag = new TreeNodeData
                {
                    NodeType = "Platform",
                    BackgroundGroup = backgroundGroup,
                    Manufacturer = game.Manufacturer,
                    Platform = game.Platform,
                    Category = GetGamesCategoryName()
                };

                TreeViewItem pcGameItem = new TreeViewItem
                {
                    Header = game.Title,
                    Tag = new TreeNodeData
                    {
                        NodeType = "GameRecord",
                        BackgroundGroup = backgroundGroup,
                        Manufacturer = game.Manufacturer,
                        Platform = game.Platform,
                        Category = GetGamesCategoryName(),
                        GameItem = game
                    }
                };

                pcPlatformItem.Items.Add(pcGameItem);
                return;
            }

            TreeViewItem familyItem = GetOrCreateTreeItem(
                manufacturerItem.Items,
                game.Family,
                backgroundGroup);

            familyItem.Tag = new TreeNodeData
            {
                NodeType = "Family",
                BackgroundGroup = backgroundGroup,
                Manufacturer = game.Manufacturer,
                Family = game.Family
            };

            TreeViewItem categoryParent;

            if (UsesDirectCategories(
                    game.Manufacturer,
                    game.Family,
                    game.Platform))
            {
                if (game.Platform == "GameCube" ||
                    game.Platform == "Panasonic Q")
                {
                    categoryParent = GetOrCreateTreeItem(
                        familyItem.Items,
                        "GameCube",
                        backgroundGroup);

                    categoryParent.Tag = new TreeNodeData
                    {
                        NodeType = "Platform",
                        BackgroundGroup = backgroundGroup,
                        Manufacturer = game.Manufacturer,
                        Family = game.Family,
                        Platform = "GameCube"
                    };
                }
                else
                {
                    categoryParent = familyItem;
                }
            }
            else
            {
                categoryParent = GetOrCreateTreeItem(
                    familyItem.Items,
                    game.Platform,
                    backgroundGroup);

                categoryParent.Tag = new TreeNodeData
                {
                    NodeType = "Platform",
                    BackgroundGroup = backgroundGroup,
                    Manufacturer = game.Manufacturer,
                    Family = game.Family,
                    Platform = game.Platform
                };
            }

            TreeViewItem gamesCategoryItem = GetOrCreateTreeItem(
                categoryParent.Items,
                GetGamesCategoryName(),
                backgroundGroup);

            gamesCategoryItem.Tag = new TreeNodeData
            {
                NodeType = "Category",
                BackgroundGroup = backgroundGroup,
                Manufacturer = game.Manufacturer,
                Family = game.Family,
                Platform = game.Platform,
                Category = GetGamesCategoryName()
            };

            TreeViewItem gameItem = new TreeViewItem
            {
                Header = game.Title,
                Tag = new TreeNodeData
                {
                    NodeType = "GameRecord",
                    BackgroundGroup = backgroundGroup,
                    Manufacturer = game.Manufacturer,
                    Family = game.Family,
                    Platform = game.Platform,
                    Category = GetGamesCategoryName(),
                    GameItem = game
                }
            };

            gamesCategoryItem.Items.Add(gameItem);
        }
        private void RecentlyAddedVisibilityMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem clickedItem)
                return;

            if (clickedItem.Tag is not string mode)
                return;

            if (clickedItem.Parent is MenuItem parentMenu)
            {
                foreach (object item in parentMenu.Items)
                {
                    if (item is MenuItem menuItem)
                        menuItem.IsChecked = false;
                }
            }

            clickedItem.IsChecked = true;

            showRecentlyAddedPanel = mode == "Show";

            settings.ShowRecentlyAddedPanel = showRecentlyAddedPanel;
            SaveSettingsChanges();

            ShowDefaultRightPanel();
        }
        private void RebuildCollectionTree()
        {
            CollectionTree.Items.Clear();

            foreach (var hardware in collection.HardwareItems)
                AddHardwareRecordToTree(hardware);

            foreach (var game in collection.Games)
                AddGameRecordToTree(game);

            foreach (var pc in collection.PcSystems)
                AddPcRecordToTree(pc);

            foreach (var accessory in collection.Accessories)
                AddAccessoryToTree(accessory);

            foreach (var accessory in collection.ArchivedAccessories)
                AddArchivedAccessoryToTree(accessory);

            foreach (var hardware in collection.ArchivedHardwareItems)
                MoveHardwareToArchive(hardware);

            foreach (var game in collection.ArchivedGames)
                MoveGameToArchive(game);

            foreach (var pc in collection.ArchivedPcSystems)
                MovePcToArchive(pc);
        }
        private string TranslateHardwareFamily(string family)
        {
            if (currentLanguage == "pl")
            {
                return family switch
                {
                    "Atari Consoles" => "Konsole",
                    "Atari Handhelds" => "Konsole przenośne",
                    "Modern Atari" => "Współczesne Atari",
                    _ => family
                };
            }

            return family;
        }
        private List<Game> GetFilteredGames()
        {
            IEnumerable<Game> games = collection.Games;

            switch (currentGameFilter)
            {
                case GameFilterMode.Favorites:
                    games = games.Where(g => g.IsFavorite);
                    break;

                case GameFilterMode.Completed:
                    games = games.Where(g => g.IsCompleted);
                    break;

                case GameFilterMode.Uncompleted:
                    games = games.Where(g => !g.IsCompleted);
                    break;

                case GameFilterMode.Planned:
                    games = games.Where(g => g.IsPlanned);
                    break;

                case GameFilterMode.Genre:
                    games = games.Where(g =>
                        (string.IsNullOrWhiteSpace(g.Genre)
                            ? "Niesklasyfikowane"
                            : g.Genre.Trim()) == currentGameGenre);
                    break;
            }

            return games.ToList();
        }
        private void CancelGameButton_Click(object sender, RoutedEventArgs e)
        {
            CancelGameFormButton_Click(sender, e);
        }
        private void ClearGameForm()
        {
            GameTitleTextBox.Text = "";
            GameShortDescriptionTextBox.Text = "";
            GamePublisherTextBox.Text = "";

            GamePurchaseDatePicker.SelectedDate = null;
            GamePurchasePriceTextBox.Text = "";
            GamePurchaseCurrencyCombo.SelectedItem = defaultCurrency;
            GameNotesTextBox.Text = "";

            GameHasBoxCheckBox.IsChecked = false;
            GameHasManualCheckBox.IsChecked = false;

            GameHasAdvertisementsCheckBox.IsChecked = false;
            GameHasMapCheckBox.IsChecked = false;
            GameHasPosterCheckBox.IsChecked = false;
            GameHasSoundtrackCheckBox.IsChecked = false;
            GameHasArtbookCheckBox.IsChecked = false;
            GameHasFigureCheckBox.IsChecked = false;
            GameHasCertificateCheckBox.IsChecked = false;
            GameHasOtherExtrasCheckBox.IsChecked = false;
            GameManufacturerCombo.SelectedIndex = 0;
            GameFamilyCombo.SelectedIndex = 0;
            GamePlatformCombo.SelectedIndex = 0;

            GameReleaseTypeCombo.SelectedIndex = 0;
            GameDigitalPlatformCombo.SelectedIndex = 0;
            GameMediaTypeCombo.SelectedIndex = 0;
            GameRegionCombo.SelectedIndex = 0;
            GameConditionCombo.SelectedIndex = 0;

            GameGenreCombo.SelectedIndex = 0;

            GameUserRatingCombo.SelectedIndex = 0;

            GameIsFavoriteCheckBox.IsChecked = false;
            GameIsCompletedCheckBox.IsChecked = false;
            GameIsPlannedCheckBox.IsChecked = false;

            UpdateGameRatingStars(0);
            GameBoxDetailsPanel.Visibility = Visibility.Collapsed;

            GameRightDetailsColumn.Visibility = Visibility.Collapsed;

            ToggleGameDetailsButton.Content =
                currentLanguage == "pl"
                    ? "Dodaj szczegóły"
                    : "Add details";

            GamePhotosButton.Content =
                currentLanguage == "pl"
                    ? "Zdjęcia / okładki"
                    : "Photos / covers";

            currentGamePhotos = new List<CollectionPhoto>();
            currentGameFrontCover = null;
            currentGameBackCover = null;
        }
        private void AddCollectionBranch(
             string manufacturer,
             string family,
             string platform,
             bool isHardware)
        {
            string backgroundGroup = GetBackgroundGroupForManufacturer(manufacturer);

            TreeViewItem manufacturerItem = GetOrCreateTreeItem(
                CollectionTree.Items,
                manufacturer,
                backgroundGroup);

            TreeViewItem familyItem = GetOrCreateTreeItem(
                manufacturerItem.Items,
                family,
                backgroundGroup);

            TreeViewItem categoryParent;

            if (UsesDirectCategories(
                manufacturer,
                family,
                platform))
            {
                if (platform == "GameCube" ||
                    platform == "Panasonic Q")
                {
                    categoryParent = GetOrCreateTreeItem(
                        familyItem.Items,
                        "GameCube",
                        backgroundGroup);
                }
                else
                {
                    categoryParent = familyItem;
                }
            }
            else
            {
                categoryParent = GetOrCreateTreeItem(
                    familyItem.Items,
                    platform,
                    backgroundGroup);
            }

            GetOrCreateTreeItem(
                categoryParent.Items,
                isHardware ? GetHardwareCategoryName() : GetGamesCategoryName(),
                backgroundGroup);

            manufacturerItem.IsExpanded = true;
            familyItem.IsExpanded = true;
            categoryParent.IsExpanded = true;

            PreviewPlaceholder.Visibility = Visibility.Collapsed;
        }
        private void UpdatePageBackgroundButtonText()
        {
            bool hasBackground = HasSelectedRecordBackground();

            AddPageBackgroundButton.Content =
                hasBackground
                    ? (currentLanguage == "pl" ? "Usuń tło podstrony" : "Remove Page Background")
                    : (currentLanguage == "pl" ? "Dodaj tło podstrony" : "Add Page Background");

            if (selectedPcItem != null)
            {
                PcAddPageBackgroundButton.Content =
                    HasSelectedRecordBackground()
                        ? (currentLanguage == "pl" ? "Usuń tło" : "Remove")
                        : (currentLanguage == "pl" ? "Tło" : "BG");
            }
        }
        private TreeViewItem GetOrCreateTreeItem(
            ItemCollection items,
            string header,
            string backgroundGroup)
        {
            foreach (object obj in items)
            {
                if (obj is TreeViewItem item &&
                    item.Header?.ToString() == header)
                {
                    return item;
                }
            }

            TreeViewItem newItem = new TreeViewItem
            {
                Header = header,
                Tag = new TreeNodeData
                {
                    NodeType = "Group",
                    BackgroundGroup = backgroundGroup
                }
            };

            items.Add(newItem);
            return newItem;
        }

        private string GetBundledControllerDescription()
        {
            string customName =
                HardwareCustomNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(customName))
            {
                return currentLanguage == "pl"
                    ? "Pad / kontroler z zestawu"
                    : "Bundled pad / controller";
            }

            return currentLanguage == "pl"
                ? $"Pad / kontroler z zestawu {customName}"
                : $"Bundled pad / controller {customName}";
        }
        private string GetBackgroundGroupForManufacturer(string manufacturer)
        {
            return manufacturer switch
            {
                "Sony" => "Sony",
                "Microsoft" => "Microsoft",
                "Nintendo" => "Nintendo",
                "SEGA" => "SEGA",
                "Atari" => "Atari",
                "Commodore" => "Commodore",
                "Amiga" => "Amiga",
                "PC" => "PC",
                _ => "Default"
            };
        }

        // =========================
        // DATABASE TREE - NOT USED ON START
        // =========================

        private void LoadConsoleDatabaseToTree()
        {
            CollectionTree.Items.Clear();

            var manufacturers = ConsolePlatformDatabase.GetManufacturers();
            var families = ConsolePlatformDatabase.GetFamilies();
            var platforms = ConsolePlatformDatabase.GetPlatforms();

            foreach (var manufacturer in manufacturers)
            {
                TreeViewItem manufacturerItem = new TreeViewItem
                {
                    Header = manufacturer.Name,
                    Tag = manufacturer.Name
                };

                var manufacturerFamilies = families
                    .Where(f => f.Manufacturer == manufacturer.Name)
                    .ToList();

                if (manufacturerFamilies.Count == 1)
                {
                    var family = manufacturerFamilies.First();

                    var familyPlatforms = platforms
                        .Where(p =>
                            p.Manufacturer == manufacturer.Name &&
                            p.Family == family.Name)
                        .ToList();

                    foreach (var platform in familyPlatforms)
                    {
                        TreeViewItem platformItem = new TreeViewItem
                        {
                            Header = platform.Name,
                            Tag = platform.BackgroundGroup
                        };

                        manufacturerItem.Items.Add(platformItem);
                    }
                }
                else
                {
                    foreach (var family in manufacturerFamilies)
                    {
                        TreeViewItem familyItem = new TreeViewItem
                        {
                            Header = family.Name,
                            Tag = family.BackgroundGroup
                        };

                        var familyPlatforms = platforms
                            .Where(p =>
                                p.Manufacturer == manufacturer.Name &&
                                p.Family == family.Name)
                            .ToList();

                        foreach (var platform in familyPlatforms)
                        {
                            TreeViewItem platformItem = new TreeViewItem
                            {
                                Header = platform.Name,
                                Tag = platform.BackgroundGroup
                            };

                            familyItem.Items.Add(platformItem);
                        }

                        manufacturerItem.Items.Add(familyItem);
                    }
                }

                if (manufacturerItem.Items.Count > 0)
                    CollectionTree.Items.Add(manufacturerItem);
            }
        }

        private void ArchiveHardware(
         Hardware hardware,
         string reason = "")
        {
            if (hardware == null)
                return;

            hardware.IsArchived = true;
            hardware.ArchiveDate = DateTime.Now;
            hardware.ArchiveReason = reason;

            collection.HardwareItems.Remove(hardware);
            collection.ArchivedHardwareItems.Add(hardware);

            RemoveHardwareFromTree(CollectionTree.Items, hardware);

            ShowDefaultRightPanel();
            RefreshStats();
        }
        private void RestoreHardware(
        Hardware hardware)
        {
            if (hardware == null)
                return;

            hardware.IsArchived = false;
            hardware.ArchiveDate = null;
            hardware.ArchiveReason = "";

            collection.ArchivedHardwareItems.Remove(hardware);

            collection.HardwareItems.Add(hardware);
        }
        private void DeleteHardwarePermanently(Hardware hardware)
        {
            if (hardware == null)
                return;

            collection.HardwareItems.Remove(hardware);
            collection.ArchivedHardwareItems.Remove(hardware);

            RemoveHardwareFromTree(CollectionTree.Items, hardware);

            SaveCollectionChanges();
        }      
        private void ArchiveGame(
        Game game,
        string reason = "")
        {
            if (game == null)
                return;

            game.IsArchived = true;
            game.ArchiveDate = DateTime.Now;
            game.ArchiveReason = reason;

            collection.Games.Remove(game);

            collection.ArchivedGames.Add(game);
        }
        private void RestoreGame(
        Game game)
        {
            if (game == null)
                return;

            game.IsArchived = false;
            game.ArchiveDate = null;
            game.ArchiveReason = "";

            collection.ArchivedGames.Remove(game);

            collection.Games.Add(game);
        }
        private void ShowRecordActionsPanel()
        {
            PreviewPlaceholder.Visibility = Visibility.Collapsed;
            AddChoicePanel.Visibility = Visibility.Collapsed;
            AddHardwareFormPanel.Visibility = Visibility.Collapsed;

            RecordActionsPanel.Visibility = Visibility.Visible;

            EnsureRestoreRecordButton();
            ApplyRecordActionsLanguage();
        }
        private void EditRecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedHardwareItem != null)
            {
                isEditingHardware = true;

                Hardware hardwareToEdit = selectedHardwareItem;
                
                editingHardwareItem = selectedHardwareItem;

                AddHardwareFormTitle.Text =
                currentLanguage == "pl"
                    ? "Edytuj sprzęt"
                    : "Edit hardware";

                AddHardwarePhotoButton.Content =
                currentLanguage == "pl"
                    ? "Edytuj zdjęcia"
                    : "Edit photos";

                currentHardwarePhotos =
                    editingHardwareItem.Photos ?? new List<CollectionPhoto>();

                if (CollectionTree.SelectedItem is TreeViewItem selectedItem)
                    editingHardwareTreeItem = selectedItem;

                ShowAddHardwareForm();
                FillHardwareForm(hardwareToEdit);

                SaveHardwareButton.Content =
                    currentLanguage == "pl"
                        ? "Zapisz zmiany"
                        : "Save changes";

                return;
            }

            if (selectedGameItem != null)
            {
                isEditingGame = true;
                editingGameItem = selectedGameItem;

                if (CollectionTree.SelectedItem is TreeViewItem selectedItem)
                    editingGameTreeItem = selectedItem;

                ShowAddGameFormForEdit(selectedGameItem);

                AddGameFormTitle.Text =
                    currentLanguage == "pl"
                        ? "Edytuj grę"
                        : "Edit game";

                SaveGameButton.Content =
                    currentLanguage == "pl"
                        ? "Zapisz zmiany"
                        : "Save changes";

                return;
            }
        }
        private void ShowAddGameFormForEdit(Game game)
        {
            PreviewPlaceholder.Visibility = Visibility.Collapsed;
            AddChoicePanel.Visibility = Visibility.Collapsed;
            AddHardwareFormPanel.Visibility = Visibility.Collapsed;
            HardwareDetailsViewPanel.Visibility = Visibility.Collapsed;
            AddServicePanel.Visibility = Visibility.Collapsed;
            CollectionTilesPanel.Visibility = Visibility.Collapsed;

            AddGameFormPanel.Visibility = Visibility.Visible;

            GamePublisherTextBox.Text = game.Publisher;

            ApplyGameFormLanguage();
            LoadGameFormDefaults();

            GameManufacturerCombo.SelectedItem = game.Manufacturer;
            LoadGameFamiliesForSelectedManufacturer();

            GameFamilyCombo.SelectedItem = game.Family;
            LoadGamePlatformsForSelectedFamily();

            GamePlatformCombo.SelectedItem = game.Platform;
            UpdateGamePlatformSpecificFields();

            if (!string.IsNullOrWhiteSpace(game.OperatingSystem))
                GameOperatingSystemCombo.SelectedItem = game.OperatingSystem;

            GameBoxConditionCombo.SelectedItem = game.BoxCondition;

            GameUserRatingCombo.SelectedItem =
            game.UserRating.HasValue
            ? game.UserRating.Value.ToString("0.0")
            : (currentLanguage == "pl" ? "Brak" : "None");

            UpdateGameRatingStars(game.UserRating ?? 0);

            GameIsFavoriteCheckBox.IsChecked = game.IsFavorite;
            GameIsCompletedCheckBox.IsChecked = game.IsCompleted;
            GameIsPlannedCheckBox.IsChecked = game.IsPlanned;

            GameBoxDetailsPanel.Visibility =
                game.HasBox
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            GameTitleTextBox.Text = game.Title;

            GameShortDescriptionTextBox.Text =
                game.ShortDescription;

            if (!string.IsNullOrWhiteSpace(game.Publisher))
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Wydawca" : "Publisher")}: {game.Publisher}";
            }

            GameReleaseTypeCombo.SelectedItem =
                TranslateGameComboValueForCurrentLanguage(game.ReleaseType);
            UpdateGameReleaseFields();

            if (!string.IsNullOrWhiteSpace(game.DigitalPlatform))
                GameDigitalPlatformCombo.SelectedItem = game.DigitalPlatform;

            GameRegionCombo.SelectedItem = game.Region;
            GameMediaTypeCombo.SelectedItem = game.MediaType;

            GameBoxTypeCombo.SelectedItem =
                TranslateGameBoxTypeForCurrentLanguage(game.BoxType);

            if (!string.IsNullOrWhiteSpace(game.Genre))
            {
                string displayGenre =
                    GenreDatabase.TranslateGenre(game.Genre, currentLanguage);

                if (!GameGenreCombo.Items.Contains(displayGenre))
                    GameGenreCombo.Items.Add(displayGenre);

                GameGenreCombo.SelectedItem = displayGenre;
            }
            else
            {
                GameGenreCombo.SelectedIndex = 0;
            }

            GameConditionCombo.SelectedItem =
                ConditionDatabase.Translate(game.Condition, currentLanguage);

            GameBoxConditionCombo.SelectedItem =
                ConditionDatabase.Translate(game.BoxCondition, currentLanguage);

            GameHasBoxCheckBox.IsChecked = game.HasBox;
            GameHasManualCheckBox.IsChecked = game.HasManual;

            GameHasAdvertisementsCheckBox.IsChecked = game.HasAdvertisements;
            GameHasMapCheckBox.IsChecked = game.HasMap;
            GameHasPosterCheckBox.IsChecked = game.HasPoster;
            GameHasSoundtrackCheckBox.IsChecked = game.HasSoundtrack;
            GameHasArtbookCheckBox.IsChecked = game.HasArtbook;
            GameHasFigureCheckBox.IsChecked = game.HasFigure;
            GameHasCertificateCheckBox.IsChecked = game.HasCertificate;
            GameHasOtherExtrasCheckBox.IsChecked = game.HasOtherExtras;

            GamePurchaseDatePicker.SelectedDate = game.PurchaseDate;

            GamePurchasePriceTextBox.Text =
                game.PurchasePrice.HasValue
                    ? game.PurchasePrice.Value.ToString("0.00")
                    : "";

            GamePurchaseCurrencyCombo.SelectedItem =
                string.IsNullOrWhiteSpace(game.PurchaseCurrency)
                    ? defaultCurrency
                    : game.PurchaseCurrency;

            GameNotesTextBox.Text = game.Notes;

            currentGamePhotos = game.Photos ?? new List<CollectionPhoto>();
            currentGameFrontCover = game.FrontCoverPhoto;
            currentGameBackCover = game.BackCoverPhoto;

            int photosCount = currentGamePhotos?.Count ?? 0;

            if (currentGameFrontCover != null)
                photosCount++;

            if (currentGameBackCover != null)
                photosCount++;

            GamePhotosButton.Content =
                currentLanguage == "pl"
                    ? $"Zdjęcia / okładki ({photosCount})"
                    : $"Photos / covers ({photosCount})";

            Dispatcher.BeginInvoke(new Action(() =>
            {
                AddGameFormPanel.ScrollToVerticalOffset(0);
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }
        private string TranslateGameBoxTypeForCurrentLanguage(string boxType)
        {
            if (currentLanguage == "pl")
            {
                return boxType switch
                {
                    "Plastic Case" => "Plastikowe pudełko",
                    "Cardboard Box" => "Kartonowe pudełko",
                    _ => boxType
                };
            }

            return boxType switch
            {
                "Plastikowe pudełko" => "Plastic Case",
                "Kartonowe pudełko" => "Cardboard Box",
                _ => boxType
            };
        }
        private string TranslateGameComboValueForCurrentLanguage(string value)
        {
            if (currentLanguage == "pl")
                return value;

            return value switch
            {
                "Nośnik fizyczny" => "Physical media",
                "Cyfrowa" => "Digital",
                "Bardzo dobra" => "Very good",
                "Dobra" => "Good",
                "Przeciętna" => "Average",
                "Słaba" => "Poor",
                "Bardzo słaba" => "Very poor",
                "Plastikowe pudełko" => "Plastic box",
                "Kartonowe pudełko" => "Cardboard box",
                "Steelbook" => "Steelbook",
                "Bardzo dobry" => "Very good",
                "Dobry" => "Good",
                "Przeciętny" => "Average",
                "Słaby" => "Poor",
                "Bardzo słaby" => "Very poor",
                _ => GenreDatabase.TranslateGenre(value, currentLanguage)
            };
        }
        private void AccessoryPhotosButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoGalleryWindow gallery =
                new PhotoGalleryWindow(
                    currentAccessoryPhotos,
                    currentLanguage);

            gallery.Owner = this;

            if (gallery.ShowDialog() != true)
                return;

            currentAccessoryPhotos = gallery.Photos;
        }
        private void FillHardwareForm(Hardware hardware)
        {
            if (hardware == null)
                return; 
            
            HardwareManufacturerCombo.SelectedItem = hardware.Manufacturer;

            LoadHardwareTypesForSelectedManufacturer();

            HardwareTypeCombo.SelectedItem = hardware.Family;

            LoadHardwarePlatformsForSelectedType();

            HardwarePlatformCombo.SelectedItem = hardware.Platform;

            LoadHardwareDetailsForSelectedPlatform();

            HardwareCustomNameTextBox.Text = hardware.CustomName;

            HardwareModelCombo.SelectedItem = hardware.Model;

            LoadRevisionsForSelectedModelVariant();

            HardwareRevisionCombo.SelectedItem = hardware.Revision;

            LoadRegionForSelectedRevision();

            HardwareBoardRevisionCombo.SelectedItem = hardware.BoardRevision;
            HardwareRegionCombo.SelectedItem = hardware.Region;
            HardwareColorCombo.SelectedItem = hardware.Color;
            HardwareEditionCombo.SelectedItem = hardware.SpecialEdition;

            HardwareSerialTextBox.Text = hardware.SerialNumber;

            HardwareConditionCombo.SelectedItem = hardware.Condition;

            HardwareHasBoxCheckBox.IsChecked = hardware.HasBox;
            HardwareBoxConditionCombo.SelectedItem = hardware.BoxCondition;
            HardwareHasManualCheckBox.IsChecked = hardware.HasManual;
            HardwareHasInsertsCheckBox.IsChecked = hardware.HasInserts;
            HardwareHasPowerSupplyCheckBox.IsChecked = hardware.HasPowerSupply;
            HardwareHasCableCheckBox.IsChecked = hardware.HasCable;
            HardwareHasControllerCheckBox.IsChecked = hardware.HasController;
            HardwareHasMemoryCardCheckBox.IsChecked = hardware.HasMemoryCard;

            HardwarePurchaseDatePicker.SelectedDate = hardware.PurchaseDate;
            HardwarePurchasePriceTextBox.Text =
                hardware.PurchasePrice.HasValue
                    ? hardware.PurchasePrice.Value.ToString("0.00")
                    : "";

            HardwarePurchaseCurrencyCombo.SelectedItem =
            string.IsNullOrWhiteSpace(hardware.PurchaseCurrency)
                ? defaultCurrency
                : hardware.PurchaseCurrency;

            HardwareNotesTextBox.Text = hardware.Notes;

            HardwareRightDetailsColumn.Visibility = Visibility.Visible;
            ToggleHardwareDetailsButton.Content =
                currentLanguage == "pl" ? "Ukryj szczegóły" : "Hide details";
        }
        private void RebaseCurrencyRates(string oldDefaultCurrency, string newDefaultCurrency)
        {
            oldDefaultCurrency = oldDefaultCurrency.Trim().ToUpper();
            newDefaultCurrency = newDefaultCurrency.Trim().ToUpper();

            if (oldDefaultCurrency == newDefaultCurrency)
                return;

            decimal newDefaultRateInOldCurrency =
                GetStoredRateForCurrency(newDefaultCurrency, oldDefaultCurrency);

            if (newDefaultRateInOldCurrency <= 0)
                return;

            foreach (string currency in SupportedCurrencies)
            {
                decimal oldRate =
                    currency == oldDefaultCurrency
                        ? 1m
                        : GetStoredRateForCurrency(currency, oldDefaultCurrency);

                decimal newRate =
                    currency == newDefaultCurrency
                        ? 1m
                        : oldRate / newDefaultRateInOldCurrency;

                CurrencyRate? existingRate =
                    collection.CurrencyRates.FirstOrDefault(r => r.Currency == currency);

                if (existingRate == null)
                {
                    collection.CurrencyRates.Add(new CurrencyRate
                    {
                        Currency = currency,
                        Rate = newRate
                    });
                }
                else
                {
                    existingRate.Rate = newRate;
                }
            }
        }

        private decimal GetStoredRateForCurrency(string currency, string currentBaseCurrency)
        {
            currency = currency.Trim().ToUpper();
            currentBaseCurrency = currentBaseCurrency.Trim().ToUpper();

            if (currency == currentBaseCurrency)
                return 1m;

            CurrencyRate? rate =
                collection.CurrencyRates.FirstOrDefault(r => r.Currency == currency);

            return rate?.Rate ?? 1m;
        }
        private void AddServiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedHardwareItem == null)
            {
                HgcMessageBox.Show(
                this,
                currentLanguage == "pl"
                    ? "Nie wybrano rekordu sprzętu."
                    : "No hardware record selected.",
                "HGC",
                HgcMessageBoxType.Information,
                currentLanguage);

                return;
            }

            PreviewPlaceholder.Visibility = Visibility.Collapsed;
            AddChoicePanel.Visibility = Visibility.Collapsed;
            AddHardwareFormPanel.Visibility = Visibility.Collapsed;
            HardwareDetailsViewPanel.Visibility = Visibility.Collapsed;

            AddServicePanel.Visibility = Visibility.Visible;

            ApplyServiceLanguage();
            LoadServiceTypes();
            RefreshServiceHistoryList();

            ServiceCostCurrencyCombo.Items.Clear();
            ServiceCostCurrencyCombo.Items.Add(defaultCurrency);
            ServiceCostCurrencyCombo.SelectedItem = defaultCurrency;

            ServiceDatePicker.SelectedDate = DateTime.Today;
            ServiceCostTextBox.Text = "";
            ServiceNotesTextBox.Text = "";
            editingServiceEntry = null;
            isEditingService = false;
        }
        private void SaveServiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedHardwareItem == null)
                return;

            ServiceEntry entry;

            if (isEditingService && editingServiceEntry != null)
            {
                entry = editingServiceEntry;
            }
            else
            {
                entry = new ServiceEntry();
                selectedHardwareItem.ServiceHistory.Add(entry);
            }

            entry.ServiceDate = ServiceDatePicker.SelectedDate ?? DateTime.Today;
            entry.ServiceType =
                ServiceTypeDatabase.Translate(ServiceTypeCombo.Text, "pl");
            entry.CustomNote = ServiceNotesTextBox.Text.Trim();
            entry.CostCurrency = ServiceCostCurrencyCombo.SelectedItem?.ToString() ?? defaultCurrency;

            if (decimal.TryParse(ServiceCostTextBox.Text, out decimal cost))
                entry.Cost = cost;
            else
                entry.Cost = null;

            AddServicePanel.Visibility = Visibility.Collapsed;

            ShowHardwareDetails(selectedHardwareItem);

            RefreshStats();

            SaveCollectionChanges();

            RefreshServiceHistoryList();

            editingServiceEntry = null;
            isEditingService = false;

            SaveServiceButton.Content =
                currentLanguage == "pl"
                    ? "Dodaj wpis"
                    : "Add entry";
        }
        private void CancelServiceButton_Click(
        object sender,
        RoutedEventArgs e)
        {
            AddServicePanel.Visibility = Visibility.Collapsed;

            if (selectedHardwareItem != null)
                ShowHardwareDetails(selectedHardwareItem);
        }
        private void ArchiveRecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameItem != null)
            {
                bool confirm = HgcMessageBox.ShowYesNo(
                    this,
                    currentLanguage == "pl"
                        ? "Czy na pewno przenieść grę do archiwum?"
                        : "Are you sure you want to move this game to the archive?",
                    "HGC",
                    HgcMessageBoxType.Question,
                    currentLanguage);

                if (!confirm)
                    return;

                selectedGameItem.IsArchived = true;
                selectedGameItem.ArchiveDate = DateTime.Now;

                MoveGameToArchive(selectedGameItem);

                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? "Gra została przeniesiona do archiwum."
                        : "Game moved to archive.",
                    "HGC",
                    HgcMessageBoxType.Information,
                    currentLanguage);

                selectedGameItem = null;

                ShowDefaultRightPanel();
                RefreshStats();
                return;
            }

            if (selectedHardwareItem == null)
                return;

            bool hardwareConfirm = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "Czy na pewno przenieść rekord do archiwum?"
                    : "Are you sure you want to move this record to the archive?",
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!hardwareConfirm)
                return;

            selectedHardwareItem.IsArchived = true;
            selectedHardwareItem.ArchiveDate = DateTime.Now;

            MoveHardwareToArchive(selectedHardwareItem);

            HgcMessageBox.Show(
                this,
                currentLanguage == "pl"
                    ? "Rekord został przeniesiony do archiwum."
                    : "Record moved to archive.",
                "HGC",
                HgcMessageBoxType.Information,
                currentLanguage);

            selectedHardwareItem = null;

            ShowDefaultRightPanel();
            RefreshStats();
        }
        
        private void CollapseTreeItems(ItemCollection items)
        {
            foreach (object item in items)
            {
                if (item is not TreeViewItem treeItem)
                    continue;

                CollapseTreeItems(treeItem.Items);
                treeItem.IsExpanded = false;
            }
        }
        private void UpdateLanguageMenuChecks()
        {
            MenuPolish.IsChecked = currentLanguage == "pl";
            MenuEnglish.IsChecked = currentLanguage == "en";
        }
        private void MoveGameToArchive(Game game)
        {
            if (game == null)
                return;

            collection.Games.Remove(game);

            currentGameFilter = GameFilterMode.All;
            currentGameGenre = "";

            if (!collection.ArchivedGames.Contains(game))
                collection.ArchivedGames.Add(game);

            RemoveGameFromTree(CollectionTree.Items, game);
            CleanupEmptyTreeBranches();
            CollapseTreeItems(CollectionTree.Items);

            BuildGamesPanel();

            ShowDefaultRightPanel();

            TreeViewItem archiveRoot = GetArchiveRoot();

            TreeViewItem archiveGamesNode =
                GetOrCreateTreeItem(
                    archiveRoot.Items,
                    GetGamesCategoryName(),
                    "Default");

            archiveGamesNode.Tag = new TreeNodeData
            {
                NodeType = "ArchiveGameCategory",
                BackgroundGroup = "Default"
            };

            TreeViewItem archivedItem = new TreeViewItem
            {
                Header = game.Title,
                Tag = new TreeNodeData
                {
                    NodeType = "GameRecord",
                    BackgroundGroup = "Default",
                    GameItem = game
                }
            };

            archiveGamesNode.Items.Add(archivedItem);

            SaveCollectionChanges();
        }       
        private void CleanupEmptyTreeBranches()
        {
            CleanupEmptyTreeBranches(CollectionTree.Items);

            if (CollectionTree.Items.Count == 0)
            {
                currentPlatformGroup = "Default";
                UpdateBackgroundForCurrentState();
                ShowDefaultRightPanel();
            }
        }

        private bool CleanupEmptyTreeBranches(ItemCollection items)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i] is not TreeViewItem item)
                    continue;

                CleanupEmptyTreeBranches(item.Items);

                if (item.Items.Count == 0 &&
                    item.Tag is TreeNodeData node &&
                    node.NodeType != "HardwareRecord" &&
                    node.NodeType != "GameRecord" &&
                    node.NodeType != "PcRecord" &&
                    node.NodeType != "AccessoryRecord" &&
                    node.NodeType != "ArchiveRoot")
                {
                    items.RemoveAt(i);
                }
            }

            return items.Count == 0;
        }
        private void MoveHardwareToArchive(Hardware hardware)
        {
            if (hardware == null)
                return;

            collection.HardwareItems.Remove(hardware);

            if (!collection.ArchivedHardwareItems.Contains(hardware))
                collection.ArchivedHardwareItems.Add(hardware);

            RemoveHardwareFromTree(CollectionTree.Items, hardware);
            CleanupEmptyTreeBranches();
            CollapseTreeItems(CollectionTree.Items);

            TreeViewItem archiveRoot = GetArchiveRoot();

            TreeViewItem archiveHardwareNode =
                GetOrCreateTreeItem(
                    archiveRoot.Items,
                    GetHardwareCategoryName(),
                    "Default");

            archiveHardwareNode.Tag = new TreeNodeData
            {
                NodeType = "ArchiveHardwareCategory",
                BackgroundGroup = "Default"
            };

            TreeViewItem archivedItem = new TreeViewItem
            {
                Header = GetHardwareDisplayName(hardware),
                Tag = new TreeNodeData
                {
                    NodeType = "HardwareRecord",
                    BackgroundGroup = "Default",
                    HardwareItem = hardware
                }
            };

            archiveHardwareNode.Items.Add(archivedItem);

            SaveCollectionChanges();
        }
        private bool RemoveHardwareFromTree(
        ItemCollection items,
        Hardware hardware)
        {
            foreach (object item in items)
            {
                if (item is not TreeViewItem treeItem)
                    continue;

                if (treeItem.Tag is TreeNodeData node &&
                    node.HardwareItem == hardware)
                {
                    items.Remove(treeItem);
                    return true;
                }

                if (RemoveHardwareFromTree(
                        treeItem.Items,
                        hardware))
                {
                    return true;
                }
            }

            return false;
        }
        private bool RemoveGameFromTree(
    ItemCollection items,
    Game game)
        {
            foreach (object item in items)
            {
                if (item is not TreeViewItem treeItem)
                    continue;

                if (treeItem.Tag is TreeNodeData node &&
                    node.GameItem == game)
                {
                    items.Remove(treeItem);
                    return true;
                }

                if (RemoveGameFromTree(
                        treeItem.Items,
                        game))
                {
                    return true;
                }
            }

            return false;
        }       
        private void RestoreRecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameItem != null && selectedGameItem.IsArchived)
            {
                bool confirm = HgcMessageBox.ShowYesNo(
                    this,
                    currentLanguage == "pl"
                        ? "Czy na pewno przywrócić grę z archiwum?"
                        : "Are you sure you want to restore this game from the archive?",
                    "HGC",
                    HgcMessageBoxType.Question,
                    currentLanguage);

                if (!confirm)
                    return;

                RestoreGameFromArchive(selectedGameItem);

                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? "Gra została przywrócona z archiwum."
                        : "Game restored from the archive.",
                    "HGC",
                    HgcMessageBoxType.Information,
                    currentLanguage);

                selectedGameItem = null;
                ShowDefaultRightPanel();
                RefreshStats();
                return;
            }

            if (selectedHardwareItem != null && selectedHardwareItem.IsArchived)
            {
                bool confirm = HgcMessageBox.ShowYesNo(
                    this,
                    currentLanguage == "pl"
                        ? "Czy na pewno przywrócić rekord z archiwum?"
                        : "Are you sure you want to restore this record from the archive?",
                    "HGC",
                    HgcMessageBoxType.Question,
                    currentLanguage);

                if (!confirm)
                    return;

                RestoreHardwareFromArchive(selectedHardwareItem);

                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? "Rekord został przywrócony z archiwum."
                        : "Record restored from the archive.",
                    "HGC",
                    HgcMessageBoxType.Information,
                    currentLanguage);

                selectedHardwareItem = null;
                ShowDefaultRightPanel();
                RefreshStats();
            }
        }

        private void RestoreGameFromArchive(Game game)
        {
            if (game == null)
                return;

            game.IsArchived = false;
            game.ArchiveDate = null;
            game.ArchiveReason = "";

            collection.ArchivedGames.Remove(game);

            if (!collection.Games.Contains(game))
                collection.Games.Add(game);

            RemoveGameFromTree(CollectionTree.Items, game);
            CleanupArchiveTreeBranchesOnly();
            AddGameRecordToTree(game);

            currentGameFilter = GameFilterMode.All;
            currentGameGenre = "";

            BuildGamesPanel();

            SaveCollectionChanges();
        }

        private void RestoreHardwareFromArchive(Hardware hardware)
        {
            if (hardware == null)
                return;

            hardware.IsArchived = false;
            hardware.ArchiveDate = null;
            hardware.ArchiveReason = "";

            collection.ArchivedHardwareItems.Remove(hardware);

            if (!collection.HardwareItems.Contains(hardware))
                collection.HardwareItems.Add(hardware);

            RemoveHardwareFromTree(CollectionTree.Items, hardware);
            CleanupArchiveTreeBranchesOnly();
            AddHardwareRecordToTree(hardware);
            SaveCollectionChanges();
        }

        private void DeleteRecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameItem != null)
            {
                if (selectedGameItem.IsArchived)
                {
                    bool archivedGameDeleteResult = HgcMessageBox.ShowYesNo(
                        this,
                        currentLanguage == "pl"
                            ? "To działanie jest nieodwracalne.\n\nCzy na pewno chcesz trwale usunąć grę z archiwum?"
                            : "This action cannot be undone.\n\nAre you sure you want to permanently delete this archived game?",
                        "HGC",
                        HgcMessageBoxType.Warning,
                        currentLanguage);

                    if (!archivedGameDeleteResult)
                        return;

                    DeleteGamePermanently(selectedGameItem);
                    CleanupArchiveTreeBranchesOnly();

                    selectedGameItem = null;

                    ShowDefaultRightPanel();
                    RefreshStats();

                    return;
                }

                HgcMessageBoxResult gameArchiveResult = HgcMessageBox.ShowYesNoCancel(
                this,
                currentLanguage == "pl"
                    ? "Usunięcie gry spowoduje trwałą utratę danych.\n\nZalecane jest przeniesienie gry do archiwum.\n\nCzy chcesz przenieść grę do archiwum?"
                    : "Deleting this game will permanently remove its data.\n\nWe recommend moving it to the archive instead.\n\nDo you want to move this game to the archive?",
                "HGC",
                HgcMessageBoxType.Warning,
                currentLanguage);

                if (gameArchiveResult == HgcMessageBoxResult.Cancel)
                    return;

                if (gameArchiveResult == HgcMessageBoxResult.Yes)
                {
                    ArchiveRecordButton_Click(sender, e);
                    return;
                }

                bool gameDeleteResult = HgcMessageBox.ShowYesNo(
    this,
    currentLanguage == "pl"
        ? "To działanie jest nieodwracalne.\n\nCzy na pewno chcesz trwale usunąć grę?"
        : "This action cannot be undone.\n\nAre you sure you want to permanently delete this game?",
    "HGC",
    HgcMessageBoxType.Warning,
    currentLanguage);

                if (!gameDeleteResult)
                    return;

                DeleteGamePermanently(selectedGameItem);
                CleanupEmptyTreeBranches();

                selectedGameItem = null;

                ShowDefaultRightPanel();
                RefreshStats();

                return;
            }

            if (selectedHardwareItem == null)
                return;

            if (selectedHardwareItem.IsArchived)
            {
                bool archivedHardwareDeleteResult = HgcMessageBox.ShowYesNo(
                 this,
                 currentLanguage == "pl"
                     ? "To działanie jest nieodwracalne.\n\nCzy na pewno chcesz trwale usunąć rekord z archiwum?"
                     : "This action cannot be undone.\n\nAre you sure you want to permanently delete this archived record?",
                 "HGC",
                 HgcMessageBoxType.Warning,
                 currentLanguage);

                if (!archivedHardwareDeleteResult)
                    return;

                collection.ArchivedHardwareItems.Remove(selectedHardwareItem);

                if (CollectionTree.SelectedItem is TreeViewItem archivedHardwareTreeItem)
                {
                    RemoveTreeViewItem(archivedHardwareTreeItem);
                    CleanupArchiveTreeBranchesOnly();
                }

                selectedHardwareItem = null;

                ShowDefaultRightPanel();
                RefreshStats();

                return;
            }

            HgcMessageBoxResult firstResult = HgcMessageBox.ShowYesNoCancel(
             this,
             currentLanguage == "pl"
                 ? "Usunięcie rekordu spowoduje trwałą utratę danych.\n\nZalecane jest przeniesienie rekordu do archiwum.\n\nCzy chcesz przenieść rekord do archiwum?"
                 : "Deleting this record will permanently remove its data.\n\nWe recommend moving it to the archive instead.\n\nDo you want to move this record to the archive?",
             "HGC",
             HgcMessageBoxType.Warning,
             currentLanguage);

            if (firstResult == HgcMessageBoxResult.Cancel)
                return;

            if (firstResult == HgcMessageBoxResult.Yes)
            {
                ArchiveRecordButton_Click(sender, e);
                return;
            }

            bool secondResult = HgcMessageBox.ShowYesNo(
            this,
            currentLanguage == "pl"
                ? "To działanie jest nieodwracalne.\n\nCzy na pewno chcesz trwale usunąć rekord?"
                : "This action cannot be undone.\n\nAre you sure you want to permanently delete this record?",
            "HGC",
            HgcMessageBoxType.Warning,
            currentLanguage);

            if (!secondResult)
                return;

            DeleteHardwarePermanently(selectedHardwareItem);
            CleanupEmptyTreeBranches();

            selectedHardwareItem = null;

            ShowDefaultRightPanel();
            RefreshStats();
        }
        private void RemoveTreeViewItem(TreeViewItem item)
        {
            ItemsControl parent =
                ItemsControl.ItemsControlFromItemContainer(item);

            parent?.Items.Remove(item);
        }
        private void CleanupArchiveTreeBranchesOnly()
        {
            TreeViewItem? archiveRoot = null;

            foreach (object item in CollectionTree.Items)
            {
                if (item is TreeViewItem treeItem &&
                    treeItem.Tag is TreeNodeData node &&
                    node.NodeType == "ArchiveRoot")
                {
                    archiveRoot = treeItem;
                    break;
                }
            }

            if (archiveRoot == null)
                return;

            for (int i = archiveRoot.Items.Count - 1; i >= 0; i--)
            {
                if (archiveRoot.Items[i] is not TreeViewItem archiveSection)
                    continue;

                if (archiveSection.Items.Count == 0)
                    archiveRoot.Items.RemoveAt(i);
            }

            bool archiveHasRecords =
                collection.ArchivedHardwareItems.Count > 0 ||
                collection.ArchivedGames.Count > 0 ||
                collection.ArchivedPcSystems.Count > 0 ||
                collection.ArchivedAccessories.Count > 0;

            if (!archiveHasRecords && archiveRoot.Items.Count == 0)
            {
                CollectionTree.Items.Remove(archiveRoot);
                archiveRootItem = null;
            }
        }
        private void AddHardwareRecordToTree(Hardware hardware)
        {
            if (hardware == null)
                return;

            string backgroundGroup = GetBackgroundGroupForManufacturer(hardware.Manufacturer);

            TreeViewItem manufacturerItem = GetOrCreateTreeItem(
                CollectionTree.Items,
                hardware.Manufacturer,
                backgroundGroup);

            manufacturerItem.Tag = new TreeNodeData
            {
                NodeType = "Manufacturer",
                BackgroundGroup = backgroundGroup,
                Manufacturer = hardware.Manufacturer
            };

            TreeViewItem familyItem = GetOrCreateTreeItem(
                manufacturerItem.Items,
                hardware.Family,
                backgroundGroup);

            familyItem.Tag = new TreeNodeData
            {
                NodeType = "Family",
                BackgroundGroup = backgroundGroup,
                Manufacturer = hardware.Manufacturer,
                Family = hardware.Family
            };

            TreeViewItem categoryParent;

            if (UsesDirectCategories(
                    hardware.Manufacturer,
                    hardware.Family,
                    hardware.Platform))
            {
                if (hardware.Platform == "GameCube" ||
                    hardware.Platform == "Panasonic Q")
                {
                    categoryParent = GetOrCreateTreeItem(
                        familyItem.Items,
                        "GameCube",
                        backgroundGroup);

                    categoryParent.Tag = new TreeNodeData
                    {
                        NodeType = "Platform",
                        BackgroundGroup = backgroundGroup,
                        Manufacturer = hardware.Manufacturer,
                        Family = hardware.Family,
                        Platform = "GameCube"
                    };
                }
                else
                {
                    categoryParent = familyItem;
                }
            }
            else
            {
                categoryParent = GetOrCreateTreeItem(
                    familyItem.Items,
                    hardware.Platform,
                    backgroundGroup);

                categoryParent.Tag = new TreeNodeData
                {
                    NodeType = "Platform",
                    BackgroundGroup = backgroundGroup,
                    Manufacturer = hardware.Manufacturer,
                    Family = hardware.Family,
                    Platform = hardware.Platform
                };
            }

            TreeViewItem hardwareCategoryItem = GetOrCreateTreeItem(
                categoryParent.Items,
                GetHardwareCategoryName(),
                backgroundGroup);

            hardwareCategoryItem.Tag = new TreeNodeData
            {
                NodeType = "Category",
                BackgroundGroup = backgroundGroup,
                Manufacturer = hardware.Manufacturer,
                Family = hardware.Family,
                Platform = hardware.Platform,
                Category = GetHardwareCategoryName()
            };

            string displayName = GetHardwareDisplayName(hardware);

            TreeViewItem hardwareItem = new TreeViewItem
            {
                Header = displayName,
                Tag = new TreeNodeData
                {
                    NodeType = "HardwareRecord",
                    BackgroundGroup = backgroundGroup,
                    Manufacturer = hardware.Manufacturer,
                    Family = hardware.Family,
                    Platform = hardware.Platform,
                    Category = GetHardwareCategoryName(),
                    HardwareItem = hardware
                }
            };

            hardwareCategoryItem.Items.Add(hardwareItem);
            PreviewPlaceholder.Visibility = Visibility.Collapsed;

            BuildGamesPanel();
        }
        private void LoadAccessoryPhotosPreview(Accessory accessory)
        {
            AccessoryPhotosPanel.Children.Clear();

            List<CollectionPhoto> photos =
                accessory.Photos ?? new List<CollectionPhoto>();

            if (photos.Count > 0 && !photos.Any(x => x.IsMain))
                photos[0].IsMain = true;

            AccessoryNoPhotoPanel.Visibility =
                    photos.Count == 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            foreach (CollectionPhoto photo in photos)
            {
                if (photo == null ||
                    string.IsNullOrWhiteSpace(photo.OriginalPath) ||
                    !File.Exists(photo.OriginalPath))
                    continue;

                var previewSize = GetAccessoryPhotoPreviewSize(photo);

                double photoWidth = previewSize.Width;
                double photoHeight = previewSize.Height;

                Image image = new Image
                {
                    Width = photoWidth,
                    Height = photoHeight,
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(0),
                    Source = LoadImageWithoutLock(photo.OriginalPath, 320),
                    Cursor = Cursors.Hand
                };

                Border imageContainer = new Border
                {
                    Width = photoWidth,
                    Height = photoHeight,
                    CornerRadius = new CornerRadius(8),
                    ClipToBounds = true,
                    Margin = new Thickness(10, 0, 10, 0),
                    Child = image
                };

                Grid imageGrid = new Grid();

                imageGrid.Children.Add(imageContainer);

                if (photo.IsMain)
                {
                    Border mainBadge = new Border
                    {
                        Background = Brushes.Gold,
                        CornerRadius = new CornerRadius(4),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(14, 4, 14, 0),
                        Padding = new Thickness(6, 2, 6, 2)
                    };

                    mainBadge.Child = new TextBlock
                    {
                        Text = "★",
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.Black
                    };

                    imageGrid.Children.Add(mainBadge);
                }

                image.MouseLeftButtonUp += (s, e) =>
                {
                    PhotoPreviewWindow preview =
                        new PhotoPreviewWindow(
                            photo.OriginalPath,
                            currentLanguage);

                    preview.Owner = this;
                    preview.ShowDialog();
                };

                Border photoBorder = new Border
                {
                    CornerRadius = new CornerRadius(8),
                    BorderThickness = new Thickness(photo.IsMain ? 2 : 1),
                    BorderBrush = photo.IsMain
                        ? Brushes.Gold
                        : (Brush)FindResource("AppBorderBrush"),
                    Child = imageGrid,
                    Margin = new Thickness(0, 0, 10, 0),
                    Padding = new Thickness(0),
                    ClipToBounds = false
                };

                AccessoryPhotosPanel.Children.Add(photoBorder);
            }

        }
        private string GetGamesCollectionTitle()
        {
            return currentGameFilter switch
            {
                GameFilterMode.All =>
                    currentLanguage == "pl"
                        ? "Gry • Wszystkie"
                        : "Games • All",

                GameFilterMode.Favorites =>
                    currentLanguage == "pl"
                        ? "Gry • Ulubione"
                        : "Games • Favorites",

                GameFilterMode.Completed =>
                    currentLanguage == "pl"
                        ? "Gry • Ukończone"
                        : "Games • Completed",

                GameFilterMode.Uncompleted =>
                    currentLanguage == "pl"
                        ? "Gry • Nieukończone"
                        : "Games • Uncompleted",

                GameFilterMode.Planned =>
                    currentLanguage == "pl"
                        ? "Gry • Planowane"
                        : "Games • Planned",

                GameFilterMode.Genre =>
                    $"{(currentLanguage == "pl" ? "Gry" : "Games")} • {currentGameGenre}",

                _ =>
                    currentLanguage == "pl"
                        ? "Gry"
                        : "Games"
            };
        }
        private (double Width, double Height) GetAccessoryPhotoPreviewSize(CollectionPhoto photo)
        {
            const double targetHeight = 210;
            const double minWidth = 140;
            const double maxWidth = 320;

            try
            {
                BitmapImage bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.UriSource = new Uri(photo.OriginalPath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                double ratio = (double)bitmap.PixelWidth / bitmap.PixelHeight;

                double width = targetHeight * ratio;

                width = Math.Max(minWidth, width);
                width = Math.Min(maxWidth, width);

                return (width, targetHeight);
            }
            catch
            {
                return (210, targetHeight);
            }
        }
        private void HideRightPanels()
        {
            PreviewPlaceholder.Visibility = Visibility.Collapsed;

            AddChoicePanel.Visibility = Visibility.Collapsed;
            AddHardwareSubChoicePanel.Visibility = Visibility.Collapsed;
            AccessoryDetailsPanel.Visibility = Visibility.Collapsed;
            AddHardwareFormPanel.Visibility = Visibility.Collapsed;
            AddGameFormPanel.Visibility = Visibility.Collapsed;
            AddPcFormPanel.Visibility = Visibility.Collapsed;

            HardwareDetailsViewPanel.Visibility = Visibility.Collapsed;
            PcDetailsPanel.Visibility = Visibility.Collapsed;

            AddServicePanel.Visibility = Visibility.Collapsed;
            CollectionTilesPanel.Visibility = Visibility.Collapsed;

            if (RestoreRecordButton != null)
                RestoreRecordButton.Visibility = Visibility.Collapsed;
        }
        private void ToggleGameDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            bool show = GameRightDetailsColumn.Visibility != Visibility.Visible;

            GameRightDetailsColumn.Visibility = show
                ? Visibility.Visible
                : Visibility.Collapsed;

            ToggleGameDetailsButton.Content = show
                ? (currentLanguage == "pl" ? "Ukryj szczegóły" : "Hide details")
                : (currentLanguage == "pl" ? "Dodaj szczegóły" : "Add details");
        }
        private TreeViewItem? FindGameTreeItem(
        ItemCollection items,
        Game game)
        {
            foreach (object item in items)
            {
                if (item is not TreeViewItem treeItem)
                    continue;

                if (ReferenceEquals(treeItem.Tag, game))
                    return treeItem;

                TreeViewItem? found =
                    FindGameTreeItem(treeItem.Items, game);

                if (found != null)
                    return found;
            }

            return null;
        }
        private void ShowCollectionTiles(
             TreeNodeData node,
             bool showRecentlyAdded = false)
        {
            lastCollectionTilesNode = node;
            lastCollectionTilesShowRecentlyAdded = showRecentlyAdded;

            HideRightPanels();

            CollectionTilesPanel.Visibility = Visibility.Visible;
            CollectionTilesSectionsPanel.Children.Clear();
            currentCollectionTilesWrap = null;

            if (showRecentlyAdded)
                UpdateRecentlyAddedPanel();
            else
                RecentlyAddedBorder.Visibility = Visibility.Collapsed;

            CollectionTilesTitle.Text = node.NodeType switch
            {
                "Manufacturer" => node.Manufacturer,
                "Family" => node.Family,
                "Platform" => node.Platform,

                "Category" => !string.IsNullOrWhiteSpace(node.Platform)
                    ? node.Platform
                    : node.Category,

                _ => ""

            };

            if (IsGameFilterActive())
            {
                CollectionTilesTitle.Text = GetGamesCollectionTitle();
            }

            CollectionTilesTitle.Visibility =
            string.IsNullOrWhiteSpace(CollectionTilesTitle.Text)
                ? Visibility.Collapsed
                : Visibility.Visible;

            var allHardwareItems = collection.HardwareItems
                .Where(x =>
                    (string.IsNullOrWhiteSpace(node.Manufacturer) || x.Manufacturer == node.Manufacturer) &&
                    (string.IsNullOrWhiteSpace(node.Family) || x.Family == node.Family) &&
                    (string.IsNullOrWhiteSpace(node.Platform) || x.Platform == node.Platform))
                .ToList();

            var gameItems = IsGameFilterActive()
                ? GetFilteredGames()
                : collection.Games
                    .Where(x =>
                        (string.IsNullOrWhiteSpace(node.Manufacturer) || x.Manufacturer == node.Manufacturer) &&
                        (string.IsNullOrWhiteSpace(node.Family) || x.Family == node.Family) &&
                        (string.IsNullOrWhiteSpace(node.Platform) || x.Platform == node.Platform))
                    .ToList();

            gameItems = SortGameItems(gameItems);

            if (node.NodeType == "HardwareAccessoriesOnly")
            {
                gameItems.Clear();

                CollectionTilesTitle.Text =
                    currentLanguage == "pl"
                        ? "Sprzęt i akcesoria"
                        : "Hardware and accessories";
            }

            var newAccessoryItems = collection.Accessories
                .Where(x =>
                    (string.IsNullOrWhiteSpace(node.Manufacturer) || x.AssignedManufacturer == node.Manufacturer) &&
                    (string.IsNullOrWhiteSpace(node.Family) || x.AssignedFamily == node.Family) &&
                    (string.IsNullOrWhiteSpace(node.Platform) || x.AssignedPlatform == node.Platform))
                .ToList();

            if (node.NodeType == "AccessoriesRoot" ||
                node.NodeType == "AccessoryRoot")
            {
                newAccessoryItems = newAccessoryItems
                    .Where(x =>
                        string.IsNullOrWhiteSpace(x.AssignedManufacturer) &&
                        string.IsNullOrWhiteSpace(x.AssignedFamily) &&
                        string.IsNullOrWhiteSpace(x.AssignedPlatform))
                    .ToList();
            }

            if (node.NodeType == "AccessoryType")
            {
                newAccessoryItems = newAccessoryItems
                    .Where(x =>
                        string.IsNullOrWhiteSpace(x.AssignedManufacturer) &&
                        string.IsNullOrWhiteSpace(x.AssignedFamily) &&
                        string.IsNullOrWhiteSpace(x.AssignedPlatform) &&
                        x.AccessoryType == node.Category)
                    .ToList();
            }

            newAccessoryItems = SortAccessoryItems(newAccessoryItems);

            if (node.Category == GetGamesCategoryName())
            {
                allHardwareItems.Clear();
                newAccessoryItems.Clear();
            }

            if (node.Category == GetHardwareCategoryName())
                gameItems.Clear();

            var accessoryItems = SortHardwareItems(
                allHardwareItems.Where(IsAccessoryHardware));

            var hardwareItems = SortHardwareItems(
                allHardwareItems.Where(x => !IsAccessoryHardware(x)));

            if (
             node.NodeType == "AccessoriesRoot" ||
             node.NodeType == "AccessoryRoot" ||
             (node.NodeType == "Category" &&
              (node.Category == "Akcesoria" || node.Category == "Accessories") &&
              string.IsNullOrWhiteSpace(node.Manufacturer) &&
              string.IsNullOrWhiteSpace(node.Family) &&
              string.IsNullOrWhiteSpace(node.Platform))
            )
            {
                accessoryItems.Clear();

                newAccessoryItems = newAccessoryItems
                    .Where(x =>
                        string.IsNullOrWhiteSpace(x.AssignedManufacturer) &&
                        string.IsNullOrWhiteSpace(x.AssignedFamily) &&
                        string.IsNullOrWhiteSpace(x.AssignedPlatform))
                    .ToList();
            }

            bool isDefaultCollectionView = node.NodeType == "Collection";

            bool isPcRootNode =
                node.Manufacturer == "PC" &&
                string.IsNullOrWhiteSpace(node.Family) &&
                string.IsNullOrWhiteSpace(node.Platform) &&
                string.IsNullOrWhiteSpace(node.Category);

            var pcItems =
                isDefaultCollectionView || isPcRootNode
                    ? SortPcItems(collection.PcSystems)
                    : new List<PcSystem>();

            if (IsGameFilterActive())
            {
                hardwareItems.Clear();
                accessoryItems.Clear();
                newAccessoryItems.Clear();
                pcItems.Clear();
            }

            bool isAccessoryNode =
                node.NodeType == "AccessoryRoot" ||
                node.NodeType == "AccessoriesRoot" ||
                node.NodeType == "AccessoryCategory" ||
                node.NodeType == "AccessoryType" ||
                node.NodeType == "AccessoryPlatform" ||
                node.NodeType == "AccessoryManufacturer" ||
                node.NodeType == "AccessoryFamily" ||
                node.Category == "Akcesoria" ||
                node.Category == "Accessories";

            if (isAccessoryNode)
            {
                hardwareItems.Clear();
                pcItems.Clear();
                gameItems.Clear();
            }

            int totalVisibleItems =
                hardwareItems.Count +
                pcItems.Count +
                accessoryItems.Count +
                newAccessoryItems.Count +
                gameItems.Count;

            BuildCollectionPagination(totalVisibleItems);

            int pageLimit = collectionItemsPerPage;
            bool hasAdditionalPages = totalVisibleItems > collectionItemsPerPage;

            bool isNoLimitCategoryNode =
                node.NodeType == "HardwareAccessoriesOnly" ||
                node.Category == GetHardwareCategoryName() ||
                node.Category == GetGamesCategoryName() ||
                node.Category == "Akcesoria" ||
                node.Category == "Accessories";

            int pageHardwareLimit = isNoLimitCategoryNode
                ? int.MaxValue
                : GetCollectionCategoryTileLimit();

            int pageAccessoriesLimit = isNoLimitCategoryNode
                ? int.MaxValue
                : GetCollectionCategoryTileLimit();

            List<object> hardwarePageItems = new();
            hardwarePageItems.AddRange(hardwareItems.Cast<object>());
            hardwarePageItems.AddRange(pcItems.Cast<object>());

            hardwarePageItems = collectionSortMode == "ZA"
                ? hardwarePageItems
                    .OrderByDescending(x => x switch
                    {
                        Hardware hardware => GetHardwareDisplayName(hardware),
                        PcSystem pc => pc.Name,
                        _ => ""
                    })
                    .ToList()
                : hardwarePageItems
                    .OrderBy(x => x switch
                    {
                        Hardware hardware => GetHardwareDisplayName(hardware),
                        PcSystem pc => pc.Name,
                        _ => ""
                    })
                    .ToList();

            List<object> accessoriesPageItems = new();
            accessoriesPageItems.AddRange(accessoryItems.Cast<object>());
            accessoriesPageItems.AddRange(newAccessoryItems.Cast<object>());

            int hardwareIndex = 0;
            int accessoriesIndex = 0;
            int gamesIndex = 0;

            for (int page = 1; page <= collectionCurrentPage; page++)
            {
                int pageAdded = 0;
                bool drawThisPage = page == collectionCurrentPage;

                foreach (string category in collectionCategoryOrder.Split(','))
                {
                    if (pageAdded >= pageLimit)
                        break;

                    switch (category)
                    {
                        case "Hardware":
                            {
                                if (!showHardwareCategory || hardwarePageItems.Count == 0)
                                    break;

                                int available = pageLimit - pageAdded;
                                int limit = pageHardwareLimit;

                                int take = Math.Min(
                                    Math.Min(limit, available),
                                    hardwarePageItems.Count - hardwareIndex);

                                if (take <= 0)
                                    break;

                                if (drawThisPage)
                                    AddTilesSectionTitle(currentLanguage == "pl" ? "Sprzęt" : "Hardware");

                                for (int i = 0; i < take; i++)
                                {
                                    object item = hardwarePageItems[hardwareIndex + i];

                                    if (drawThisPage)
                                    {
                                        if (item is Hardware hardware)
                                            AddHardwareTile(hardware);
                                        else if (item is PcSystem pc)
                                            AddPcTile(pc);
                                    }
                                }

                                hardwareIndex += take;
                                pageAdded += take;
                                break;
                            }

                        case "Accessories":
                            {
                                if (!showAccessoriesCategory || accessoriesPageItems.Count == 0)
                                    break;

                                int available = pageLimit - pageAdded;
                                int limit = pageAccessoriesLimit;

                                int take = Math.Min(
                                    Math.Min(limit, available),
                                    accessoriesPageItems.Count - accessoriesIndex);

                                if (take <= 0)
                                    break;

                                if (drawThisPage)
                                    AddTilesSectionTitle(currentLanguage == "pl" ? "Akcesoria" : "Accessories");

                                for (int i = 0; i < take; i++)
                                {
                                    object item = accessoriesPageItems[accessoriesIndex + i];

                                    if (drawThisPage)
                                    {
                                        if (item is Hardware accessoryHardware)
                                            AddHardwareTile(accessoryHardware);
                                        else if (item is Accessory accessory)
                                            AddAccessoryTile(accessory);
                                    }
                                }

                                accessoriesIndex += take;
                                pageAdded += take;
                                break;
                            }

                        case "Games":
                            {
                                if (!showGamesCategory || gameItems.Count == 0)
                                    break;

                                int available = pageLimit - pageAdded;

                                int take = Math.Min(
                                    available,
                                    gameItems.Count - gamesIndex);

                                if (take <= 0)
                                    break;

                                if (drawThisPage)
                                {
                                    AddTilesSectionTitle(
                                        currentLanguage == "pl"
                                            ? $"Gry ({gameItems.Count})"
                                            : $"Games ({gameItems.Count})");
                                }

                                for (int i = 0; i < take; i++)
                                {
                                    if (drawThisPage)
                                        AddGameTile(gameItems[gamesIndex + i]);
                                }

                                gamesIndex += take;
                                pageAdded += take;
                                break;
                            }
                    }
                }

                if (!hasAdditionalPages)
                    break;
            }
        }
        private void ClearGamesTreeSelection()
        {
            foreach (object item in GamesTree.Items)
            {
                if (item is TreeViewItem treeItem)
                    ClearTreeViewItemSelection(treeItem);
            }
        }

        private void ClearTreeViewItemSelection(TreeViewItem item)
        {
            item.IsSelected = false;

            foreach (object child in item.Items)
            {
                if (child is TreeViewItem childItem)
                    ClearTreeViewItemSelection(childItem);
            }
        }
        private void SetGameFilter(GameFilterMode filter, string genre = "")
        {
            currentPlatformGroup = "Default";
            UpdateBackgroundForCurrentState(); 
            
            isGamesViewActive = true;

            if (CollectionTree.SelectedItem is TreeViewItem selectedCollectionItem)
                selectedCollectionItem.IsSelected = false;

            currentGameFilter = filter;
            currentGameGenre = genre;

            switch (filter)
            {
                case GameFilterMode.All:
                    currentGamesTitle =
                        currentLanguage == "pl"
                            ? "Gry • Wszystkie"
                            : "Games • All";
                    break;

                case GameFilterMode.Favorites:
                    currentGamesTitle =
                        currentLanguage == "pl"
                            ? "Gry • Ulubione"
                            : "Games • Favorites";
                    break;

                case GameFilterMode.Completed:
                    currentGamesTitle =
                        currentLanguage == "pl"
                            ? "Gry • Ukończone"
                            : "Games • Completed";
                    break;

                case GameFilterMode.Uncompleted:
                    currentGamesTitle =
                        currentLanguage == "pl"
                            ? "Gry • Nieukończone"
                            : "Games • Uncompleted";
                    break;

                case GameFilterMode.Planned:
                    currentGamesTitle =
                        currentLanguage == "pl"
                            ? "Gry • Planowane"
                            : "Games • Planned";
                    break;

                case GameFilterMode.Genre:
                    currentGamesTitle =
                        $"{(currentLanguage == "pl" ? "Gry" : "Games")} • {genre}";
                    break;
            }

            collectionCurrentPage = 1;

            UpdateGamesCollectionTitle();

            ShowCollectionTiles(
                new TreeNodeData
                {
                    NodeType = "Category",
                    Category = currentLanguage == "pl" ? "Gry" : "Games"
                },
                filter == GameFilterMode.All);
        }
        private bool IsGameFilterActive()
        {
            return isGamesViewActive;
        }
        private void ShowAccessoryDetails(Accessory accessory)
        {
            GameShortDescriptionBorder.Visibility = Visibility.Collapsed;
            GameShortDescriptionText.Text = "";

            if (accessory == null)
                return;

            HideRightPanels();

            AccessoryDetailsPanel.Visibility = Visibility.Visible;

            selectedAccessoryItem = accessory;

            AccessoryDetailsTitle.Text = GetAccessoryDisplayName(accessory);

            ApplyAccessoryCardOpacity(accessory);

            AccessoryCardOpacityButton.Content =
                $"{(currentLanguage == "pl" ? "Przezroczystość" : "Opacity")} {accessory.CardOpacity}% ▼";

            LoadAccessoryPhotosPreview(accessory);

            bool isArchivedAccessory =
                accessory.IsArchived ||
                collection.ArchivedAccessories.Any(x => x.Id == accessory.Id);

            ArchiveAccessoryButton.Content =
                isArchivedAccessory
                    ? (currentLanguage == "pl" ? "Przywróć" : "Restore")
                    : (currentLanguage == "pl" ? "Archiwizuj" : "Archive");

            AccessoryDetailsContentPanel.Children.Clear();
            AccessoryExtraDetailsPanel.Children.Clear();

            AddAccessoryDetailRow(currentLanguage == "pl" ? "Typ" : "Type", accessory.AccessoryType);
            AddAccessoryDetailRow(currentLanguage == "pl" ? "Producent" : "Manufacturer", accessory.Manufacturer);
            AddAccessoryDetailRow(currentLanguage == "pl" ? "Model" : "Model", accessory.Model);
            AddAccessoryDetailRow(currentLanguage == "pl" ? "Przypisany producent / grupa" : "Assigned manufacturer / group", accessory.AssignedManufacturer);
            AddAccessoryDetailRow(currentLanguage == "pl" ? "Rodzina" : "Family", accessory.AssignedFamily);
            AddAccessoryDetailRow(currentLanguage == "pl" ? "Platforma" : "Platform", accessory.AssignedPlatform);
            AddAccessoryDetailRow(
                currentLanguage == "pl" ? "Stan" : "Condition",
                ConditionDatabase.Translate(accessory.Condition, currentLanguage));
            AddAccessoryDetailRow(currentLanguage == "pl" ? "Numer seryjny" : "Serial number", accessory.SerialNumber);
            AddAccessoryDetailRow(currentLanguage == "pl" ? "Data zakupu" : "Purchase date", accessory.PurchaseDate?.ToShortDateString());

            AddAccessoryDetailRow(
                currentLanguage == "pl" ? "Cena zakupu" : "Purchase price",
                accessory.PurchasePrice > 0
                    ? $"{accessory.PurchasePrice:0.00} {accessory.PurchaseCurrency}"
                    : "");

            AddAccessoryExtraDetailRow(currentLanguage == "pl" ? "Producent pada" : "Controller manufacturer", accessory.ControllerManufacturer);
            AddAccessoryExtraDetailRow(currentLanguage == "pl" ? "Model pada" : "Controller model", accessory.ControllerModel);
            AddAccessoryExtraDetailRow(currentLanguage == "pl" ? "Przekątna" : "Screen size", accessory.DisplaySize);
            AddAccessoryExtraDetailRow(currentLanguage == "pl" ? "Rozdzielczość" : "Resolution", accessory.DisplayResolution);
            AddAccessoryExtraDetailRow(currentLanguage == "pl" ? "Typ matrycy" : "Panel type", accessory.DisplayPanelType);
            AddAccessoryExtraDetailRow(currentLanguage == "pl" ? "Podświetlenie" : "Backlight", accessory.DisplayBacklightType);
            AddAccessoryExtraDetailRow(currentLanguage == "pl" ? "Odświeżanie" : "Refresh rate", accessory.RefreshRate);
            AddAccessoryExtraDetailRow(currentLanguage == "pl" ? "Format obrazu" : "Aspect ratio", accessory.AspectRatio);
            AddAccessoryExtraDetailRow(currentLanguage == "pl" ? "Czas reakcji" : "Response time", accessory.ResponseTime);
            AddAccessoryExtraDetailRow(currentLanguage == "pl" ? "Synchronizacja" : "Sync technology", accessory.SyncTechnology);

            AddAccessoryExtraDetailRow(
                "HDR",
                accessory.HdrSupport
                    ? (currentLanguage == "pl" ? "Tak" : "Yes")
                    : "");

            AddAccessoryExtraDetailRow(
                "Smart TV",
                accessory.SmartTv
                    ? (currentLanguage == "pl" ? "Tak" : "Yes")
                    : "");

            AddAccessoryExtraDetailRow(currentLanguage == "pl" ? "System TV" : "TV system", accessory.TvSystem);

            AddAccessoryExtraDetailRow(
                currentLanguage == "pl" ? "Tuner TV" : "TV tuner",
                accessory.TvTuner
                    ? (currentLanguage == "pl" ? "Tak" : "Yes")
                    : "");

            AddAccessoryExtraDetailRow(currentLanguage == "pl" ? "Technologia projekcji" : "Projection technology", accessory.ProjectionTechnology);
            AddAccessoryExtraDetailRow(currentLanguage == "pl" ? "Jasność ANSI" : "ANSI brightness", accessory.AnsiLumens);
            AddAccessoryExtraDetailRow(currentLanguage == "pl" ? "Żywotność lampy" : "Lamp life", accessory.LampLife);

            if (!string.IsNullOrWhiteSpace(accessory.Notes))
            {
                AddAccessoryExtraDetailRow(currentLanguage == "pl" ? "Notatki" : "Notes", accessory.Notes);
            }

            AccessoryDetailsContentBorder.Visibility =
                AccessoryDetailsContentPanel.Children.Count > 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            AccessoryExtraDetailsBorder.Visibility =
                AccessoryExtraDetailsPanel.Children.Count > 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
        private void BuildDefaultCurrencyMenu()
        {
            MenuDefaultCurrency.Items.Clear();

            foreach (string currency in SupportedCurrencies)
            {
                MenuItem item = new MenuItem
                {
                    Header = currency,
                    Tag = currency,
                    IsCheckable = true,
                    IsChecked = currency == defaultCurrency
                };

                item.Click += DefaultCurrencyMenu_Click;

                MenuDefaultCurrency.Items.Add(item);
            }
        }
        private void DefaultCurrencyMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem clickedItem)
                return;

            if (clickedItem.Tag is not string currency)
                return;

            string oldDefaultCurrency = defaultCurrency;
            string newDefaultCurrency = currency;

            if (oldDefaultCurrency != newDefaultCurrency)
                RebaseCurrencyRates(oldDefaultCurrency, newDefaultCurrency);

            defaultCurrency = newDefaultCurrency;

            settings.DefaultCurrency = defaultCurrency;
            SaveSettingsChanges();

            foreach (object item in MenuDefaultCurrency.Items)
            {
                if (item is MenuItem menuItem)
                    menuItem.IsChecked = false;
            }

            clickedItem.IsChecked = true;

            GamePurchaseCurrencyCombo.SelectedItem = defaultCurrency;
            HardwarePurchaseCurrencyCombo.SelectedItem = defaultCurrency;
            PcPurchaseCurrencyCombo.SelectedItem = defaultCurrency;
            AddAccessoryPurchaseCurrencyCombo.SelectedItem = defaultCurrency;
        }
        private void AddAccessoryDetailRow(string label, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            AccessoryDetailsContentPanel.Children.Add(new TextBlock
            {
                Text = $"{label}: {value}",
                Margin = new Thickness(0, 0, 0, 8),
                TextWrapping = TextWrapping.Wrap
            });
        }

        private void AddAccessoryExtraDetailRow(string label, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            AccessoryExtraDetailsPanel.Children.Add(new TextBlock
            {
                Text = $"{label}: {value}",
                Margin = new Thickness(0, 0, 0, 8),
                TextWrapping = TextWrapping.Wrap
            });
        }
        private void AddAccessoryTile(Accessory accessory)
        {
            double tileWidth = GetCollectionTileWidth();
            double tileHeight = GetCollectionHardwareTileHeight();
            double imageSize = GetCollectionTileImageSize();
            double imageHeight = GetCollectionHardwareImageHeight();

            Button tile = new Button
            {
                Width = tileWidth,
                Height = tileHeight,
                Margin = new Thickness(0, 0, 16, 12),
                Tag = accessory
            };

            tile.Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 8,
                ShadowDepth = 5,
                Opacity = 0.45
            };

            StackPanel panel = new StackPanel();

            CollectionPhoto mainPhoto =
                accessory.Photos?.FirstOrDefault(x => x.IsMain)
                ?? accessory.Photos?.FirstOrDefault();

            ImageSource tileImageSource;
            double tileImageOpacity = 0.9;

            if (mainPhoto != null &&
                File.Exists(mainPhoto.OriginalPath))
            {
                tileImageSource = LoadImageWithoutLock(mainPhoto.OriginalPath, 260);
            }
            else
            {
                tileImageSource = new BitmapImage(
                    new Uri("pack://application:,,,/Assets/Icons/nophoto.png"));

                tileImageOpacity = 0.55;
            }

            Border imageContainer = new Border
            {
                Width = imageSize,
                Height = imageHeight,
                CornerRadius = new CornerRadius(7),
                Margin = new Thickness(0, 0, 0, 6),
                HorizontalAlignment = HorizontalAlignment.Center,
                Opacity = tileImageOpacity,
                Background = new ImageBrush
                {
                    ImageSource = tileImageSource,
                    Stretch = Stretch.UniformToFill
                },
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 10,
                    ShadowDepth = 6,
                    Opacity = 0.45
                }
            };

            TextBlock title = new TextBlock
            {
                Text = GetTileDisplayName(GetAccessoryDisplayName(accessory)),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                TextTrimming = TextTrimming.CharacterEllipsis,
                MaxHeight = 40,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)FindResource("ButtonTextBrush"),
                Margin = new Thickness(4, 0, 4, 0),
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 4,
                    ShadowDepth = 1,
                    Opacity = 0.45
                }
            };

            panel.Children.Add(imageContainer);
            panel.Children.Add(title);

            tile.Content = panel;
            tile.Click += AccessoryTile_Click;

            currentCollectionTilesWrap?.Children.Add(tile);
        }
        private void LoadDisplayPanelTypes()
        {
            string[] panelTypes =
            {
                "CRT",
                "TN",
                "IPS",
                "VA",
                "OLED",
                "QD-OLED",
                "Plazma"
            };

            AddAccessoryDisplayPanelTypeCombo.Items.Clear();
            AddAccessoryTvPanelTypeCombo.Items.Clear();

            AddAccessoryDisplayPanelTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            AddAccessoryTvPanelTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            foreach (string panel in panelTypes)
            {
                AddAccessoryDisplayPanelTypeCombo.Items.Add(panel);
                AddAccessoryTvPanelTypeCombo.Items.Add(panel);
            }

            AddAccessoryDisplayPanelTypeCombo.SelectedIndex = 0;
            AddAccessoryTvPanelTypeCombo.SelectedIndex = 0;
        }
        private void LoadDisplayBacklightTypes()
        {
            string[] backlights = currentLanguage == "pl"
            ? new[]
            {
                "Brak",
                "CCFL",
                "LED",
                "Mini LED"
            }
            : new[]
            {
                "None",
                "CCFL",
                "LED",
                "Mini LED"
            };

            AddAccessoryDisplayBacklightTypeCombo.Items.Clear();
            AddAccessoryTvBacklightTypeCombo.Items.Clear();

            AddAccessoryDisplayBacklightTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            AddAccessoryTvBacklightTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            foreach (string item in backlights)
            {
                AddAccessoryDisplayBacklightTypeCombo.Items.Add(item);
                AddAccessoryTvBacklightTypeCombo.Items.Add(item);
            }

            AddAccessoryDisplayBacklightTypeCombo.SelectedIndex = 0;
            AddAccessoryTvBacklightTypeCombo.SelectedIndex = 0;
        }
        private void PcPhotoGalleryButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPcItem == null)
                return;

            selectedPcItem.Photos ??= new List<CollectionPhoto>();

            PhotoGalleryWindow galleryWindow = new PhotoGalleryWindow(
                selectedPcItem.Photos,
                currentLanguage);

            galleryWindow.Owner = this;

            if (galleryWindow.ShowDialog() == true)
            {
                selectedPcItem.Photos = galleryWindow.Photos;

                ShowPcDetails(selectedPcItem);
                ShowDefaultRightPanel();
            }
        }
        private void PcFormPhotoGalleryButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditingPc && editingPcItem != null)
            {
                PhotoGalleryWindow window = new PhotoGalleryWindow(
                    editingPcItem.Photos,
                    currentLanguage)
                {
                    Owner = this
                };

                window.ShowDialog();
                return;
            }

            if (currentPcPhotos == null)
                currentPcPhotos = new List<CollectionPhoto>();

            PhotoGalleryWindow addWindow = new PhotoGalleryWindow(
                currentPcPhotos,
                currentLanguage)
            {
                Owner = this
            };

            addWindow.ShowDialog();
        }
        private void AccessoryTile_Click(
        object sender,
        RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            if (button.Tag is not Accessory accessory)
                return;

            selectedAccessoryItem = accessory;

            ShowAccessoryDetails(accessory);
        }        
        private void HomePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddHardwareFormPanel.Visibility == Visibility.Visible)
            {
                bool cancel = HgcMessageBox.ShowYesNo(
                    this,
                    currentLanguage == "pl"
                        ? "Zamknąć formularz sprzętu?\n\nNiezapisane zmiany zostaną utracone."
                        : "Close the hardware form?\n\nUnsaved changes will be lost.",
                    "HGC",
                    HgcMessageBoxType.Question,
                    currentLanguage);

                if (!cancel)
                    return;

                CloseAddHardwareForm();
            }

            if (AddGameFormPanel.Visibility == Visibility.Visible)
            {
                bool cancel = HgcMessageBox.ShowYesNo(
                    this,
                    currentLanguage == "pl"
                        ? "Zamknąć formularz gry?\n\nNiezapisane zmiany zostaną utracone."
                        : "Close the game form?\n\nUnsaved changes will be lost.",
                    "HGC",
                    HgcMessageBoxType.Question,
                    currentLanguage);

                if (!cancel)
                    return;

                ClearGameForm();
            }

            if (AddAccessoryFormPanel.Visibility == Visibility.Visible)
            {
                bool cancel = HgcMessageBox.ShowYesNo(
                    this,
                    currentLanguage == "pl"
                        ? "Zamknąć formularz akcesorium?\n\nNiezapisane zmiany zostaną utracone."
                        : "Close the accessory form?\n\nUnsaved changes will be lost.",
                    "HGC",
                    HgcMessageBoxType.Question,
                    currentLanguage);

                if (!cancel)
                    return;

                AddAccessoryFormPanel.Visibility = Visibility.Collapsed;
            }

            if (AddPcFormPanel.Visibility == Visibility.Visible)
            {
                bool cancel = HgcMessageBox.ShowYesNo(
                    this,
                    currentLanguage == "pl"
                        ? "Zamknąć formularz komputera?\n\nNiezapisane zmiany zostaną utracone."
                        : "Close the computer form?\n\nUnsaved changes will be lost.",
                    "HGC",
                    HgcMessageBoxType.Question,
                    currentLanguage);

                if (!cancel)
                    return;

                AddPcFormPanel.Visibility = Visibility.Collapsed;
            }

            if (CollectionTree.SelectedItem is TreeViewItem selectedTreeItem)
                selectedTreeItem.IsSelected = false;

            if (GamesTree.SelectedItem is TreeViewItem selectedGameTreeItem)
                selectedGameTreeItem.IsSelected = false;

            isGamesViewActive = false;
            currentGameFilter = GameFilterMode.All;
            currentGameGenre = "";

            isSearchActive = false;
            TopSearchTextBox.Clear();
            SearchLabel.Visibility = Visibility.Visible;
            ClearSearchButton.Visibility = Visibility.Collapsed;

            collectionCurrentPage = 1;

            ShowDefaultRightPanel();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                CollectionTilesScrollViewer.ScrollToTop();
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }
        private void SearchIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TopSearchTextBox.Text))
                return;

            ShowSearchResults(TopSearchTextBox.Text);

            TopSearchTextBox.Focus();
            Keyboard.Focus(TopSearchTextBox);
        }

        private void ArchiveAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccessoryItem == null)
                return;

            if (selectedAccessoryItem.IsArchived ||
                collection.ArchivedAccessories.Any(x => x.Id == selectedAccessoryItem.Id))
            {
                RestoreAccessoryFromArchive(selectedAccessoryItem);
                return;
            }

            if (selectedAccessoryItem.IsArchived ||
                collection.ArchivedAccessories.Any(x => x.Id == selectedAccessoryItem.Id))
                return;

            bool confirm = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "Czy na pewno przenieść akcesorium do archiwum?"
                    : "Are you sure you want to move this accessory to the archive?",
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!confirm)
                return;

            Accessory accessoryToArchive = selectedAccessoryItem;

            accessoryToArchive.IsArchived = true;
            accessoryToArchive.ArchiveDate = DateTime.Now;

            collection.Accessories.RemoveAll(x => x.Id == accessoryToArchive.Id);
            collection.ArchivedAccessories.RemoveAll(x => x.Id == accessoryToArchive.Id);
            collection.ArchivedAccessories.Add(accessoryToArchive);

            RemoveAccessoryFromTree(accessoryToArchive);
            AddArchivedAccessoryToTree(accessoryToArchive);

            selectedAccessoryItem = null;

            AccessoryDetailsPanel.Visibility = Visibility.Collapsed;
            AccessoryDetailsContentPanel.Children.Clear();
            AccessoryExtraDetailsPanel.Children.Clear();

            RefreshStats();
            SaveCollectionChanges();

            if (CollectionTree.SelectedItem is TreeViewItem selectedTreeItem)
                selectedTreeItem.IsSelected = false;

            currentPlatformGroup = "Default";
            UpdateBackgroundForCurrentState();

            ShowDefaultRightPanel();

            HgcMessageBox.Show(
                this,
                currentLanguage == "pl"
                    ? "Akcesorium zostało przeniesione do archiwum."
                    : "Accessory moved to archive.",
                "HGC",
                HgcMessageBoxType.Information,
                currentLanguage);
        }
        private void RestoreAccessoryFromArchive(Accessory accessory)
        {
            bool confirm = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "Czy chcesz przywrócić akcesorium z archiwum?"
                    : "Do you want to restore this accessory from archive?",
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!confirm)
                return;

            accessory.IsArchived = false;
            accessory.ArchiveDate = null;

            collection.ArchivedAccessories.RemoveAll(x => x.Id == accessory.Id);
            collection.Accessories.RemoveAll(x => x.Id == accessory.Id);
            collection.Accessories.Add(accessory);

            RemoveAccessoryFromTree(accessory);
            CleanupArchiveTreeBranchesOnly();
            AddAccessoryToTree(accessory);
            BuildGamesPanel();

            selectedAccessoryItem = null;

            AccessoryDetailsPanel.Visibility = Visibility.Collapsed;
            AccessoryDetailsContentPanel.Children.Clear();
            AccessoryExtraDetailsPanel.Children.Clear();

            RefreshStats();
            SaveCollectionChanges();

            ShowDefaultRightPanel(false);

            HgcMessageBox.Show(
                this,
                currentLanguage == "pl"
                    ? "Akcesorium zostało przywrócone z archiwum."
                    : "Accessory restored from archive.",
                "HGC",
                HgcMessageBoxType.Information,
                currentLanguage);
        }
        private void AddArchivedAccessoryToTree(Accessory accessory)
        {
            if (accessory == null)
                return;

            TreeViewItem archiveRoot = GetOrCreateTreeItem(
                CollectionTree.Items,
                currentLanguage == "pl" ? "Archiwum" : "Archive",
                "Default");

            archiveRoot.Tag = new TreeNodeData
            {
                NodeType = "ArchiveRoot",
                BackgroundGroup = "Default"
            };

            TreeViewItem accessoriesNode = GetOrCreateTreeItem(
                archiveRoot.Items,
                currentLanguage == "pl" ? "Akcesoria" : "Accessories",
                "Default");

            accessoriesNode.Tag = new TreeNodeData
            {
                NodeType = "ArchiveAccessoryCategory",
                BackgroundGroup = "Default"
            };

            TreeViewItem typeNode = GetOrCreateTreeItem(
                accessoriesNode.Items,
                accessory.AccessoryType,
                "Default");

            typeNode.Tag = new TreeNodeData
            {
                NodeType = "ArchiveAccessoryType",
                Category = accessory.AccessoryType,
                BackgroundGroup = "Default"
            };

            TreeViewItem accessoryItem = new TreeViewItem
            {
                Header = GetAccessoryDisplayName(accessory),
                Tag = new TreeNodeData
                {
                    NodeType = "AccessoryRecord",
                    BackgroundGroup = "Default",
                    AccessoryItem = accessory
                }
            };

            typeNode.Items.Add(accessoryItem);

            archiveRoot.Items.Refresh();
            accessoriesNode.Items.Refresh();
            typeNode.Items.Refresh();
        }
        private void RemoveAccessoryFromTree(Accessory accessory)
        {
            RemoveAccessoryFromTreeItems(CollectionTree.Items, accessory);
        }

        private bool RemoveAccessoryFromTreeItems(ItemCollection items, Accessory accessory)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i] is not TreeViewItem item)
                    continue;

                if (item.Tag is TreeNodeData node &&
                      node.AccessoryItem != null &&
                      node.AccessoryItem.Id == accessory.Id)
                {
                    items.RemoveAt(i);
                    return true;
                }

                if (RemoveAccessoryFromTreeItems(item.Items, accessory))
                {
                    if (item.Items.Count == 0 &&
                        item.Tag is TreeNodeData parentNode &&
                        (parentNode.NodeType == "AccessoryType" ||
                         parentNode.NodeType == "AccessoriesRoot" ||
                         parentNode.NodeType == "AccessoryCategory" ||
                         parentNode.NodeType == "ArchiveAccessoryType" ||
                         parentNode.NodeType == "ArchiveAccessoryCategory"))
                    {
                        items.RemoveAt(i);
                    }

                    return true;
                }
            }

            return false;
        }
        private void DeleteAccessoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccessoryItem == null)
                return;

            Accessory accessoryToDelete = selectedAccessoryItem;

            bool isArchived =
                accessoryToDelete.IsArchived ||
                collection.ArchivedAccessories.Any(x => x.Id == accessoryToDelete.Id);

            if (!isArchived)
            {
                HgcMessageBoxResult archiveResult = HgcMessageBox.ShowYesNoCancel(
                    this,
                    currentLanguage == "pl"
                        ? "Usunięcie akcesorium spowoduje trwałą utratę danych.\n\nZalecane jest przeniesienie akcesorium do archiwum.\n\nCzy chcesz przenieść akcesorium do archiwum?"
                        : "Deleting this accessory will permanently remove its data.\n\nWe recommend moving it to the archive instead.\n\nDo you want to move this accessory to the archive?",
                    "HGC",
                    HgcMessageBoxType.Warning,
                    currentLanguage);

                if (archiveResult == HgcMessageBoxResult.Cancel)
                    return;

                if (archiveResult == HgcMessageBoxResult.Yes)
                {
                    ArchiveAccessoryButton_Click(sender, e);
                    return;
                }
            }

            bool deleteResult = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "To działanie jest nieodwracalne.\n\nCzy na pewno chcesz trwale usunąć akcesorium?"
                    : "This action cannot be undone.\n\nAre you sure you want to permanently delete this accessory?",
                "HGC",
                HgcMessageBoxType.Warning,
                currentLanguage);

            if (!deleteResult)
                return;

            collection.Accessories.RemoveAll(x => x.Id == accessoryToDelete.Id);
            collection.ArchivedAccessories.RemoveAll(x => x.Id == accessoryToDelete.Id);

            selectedAccessoryItem = null;

            AccessoryDetailsPanel.Visibility = Visibility.Collapsed;
            AccessoryDetailsContentPanel.Children.Clear();
            AccessoryExtraDetailsPanel.Children.Clear();

            RemoveAccessoryFromTree(accessoryToDelete);
            CleanupArchiveTreeBranchesOnly();
            BuildGamesPanel();

            RefreshStats();
            SaveCollectionChanges();
            ShowDefaultRightPanel(false);
        }
        private void ShowCollectionTiles()
        {
            ShowCollectionTiles(
                new TreeNodeData
                {
                    NodeType = "Collection",
                    Manufacturer = "",
                    Family = "",
                    Platform = "",
                    Category = ""
                },
                true);
        }
        private bool IsAccessoryHardware(Hardware hardware)
        {
            if (hardware == null)
                return false;

            string text =
                $"{hardware.Family} {hardware.Platform} {hardware.Model} {hardware.CustomName}"
                .ToLowerInvariant();

            return
                text.Contains("accessory") ||
                text.Contains("accessories") ||
                text.Contains("akces") ||
                text.Contains("controller") ||
                text.Contains("kontrol") ||
                text.Contains("pad") ||
                text.Contains("joystick") ||
                text.Contains("kierown") ||
                text.Contains("steering") ||
                text.Contains("memory card") ||
                text.Contains("karta pamięci") ||
                text.Contains("remote") ||
                text.Contains("pilot") ||
                text.Contains("camera") ||
                text.Contains("kamera") ||
                text.Contains("adapter") ||
                text.Contains("keyboard") ||
                text.Contains("klawiatura") ||
                text.Contains("mouse") ||
                text.Contains("mysz");
        }
        private void ApplyPcDetailsLanguage()
        {
            EditPcButton.Content =
                currentLanguage == "pl"
                    ? "Edytuj"
                    : "Edit";

            ArchivePcButton.Content =
                currentLanguage == "pl"
                    ? "Archiwizuj"
                    : "Archive";

            DeletePcButton.Content =
                currentLanguage == "pl"
                    ? "Usuń"
                    : "Delete";

            PcActionsTitle.Text =
                currentLanguage == "pl"
                    ? "Zarządzanie"
                    : "Management";

            PcComponentsTitle.Text =
                currentLanguage == "pl"
                    ? "Szczegóły komputera"
                    : "PC details";

        }
        private void EditPcButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPcItem == null)
                return;

            ShowPcDetailsForEdit(selectedPcItem);
        }

        private void ArchivePcButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPcItem == null)
                return;

            if (selectedPcItem.IsArchived ||
            collection.ArchivedPcSystems.Contains(selectedPcItem))
            {
                selectedPcItem.IsArchived = false;
                selectedPcItem.ArchiveDate = null;

                collection.ArchivedPcSystems.Remove(selectedPcItem);

                if (!collection.PcSystems.Contains(selectedPcItem))
                    collection.PcSystems.Add(selectedPcItem);

                RemovePcFromTree(CollectionTree.Items, selectedPcItem);
                CleanupArchiveTreeBranchesOnly();
                AddPcRecordToTree(selectedPcItem);
                BuildGamesPanel();

                selectedPcItem = null;

                ShowDefaultRightPanel();
                RefreshStats();
                SaveCollectionChanges();
                return;
            }

            bool result = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "Czy na pewno chcesz przenieść komputer do archiwum?"
                    : "Are you sure you want to move this PC to the archive?",
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!result)
                return;

            selectedPcItem.IsArchived = true;
            selectedPcItem.ArchiveDate = DateTime.Now;

            collection.PcSystems.Remove(selectedPcItem);

            if (!collection.ArchivedPcSystems.Contains(selectedPcItem))
                collection.ArchivedPcSystems.Add(selectedPcItem);

            MovePcToArchive(selectedPcItem);

            selectedPcItem = null;

            ShowDefaultRightPanel();
            RefreshStats();
            SaveCollectionChanges();
        }

        private void DeletePcButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPcItem == null)
                return;

            bool isArchived =
                selectedPcItem.IsArchived ||
                collection.ArchivedPcSystems.Contains(selectedPcItem);

            if (!isArchived)
            {
                HgcMessageBoxResult archiveResult = HgcMessageBox.ShowYesNoCancel(
                    this,
                    currentLanguage == "pl"
                        ? "Usunięcie komputera spowoduje trwałą utratę danych.\n\nZalecane jest przeniesienie komputera do archiwum.\n\nCzy chcesz przenieść komputer do archiwum?"
                        : "Deleting this PC will permanently remove its data.\n\nWe recommend moving it to the archive instead.\n\nDo you want to move this PC to the archive?",
                    "HGC",
                    HgcMessageBoxType.Warning,
                    currentLanguage);

                if (archiveResult == HgcMessageBoxResult.Cancel)
                    return;

                if (archiveResult == HgcMessageBoxResult.Yes)
                {
                    ArchivePcButton_Click(sender, e);
                    return;
                }
            }

            bool deleteResult = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "To działanie jest nieodwracalne.\n\nCzy na pewno chcesz trwale usunąć komputer?"
                    : "This action cannot be undone.\n\nAre you sure you want to permanently delete this PC?",
                "HGC",
                HgcMessageBoxType.Warning,
                currentLanguage);

            if (!deleteResult)
                return;

            collection.PcSystems.Remove(selectedPcItem);
            collection.ArchivedPcSystems.Remove(selectedPcItem);

            RemovePcFromTree(CollectionTree.Items, selectedPcItem);

            if (isArchived)
                CleanupArchiveTreeBranchesOnly();
            else
                CleanupEmptyTreeBranches();

            BuildGamesPanel();

            selectedPcItem = null;

            ShowDefaultRightPanel();
            RefreshStats();
            SaveCollectionChanges();
        }
        private bool RemovePcFromTree(
        ItemCollection items,
        PcSystem pc)
        {
            foreach (object item in items)
            {
                if (item is not TreeViewItem treeItem)
                    continue;

                if (treeItem.Tag is TreeNodeData node &&
                    node.PcItem == pc)
                {
                    items.Remove(treeItem);
                    return true;
                }

                if (RemovePcFromTree(treeItem.Items, pc))
                    return true;
            }

            return false;
        }
        private void MovePcToArchive(PcSystem pc)
        {
            if (pc == null)
                return;

            RemovePcFromTree(CollectionTree.Items, pc);
            CleanupEmptyTreeBranches();
            CollapseTreeItems(CollectionTree.Items);

            TreeViewItem archiveRoot = GetArchiveRoot();

            TreeViewItem archivePcNode =
                GetOrCreateTreeItem(
                    archiveRoot.Items,
                    "PC",
                    "Default");

            archivePcNode.Tag = new TreeNodeData
            {
                NodeType = "ArchivePcCategory",
                BackgroundGroup = "Default"
            };

            TreeViewItem archivedItem = new TreeViewItem
            {
                Header = pc.Name,
                Tag = new TreeNodeData
                {
                    NodeType = "PcRecord",
                    BackgroundGroup = "Default",
                    PcItem = pc
                }
            };

            archivePcNode.Items.Add(archivedItem);
        }
        private void ShowPcDetailsForEdit(PcSystem pc)
        {
            if (pc == null)
                return;

            HideRightPanels();

            AddPcFormPanel.Visibility = Visibility.Visible;
            PcRightDetailsColumn.Visibility = Visibility.Collapsed;

            ApplyPcFormLanguage();
            LoadPcFormDefaults();
            ClearPcForm();

            isEditingPc = true;
            editingPcItem = pc;

            currentPcPhotos =
            editingPcItem.Photos ?? new List<CollectionPhoto>();

            PcPhotoGalleryButton.Content =
                currentLanguage == "pl"
                    ? "Edytuj zdjęcia"
                    : "Edit photos";

            editingPcTreeItem = FindPcTreeItem(CollectionTree.Items, pc);

            AddPcFormTitle.Text =
                currentLanguage == "pl"
                    ? "Edytuj komputer"
                    : "Edit PC";

            SavePcButton.Content =
                currentLanguage == "pl"
                    ? "Zapisz zmiany"
                    : "Save changes";

            PcNameTextBox.Text = pc.Name;

            PcTypeCombo.Text = pc.ComputerType;
            PcPortableTypeCombo.Text = pc.PortableType;

            PcManufacturerCombo.Text = pc.Manufacturer;

            if (!string.IsNullOrWhiteSpace(pc.Model) ||
                !string.IsNullOrWhiteSpace(pc.SerialNumber))
            {
                PcAddModelSerialCheckBox.IsChecked = true;
                PcModelSerialPanel.Visibility = Visibility.Visible;
            }

            PcModelTextBox.Text = pc.Model;
            PcSerialTextBox.Text = pc.SerialNumber;

            PcOperatingSystemCombo.Text = pc.OperatingSystem;
            PcOperatingSystemEditionCombo.Text = pc.OperatingSystemEdition;
            PcOperatingSystemRevisionTextBox.Text = pc.OperatingSystemRevision;

            PcPurchaseDatePicker.SelectedDate = pc.PurchaseDate;
            PcPurchasePriceTextBox.Text =
                pc.PurchasePrice.HasValue
                    ? pc.PurchasePrice.Value.ToString("0.00")
                    : "";

            PcPurchaseCurrencyCombo.SelectedItem =
                string.IsNullOrWhiteSpace(pc.PurchaseCurrency)
                    ? defaultCurrency
                    : pc.PurchaseCurrency;

            UpdatePcTypeFields();
            UpdatePcManufacturerFields();
            UpdatePcOperatingSystemFields();

            PcRightDetailsColumn.Visibility = Visibility.Visible;

            BuildPcEditComponentsPanel(pc);
        }
        private void BuildPcEditComponentsPanel(PcSystem pc)
        {
            if (PcDetailsSectionTitle.Parent is not StackPanel host)
                return;

            host.Children.Clear();

            PcDetailsSectionTitle.Text =
                currentLanguage == "pl"
                    ? "Szczegóły komputera"
                    : "PC details";

            host.Children.Add(PcDetailsSectionTitle);

            Border cpuBorder = new Border
            {
                Padding = new Thickness(14),
                CornerRadius = new CornerRadius(10),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 14, 0, 12)
            };

            ApplyDynamicPcPanelStyle(cpuBorder);

            StackPanel cpuPanel = new StackPanel();

            cpuPanel.Children.Add(new TextBlock
            {
                Text = pc.Cpus.Count > 0
                    ? (currentLanguage == "pl"
                        ? $"Procesor ({pc.Cpus.Count})"
                        : $"Processor ({pc.Cpus.Count})")
                    : (currentLanguage == "pl"
                        ? "Procesor"
                        : "Processor"),
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            foreach (PcCpu cpu in pc.Cpus)
            {
                Border itemBorder = new Border
                {
                    BorderBrush = (Brush)FindResource("AppBorderBrush"),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8),
                    Margin = new Thickness(0, 0, 0, 8)
                };

                StackPanel itemPanel = new StackPanel();

                itemPanel.Children.Add(new TextBlock
                {
                    Text = $"{cpu.Manufacturer} {cpu.Model}",
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 8)
                });

                StackPanel buttons = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                Button editButton = new Button
                {
                    Content = currentLanguage == "pl" ? "Edytuj" : "Edit",
                    Tag = cpu,
                    Width = 90,
                    Margin = new Thickness(0, 0, 8, 0)
                };
                editButton.Click += EditCpuButton_Click;

                Button deleteButton = new Button
                {
                    Content = currentLanguage == "pl" ? "Usuń" : "Delete",
                    Tag = cpu,
                    Width = 90
                };
                deleteButton.Click += DeleteCpuButton_Click;

                buttons.Children.Add(editButton);
                buttons.Children.Add(deleteButton);

                itemPanel.Children.Add(buttons);
                itemBorder.Child = itemPanel;

                cpuPanel.Children.Add(itemBorder);
            }

            Button addCpuButton = new Button
            {
                Content = currentLanguage == "pl" ? "Dodaj procesor" : "Add CPU",
                Height = 34,
                Tag = "CPU",
                HorizontalAlignment = HorizontalAlignment.Left,
                MinWidth = 150,
                Margin = new Thickness(0, 8, 0, 0)
            };

            addCpuButton.Click += AddPcComponentButton_Click;

            cpuPanel.Children.Add(addCpuButton);

            cpuBorder.Child = cpuPanel;
            host.Children.Add(cpuBorder);

            Border motherboardBorder = new Border
            {
                Padding = new Thickness(14),
                CornerRadius = new CornerRadius(10),
                Background = (Brush)FindResource("PanelBackgroundBrush"),
                BorderBrush = (Brush)FindResource("AppBorderBrush"),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 12)
            };

            ApplyDynamicPcPanelStyle(motherboardBorder);

            StackPanel motherboardPanel = new StackPanel();

            motherboardPanel.Children.Add(new TextBlock
            {
                Text = currentLanguage == "pl"
                    ? "Płyta główna"
                    : "Motherboard",
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            if (pc.Motherboard != null)
            {
                motherboardPanel.Children.Add(new TextBlock
                {
                    Text = $"{pc.Motherboard.Manufacturer} {pc.Motherboard.Model}",
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 8)
                });

                StackPanel buttonsPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                Button editBoardButton = new Button
                {
                    Content = currentLanguage == "pl"
                        ? "Edytuj"
                        : "Edit",
                    Tag = pc.Motherboard,
                    Width = 90,
                    Margin = new Thickness(0, 0, 8, 0)
                };

                editBoardButton.Click += EditMotherboardButton_Click;

                Button deleteBoardButton = new Button
                {
                    Content = currentLanguage == "pl"
                        ? "Usuń"
                        : "Delete",
                    Width = 90
                };

                deleteBoardButton.Click += DeleteMotherboardButton_Click;

                buttonsPanel.Children.Add(editBoardButton);
                buttonsPanel.Children.Add(deleteBoardButton);

                motherboardPanel.Children.Add(buttonsPanel);
            }
            else
            {
                Button addBoardButton = new Button
                {
                    Content = currentLanguage == "pl"
                        ? "Dodaj płytę główną"
                        : "Add motherboard",
                    Tag = "Motherboard",
                    Height = 34,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    MinWidth = 180
                };

                addBoardButton.Click += AddPcComponentButton_Click;

                motherboardPanel.Children.Add(addBoardButton);
            }

            motherboardBorder.Child = motherboardPanel;
            host.Children.Add(motherboardBorder);

            Border gpuBorder = new Border
            {
                Padding = new Thickness(14),
                CornerRadius = new CornerRadius(10),
                Background = (Brush)FindResource("PanelBackgroundBrush"),
                BorderBrush = (Brush)FindResource("AppBorderBrush"),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 12)
            };

            ApplyDynamicPcPanelStyle(gpuBorder);

            StackPanel gpuPanel = new StackPanel();

            gpuPanel.Children.Add(new TextBlock
            {
                Text = currentLanguage == "pl"
                    ? "Karty graficzne"
                    : "Graphics cards",
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            foreach (PcGpu gpu in pc.Gpus)
            {
                Border itemBorder = new Border
                {
                    BorderBrush = (Brush)FindResource("AppBorderBrush"),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8),
                    Margin = new Thickness(0, 0, 0, 8)
                };

                StackPanel itemPanel = new StackPanel();

                itemPanel.Children.Add(new TextBlock
                {
                    Text = $"{gpu.CardManufacturer} {gpu.Model}",
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 8)
                });

                StackPanel buttonsPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                Button editGpuButton = new Button
                {
                    Content = currentLanguage == "pl"
                        ? "Edytuj"
                        : "Edit",
                    Tag = gpu,
                    Width = 90,
                    Margin = new Thickness(0, 0, 8, 0)
                };

                editGpuButton.Click += EditGpuButton_Click;

                Button deleteGpuButton = new Button
                {
                    Content = currentLanguage == "pl"
                        ? "Usuń"
                        : "Delete",
                    Tag = gpu,
                    Width = 90
                };

                deleteGpuButton.Click += DeleteGpuButton_Click;

                buttonsPanel.Children.Add(editGpuButton);
                buttonsPanel.Children.Add(deleteGpuButton);

                itemPanel.Children.Add(buttonsPanel);

                itemBorder.Child = itemPanel;

                gpuPanel.Children.Add(itemBorder);
            }

            Button addGpuButton = new Button
            {
                Content = currentLanguage == "pl"
                    ? "Dodaj kartę graficzną"
                    : "Add graphics card",
                Height = 34,
                Tag = "GPU",
                HorizontalAlignment = HorizontalAlignment.Left,
                MinWidth = 180
            };

            addGpuButton.Click += AddPcComponentButton_Click;

            gpuPanel.Children.Add(addGpuButton);

            gpuBorder.Child = gpuPanel;

            host.Children.Add(gpuBorder);

            // ================= RAM =================

            Border ramBorder = new Border
            {
                Padding = new Thickness(14),
                CornerRadius = new CornerRadius(10),
                Background = (Brush)FindResource("PanelBackgroundBrush"),
                BorderBrush = (Brush)FindResource("AppBorderBrush"),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 12)
            };

            ApplyDynamicPcPanelStyle(ramBorder);

            StackPanel ramPanel = new StackPanel();

            ramPanel.Children.Add(new TextBlock
            {
                Text = "RAM",
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            foreach (PcRam ram in pc.RamModules)
            {
                Border itemBorder = new Border
                {
                    BorderBrush = (Brush)FindResource("AppBorderBrush"),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8),
                    Margin = new Thickness(0, 0, 0, 8)
                };

                StackPanel itemPanel = new StackPanel();

                itemPanel.Children.Add(new TextBlock
                {
                    Text = $"{ram.Manufacturer} {ram.Model} {ram.TotalCapacity} {ram.MemoryType}".Trim(),
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 8),
                    TextWrapping = TextWrapping.Wrap
                });

                StackPanel buttonsPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                Button editRamButton = new Button
                {
                    Content = currentLanguage == "pl" ? "Edytuj" : "Edit",
                    Tag = ram,
                    Width = 90,
                    Margin = new Thickness(0, 0, 8, 0)
                };

                editRamButton.Click += EditPcComponentButton_Click;

                Button deleteRamButton = new Button
                {
                    Content = currentLanguage == "pl" ? "Usuń" : "Delete",
                    Tag = ram,
                    Width = 90
                };

                deleteRamButton.Click += DeletePcComponentButton_Click;

                buttonsPanel.Children.Add(editRamButton);
                buttonsPanel.Children.Add(deleteRamButton);

                itemPanel.Children.Add(buttonsPanel);
                itemBorder.Child = itemPanel;

                ramPanel.Children.Add(itemBorder);
            }

            Button addRamButton = new Button
            {
                Content = currentLanguage == "pl" ? "Dodaj RAM" : "Add RAM",
                Height = 34,
                Tag = "RAM",
                HorizontalAlignment = HorizontalAlignment.Left,
                MinWidth = 150
            };

            addRamButton.Click += AddPcComponentButton_Click;

            ramPanel.Children.Add(addRamButton);

            ramBorder.Child = ramPanel;
            host.Children.Add(ramBorder);


            // ================= STORAGE =================

            Border storageBorder = new Border
            {
                Padding = new Thickness(14),
                CornerRadius = new CornerRadius(10),
                Background = (Brush)FindResource("PanelBackgroundBrush"),
                BorderBrush = (Brush)FindResource("AppBorderBrush"),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 12)
            };

            ApplyDynamicPcPanelStyle(storageBorder);

            StackPanel storagePanel = new StackPanel();

            storagePanel.Children.Add(new TextBlock
            {
                Text = currentLanguage == "pl" ? "Dyski" : "Storage",
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            foreach (PcStorage storage in pc.StorageDevices)
            {
                Border itemBorder = new Border
                {
                    BorderBrush = (Brush)FindResource("AppBorderBrush"),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8),
                    Margin = new Thickness(0, 0, 0, 8)
                };

                StackPanel itemPanel = new StackPanel();

                itemPanel.Children.Add(new TextBlock
                {
                    Text = $"{storage.Manufacturer} {storage.Model} {storage.Capacity} {storage.DriveType} {storage.Interface}".Trim(),
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 8),
                    TextWrapping = TextWrapping.Wrap
                });

                StackPanel buttonsPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                Button editStorageButton = new Button
                {
                    Content = currentLanguage == "pl" ? "Edytuj" : "Edit",
                    Tag = storage,
                    Width = 90,
                    Margin = new Thickness(0, 0, 8, 0)
                };

                editStorageButton.Click += EditPcComponentButton_Click;

                Button deleteStorageButton = new Button
                {
                    Content = currentLanguage == "pl" ? "Usuń" : "Delete",
                    Tag = storage,
                    Width = 90
                };

                deleteStorageButton.Click += DeletePcComponentButton_Click;

                buttonsPanel.Children.Add(editStorageButton);
                buttonsPanel.Children.Add(deleteStorageButton);

                itemPanel.Children.Add(buttonsPanel);
                itemBorder.Child = itemPanel;

                storagePanel.Children.Add(itemBorder);
            }

            Button addStorageButton = new Button
            {
                Content = currentLanguage == "pl" ? "Dodaj dysk" : "Add drive",
                Height = 34,
                Tag = "Storage",
                HorizontalAlignment = HorizontalAlignment.Left,
                MinWidth = 150
            };

            addStorageButton.Click += AddPcComponentButton_Click;

            storagePanel.Children.Add(addStorageButton);

            storageBorder.Child = storagePanel;
            host.Children.Add(storageBorder);


            // ================= PSU =================

            Border psuBorder = new Border
            {
                Padding = new Thickness(14),
                CornerRadius = new CornerRadius(10),
                Background = (Brush)FindResource("PanelBackgroundBrush"),
                BorderBrush = (Brush)FindResource("AppBorderBrush"),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 12)
            };

            ApplyDynamicPcPanelStyle(psuBorder);

            StackPanel psuPanel = new StackPanel();

            psuPanel.Children.Add(new TextBlock
            {
                Text = currentLanguage == "pl" ? "Zasilacze" : "Power supplies",
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            foreach (PcPowerSupply psu in pc.PowerSupplies)
            {
                Border itemBorder = new Border
                {
                    BorderBrush = (Brush)FindResource("AppBorderBrush"),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8),
                    Margin = new Thickness(0, 0, 0, 8)
                };

                StackPanel itemPanel = new StackPanel();

                itemPanel.Children.Add(new TextBlock
                {
                    Text = $"{psu.Manufacturer} {psu.Model} {psu.Power}".Trim(),
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 8),
                    TextWrapping = TextWrapping.Wrap
                });

                StackPanel buttonsPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                Button editPsuButton = new Button
                {
                    Content = currentLanguage == "pl" ? "Edytuj" : "Edit",
                    Tag = psu,
                    Width = 90,
                    Margin = new Thickness(0, 0, 8, 0)
                };

                editPsuButton.Click += EditPcComponentButton_Click;

                Button deletePsuButton = new Button
                {
                    Content = currentLanguage == "pl" ? "Usuń" : "Delete",
                    Tag = psu,
                    Width = 90
                };

                deletePsuButton.Click += DeletePcComponentButton_Click;

                buttonsPanel.Children.Add(editPsuButton);
                buttonsPanel.Children.Add(deletePsuButton);

                itemPanel.Children.Add(buttonsPanel);
                itemBorder.Child = itemPanel;

                psuPanel.Children.Add(itemBorder);
            }

            Button addPsuButton = new Button
            {
                Content = currentLanguage == "pl" ? "Dodaj zasilacz" : "Add PSU",
                Height = 34,
                Tag = "PSU",
                HorizontalAlignment = HorizontalAlignment.Left,
                MinWidth = 150
            };

            addPsuButton.Click += AddPcComponentButton_Click;

            psuPanel.Children.Add(addPsuButton);

            psuBorder.Child = psuPanel;
            host.Children.Add(psuBorder);


            // ================= CASE =================

            Border caseBorder = new Border
            {
                Padding = new Thickness(14),
                CornerRadius = new CornerRadius(10),
                Background = (Brush)FindResource("PanelBackgroundBrush"),
                BorderBrush = (Brush)FindResource("AppBorderBrush"),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 12)
            };

            ApplyDynamicPcPanelStyle(caseBorder);

            StackPanel casePanel = new StackPanel();

            casePanel.Children.Add(new TextBlock
            {
                Text = currentLanguage == "pl" ? "Obudowa" : "Case",
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            if (pc.Case != null)
            {
                Border itemBorder = new Border
                {
                    BorderBrush = (Brush)FindResource("AppBorderBrush"),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(8),
                    Margin = new Thickness(0, 0, 0, 8)
                };

                StackPanel itemPanel = new StackPanel();

                itemPanel.Children.Add(new TextBlock
                {
                    Text = $"{pc.Case.Manufacturer} {pc.Case.Model} {pc.Case.CaseType}".Trim(),
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 8),
                    TextWrapping = TextWrapping.Wrap
                });

                StackPanel buttonsPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                Button editCaseButton = new Button
                {
                    Content = currentLanguage == "pl" ? "Edytuj" : "Edit",
                    Tag = pc.Case,
                    Width = 90,
                    Margin = new Thickness(0, 0, 8, 0)
                };

                editCaseButton.Click += EditPcComponentButton_Click;

                Button deleteCaseButton = new Button
                {
                    Content = currentLanguage == "pl" ? "Usuń" : "Delete",
                    Tag = pc.Case,
                    Width = 90
                };

                deleteCaseButton.Click += DeletePcComponentButton_Click;

                buttonsPanel.Children.Add(editCaseButton);
                buttonsPanel.Children.Add(deleteCaseButton);

                itemPanel.Children.Add(buttonsPanel);
                itemBorder.Child = itemPanel;

                casePanel.Children.Add(itemBorder);
            }
            else
            {
                Button addCaseButton = new Button
                {
                    Content = currentLanguage == "pl" ? "Dodaj obudowę" : "Add case",
                    Height = 34,
                    Tag = "Case",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    MinWidth = 150
                };

                addCaseButton.Click += AddPcComponentButton_Click;

                casePanel.Children.Add(addCaseButton);
            }

            caseBorder.Child = casePanel;
            host.Children.Add(caseBorder);

            // ================= COOLING =================

            Border coolingBorder = new Border
            {
                Padding = new Thickness(14),
                CornerRadius = new CornerRadius(10),
                Background = (Brush)FindResource("PanelBackgroundBrush"),
                BorderBrush = (Brush)FindResource("AppBorderBrush"),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 12)
            };

            ApplyDynamicPcPanelStyle(coolingBorder);

            StackPanel coolingPanel = new StackPanel();

            coolingPanel.Children.Add(new TextBlock
            {
                Text = currentLanguage == "pl"
                    ? "Chłodzenie"
                    : "Cooling",
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            if (pc.Cooling != null)
            {
                coolingPanel.Children.Add(new TextBlock
                {
                    Text = GetCoolingDisplayText(pc.Cooling),
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 8),
                    TextWrapping = TextWrapping.Wrap
                });

                StackPanel buttonsPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                Button editCoolingButton = new Button
                {
                    Content = currentLanguage == "pl" ? "Edytuj" : "Edit",
                    Width = 90,
                    Margin = new Thickness(0, 0, 8, 0)
                };

                editCoolingButton.Click += EditCoolingButton_Click;

                Button deleteCoolingButton = new Button
                {
                    Content = currentLanguage == "pl" ? "Usuń" : "Delete",
                    Width = 90
                };

                deleteCoolingButton.Click += DeleteCoolingButton_Click;

                buttonsPanel.Children.Add(editCoolingButton);
                buttonsPanel.Children.Add(deleteCoolingButton);

                coolingPanel.Children.Add(buttonsPanel);
            }
            else
            {
                Button addCoolingButton = new Button
                {
                    Content = currentLanguage == "pl"
                        ? "Dodaj chłodzenie"
                        : "Add cooling",
                    Tag = "Cooling",
                    Height = 34,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    MinWidth = 180
                };

                addCoolingButton.Click += AddPcComponentButton_Click;

                coolingPanel.Children.Add(addCoolingButton);
            }

            coolingBorder.Child = coolingPanel;
            host.Children.Add(coolingBorder);
        }
        private void EditPcComponentButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button ||
                button.Tag == null)
                return;

            PcSystem? targetPc =
                isEditingPc ? selectedPcItem : currentPcDraft;

            if (targetPc == null)
                return;

            if (button.Tag is PcRam ram)
            {
                PcComponentWindow window =
                    new PcComponentWindow(currentLanguage, "RAM", ram)
                    {
                        Owner = this
                    };

                if (window.ShowDialog() == true)
                    RefreshPcComponentsPanel(targetPc);

                return;
            }

            if (button.Tag is PcStorage storage)
            {
                PcComponentWindow window =
                    new PcComponentWindow(currentLanguage, "Storage", storage)
                    {
                        Owner = this
                    };

                if (window.ShowDialog() == true)
                    RefreshPcComponentsPanel(targetPc);

                return;
            }

            if (button.Tag is PcPowerSupply psu)
            {
                PcComponentWindow window =
                    new PcComponentWindow(currentLanguage, "PSU", psu)
                    {
                        Owner = this
                    };

                if (window.ShowDialog() == true)
                    RefreshPcComponentsPanel(targetPc);

                return;
            }

            if (button.Tag is PcCase pcCase)
            {
                PcComponentWindow window =
                    new PcComponentWindow(currentLanguage, "Case", pcCase)
                    {
                        Owner = this
                    };

                if (window.ShowDialog() == true)
                    RefreshPcComponentsPanel(targetPc);
            }
        }

        private void DeletePcComponentButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button ||
                button.Tag == null)
                return;

            PcSystem? targetPc =
                isEditingPc ? selectedPcItem : currentPcDraft;

            if (targetPc == null)
                return;

            bool result = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "Usunąć komponent?"
                    : "Delete component?",
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!result)
                return;

            if (button.Tag is PcRam ram)
                targetPc.RamModules.Remove(ram);
            else if (button.Tag is PcStorage storage)
                targetPc.StorageDevices.Remove(storage);
            else if (button.Tag is PcPowerSupply psu)
                targetPc.PowerSupplies.Remove(psu);
            else if (button.Tag is PcCase)
                targetPc.Case = null;

            RefreshPcComponentsPanel(targetPc);
        }
        private void EditCoolingButton_Click(object sender, RoutedEventArgs e)
        {
            PcSystem? targetPc =
                isEditingPc ? selectedPcItem : currentPcDraft;

            if (targetPc == null || targetPc.Cooling == null)
                return;

            PcCoolingWindow window =
                new PcCoolingWindow(currentLanguage, targetPc.Cooling)
                {
                    Owner = this
                };

            if (window.ShowDialog() == true &&
                window.ResultCooling != null)
            {
                targetPc.Cooling = window.ResultCooling;
                RefreshPcComponentsPanel(targetPc);
            }
        }

        private void DeleteCoolingButton_Click(object sender, RoutedEventArgs e)
        {
            PcSystem? targetPc =
                isEditingPc ? selectedPcItem : currentPcDraft;

            if (targetPc == null || targetPc.Cooling == null)
                return;

            bool result = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "Usunąć chłodzenie?"
                    : "Delete cooling?",
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!result)
                return;

            targetPc.Cooling = null;

            RefreshPcComponentsPanel(targetPc);
        }
        private void EditGpuButton_Click(object sender, RoutedEventArgs e)
        {
            PcSystem? targetPc =
                isEditingPc ? selectedPcItem : currentPcDraft;

            if (targetPc == null)
                return;

            if (sender is not Button button)
                return;

            if (button.Tag is not PcGpu gpu)
                return;

            PcGpuWindow window =
                new PcGpuWindow(currentLanguage, gpu)
                {
                    Owner = this
                };

            if (window.ShowDialog() == true)
            {
                RefreshPcComponentsPanel(targetPc);
            }
        }

        private void DeleteGpuButton_Click(object sender, RoutedEventArgs e)
        {
            PcSystem? targetPc =
                isEditingPc ? selectedPcItem : currentPcDraft;

            if (targetPc == null)
                return;

            if (sender is not Button button)
                return;

            if (button.Tag is not PcGpu gpu)
                return;

            bool result = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "Usunąć kartę graficzną?"
                    : "Delete graphics card?",
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!result)
                return;

            targetPc.Gpus.Remove(gpu);

            RefreshPcComponentsPanel(targetPc);
        }
        private void EditMotherboardButton_Click(object sender, RoutedEventArgs e)
        {
            PcSystem? targetPc =
                isEditingPc ? selectedPcItem : currentPcDraft;

            if (targetPc == null || targetPc.Motherboard == null)
                return;

            PcMotherboardWindow window =
                new PcMotherboardWindow(
                    currentLanguage,
                    targetPc.Motherboard)
                {
                    Owner = this
                };

            if (window.ShowDialog() == true)
            {
                RefreshPcComponentsPanel(targetPc);
            }
        }

        private void DeleteMotherboardButton_Click(object sender, RoutedEventArgs e)
        {
            PcSystem? targetPc =
                isEditingPc ? selectedPcItem : currentPcDraft;

            if (targetPc == null)
                return;

            bool result = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "Usunąć płytę główną?"
                    : "Delete motherboard?",
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!result)
                return;

            targetPc.Motherboard = null;

            RefreshPcComponentsPanel(targetPc);
        }
        private void DeleteCpuButton_Click(object sender, RoutedEventArgs e)
        {
            PcSystem? targetPc =
                isEditingPc ? selectedPcItem : currentPcDraft;

            if (targetPc == null)
                return;

            if (sender is not Button button)
                return;

            if (button.Tag is not PcCpu cpu)
                return;

            bool result = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "Usunąć procesor?"
                    : "Delete CPU?",
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!result)
                return;

            targetPc.Cpus.Remove(cpu);

            RefreshPcComponentsPanel(targetPc);
        }
        private void EditCpuButton_Click(
        object sender,
        RoutedEventArgs e)
        {
            PcSystem? targetPc =
            isEditingPc ? selectedPcItem : currentPcDraft;

            if (targetPc == null)
                return;

            if (sender is not Button button)
                return;

            if (button.Tag is not PcCpu cpu)
                return;

            PcCpuWindow window =
                new PcCpuWindow(
                    currentLanguage,
                    cpu)
                {
                    Owner = this
                };

            if (window.ShowDialog() == true)
            {
                RefreshPcComponentsPanel(targetPc);
            }
        }
        private void CloseAddChoiceButton_Click(
    object sender,
    RoutedEventArgs e)
        {
            AddChoicePanel.Visibility = Visibility.Collapsed;
            AddHardwareSubChoicePanel.Visibility = Visibility.Collapsed;

            isAddChoicePanelOpen = false;

            ShowDefaultRightPanel();
        }
        private TreeViewItem? FindPcTreeItem(
        ItemCollection items,
        PcSystem pc)
        {
            foreach (object item in items)
            {
                if (item is not TreeViewItem treeItem)
                    continue;

                if (treeItem.Tag is TreeNodeData node &&
                    node.PcItem == pc)
                {
                    return treeItem;
                }

                TreeViewItem? found =
                    FindPcTreeItem(treeItem.Items, pc);

                if (found != null)
                    return found;
            }

            return null;
        }
        private void AddTilesSectionTitle(string title)
        {
            TextBlock sectionTitle = new TextBlock
            {
                Text = title,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 8, 0, 8),
                Opacity = 0.85,

                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 6,
                    ShadowDepth = 2,
                    Opacity = 0.85
                }
            };

            WrapPanel sectionWrap = new WrapPanel
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                CacheMode = new BitmapCache()
            };

            CollectionTilesSectionsPanel.Children.Add(sectionTitle);
            CollectionTilesSectionsPanel.Children.Add(sectionWrap);

            currentCollectionTilesWrap = sectionWrap;
        }

        private void AddHardwareTile(Hardware hardware)
        {
            double tileWidth = GetCollectionTileWidth();
            double tileHeight = GetCollectionHardwareTileHeight();
            double imageSize = GetCollectionTileImageSize();
            double hardwareImageHeight = GetCollectionHardwareImageHeight();

            Button tile = new Button
            {
                Width = tileWidth,
                Height = tileHeight,
                Margin = new Thickness(0, 0, 16, 12),
                Tag = hardware
            };

            tile.Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 8,
                ShadowDepth = 5,
                Opacity = 0.45
            };

            StackPanel panel = new StackPanel();

            Image image = new Image
            {
                Width = imageSize,
                Height = hardwareImageHeight,
                Opacity = 0.85,
                Stretch = Stretch.Uniform
            };

            image.Loaded += (s, e) =>
            {
                image.Clip = new RectangleGeometry(
                    new Rect(0, 0, image.ActualWidth, image.ActualHeight),
                    7,
                    7);
            };

            CollectionPhoto mainPhoto =
                hardware.Photos?.FirstOrDefault(x => x.IsMain)
                ?? hardware.Photos?.FirstOrDefault();

            if (mainPhoto != null &&
                File.Exists(mainPhoto.OriginalPath))
            {
                image.Source = LoadImageWithoutLock(mainPhoto.OriginalPath, 220);
            }
            else
            {
                image.Source = new BitmapImage(
                    new Uri("pack://application:,,,/Assets/Icons/nophoto.png"));
                image.Opacity = 0.55;
            }

            TextBlock title = new TextBlock
            {
                Text = GetTileDisplayName(GetHardwareDisplayName(hardware)),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                TextTrimming = TextTrimming.CharacterEllipsis,
                MaxHeight = 40,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)FindResource("ButtonTextBrush"),
                Margin = new Thickness(4, 0, 4, 0),
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 4,
                    ShadowDepth = 1,
                    Opacity = 0.45
                }
            };

            Border imageContainer = new Border
            {
                Width = imageSize,
                Height = hardwareImageHeight,
                CornerRadius = new CornerRadius(4),
                Margin = new Thickness(0, 0, 0, 6),
                HorizontalAlignment = HorizontalAlignment.Center,
                Child = image,
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 10,
                    ShadowDepth = 6,
                    Opacity = 0.45
                }
            };

            panel.Children.Add(imageContainer);
            panel.Children.Add(title);

            tile.Content = panel;
            tile.Click += HardwareTile_Click;

            currentCollectionTilesWrap?.Children.Add(tile);
        }
        private void AddGameTile(Game game)
        {
            double tileWidth = GetCollectionTileWidth();
            double tileHeight = GetCollectionGameTileHeight();
            double imageSize = GetCollectionTileImageSize();

            Button tile = new Button
            {
                Width = tileWidth,
                Height = tileHeight,
                Margin = new Thickness(0, 0, 16, 12),
                Tag = game
            };

            tile.Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 8,
                ShadowDepth = 5,
                Opacity = 0.45
            };

            StackPanel panel = new StackPanel();

            CollectionPhoto mainPhoto = null;

            if (game.FrontCoverPhoto != null && game.FrontCoverPhoto.IsMain)
                mainPhoto = game.FrontCoverPhoto;

            if (mainPhoto == null &&
                game.BackCoverPhoto != null &&
                game.BackCoverPhoto.IsMain)
            {
                mainPhoto = game.BackCoverPhoto;
            }

            if (mainPhoto == null)
                mainPhoto = game.Photos?.FirstOrDefault(x => x.IsMain);

            if (mainPhoto == null)
                mainPhoto = game.FrontCoverPhoto;

            if (mainPhoto == null)
                mainPhoto = game.BackCoverPhoto;

            if (mainPhoto == null)
                mainPhoto = game.Photos?.FirstOrDefault();

            ImageSource tileImageSource;
            double tileImageOpacity = 0.9;

            if (mainPhoto != null &&
                File.Exists(mainPhoto.OriginalPath))
            {
                tileImageSource = GetCachedTileImage(mainPhoto.OriginalPath, 260);
            }
            else
            {
                tileImageSource = new BitmapImage(
                    new Uri("pack://application:,,,/Assets/Icons/nophoto.png"));

                tileImageOpacity = 0.55;
            }

            TextBlock title = new TextBlock
            {
                Text = GetTileDisplayName(game.Title),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                FontWeight = FontWeights.SemiBold,
                MaxHeight = 40,
                TextTrimming = TextTrimming.CharacterEllipsis,
                Foreground = (Brush)FindResource("ButtonTextBrush"),
                Margin = new Thickness(4, 2, 4, 0),
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 4,
                    ShadowDepth = 1,
                    Opacity = 0.45
                }
            };

            Image tileImage = new Image
            {
                Width = imageSize,
                Height = imageSize,
                Source = tileImageSource,
                Stretch = Stretch.Uniform,
                Opacity = tileImageOpacity
            };

            tileImage.OpacityMask = new VisualBrush
            {
                Visual = new Border
                {
                    Width = imageSize,
                    Height = imageSize,
                    CornerRadius = new CornerRadius(7),
                    Background = Brushes.Black
                }
            };

            Border imageContainer = new Border
            {
                Width = imageSize,
                Height = imageSize,
                CornerRadius = new CornerRadius(7),
                Margin = new Thickness(0, 2, 0, 6),
                HorizontalAlignment = HorizontalAlignment.Center,
                Child = tileImage,
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 10,
                    ShadowDepth = 6,
                    Opacity = 0.45
                }
            };

            panel.Children.Add(imageContainer);

            panel.Children.Add(title);

            if (!string.IsNullOrWhiteSpace(game.Platform))
            {
                TextBlock platformText = new TextBlock
                {
                    Text = game.Manufacturer == "PC"
                     ? "PC"
                     : game.Platform,
                    FontSize = 11,
                    Opacity = 0.70,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = (Brush)FindResource("ButtonTextBrush"),
                    Margin = new Thickness(4, 1, 4, 3)
                };

                panel.Children.Add(platformText);
            }

            if (game.UserRating.HasValue)
            {
                StackPanel ratingPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, -1, 0, 2)
                };

                double rating = game.UserRating.Value;

                Brush filledBrush = Brushes.Gold;
                Brush emptyBrush = new SolidColorBrush(Color.FromArgb(70, 255, 255, 255));

                bool isSmallTile = collectionTileSize == "Small";
                bool isLargeTile = collectionTileSize == "Large";

                double starFontSize = isSmallTile ? 9 : isLargeTile ? 15 : 12;
                double starWidth = isSmallTile ? 9 : isLargeTile ? 15 : 12;
                double starHeight = isSmallTile ? 11 : isLargeTile ? 17 : 14;
                double starClipWidth = isSmallTile ? 7 : isLargeTile ? 12 : 9;

                for (int i = 1; i <= 10; i++)
                {
                    double fillAmount;

                    if (rating >= i)
                        fillAmount = 1.0;
                    else if (rating >= i - 0.5)
                        fillAmount = 0.42;
                    else
                        fillAmount = 0.0;

                    Grid starGrid = new Grid
                    {
                        Width = starWidth,
                        Height = starHeight,
                        Margin = new Thickness(0, 0, 1, 0)
                    };

                    TextBlock emptyStar = new TextBlock
                    {
                        Text = "★",
                        FontSize = starFontSize,
                        FontWeight = FontWeights.Bold,
                        Foreground = emptyBrush
                    };

                    TextBlock filledStar = new TextBlock
                    {
                        Text = "★",
                        FontSize = starFontSize,
                        FontWeight = FontWeights.Bold,
                        Foreground = filledBrush
                    };

                    if (fillAmount < 1.0)
                    {
                        filledStar.Clip = new RectangleGeometry(
                            new Rect(0, 0, starClipWidth * fillAmount, starHeight));
                    }

                    starGrid.Children.Add(emptyStar);

                    if (fillAmount > 0)
                        starGrid.Children.Add(filledStar);

                    ratingPanel.Children.Add(starGrid);
                }

                panel.Children.Add(ratingPanel);
            }

            tile.Content = panel;
            tile.Click += GameTile_Click;

            currentCollectionTilesWrap?.Children.Add(tile);
        }

        private void HardwareTile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button &&
                button.Tag is Hardware hardware)
            {
                currentPlatformGroup =
                    GetBackgroundGroupForManufacturer(hardware.Manufacturer);

                UpdateBackgroundForCurrentState();

                selectedHardwareItem = hardware;
                ShowHardwareDetails(hardware);
            }
        }
        private void GameTile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button &&
                button.Tag is Game game)
            {
                selectedGameItem = game;
                selectedHardwareItem = null;

                ClearGamesTreeSelection();
                ShowGameDetails(game);
            }
        }
        private void SaveHardwareButton_Click(object sender, RoutedEventArgs e)
        {
            Hardware hardware = isEditingHardware && editingHardwareItem != null
                ? editingHardwareItem
                : new Hardware();

            hardware.Photos = currentHardwarePhotos;

            if (HardwareManufacturerCombo.SelectedIndex <= 0)
            {
                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? "Wybierz producenta przed zapisaniem konsoli."
                        : "Select manufacturer before saving the console.",
                    "HGC",
                    HgcMessageBoxType.Warning,
                    currentLanguage);

                return;
            }

            bool isOtherManufacturer =
                HardwareManufacturerCombo.Text == (currentLanguage == "pl" ? "Inny" : "Other");

            string manufacturer = HardwareManufacturerCombo.Text;
            string family = HardwareTypeCombo.Text;
            string platform = isOtherManufacturer
                ? HardwarePlatformTextBox.Text.Trim()
                : HardwarePlatformCombo.Text;

            if (isOtherManufacturer)
            {
                manufacturer = HardwareCustomNameTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(manufacturer))
                {
                    HgcMessageBox.Show(
                        this,
                        currentLanguage == "pl"
                            ? "Wpisz nazwę producenta przed zapisaniem."
                            : "Enter manufacturer name before saving.",
                        "HGC",
                        HgcMessageBoxType.Warning,
                        currentLanguage);

                    return;
                }

                if (string.IsNullOrWhiteSpace(platform))
                {
                    HgcMessageBox.Show(
                        this,
                        currentLanguage == "pl"
                            ? "Wpisz nazwę platformy przed zapisaniem."
                            : "Enter platform name before saving.",
                        "HGC",
                        HgcMessageBoxType.Warning,
                        currentLanguage);

                    return;
                }
            }

            hardware.Manufacturer = manufacturer;
            hardware.Family = family;
            hardware.Platform = platform;

            if (!ValidateNameLength(GetHardwareDisplayName(hardware)))
                return;

            hardware.CustomName = HardwareCustomNameTextBox.Text.Trim();

            hardware.Model = GetComboOrTextValue(
                HardwareModelCombo,
                HardwareModelTextBox);

            hardware.Revision = GetComboOrTextValue(
                HardwareRevisionCombo,
                HardwareRevisionTextBox);

            hardware.BoardRevision = GetComboOrTextValue(
                HardwareBoardRevisionCombo,
                HardwareBoardRevisionTextBox);

            hardware.Region = GetComboOrTextValue(
                HardwareRegionCombo,
                HardwareRegionTextBox);

            hardware.Color = GetComboOrTextValue(
                HardwareColorCombo,
                HardwareColorTextBox);

            hardware.SpecialEdition = GetComboOrTextValue(
                HardwareEditionCombo,
                HardwareEditionTextBox);

            hardware.SerialNumber = HardwareSerialTextBox.Text.Trim();

            hardware.Condition =
                HardwareConditionCombo.Text ==
                    (currentLanguage == "pl"
                        ? "Wybierz stan sprzętu"
                        : "Select hardware condition")
                ? ""
                : NormalizeConditionForSave(HardwareConditionCombo.Text);

            hardware.HasBox = HardwareHasBoxCheckBox.IsChecked == true;

            hardware.BoxCondition =
                HardwareBoxConditionCombo.Text ==
                    (currentLanguage == "pl"
                        ? "Wybierz stan pudełka"
                        : "Select box condition")
                ? ""
                : NormalizeConditionForSave(HardwareBoxConditionCombo.Text);

            hardware.HasManual = HardwareHasManualCheckBox.IsChecked == true;
            hardware.HasInserts = HardwareHasInsertsCheckBox.IsChecked == true;
            hardware.HasPowerSupply = HardwareHasPowerSupplyCheckBox.IsChecked == true;
            hardware.HasCable = HardwareHasCableCheckBox.IsChecked == true;
            hardware.HasController = HardwareHasControllerCheckBox.IsChecked == true;
            hardware.HasMemoryCard = HardwareHasMemoryCardCheckBox.IsChecked == true;

            hardware.PurchaseDate = HardwarePurchaseDatePicker.SelectedDate;
            hardware.Notes = HardwareNotesTextBox.Text.Trim();

            if (decimal.TryParse(HardwarePurchasePriceTextBox.Text, out decimal price))
                hardware.PurchasePrice = price;
            else
                hardware.PurchasePrice = null;

            hardware.PurchaseCurrency =
                HardwarePurchaseCurrencyCombo.SelectedItem?.ToString()
                ?? defaultCurrency;

            if (isEditingHardware)
            {
                if (editingHardwareTreeItem != null)
                    editingHardwareTreeItem.Header = GetHardwareDisplayName(hardware);

                HgcMessageBox.Show(
                this,
                currentLanguage == "pl"
                    ? $"{GetHardwareDisplayName(hardware)} — zapisano zmiany."
                    : $"{GetHardwareDisplayName(hardware)} — changes saved.",
                "HGC",
                HgcMessageBoxType.Information,
                currentLanguage);
            }
            else
            {
                collection.HardwareItems.Add(hardware);
                AddHardwareRecordToTree(hardware);

                HgcMessageBox.Show(
                this,
                currentLanguage == "pl"
                    ? $"{GetHardwareDisplayName(hardware)} zostało dodane."
                    : $"{GetHardwareDisplayName(hardware)} has been added.",
                "HGC",
                HgcMessageBoxType.Information,
                currentLanguage);
            }

            isEditingHardware = false;
            editingHardwareItem = null;
            editingHardwareTreeItem = null;
            currentHardwarePhotos = new List<CollectionPhoto>();

            SaveHardwareButton.Content =
                currentLanguage == "pl" ? "Zapisz" : "Save";

            ShowDefaultRightPanel();
            RefreshStats();
            SaveCollectionChanges();
        }
        private void RemoveEmptyHardwareGroups()
        {
            for (int i = CollectionTree.Items.Count - 1; i >= 0; i--)
            {
                if (CollectionTree.Items[i] is TreeViewItem groupItem)
                {
                    string header = groupItem.Header?.ToString() ?? "";

                    if (header == "Archiwum" || header == "Archive")
                        continue;

                    if (groupItem.Items.Count == 0)
                        CollectionTree.Items.RemoveAt(i);
                }
            }
        }
        private void ShowDefaultRightPanel(bool resetBackground = true)
        {
            if (resetBackground)
            {
                currentPlatformGroup = "Default";
                UpdateBackgroundForCurrentState();
            }

            AddChoicePanel.Visibility = Visibility.Collapsed;
            AddHardwareFormPanel.Visibility = Visibility.Collapsed;
            AddGameFormPanel.Visibility = Visibility.Collapsed;
            PcDetailsPanel.Visibility = Visibility.Collapsed;
            AddPcFormPanel.Visibility = Visibility.Collapsed;
            HardwareDetailsViewPanel.Visibility = Visibility.Collapsed;
            AddServicePanel.Visibility = Visibility.Collapsed;
            RecordActionsPanel.Visibility = Visibility.Collapsed;

            if (RestoreRecordButton != null)
                RestoreRecordButton.Visibility = Visibility.Collapsed;

            CollectionTilesPanel.Visibility = Visibility.Collapsed;
            AddHardwareSubChoicePanel.Visibility = Visibility.Collapsed;

            PreviewPlaceholder.Visibility = Visibility.Visible;

            bool hasCollection =
                collection.HardwareItems.Count > 0 ||
                collection.Games.Count > 0 ||
                collection.PcSystems.Count > 0 ||
                collection.Accessories.Count > 0;

            if (hasCollection)
            {
                ShowCollectionTiles();
                return;
            }

            PreviewPlaceholder.Visibility = Visibility.Visible;

            PreviewTitle.Text =
                currentLanguage == "pl"
                    ? "Rozpocznij budowę swojej kolekcji"
                    : "Start building your collection";

            PreviewSubtitle.Text =
                currentLanguage == "pl"
                    ? "Dodaj pierwszą grę, konsolę lub komputer."
                    : "Add your first game, console or computer.";
        }
        private string GetHardwareDisplayName(Hardware hardware)
        {
            if (!string.IsNullOrWhiteSpace(hardware.CustomName))
                return hardware.CustomName;

            if (!string.IsNullOrWhiteSpace(hardware.Model))
            {
                if (hardware.Model == "Other / Custom" ||
                    hardware.Model == "Other" ||
                    hardware.Model == "Inny" ||
                    hardware.Model == "Niestandardowy")
                {
                    string customText =
                        currentLanguage == "pl"
                            ? "Niestandardowy"
                            : "Other / Custom";

                    return $"{hardware.Platform} {customText}".Trim();
                }

                return hardware.Model;
            }

            if (!string.IsNullOrWhiteSpace(hardware.Platform))
                return hardware.Platform;

            return currentLanguage == "pl" ? "Sprzęt" : "Hardware";
        }

        private void DeleteGamePermanently(Game game)
        {
            if (game == null)
                return;

            collection.Games.Remove(game);
            collection.ArchivedGames.Remove(game);

            RemoveGameFromTree(CollectionTree.Items, game);
            CleanupEmptyTreeBranches();

            bool archiveStillHasRecords =
                collection.ArchivedHardwareItems.Count > 0 ||
                collection.ArchivedGames.Count > 0 ||
                collection.ArchivedPcSystems.Count > 0 ||
                collection.ArchivedAccessories.Count > 0;

            if (!archiveStillHasRecords)
                CollapseTreeItems(CollectionTree.Items);

            BuildGamesPanel();
            RefreshStats();
            ShowDefaultRightPanel();
            SaveCollectionChanges();
        }
        // =========================
        // TREE SELECTION
        // =========================

        private void CollectionTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            isGamesViewActive = false;

            isClearingTreeSelection = true;
            try
            {
                if (GamesTree.SelectedItem is TreeViewItem selectedGameTreeItem)
                    selectedGameTreeItem.IsSelected = false;
            }
            finally
            {
                isClearingTreeSelection = false;
            }

            currentGameFilter = GameFilterMode.All;
            currentGameGenre = "";

            if (CollectionTree.SelectedItem is not TreeViewItem item)
                return;

            if (item.Tag is not TreeNodeData nodeData)
                return;

            collectionCurrentPage = 1;

            string selectedHeader = item.Header?.ToString() ?? "";

            if (AddGameFormPanel.Visibility == Visibility.Visible ||
                AddHardwareFormPanel.Visibility == Visibility.Visible ||
                AddPcFormPanel.Visibility == Visibility.Visible ||
                AddAccessoryFormPanel.Visibility == Visibility.Visible)
            {
                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? "Najpierw zapisz albo anuluj obecną czynność."
                        : "Finish or cancel the current operation before continuing.",
                    "HGC",
                    HgcMessageBoxType.Warning,
                    currentLanguage);

                return;
            }

            currentPlatformGroup =
                GetBackgroundGroupForSelectedTreeItem(item, nodeData);

            UpdateBackgroundForCurrentState();

            if (nodeData.NodeType == "Manufacturer")
            {
                ShowCollectionTiles(nodeData);
                return;
            }

            if (nodeData.NodeType == "ArchiveRoot")
            {
                ShowArchiveTiles();
                return;
            }

            if (nodeData.NodeType == "ArchiveHardwareCategory")
            {
                ShowArchiveHardwareTiles();
                return;
            }

            if (nodeData.NodeType == "ArchiveGameCategory")
            {
                ShowArchiveGameTiles();
                return;
            }

            if (nodeData.NodeType == "ArchivePcCategory")
            {
                ShowArchivePcTiles();
                return;
            }

            if (nodeData.NodeType == "ArchiveAccessoryCategory" ||
            nodeData.NodeType == "ArchiveAccessoryType")
            {
                ShowArchiveAccessoryTiles();
                return;
            }

            if (nodeData.NodeType == "ArchiveAccessoryRecord" &&
                nodeData.AccessoryItem != null)
            {
                selectedAccessoryItem = nodeData.AccessoryItem;
                ShowAccessoryDetails(selectedAccessoryItem);
                return;
            }

            if (nodeData.NodeType == "AccessoryRoot" ||
                 nodeData.NodeType == "AccessoriesRoot" ||
                 nodeData.NodeType == "AccessoryCategory" ||
                 nodeData.NodeType == "AccessoryType" ||
                 nodeData.NodeType == "AccessoryPlatform" ||
                 nodeData.NodeType == "AccessoryManufacturer" ||
                 nodeData.NodeType == "AccessoryFamily")
            {
                if (nodeData.NodeType == "AccessoryRoot" ||
                    nodeData.NodeType == "AccessoriesRoot")
                {
                    ShowCollectionTiles(new TreeNodeData
                    {
                        NodeType = "Category",
                        Category = currentLanguage == "pl"
                            ? "Akcesoria"
                            : "Accessories"
                    });
                }
                else
                {
                    ShowCollectionTiles(nodeData);
                }

                return;
            }

            if (nodeData.NodeType == "AccessoryRecord" &&
                nodeData.AccessoryItem != null)
            {
                selectedAccessoryItem = nodeData.AccessoryItem;
                selectedGameItem = null;
                selectedHardwareItem = null;
                selectedPcItem = null;

                ShowAccessoryDetails(selectedAccessoryItem);
                return;
            }

            if (nodeData.NodeType == "ArchiveHardwareRecord" &&
                nodeData.HardwareItem != null)
            {
                selectedHardwareItem = nodeData.HardwareItem;
                selectedAccessoryItem = null;
                selectedGameItem = null;
                selectedPcItem = null;

                ShowHardwareDetails(selectedHardwareItem);
                return;
            }

            if (nodeData.NodeType == "ArchiveGameRecord" &&
                nodeData.GameItem != null)
            {
                selectedGameItem = nodeData.GameItem;
                selectedHardwareItem = null;
                selectedAccessoryItem = null;
                selectedPcItem = null;

                ShowGameDetails(selectedGameItem);
                return;
            }

            if (nodeData.NodeType == "ArchivePcRecord" &&
                nodeData.PcItem != null)
            {
                selectedPcItem = nodeData.PcItem;
                selectedGameItem = null;
                selectedHardwareItem = null;
                selectedAccessoryItem = null;

                ShowPcDetails(selectedPcItem);
                return;
            }

            if (nodeData.NodeType == "HardwareRecord" &&
                nodeData.HardwareItem != null)
            {
                selectedHardwareItem = nodeData.HardwareItem;
                ShowHardwareDetails(selectedHardwareItem);
                return;
            }

            if (nodeData.NodeType == "GameRecord" &&
                nodeData.GameItem != null)
            {
                selectedGameItem = nodeData.GameItem;
                selectedHardwareItem = null;

                ShowGameDetails(selectedGameItem);
                return;
            }

            if (nodeData.NodeType == "PcRecord" &&
                nodeData.PcItem != null)
            {
                selectedPcItem = nodeData.PcItem;
                selectedGameItem = null;
                selectedHardwareItem = null;

                ShowPcDetails(selectedPcItem);
                return;
            }

            if (nodeData.NodeType == "Family" ||
                nodeData.NodeType == "Platform" ||
                nodeData.NodeType == "Category")
            {
                ShowCollectionTiles(nodeData);
                return;
            }

            if (nodeData.NodeType == "PcRoot" ||
                nodeData.NodeType == "PcCategory" ||
                nodeData.NodeType == "PcType")
            {
                ShowPcCollectionTiles(nodeData);
                return;
            }

            if (nodeData.NodeType == "ArchiveAccessoryRecord" &&
                nodeData.AccessoryItem != null)
            {
                selectedAccessoryItem = nodeData.AccessoryItem;
                ShowAccessoryDetails(selectedAccessoryItem);
                return;
            }

            ShowDefaultRightPanel();
        }
        private void ShowArchiveAccessoryTiles()
        {
            HideRightPanels();

            CollectionTilesPanel.Visibility = Visibility.Visible;
            CollectionTilesSectionsPanel.Children.Clear();
            currentCollectionTilesWrap = null;

            CollectionTilesTitle.Text =
                currentLanguage == "pl"
                    ? "Archiwum - akcesoria"
                    : "Archive - accessories";

            if (collection.ArchivedAccessories.Count == 0)
                return;

            AddTilesSectionTitle(
                currentLanguage == "pl" ? "Akcesoria" : "Accessories");

            foreach (var accessory in collection.ArchivedAccessories)
                AddAccessoryTile(accessory);
        }
        private void ShowPcCollectionTiles(TreeNodeData nodeData)
        {
            HideRightPanels();

            RecentlyAddedBorder.Visibility = Visibility.Collapsed;

            CollectionTilesPanel.Visibility = Visibility.Visible;
            CollectionTilesSectionsPanel.Children.Clear();
            currentCollectionTilesWrap = null;

            bool isComputersCategory =
                nodeData.NodeType == "PcCategory" &&
                (nodeData.Category == "Komputery" ||
                 nodeData.Category == "Computers");

            bool isGamesCategory =
                nodeData.NodeType == "PcCategory" &&
                (nodeData.Category == "Gry" ||
                 nodeData.Category == "Games");

            bool isPcType =
                nodeData.NodeType == "PcType";

            CollectionTilesTitle.Text = "PC";

            var pcItems = collection.PcSystems.ToList();

            var pcGameItems = collection.Games
                .Where(x => x.Manufacturer == "PC")
                .ToList();

            if (isComputersCategory)
                pcGameItems.Clear();

            if (isGamesCategory)
                pcItems.Clear();

            if (isPcType)
            {
                pcGameItems.Clear();

                pcItems = pcItems
                    .Where(x => x.ComputerType == nodeData.Category)
                    .ToList();
            }

            if (pcItems.Count > 0)
            {
                AddTilesSectionTitle(
                    currentLanguage == "pl" ? "Komputery" : "Computers");

                foreach (var pc in pcItems)
                    AddPcTile(pc);
            }

            if (pcGameItems.Count > 0)
            {
                AddTilesSectionTitle(
                    currentLanguage == "pl"
                        ? $"Gry ({pcGameItems.Count})"
                        : $"Games ({pcGameItems.Count})");

                foreach (var game in pcGameItems)
                    AddGameTile(game);
            }
        }
        private void AddPcTile(PcSystem pc)
        {
            double tileWidth = GetCollectionTileWidth();
            double tileHeight = GetCollectionHardwareTileHeight();
            double imageSize = GetCollectionTileImageSize();
            double imageHeight = GetCollectionHardwareImageHeight();

            Button tile = new Button
            {
                Width = tileWidth,
                Height = tileHeight,
                Margin = new Thickness(0, 0, 16, 12),
                Tag = pc
            };

            tile.Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 8,
                ShadowDepth = 5,
                Opacity = 0.45
            };

            StackPanel panel = new StackPanel();

            Image image = new Image
            {
                Width = imageSize,
                Height = imageHeight,
                Opacity = 0.85,
                Stretch = Stretch.Uniform
            };

            image.Loaded += (s, e) =>
            {
                image.Clip = new RectangleGeometry(
                    new Rect(0, 0, image.ActualWidth, image.ActualHeight),
                    7,
                    7);
            };

            CollectionPhoto mainPhoto =
                pc.Photos?.FirstOrDefault(x => x.IsMain)
                ?? pc.Photos?.FirstOrDefault();

            if (mainPhoto != null &&
                File.Exists(mainPhoto.OriginalPath))
            {
                image.Source = LoadImageWithoutLock(mainPhoto.OriginalPath, 220);
            }
            else
            {
                image.Source = new BitmapImage(
                    new Uri("pack://application:,,,/Assets/Icons/nophoto.png"));
                image.Opacity = 0.55;
            }

            TextBlock title = new TextBlock
            {
                Text = GetTileDisplayName(pc.Name),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                TextTrimming = TextTrimming.CharacterEllipsis,
                MaxHeight = 40,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)FindResource("ButtonTextBrush"),
                Margin = new Thickness(4, 0, 4, 0),
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 4,
                    ShadowDepth = 1,
                    Opacity = 0.45
                }
            };

            Border imageContainer = new Border
            {
                CornerRadius = new CornerRadius(4),
                Margin = new Thickness(0, 0, 0, 6),
                HorizontalAlignment = HorizontalAlignment.Center,
                Child = image,
                Width = imageSize,
                Height = imageHeight,
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 10,
                    ShadowDepth = 6,
                    Opacity = 0.45
                }
            };

            panel.Children.Add(imageContainer);
            panel.Children.Add(title);

            tile.Content = panel;
            tile.Click += PcTile_Click;

            currentCollectionTilesWrap?.Children.Add(tile);
        }
        private void PcTile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button &&
                button.Tag is PcSystem pc)
            {
                selectedPcItem = pc;
                selectedHardwareItem = null;
                selectedGameItem = null;

                ShowPcDetails(pc);
            }
        }
        private void ShowArchiveTiles()
        {
            HideRightPanels();
            RecentlyAddedBorder.Visibility = Visibility.Collapsed;

            CollectionTilesPanel.Visibility = Visibility.Visible;
            CollectionTilesSectionsPanel.Children.Clear();
            currentCollectionTilesWrap = null;

            CollectionTilesTitle.Text =
                currentLanguage == "pl" ? "Archiwum" : "Archive";

            var archivedHardware = collection.ArchivedHardwareItems.ToList();
            var archivedGames = collection.ArchivedGames.ToList();
            var archivedPcs = collection.ArchivedPcSystems.ToList();
            var archivedAccessories = collection.ArchivedAccessories.ToList();

            if (archivedHardware.Count > 0)
            {
                AddTilesSectionTitle(currentLanguage == "pl" ? "Sprzęt" : "Hardware");

                foreach (var hardware in archivedHardware)
                    AddHardwareTile(hardware);
            }

            if (archivedGames.Count > 0)
            {
                AddTilesSectionTitle(currentLanguage == "pl" ? "Gry" : "Games");

                foreach (var game in archivedGames)
                    AddGameTile(game);
            }

            if (archivedPcs.Count > 0)
            {
                AddTilesSectionTitle("PC");

                foreach (var pc in archivedPcs)
                    AddPcTile(pc);
            }

            if (archivedAccessories.Count > 0)
            {
                AddTilesSectionTitle(
                    currentLanguage == "pl" ? "Akcesoria" : "Accessories");

                foreach (var accessory in archivedAccessories)
                    AddAccessoryTile(accessory);
            }
        }

        private void ShowArchiveHardwareTiles()
        {
            HideRightPanels();
            RecentlyAddedBorder.Visibility = Visibility.Collapsed;

            CollectionTilesPanel.Visibility = Visibility.Visible;
            CollectionTilesSectionsPanel.Children.Clear();
            currentCollectionTilesWrap = null;

            CollectionTilesTitle.Text =
                currentLanguage == "pl" ? "Archiwum - sprzęt" : "Archive - Hardware";

            if (collection.ArchivedHardwareItems.Count == 0)
                return;

            AddTilesSectionTitle(currentLanguage == "pl" ? "Sprzęt" : "Hardware");

            foreach (var hardware in collection.ArchivedHardwareItems.ToList())
                AddHardwareTile(hardware);
        }

        private void ShowArchiveGameTiles()
        {
            HideRightPanels();
            RecentlyAddedBorder.Visibility = Visibility.Collapsed;

            CollectionTilesPanel.Visibility = Visibility.Visible;
            CollectionTilesSectionsPanel.Children.Clear();
            currentCollectionTilesWrap = null;

            CollectionTilesTitle.Text =
                currentLanguage == "pl" ? "Archiwum - gry" : "Archive - Games";

            if (collection.ArchivedGames.Count == 0)
                return;

            AddTilesSectionTitle(currentLanguage == "pl" ? "Gry" : "Games");

            foreach (var game in collection.ArchivedGames.ToList())
                AddGameTile(game);
        }

        private void ShowArchivePcTiles()
        {
            HideRightPanels();
            RecentlyAddedBorder.Visibility = Visibility.Collapsed;

            CollectionTilesPanel.Visibility = Visibility.Visible;
            CollectionTilesSectionsPanel.Children.Clear();
            currentCollectionTilesWrap = null;

            CollectionTilesTitle.Text =
                currentLanguage == "pl" ? "Archiwum - PC" : "Archive - PC";

            if (collection.ArchivedPcSystems.Count == 0)
                return;

            AddTilesSectionTitle("PC");

            foreach (var pc in collection.ArchivedPcSystems.ToList())
                AddPcTile(pc);
        }
        private void UpdateGameDetailsRatingHeader(Game game)
        {
            GameDetailsRatingHeaderPanel.Children.Clear();

            if (!game.UserRating.HasValue)
            {
                GameDetailsRatingHeaderPanel.Visibility = Visibility.Collapsed;
                return;
            }

            GameDetailsRatingHeaderPanel.Visibility = Visibility.Visible;

            double rating = game.UserRating.Value;

            Color accent =
                ((SolidColorBrush)FindResource("AppAccentBrush")).Color;

            Brush filledBrush = new LinearGradientBrush(
                Shift(accent, +0.25),
                Shift(accent, -0.15),
                90);

            Brush emptyBrush = new SolidColorBrush(
                Color.FromArgb(45, 255, 255, 255));

            for (int i = 1; i <= 10; i++)
            {
                double fillAmount;

                if (rating >= i)
                    fillAmount = 1.0;
                else if (rating >= i - 0.5)
                    fillAmount = 0.6;
                else
                    fillAmount = 0.0;

                Grid starGrid = new Grid
                {
                    Width = 18,
                    Height = 22,
                    Margin = new Thickness(1, 0, 1, 0)
                };

                TextBlock emptyStar = new TextBlock
                {
                    Text = "★",
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Foreground = emptyBrush,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                TextBlock filledStar = new TextBlock
                {
                    Text = "★",
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Foreground = filledBrush,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                if (fillAmount < 1.0)
                {
                    filledStar.Clip = new RectangleGeometry(
                        new Rect(0, 0, 14 * fillAmount, 22));
                }

                starGrid.Children.Add(emptyStar);

                if (fillAmount > 0)
                    starGrid.Children.Add(filledStar);

                GameDetailsRatingHeaderPanel.Children.Add(starGrid);
            }

            GameDetailsRatingHeaderPanel.Children.Add(new TextBlock
            {
                Text = $"{rating:0.0}/10",
                Margin = new Thickness(8, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)FindResource("AppTextBrush"),
                Opacity = 0.85
            });
        }
        private void ShowPcDetails(PcSystem pc)
        {
            GameShortDescriptionBorder.Visibility = Visibility.Collapsed;
            GameShortDescriptionText.Text = "";

            if (pc == null)
                return;

            HideRightPanels();

            PcDetailsPanel.Visibility = Visibility.Visible;

            ApplyPcDetailsLanguage();

            selectedPcItem = pc;

            if (pc.IsArchived)
            {
                ArchivePcButton.Visibility = Visibility.Visible;
                ArchivePcButton.Content =
                    currentLanguage == "pl"
                        ? "Przywróć"
                        : "Restore";
            }
            else
            {
                ArchivePcButton.Visibility = Visibility.Visible;
                ArchivePcButton.Content =
                    currentLanguage == "pl"
                        ? "Archiwizuj"
                        : "Archive";
            }

            PcDetailsTitle.Text = pc.Name;

            ApplyPcCardOpacity(pc);

            PcCardOpacityButton.Content =
            currentLanguage == "pl"
                ? "Przezroczystość ▼"
                : "Opacity ▼";

            PcAddPageBackgroundButton.Content =
                currentLanguage == "pl"
                    ? "Tło"
                    : "Background";

            ApplyRecordBackground();
            UpdatePageBackgroundButtonText();                      

            PcDetailsContentPanel.Children.Clear();

            PcComponentsPanel.Children.Clear();

            AddPcComponentExpandableSection(
                currentLanguage == "pl" ? "Procesor" : "Processor",
                pc.Cpus,
                GetCpuSummaryText,
                GetCpuDetailsText);

            if (pc.Motherboard != null)
            {
                AddPcComponentExpandableSection(
                    currentLanguage == "pl"
                        ? "Płyta główna"
                        : "Motherboard",
                    new[] { pc.Motherboard },
                    GetMotherboardSummaryText,
                    GetMotherboardDetailsText);
            }

            AddPcComponentExpandableSection(
                currentLanguage == "pl" ? "Karty graficzne" : "Graphics cards",
                pc.Gpus,
                GetGpuSummaryText,
                GetGpuDetailsText);

            AddPcComponentExpandableSection(
                currentLanguage == "pl" ? "Pamięć RAM" : "Memory RAM",
                pc.RamModules,
                GetRamSummaryText,
                GetRamDetailsText);

            AddPcComponentExpandableSection(
                currentLanguage == "pl" ? "Dyski" : "Storage",
                pc.StorageDevices,
                GetStorageSummaryText,
                GetStorageDetailsText);

            AddPcComponentExpandableSection(
                currentLanguage == "pl" ? "Zasilacze" : "Power supplies",
                pc.PowerSupplies,
                GetPsuSummaryText,
                GetPsuDetailsText);

            if (pc.Case != null)
            {
                AddPcComponentExpandableSection(
                    currentLanguage == "pl" ? "Obudowa" : "Case",
                    new[] { pc.Case },
                    GetCaseSummaryText,
                    GetCaseDetailsText);
            }
            if (pc.Cooling != null)
            {
                AddPcComponentExpandableSection(
                    currentLanguage == "pl" ? "Chłodzenie" : "Cooling",
                    new[] { pc.Cooling },
                    GetCoolingDisplayText,
                    GetCoolingDetailsText);
            }

            PcComponentsBorder.Visibility =
            PcComponentsPanel.Children.Count > 0
                ? Visibility.Visible
                : Visibility.Collapsed;

            AddPcDetailRow(
                currentLanguage == "pl" ? "Typ komputera" : "Computer type",
                pc.ComputerType);

            AddPcDetailRow(
                currentLanguage == "pl" ? "Typ przenośnego" : "Portable type",
                pc.PortableType);

            if (!string.IsNullOrWhiteSpace(pc.Manufacturer) &&
            pc.Manufacturer != "Custom Build")
            {
                AddPcDetailRow(
                    currentLanguage == "pl" ? "Producent" : "Manufacturer",
                    pc.Manufacturer);
            }

            if (!string.IsNullOrWhiteSpace(pc.Model))
            {
                AddPcDetailRow(
                    currentLanguage == "pl" ? "Model" : "Model",
                    pc.Model);
            }

            if (!string.IsNullOrWhiteSpace(pc.SerialNumber))
            {
                AddPcDetailRow(
                    currentLanguage == "pl" ? "Numer seryjny" : "Serial number",
                    pc.SerialNumber);
            }

            string operatingSystem =
                pc.OperatingSystem;

            if (!string.IsNullOrWhiteSpace(pc.OperatingSystemEdition))
                operatingSystem += " " + pc.OperatingSystemEdition;

            if (!string.IsNullOrWhiteSpace(pc.OperatingSystemRevision))
                operatingSystem += " " + pc.OperatingSystemRevision;

            AddPcDetailRow(
                currentLanguage == "pl"
                    ? "System operacyjny"
                    : "Operating system",
                operatingSystem);

            AddPcDetailRow(
                currentLanguage == "pl"
                    ? "Data zakupu"
                    : "Purchase date",
                pc.PurchaseDate?.ToShortDateString());

            AddPcDetailRow(
                currentLanguage == "pl"
                    ? "Cena zakupu"
                    : "Purchase price",
                pc.PurchasePrice.HasValue
                    ? $"{pc.PurchasePrice.Value:0.00} {pc.PurchaseCurrency}"
                    : "");

            PcDetailsContentBorder.Visibility =
            PcDetailsContentPanel.Children.Count > 0
                ? Visibility.Visible
                : Visibility.Collapsed;

            PcPhotosPlaceholderText.Text =
                currentLanguage == "pl"
                    ? "Zdjęcia komputera"
                    : "PC photos";

            LoadPcPhotosPreview(pc);
        }
        private string GetCoolingDetailsText(PcCooling cooling)
        {
            List<string> lines = new();

            if (cooling.StandardCooling.CpuCooling.CoolingType == "AIO")
            {
                var aio = cooling.StandardCooling.CpuCooling.AioCooling;

                if (!string.IsNullOrWhiteSpace(aio.Notes))
                {
                    lines.Add(
                        currentLanguage == "pl"
                            ? $"Informacje AIO: {aio.Notes}"
                            : $"AIO information: {aio.Notes}");
                }
            }

            return string.Join(Environment.NewLine, lines);
        }
        private string GetMotherboardDisplayText(
        PcMotherboard motherboard)
        {
            List<string> lines = new();

            lines.Add(
                $"{motherboard.Manufacturer} {motherboard.Model}");

            if (motherboard.HasRevision &&
                !string.IsNullOrWhiteSpace(motherboard.Revision))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Rewizja: {motherboard.Revision}"
                        : $"Revision: {motherboard.Revision}");
            }

            if (motherboard.HasSerialNumber &&
                !string.IsNullOrWhiteSpace(
                    motherboard.SerialNumber))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Numer seryjny: {motherboard.SerialNumber}"
                        : $"Serial number: {motherboard.SerialNumber}");
            }

            if (motherboard.PurchaseDate.HasValue)
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Data zakupu: {motherboard.PurchaseDate.Value:yyyy-MM-dd}"
                        : $"Purchase date: {motherboard.PurchaseDate.Value:yyyy-MM-dd}");
            }

            if (motherboard.PurchasePrice.HasValue)
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Cena zakupu: {motherboard.PurchasePrice.Value:0.00}"
                        : $"Purchase price: {motherboard.PurchasePrice.Value:0.00}");
            }

            return string.Join(
                Environment.NewLine,
                lines);
        }
        private string GetCoolingDisplayText(PcCooling cooling)
        {
            List<string> lines = new();

            bool isCustomWaterLoop =
                cooling.CoolingMode == "CustomWaterLoop";

            if (isCustomWaterLoop)
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? "Własne chłodzenie wodne"
                        : "Custom Water Cooling");

                AddWaterLoopLine(lines, currentLanguage == "pl" ? "Blok CPU" : "CPU block", cooling.CustomWaterLoop.CpuBlock);
                AddWaterLoopLine(lines, currentLanguage == "pl" ? "Blok GPU" : "GPU block", cooling.CustomWaterLoop.GpuBlock);
                AddWaterLoopLine(lines, currentLanguage == "pl" ? "Blok płyty głównej" : "Motherboard block", cooling.CustomWaterLoop.MotherboardBlock);
                AddWaterLoopLine(lines, currentLanguage == "pl" ? "Blok RAM" : "RAM block", cooling.CustomWaterLoop.RamBlock);
                AddWaterLoopLine(lines, currentLanguage == "pl" ? "Blok dysków" : "Storage block", cooling.CustomWaterLoop.StorageBlock);
                AddWaterLoopLine(lines, currentLanguage == "pl" ? "Blok PSU" : "PSU block", cooling.CustomWaterLoop.PsuBlock);
            }
            else
            {
                AddCoolingLine(lines, "CPU", cooling.StandardCooling.CpuCooling.CoolingType);

                if (cooling.StandardCooling.CpuCooling.CoolingType == "AIO")
                {
                    var aio = cooling.StandardCooling.CpuCooling.AioCooling;

                    string aioName =
                        $"{aio.Manufacturer} {aio.Model}".Trim();

                    if (!string.IsNullOrWhiteSpace(aioName))
                        lines.Add($"AIO: {aioName}");
                }
                AddCoolingLine(lines, "GPU", cooling.StandardCooling.GpuCooling.CoolingType);
                AddCoolingLine(lines, currentLanguage == "pl" ? "Płyta główna" : "Motherboard", cooling.StandardCooling.MotherboardCooling.CoolingType);
                AddCoolingLine(lines, "RAM", cooling.StandardCooling.RamCooling.CoolingType);
                AddCoolingLine(lines, currentLanguage == "pl" ? "Dyski" : "Storage", cooling.StandardCooling.StorageCooling.CoolingType);
                AddCoolingLine(lines, currentLanguage == "pl" ? "Zasilacz" : "PSU", cooling.StandardCooling.PsuCooling.CoolingType);

                if (cooling.StandardCooling.CaseCooling.FanGroups.Count > 0)
                {
                    lines.Add(
                        currentLanguage == "pl"
                            ? $"Obudowa: {cooling.StandardCooling.CaseCooling.FanGroups.Count} sekcji wentylatorów"
                            : $"Case: {cooling.StandardCooling.CaseCooling.FanGroups.Count} fan sections");
                }
            }

            if (lines.Count == 0)
            {
                return currentLanguage == "pl"
                    ? "Dodano chłodzenie"
                    : "Cooling added";
            }

            return string.Join(Environment.NewLine, lines);
        }

        private void AddCoolingLine(
        List<string> lines,
        string label,
        string value)
        {
            if (string.IsNullOrWhiteSpace(value) ||
                value == "NotSpecified")
                return;

            lines.Add($"{label}: {GetCoolingValueDisplayText(value)}");
        }
        private string GetCoolingValueDisplayText(string value)
        {
            return value switch
            {
                "AirBoxOem" => currentLanguage == "pl" ? "Powietrzne Box / OEM" : "Air Box / OEM",
                "Air" => currentLanguage == "pl" ? "Powietrzne" : "Air",
                "AIO" => "AIO",
                "Passive" => currentLanguage == "pl" ? "Pasywne" : "Passive",
                "Peltier" => "Peltier",
                "LN2" => "LN2",
                "Other" => currentLanguage == "pl" ? "Inne" : "Other",

                "Stock" => currentLanguage == "pl" ? "Fabryczne" : "Stock",
                "Custom" => currentLanguage == "pl" ? "Niestandardowe" : "Custom",

                "None" => currentLanguage == "pl" ? "Brak" : "None",
                "Active" => currentLanguage == "pl" ? "Aktywne" : "Active",
                "HybridAio" => currentLanguage == "pl" ? "Hybrydowe AIO" : "Hybrid AIO",

                _ => value
            };
        }

        private void AddWaterLoopLine(
            List<string> lines,
            string label,
            PcWaterLoopComponent component)
        {
            if (component == null ||
                !component.IsUsed)
                return;

            string value =
                $"{component.Manufacturer} {component.Model}".Trim();

            if (string.IsNullOrWhiteSpace(value))
                value = currentLanguage == "pl" ? "dodany" : "added";

            lines.Add($"{label}: {value}");
        }
        private string GetCpuDisplayText(PcCpu cpu)
        {
            List<string> lines = new();

            lines.Add($"{cpu.Manufacturer} {cpu.Model}");

            if (cpu.HasSerialNumber &&
                !string.IsNullOrWhiteSpace(cpu.SerialNumber))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Numer seryjny: {cpu.SerialNumber}"
                        : $"Serial number: {cpu.SerialNumber}");
            }

            if (cpu.HasOverclock)
            {
                if (!string.IsNullOrWhiteSpace(cpu.CurrentClock))
                {
                    lines.Add(
                        currentLanguage == "pl"
                            ? $"Aktualny zegar: {cpu.CurrentClock}"
                            : $"Current clock: {cpu.CurrentClock}");
                }

                if (!string.IsNullOrWhiteSpace(cpu.CurrentVoltage))
                {
                    lines.Add(
                        currentLanguage == "pl"
                            ? $"Aktualne napięcie: {cpu.CurrentVoltage}"
                            : $"Current voltage: {cpu.CurrentVoltage}");
                }
            }

            if (cpu.PurchaseDate.HasValue)
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Data zakupu: {cpu.PurchaseDate.Value:yyyy-MM-dd}"
                        : $"Purchase date: {cpu.PurchaseDate.Value:yyyy-MM-dd}");
            }

            if (cpu.PurchasePrice.HasValue)
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Cena zakupu: {cpu.PurchasePrice.Value:0.00}"
                        : $"Purchase price: {cpu.PurchasePrice.Value:0.00}");
            }

            return string.Join(Environment.NewLine, lines);
        }
        private string GetCpuSummaryText(PcCpu cpu)
        {
            return $"{cpu.Manufacturer} {cpu.Model}".Trim();
        }

        private string GetCpuDetailsText(PcCpu cpu)
        {
            List<string> lines = new();

            if (cpu.HasSerialNumber &&
                !string.IsNullOrWhiteSpace(cpu.SerialNumber))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Numer seryjny: {cpu.SerialNumber}"
                        : $"Serial number: {cpu.SerialNumber}");
            }

            if (cpu.HasOverclock)
            {
                if (!string.IsNullOrWhiteSpace(cpu.CurrentClock))
                {
                    lines.Add(
                        currentLanguage == "pl"
                            ? $"Aktualny zegar: {cpu.CurrentClock}"
                            : $"Current clock: {cpu.CurrentClock}");
                }

                if (!string.IsNullOrWhiteSpace(cpu.CurrentVoltage))
                {
                    lines.Add(
                        currentLanguage == "pl"
                            ? $"Aktualne napięcie: {cpu.CurrentVoltage}"
                            : $"Current voltage: {cpu.CurrentVoltage}");
                }
            }

            if (cpu.PurchaseDate.HasValue)
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Data zakupu: {cpu.PurchaseDate.Value:yyyy-MM-dd}"
                        : $"Purchase date: {cpu.PurchaseDate.Value:yyyy-MM-dd}");
            }

            if (cpu.PurchasePrice.HasValue)
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Cena zakupu: {cpu.PurchasePrice.Value:0.00}"
                        : $"Purchase price: {cpu.PurchasePrice.Value:0.00}");
            }

            return string.Join(Environment.NewLine, lines);
        }
        private string GetMotherboardSummaryText(PcMotherboard motherboard)
        {
            return $"{motherboard.Manufacturer} {motherboard.Model}".Trim();
        }

        private string GetMotherboardDetailsText(PcMotherboard motherboard)
        {
            List<string> lines = new();

            if (motherboard.HasChipset &&
                !string.IsNullOrWhiteSpace(motherboard.Chipset))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Chipset: {motherboard.Chipset}"
                        : $"Chipset: {motherboard.Chipset}");
            }

            if (motherboard.HasBiosVersion &&
                !string.IsNullOrWhiteSpace(motherboard.BiosVersion))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"BIOS: {motherboard.BiosVersion}"
                        : $"BIOS: {motherboard.BiosVersion}");
            }

            if (motherboard.HasRevision &&
                !string.IsNullOrWhiteSpace(motherboard.Revision))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Rewizja: {motherboard.Revision}"
                        : $"Revision: {motherboard.Revision}");
            }

            if (motherboard.HasSerialNumber &&
                !string.IsNullOrWhiteSpace(motherboard.SerialNumber))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Numer seryjny: {motherboard.SerialNumber}"
                        : $"Serial number: {motherboard.SerialNumber}");
            }

            if (motherboard.PurchaseDate.HasValue)
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Data zakupu: {motherboard.PurchaseDate.Value:yyyy-MM-dd}"
                        : $"Purchase date: {motherboard.PurchaseDate.Value:yyyy-MM-dd}");
            }

            if (motherboard.PurchasePrice.HasValue)
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Cena zakupu: {motherboard.PurchasePrice.Value:0.00}"
                        : $"Purchase price: {motherboard.PurchasePrice.Value:0.00}");
            }

            return string.Join(Environment.NewLine, lines);
        }
        private string GetGpuSummaryText(PcGpu gpu)
        {
            List<string> parts = new();

            if (!string.IsNullOrWhiteSpace(gpu.CardManufacturer))
                parts.Add(gpu.CardManufacturer);

            if (!string.IsNullOrWhiteSpace(gpu.GpuManufacturer))
                parts.Add(gpu.GpuManufacturer);

            if (!string.IsNullOrWhiteSpace(gpu.Model))
                parts.Add(gpu.Model);

            if (!string.IsNullOrWhiteSpace(gpu.VramAmount))
                parts.Add($"({gpu.VramAmount})");

            return string.Join(" ", parts);
        }

        private string GetGpuDetailsText(PcGpu gpu)
        {
            List<string> lines = new();

            if (gpu.HasSerialNumber &&
                !string.IsNullOrWhiteSpace(gpu.SerialNumber))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Numer seryjny: {gpu.SerialNumber}"
                        : $"Serial number: {gpu.SerialNumber}");
            }

            if (!string.IsNullOrWhiteSpace(gpu.VramType))
                lines.Add($"VRAM type: {gpu.VramType}");

            if (!string.IsNullOrWhiteSpace(gpu.VramBusWidth))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Szyna VRAM: {gpu.VramBusWidth}"
                        : $"VRAM bus: {gpu.VramBusWidth}");
            }

            if (!string.IsNullOrWhiteSpace(gpu.VramClock))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Taktowanie VRAM: {gpu.VramClock}"
                        : $"VRAM clock: {gpu.VramClock}");
            }

            if (!string.IsNullOrWhiteSpace(gpu.CoreClock))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Taktowanie rdzenia: {gpu.CoreClock}"
                        : $"Core clock: {gpu.CoreClock}");
            }

            string interfaceText = "";

            if (!string.IsNullOrWhiteSpace(gpu.InterfaceVersion))
                interfaceText = gpu.InterfaceVersion;
            else if (!string.IsNullOrWhiteSpace(gpu.InterfaceType))
                interfaceText = gpu.InterfaceType;

            if (!string.IsNullOrWhiteSpace(interfaceText))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Interfejs: {interfaceText}"
                        : $"Interface: {interfaceText}");
            }

            if (gpu.HasOverclock)
            {
                if (!string.IsNullOrWhiteSpace(gpu.OverclockCoreClock))
                {
                    lines.Add(
                        currentLanguage == "pl"
                            ? $"Aktualne taktowanie rdzenia: {gpu.OverclockCoreClock}"
                            : $"Current core clock: {gpu.OverclockCoreClock}");
                }

                if (!string.IsNullOrWhiteSpace(gpu.OverclockMemoryClock))
                {
                    lines.Add(
                        currentLanguage == "pl"
                            ? $"Aktualne taktowanie pamięci: {gpu.OverclockMemoryClock}"
                            : $"Current memory clock: {gpu.OverclockMemoryClock}");
                }

                if (!string.IsNullOrWhiteSpace(gpu.PowerLimit))
                {
                    lines.Add(
                        currentLanguage == "pl"
                            ? $"Limit mocy: {gpu.PowerLimit}"
                            : $"Power limit: {gpu.PowerLimit}");
                }
            }

            if (gpu.HasCoreDetails)
            {
                AddGpuDetailLine(lines, "ROP", gpu.Rops);
                AddGpuDetailLine(lines, "TMU", gpu.Tmus);
                AddGpuDetailLine(lines, "VS", gpu.VertexShaders);
                AddGpuDetailLine(lines, "PS", gpu.PixelShaders);
                AddGpuDetailLine(lines, "Unified Shaders", gpu.UnifiedShaders);
                AddGpuDetailLine(lines, "CUDA", gpu.CudaCores);
                AddGpuDetailLine(lines, "Stream Processors", gpu.StreamProcessors);
                AddGpuDetailLine(lines, "RT Units", gpu.RtUnits);
                AddGpuDetailLine(lines, "Tensor Units", gpu.TensorUnits);
                AddGpuDetailLine(lines, currentLanguage == "pl" ? "Inne" : "Other", gpu.OtherCoreDetails);
            }

            if (gpu.PurchaseDate.HasValue)
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Data zakupu: {gpu.PurchaseDate.Value:yyyy-MM-dd}"
                        : $"Purchase date: {gpu.PurchaseDate.Value:yyyy-MM-dd}");
            }

            if (gpu.PurchasePrice.HasValue)
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Cena zakupu: {gpu.PurchasePrice.Value:0.00}"
                        : $"Purchase price: {gpu.PurchasePrice.Value:0.00}");
            }

            return string.Join(Environment.NewLine, lines);
        }

        private string GetRamSummaryText(PcRam ram)
        {
            return $"{ram.Manufacturer} {ram.Model} {ram.TotalCapacity} {ram.MemoryType}".Trim();
        }

        private string GetRamDetailsText(PcRam ram)
        {
            string timings = JoinFilledLines(
                FieldLine("CL", ram.CL),
                FieldLine("tRCD", ram.TRCD),
                FieldLine("tRP", ram.TRP),
                FieldLine("tRAS", ram.TRAS),
                FieldLine("tRC", ram.TRC),
                FieldLine("CR", ram.CommandRate));

            return JoinFilledLines(
                FieldLine(currentLanguage == "pl" ? "Moduły" : "Modules", ram.ModuleCount),
                FieldLine(currentLanguage == "pl" ? "Taktowanie" : "Clock", ram.Clock),
                FieldLine(currentLanguage == "pl" ? "Napięcie" : "Voltage", ram.Voltage),
                FieldLine(currentLanguage == "pl" ? "Profil" : "Profile", ram.Profile),
                string.IsNullOrWhiteSpace(timings)
                    ? ""
                    : (currentLanguage == "pl" ? "Timingi:\n" : "Timings:\n") + timings,
                FieldLine("ECC", ram.IsEcc),
                FieldLine("Registered / Buffered", ram.IsRegistered));
        }
        private string GetStorageSummaryText(PcStorage storage)
        {
            return $"{storage.Manufacturer} {storage.Model} {storage.Capacity} {storage.DriveType}".Trim();
        }

        private string GetStorageDetailsText(PcStorage storage)
        {
            return JoinFilledLines(
                FieldLine(currentLanguage == "pl" ? "Interfejs" : "Interface", storage.Interface),
                FieldLine(currentLanguage == "pl" ? "Format" : "Form factor", storage.FormFactor),
                FieldLine("RPM", storage.Rpm),
                FieldLine("Cache", storage.Cache),
                FieldLine(currentLanguage == "pl" ? "Kontroler" : "Controller", storage.Controller),
                FieldLine("NAND", storage.NandType),
                FieldLine(currentLanguage == "pl" ? "Dysk systemowy" : "Boot drive", storage.IsBootDrive),
                FieldLine("DRAM Cache", storage.HasDramCache));
        }

        private string GetPsuSummaryText(PcPowerSupply psu)
        {
            return $"{psu.Manufacturer} {psu.Model} {psu.Power}".Trim();
        }

        private string GetPsuDetailsText(PcPowerSupply psu)
        {
            string protections = string.Join(
                ", ",
                new[]
                {
            psu.HasOcp ? "OCP" : "",
            psu.HasOvp ? "OVP" : "",
            psu.HasUvp ? "UVP" : "",
            psu.HasOpp ? "OPP" : "",
            psu.HasOtp ? "OTP" : "",
            psu.HasScp ? "SCP" : "",
            psu.HasSip ? "SIP" : "",
            psu.HasNlo ? "NLO" : ""
                }.Where(x => !string.IsNullOrWhiteSpace(x)));

            string rails = JoinFilledLines(
                FieldLine("+12V1", psu.Rail12V1),
                FieldLine("+12V2", psu.Rail12V2),
                FieldLine("+12V3", psu.Rail12V3),
                FieldLine("+12V4", psu.Rail12V4),
                FieldLine("+5V", psu.Rail5V),
                FieldLine("+3.3V", psu.Rail33V));

            return JoinFilledLines(
                FieldLine(currentLanguage == "pl" ? "Format" : "Form factor", psu.FormFactor),
                FieldLine("Standard", psu.StandardVersion),
                FieldLine(currentLanguage == "pl" ? "Certyfikat" : "Certificate", psu.Certificate),
                FieldLine(currentLanguage == "pl" ? "Modularność" : "Modularity", psu.Modularity),
                string.IsNullOrWhiteSpace(protections)
                    ? ""
                    : $"{(currentLanguage == "pl" ? "Zabezpieczenia" : "Protections")}: {protections}",
                string.IsNullOrWhiteSpace(rails)
                    ? ""
                    : (currentLanguage == "pl" ? "Linie zasilania:\n" : "Power rails:\n") + rails);
        }

        private string GetCaseSummaryText(PcCase pcCase)
        {
            return $"{pcCase.Manufacturer} {pcCase.Model} {pcCase.CaseType}".Trim();
        }

        private string GetCaseDetailsText(PcCase pcCase)
        {
            return JoinFilledLines(
                FieldLine(currentLanguage == "pl" ? "Kolor" : "Color", pcCase.Color),
                FieldLine(currentLanguage == "pl" ? "Maks. płyta" : "Max motherboard size", pcCase.MaxMotherboardSize),
                FieldLine(currentLanguage == "pl" ? "Maks. długość GPU" : "Max GPU length", pcCase.MaxGpuLength),
                FieldLine(currentLanguage == "pl" ? "Maks. wysokość chłodzenia CPU" : "Max CPU cooler height", pcCase.MaxCpuCoolerHeight),
                FieldLine(currentLanguage == "pl" ? "Okno boczne" : "Side window", pcCase.HasSideWindow),
                FieldLine(currentLanguage == "pl" ? "Szkło hartowane" : "Tempered glass", pcCase.HasTemperedGlass),
                FieldLine("RGB", pcCase.HasRgb),
                FieldLine(currentLanguage == "pl" ? "Pionowy montaż GPU" : "Vertical GPU mount", pcCase.HasVerticalGpuMount),
                FieldLine("Hot-Swap", pcCase.HasHotSwap),
                FieldLine(currentLanguage == "pl" ? "Wyciszenie" : "Sound damping", pcCase.HasSoundDamping));
        }


        private void AddGpuDetailLine(
            List<string> lines,
            string label,
            string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                lines.Add($"{label}: {value}");
        }
        private string JoinFilledLines(params string[] lines)
        {
            return string.Join(
                "\n",
                lines.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private string FieldLine(string label, string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? ""
                : $"{label}: {value}";
        }

        private string FieldLine(string label, int value)
        {
            return value <= 0
                ? ""
                : $"{label}: {value}";
        }

        private string FieldLine(string label, bool value)
        {
            return value
                ? $"{label}: {(currentLanguage == "pl" ? "Tak" : "Yes")}"
                : "";
        }
        private string GetGpuDisplayText(PcGpu gpu)
        {
            List<string> lines = new();

            string title = "";

            if (!string.IsNullOrWhiteSpace(gpu.CardManufacturer))
                title += gpu.CardManufacturer + " ";

            if (!string.IsNullOrWhiteSpace(gpu.GpuManufacturer))
                title += gpu.GpuManufacturer + " ";

            title += gpu.Model;

            lines.Add(title.Trim());

            if (gpu.HasSerialNumber &&
                !string.IsNullOrWhiteSpace(gpu.SerialNumber))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Numer seryjny: {gpu.SerialNumber}"
                        : $"Serial number: {gpu.SerialNumber}");
            }

            if (!string.IsNullOrWhiteSpace(gpu.VramAmount))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"VRAM: {gpu.VramAmount}"
                        : $"VRAM: {gpu.VramAmount}");
            }

            if (!string.IsNullOrWhiteSpace(gpu.VramType))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Rodzaj VRAM: {gpu.VramType}"
                        : $"VRAM type: {gpu.VramType}");
            }

            if (!string.IsNullOrWhiteSpace(gpu.VramBusWidth))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Szyna VRAM: {gpu.VramBusWidth}"
                        : $"VRAM bus: {gpu.VramBusWidth}");
            }

            if (!string.IsNullOrWhiteSpace(gpu.VramClock))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Taktowanie VRAM: {gpu.VramClock}"
                        : $"VRAM clock: {gpu.VramClock}");
            }

            if (!string.IsNullOrWhiteSpace(gpu.CoreClock))
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Taktowanie rdzenia: {gpu.CoreClock}"
                        : $"Core clock: {gpu.CoreClock}");
            }

            if (gpu.HasOverclock)
            {
                if (!string.IsNullOrWhiteSpace(gpu.OverclockCoreClock))
                {
                    lines.Add(
                        currentLanguage == "pl"
                            ? $"Aktualne taktowanie rdzenia: {gpu.OverclockCoreClock}"
                            : $"Current core clock: {gpu.OverclockCoreClock}");
                }

                if (!string.IsNullOrWhiteSpace(gpu.OverclockMemoryClock))
                {
                    lines.Add(
                        currentLanguage == "pl"
                            ? $"Aktualne taktowanie pamięci: {gpu.OverclockMemoryClock}"
                            : $"Current memory clock: {gpu.OverclockMemoryClock}");
                }

                if (!string.IsNullOrWhiteSpace(gpu.PowerLimit))
                {
                    lines.Add(
                        currentLanguage == "pl"
                            ? $"Limit mocy: {gpu.PowerLimit}"
                            : $"Power limit: {gpu.PowerLimit}");
                }
            }

            if (gpu.PurchaseDate.HasValue)
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Data zakupu: {gpu.PurchaseDate.Value:yyyy-MM-dd}"
                        : $"Purchase date: {gpu.PurchaseDate.Value:yyyy-MM-dd}");
            }

            if (gpu.PurchasePrice.HasValue)
            {
                lines.Add(
                    currentLanguage == "pl"
                        ? $"Cena zakupu: {gpu.PurchasePrice.Value:0.00}"
                        : $"Purchase price: {gpu.PurchasePrice.Value:0.00}");
            }

            return string.Join(Environment.NewLine, lines);
        }
        private void AddPcComponentSection(
        string title,
        string buttonText,
        string sectionType,
        int count)
        {
            Border sectionBorder = new Border
            {
                Padding = new Thickness(14),
                CornerRadius = new CornerRadius(10),
                Background = (Brush)FindResource("PanelBackgroundBrush"),
                BorderBrush = (Brush)FindResource("AppBorderBrush"),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 12)
            };

            StackPanel sectionPanel = new StackPanel();

            TextBlock titleText = new TextBlock
            {
                Text = $"{title} ({count})",
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            };

            sectionPanel.Children.Add(titleText);

            if (selectedPcItem != null && sectionType == "CPU")
            {
                foreach (PcCpu cpu in selectedPcItem.Cpus)
                {
                    TextBlock cpuText = new TextBlock
                    {
                        Text = $"{cpu.Manufacturer} {cpu.Model}",
                        FontSize = 14,
                        Margin = new Thickness(0, 0, 0, 6),
                        TextWrapping = TextWrapping.Wrap
                    };

                    sectionPanel.Children.Add(cpuText);
                }
            }

            Button addButton = new Button
            {
                Content = buttonText,
                Height = 34,
                Tag = sectionType,
                HorizontalAlignment = HorizontalAlignment.Left,
                MinWidth = 150,
                Margin = new Thickness(0, 8, 0, 0)
            };

            addButton.Click += AddPcComponentButton_Click;

            sectionPanel.Children.Add(addButton);

            sectionBorder.Child = sectionPanel;

            PcComponentsPanel.Children.Add(sectionBorder);
        }
        private void AddPcComponentButton_Click(object sender, RoutedEventArgs e)
        {
            PcSystem? targetPc =
                isEditingPc
                    ? selectedPcItem
                    : currentPcDraft;

            if (targetPc == null)
                return;

            if (sender is not Button button)
                return;

            string sectionType = button.Tag?.ToString() ?? "";

            if (sectionType == "CPU")
            {
                PcCpuWindow window = new PcCpuWindow(currentLanguage)
                {
                    Owner = this
                };

                if (window.ShowDialog() == true &&
                    window.ResultCpu != null)
                {
                    targetPc.Cpus.Add(window.ResultCpu);
                    RefreshPcComponentsPanel(targetPc);
                }

                return;
            }

            if (sectionType == "Motherboard")
            {
                PcMotherboardWindow window =
                    new PcMotherboardWindow(currentLanguage)
                    {
                        Owner = this
                    };

                if (window.ShowDialog() == true &&
                    window.ResultMotherboard != null)
                {
                    targetPc.Motherboard = window.ResultMotherboard;
                    RefreshPcComponentsPanel(targetPc);
                }

                return;
            }

            if (sectionType == "GPU")
            {
                PcGpuWindow window =
                    new PcGpuWindow(currentLanguage)
                    {
                        Owner = this
                    };

                if (window.ShowDialog() == true &&
                    window.ResultGpu != null)
                {
                    targetPc.Gpus.Add(window.ResultGpu);
                    RefreshPcComponentsPanel(targetPc);
                }

                return;
            }

            if (sectionType == "RAM")
            {
                PcComponentWindow window =
                    new PcComponentWindow(currentLanguage, "RAM")
                    {
                        Owner = this
                    };

                if (window.ShowDialog() == true &&
                    window.ResultComponent is PcRam ram)
                {
                    targetPc.RamModules.Add(ram);
                    RefreshPcComponentsPanel(targetPc);
                }

                return;
            }

            if (sectionType == "Storage")
            {
                PcComponentWindow window =
                    new PcComponentWindow(currentLanguage, "Storage")
                    {
                        Owner = this
                    };

                if (window.ShowDialog() == true &&
                    window.ResultComponent is PcStorage storage)
                {
                    targetPc.StorageDevices.Add(storage);
                    RefreshPcComponentsPanel(targetPc);
                }

                return;
            }

            if (sectionType == "PSU")
            {
                PcComponentWindow window =
                    new PcComponentWindow(currentLanguage, "PSU")
                    {
                        Owner = this
                    };

                if (window.ShowDialog() == true &&
                    window.ResultComponent is PcPowerSupply psu)
                {
                    targetPc.PowerSupplies.Add(psu);
                    RefreshPcComponentsPanel(targetPc);
                }

                return;
            }

            if (sectionType == "Case")
            {
                PcComponentWindow window =
                    new PcComponentWindow(currentLanguage, "Case")
                    {
                        Owner = this
                    };

                if (window.ShowDialog() == true &&
                    window.ResultComponent is PcCase pcCase)
                {
                    targetPc.Case = pcCase;
                    RefreshPcComponentsPanel(targetPc);
                }

                return;
            }

            if (sectionType == "Cooling")
            {
                PcCoolingWindow window =
                    new PcCoolingWindow(
                        currentLanguage,
                        targetPc.Cooling)
                    {
                        Owner = this
                    };

                if (window.ShowDialog() == true &&
                    window.ResultCooling != null)
                {
                    targetPc.Cooling = window.ResultCooling;
                    RefreshPcComponentsPanel(targetPc);
                }

                return;
            }

            HgcMessageBox.Show(
                this,
                currentLanguage == "pl"
                    ? $"Sekcję {sectionType} dodamy później."
                    : $"Section {sectionType} will be added later.",
                "HGC",
                HgcMessageBoxType.Information,
                currentLanguage);
        }

        private void AddPcDetailRow(
        string label,
        string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            StackPanel row = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 8)
            };

            row.Children.Add(new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.Bold
            });

            row.Children.Add(new TextBlock
            {
                Text = value,
                Foreground = (Brush)FindResource("AppTextBrush")
            });

            PcDetailsContentPanel.Children.Add(row);
        }
        private string GetManufacturerFromTreeItem(TreeViewItem item)
        {
            TreeViewItem? current = item;
            TreeViewItem? topMostTreeItem = item;

            while (current != null)
            {
                topMostTreeItem = current;

                ItemsControl parent =
                    ItemsControl.ItemsControlFromItemContainer(current);

                if (parent is TreeViewItem parentTreeItem)
                    current = parentTreeItem;
                else
                    break;
            }

            return topMostTreeItem.Header?.ToString() ?? "";
        }

        private string GetBackgroundGroupForSelectedTreeItem(
            TreeViewItem item,
            TreeNodeData nodeData)
        {
            if (!string.IsNullOrWhiteSpace(nodeData.Manufacturer))
                return GetBackgroundGroupForManufacturer(nodeData.Manufacturer);

            string manufacturerFromTree = GetManufacturerFromTreeItem(item);

            if (!string.IsNullOrWhiteSpace(manufacturerFromTree))
                return GetBackgroundGroupForManufacturer(manufacturerFromTree);

            return "Default";
        }
        private string GetBackgroundGroupForNode(TreeNodeData nodeData)
        {
            if (!string.IsNullOrWhiteSpace(nodeData.Manufacturer))
                return GetBackgroundGroupForManufacturer(nodeData.Manufacturer);

            if (!string.IsNullOrWhiteSpace(nodeData.BackgroundGroup))
                return nodeData.BackgroundGroup;

            return "Default";
        }
        private void ShowGameDetails(Game? game)
        {
            if (game == null)
                return;

            selectedGameItem = game;
            selectedHardwareItem = null;
            selectedPcItem = null;
            selectedAccessoryItem = null;

            PreviewPlaceholder.Visibility = Visibility.Collapsed;
            AddChoicePanel.Visibility = Visibility.Collapsed;
            AddHardwareFormPanel.Visibility = Visibility.Collapsed;
            AddGameFormPanel.Visibility = Visibility.Collapsed;
            AddServicePanel.Visibility = Visibility.Collapsed;
            CollectionTilesPanel.Visibility = Visibility.Collapsed;

            HardwareDetailsViewPanel.Visibility = Visibility.Visible;
            RecordActionsPanel.Visibility = Visibility.Visible;

            AddServiceButton.Visibility = Visibility.Collapsed;

            ArchiveRecordButton.Visibility =
            game.IsArchived ? Visibility.Collapsed : Visibility.Visible;

            EnsureRestoreRecordButton();

            if (RestoreRecordButton != null)
                RestoreRecordButton.Visibility =
                    game.IsArchived ? Visibility.Visible : Visibility.Collapsed;

            HardwareDetailsViewTitle.Text = game.Title;
            UpdateGameDetailsRatingHeader(game);

            HardwareDetailsMainTitle.Text =
                currentLanguage == "pl" ? "Informacje o grze" : "Game information";

            HardwareDetailsConditionTitle.Text =
                currentLanguage == "pl" ? "Stan gry / opakowania" : "Game / package condition";

            HardwareDetailsSideTitle.Text =
                currentLanguage == "pl" ? "Zakup i notatki" : "Purchase and notes";

            HardwareDetailsViewText.Text = "";
            HardwareDetailsConditionText.Text = "";
            HardwareDetailsSideText.Text = "";

            ApplyGameCardOpacity(game);

            CardOpacityButton.Content =
                $"{(currentLanguage == "pl" ? "Przezroczystość" : "Opacity")} {game.CardOpacity}% ▼";

            ApplyRecordBackground();
            UpdatePageBackgroundButtonText();

            HardwarePhotosPlaceholderText.Text =
                currentLanguage == "pl"
                    ? "Okładka / zdjęcia gry"
                    : "Cover / game photos";

            // =========================
            // INFORMACJE O GRZE
            // =========================

            HardwareDetailsViewText.Text =
                $"{(currentLanguage == "pl" ? "Producent / system" : "Manufacturer / system")}: {game.Manufacturer}";

            if (game.Manufacturer != "PC" && !string.IsNullOrWhiteSpace(game.Family))
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Rodzina" : "Family")}: {game.Family}";
            }

            if (!string.IsNullOrWhiteSpace(game.Platform))
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Platforma" : "Platform")}: {game.Platform}";
            }

            if (!string.IsNullOrWhiteSpace(game.Publisher))
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Wydawca gry" : "Game publisher")}: {game.Publisher}";
            }

            if (!string.IsNullOrWhiteSpace(game.ReleaseType))
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Typ wydania" : "Release type")}: {TranslateGameComboValueForCurrentLanguage(game.ReleaseType)}";
            }

            if (!string.IsNullOrWhiteSpace(game.MediaType))
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Nośnik" : "Media type")}: {game.MediaType}";
            }

            if (!string.IsNullOrWhiteSpace(game.OperatingSystem))
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "System operacyjny" : "Operating system")}: {game.OperatingSystem}";
            }

            if (!string.IsNullOrWhiteSpace(game.DigitalPlatform))
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Platforma cyfrowa" : "Digital platform")}: {game.DigitalPlatform}";
            }

            if (!string.IsNullOrWhiteSpace(game.Region))
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Region" : "Region")}: {game.Region}";
            }

            if (game.IsFavorite || game.IsCompleted || game.IsPlanned)
            {
                List<string> statuses = new();

                if (game.IsFavorite)
                    statuses.Add(currentLanguage == "pl" ? "Ulubiona" : "Favorite");

                if (game.IsCompleted)
                    statuses.Add(currentLanguage == "pl" ? "Ukończona" : "Completed");

                if (game.IsPlanned)
                    statuses.Add(currentLanguage == "pl" ? "Planowana" : "Planned");

                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Status" : "Status")}: {string.Join(", ", statuses)}";
            }

            if (!string.IsNullOrWhiteSpace(game.Genre))
            {
                HardwareDetailsViewText.Text +=
                $"\n{(currentLanguage == "pl" ? "Gatunek" : "Genre")}: {GenreDatabase.TranslateGenre(game.Genre, currentLanguage)}";
            }

            // =========================
            // STAN GRY / OPAKOWANIA
            // =========================

            if (!string.IsNullOrWhiteSpace(game.Condition))
            {
                HardwareDetailsConditionText.Text +=
                    $"{(currentLanguage == "pl" ? "Stan nośnika" : "Media condition")}: {ConditionDatabase.Translate(game.Condition, currentLanguage)}";
            }

            if (game.HasBox)
            {
                if (!string.IsNullOrWhiteSpace(HardwareDetailsConditionText.Text))
                    HardwareDetailsConditionText.Text += "\n";

                HardwareDetailsConditionText.Text +=
                    $"{(currentLanguage == "pl" ? "Pudełko" : "Box")}: {(currentLanguage == "pl" ? "Tak" : "Yes")}";

                if (!string.IsNullOrWhiteSpace(game.BoxType))
                {
                    HardwareDetailsConditionText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Rodzaj pudełka" : "Box type")}: {TranslateGameComboValueForCurrentLanguage(game.BoxType)}";
                }

                if (!string.IsNullOrWhiteSpace(game.BoxCondition))
                {
                    HardwareDetailsConditionText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Stan pudełka" : "Box condition")}: {ConditionDatabase.Translate(game.BoxCondition, currentLanguage)}";
                }
            }

            List<string> contents = new();

            if (game.HasManual)
                contents.Add(currentLanguage == "pl" ? "Instrukcja" : "Manual");

            if (game.HasAdvertisements)
                contents.Add(currentLanguage == "pl" ? "Reklamy / ulotki" : "Advertisements / flyers");

            if (game.HasMap)
                contents.Add(currentLanguage == "pl" ? "Mapa" : "Map");

            if (game.HasPoster)
                contents.Add(currentLanguage == "pl" ? "Plakat" : "Poster");

            if (game.HasSoundtrack)
                contents.Add(currentLanguage == "pl" ? "Soundtrack" : "Soundtrack");

            if (game.HasArtbook)
                contents.Add(currentLanguage == "pl" ? "Artbook" : "Artbook");

            if (game.HasFigure)
                contents.Add(currentLanguage == "pl" ? "Figurka" : "Figure");

            if (game.HasCertificate)
                contents.Add(currentLanguage == "pl" ? "Certyfikat" : "Certificate");

            if (game.HasOtherExtras)
                contents.Add(currentLanguage == "pl" ? "Inne dodatki" : "Other extras");

            if (contents.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(HardwareDetailsConditionText.Text))
                    HardwareDetailsConditionText.Text += "\n";

                HardwareDetailsConditionText.Text +=
                    $"{(currentLanguage == "pl" ? "Zawartość" : "Contents")}: {string.Join(", ", contents)}";
            }

            // =========================
            // PRAWA STRONA — ZAKUP I NOTATKI
            // =========================

            if (game.PurchaseDate.HasValue)
            {
                HardwareDetailsSideText.Text +=
                    $"{(currentLanguage == "pl" ? "Data zakupu" : "Purchase date")}: {game.PurchaseDate.Value:yyyy-MM-dd}";
            }

            if (game.PurchasePrice.HasValue)
            {
                if (!string.IsNullOrWhiteSpace(HardwareDetailsSideText.Text))
                    HardwareDetailsSideText.Text += "\n";

                HardwareDetailsSideText.Text +=
                    $"{(currentLanguage == "pl" ? "Cena zakupu" : "Purchase price")}: {game.PurchasePrice.Value:0.00} {game.PurchaseCurrency}";
            }

            if (!string.IsNullOrWhiteSpace(game.Notes))
            {
                if (!string.IsNullOrWhiteSpace(HardwareDetailsSideText.Text))
                    HardwareDetailsSideText.Text += "\n\n";

                HardwareDetailsSideText.Text +=
                    $"{(currentLanguage == "pl" ? "Notatki" : "Notes")}:\n{game.Notes}";
            }

            //if (game.PurchaseDate.HasValue)
            //HardwareDetailsViewText.Text +=
            //$"\n{(currentLanguage == "pl" ? "Data zakupu" : "Purchase date")}: {game.PurchaseDate.Value:yyyy-MM-dd}";

            //if (game.PurchasePrice.HasValue)
            //HardwareDetailsViewText.Text +=
            //$"\n{(currentLanguage == "pl" ? "Cena zakupu" : "Purchase price")}: {game.PurchasePrice.Value:0.00}";

            //if (!string.IsNullOrWhiteSpace(game.Notes))
            //HardwareDetailsViewText.Text +=
            //$"\n\n{(currentLanguage == "pl" ? "Notatki" : "Notes")}:\n{game.Notes}";

            LoadGamePhotosPreview(game);

            if (!string.IsNullOrWhiteSpace(game.ShortDescription))
            {
                GameShortDescriptionText.Text = game.ShortDescription;
                GameShortDescriptionBorder.Visibility = Visibility.Visible;
            }
            else
            {
                GameShortDescriptionText.Text = "";
                GameShortDescriptionBorder.Visibility = Visibility.Collapsed;
            }

            HardwareDetailsConditionTitle.Visibility =
                !string.IsNullOrWhiteSpace(HardwareDetailsConditionText.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            HardwareDetailsConditionText.Visibility =
                !string.IsNullOrWhiteSpace(HardwareDetailsConditionText.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            HardwareDetailsSideTitle.Visibility =
                !string.IsNullOrWhiteSpace(HardwareDetailsSideText.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            HardwareDetailsSideText.Visibility =
                !string.IsNullOrWhiteSpace(HardwareDetailsSideText.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
        private void LoadGamePhotosPreview(Game game)
        {
            HardwarePhotosPanel.Children.Clear();

            List<CollectionPhoto> displayPhotos = new();

            if (game.FrontCoverPhoto != null)
                displayPhotos.Add(game.FrontCoverPhoto);

            if (game.BackCoverPhoto != null)
                displayPhotos.Add(game.BackCoverPhoto);

            if (game.Photos != null)
                displayPhotos.AddRange(game.Photos);

            HardwareNoPhotoPanel.Visibility =
                displayPhotos.Count == 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            CollectionPhoto mainPhoto =
                displayPhotos.FirstOrDefault(x => x.IsMain)
                ?? game.FrontCoverPhoto
                ?? game.BackCoverPhoto
                ?? displayPhotos.FirstOrDefault();

            foreach (CollectionPhoto photo in displayPhotos)
            {
                if (photo == null ||
                    string.IsNullOrWhiteSpace(photo.OriginalPath) ||
                    !File.Exists(photo.OriginalPath))
                    continue;

                bool isMainPhoto = photo == mainPhoto;

                bool isCover =
                     photo == game.FrontCoverPhoto ||
                     photo == game.BackCoverPhoto;

                var previewSize =
                    GetGamePhotoPreviewSize(
                    photo,
                    isCover);

                double photoWidth = previewSize.Width;
                double photoHeight = previewSize.Height;

                Stretch photoStretch =
                    isCover
                        ? Stretch.Uniform
                        : Stretch.UniformToFill;

                Image image = new Image
                {
                    Width = photoWidth,
                    Height = photoHeight,
                    Stretch = photoStretch,
                    Margin = new Thickness(0),
                    Source = LoadImageWithoutLock(photo.OriginalPath, 320),
                    Cursor = Cursors.Hand
                };

                Border imageContainer = new Border
                {
                    Width = photoWidth,
                    Height = photoHeight,
                    CornerRadius = new CornerRadius(10),
                    ClipToBounds = true,
                    Margin = new Thickness(12, 0, 10, 0),
                    Child = image
                };

                Grid imageGrid = new Grid();

                imageGrid.Children.Add(imageContainer);

                string badgeText = "";

                if (photo == game.FrontCoverPhoto)
                {
                    badgeText =
                        currentLanguage == "pl"
                            ? "OKŁADKA PRZÓD"
                            : "FRONT COVER";
                }
                else if (photo == game.BackCoverPhoto)
                {
                    badgeText =
                        currentLanguage == "pl"
                            ? "OKŁADKA TYŁ"
                            : "BACK COVER";
                }

                if (!string.IsNullOrWhiteSpace(badgeText))
                {
                    Border badge = new Border
                    {
                        Background = (Brush)FindResource("ButtonGradientBrush"),
                        CornerRadius = new CornerRadius(4),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(4),
                        Padding = new Thickness(6, 2, 6, 2)
                    };

                    badge.Child = new TextBlock
                    {
                        Text = badgeText,
                        FontSize = 10,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White
                    };

                    imageGrid.Children.Add(badge);
                }

                if (isMainPhoto)
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

                    imageGrid.Children.Add(mainBadge);
                }

                image.MouseLeftButtonUp += (s, e) =>
                {
                    PhotoPreviewWindow preview =
                        new PhotoPreviewWindow(photo.OriginalPath, currentLanguage);

                    preview.Owner = this;
                    preview.ShowDialog();
                };

                Border photoBorder = new Border
                {
                    CornerRadius = new CornerRadius(8),
                    BorderThickness = new Thickness(isMainPhoto ? 2 : 1),
                    BorderBrush = isMainPhoto
                        ? Brushes.Gold
                        : (Brush)FindResource("AppBorderBrush"),
                    Child = imageGrid,
                    Margin = new Thickness(4, 0, 10, 0),
                    ClipToBounds = true
                };

                HardwarePhotosPanel.Children.Add(photoBorder);
            }
        }
        private void ShowHardwareDetails(Hardware hardware)
        {
            GameShortDescriptionBorder.Visibility = Visibility.Collapsed;
            GameShortDescriptionText.Text = "";

            selectedHardwareItem = hardware;
            selectedGameItem = null;
            selectedPcItem = null;
            selectedAccessoryItem = null;

            PreviewPlaceholder.Visibility = Visibility.Collapsed;
            AddChoicePanel.Visibility = Visibility.Collapsed;
            AddHardwareFormPanel.Visibility = Visibility.Collapsed;
            AddGameFormPanel.Visibility = Visibility.Collapsed;
            AddServicePanel.Visibility = Visibility.Collapsed;
            CollectionTilesPanel.Visibility = Visibility.Collapsed;

            HardwareDetailsViewPanel.Visibility = Visibility.Visible;
            RecordActionsPanel.Visibility = Visibility.Visible;

            AddServiceButton.Visibility = Visibility.Visible;

            ArchiveRecordButton.Visibility =
                hardware.IsArchived ? Visibility.Collapsed : Visibility.Visible;

            EnsureRestoreRecordButton();

            if (RestoreRecordButton != null)
                RestoreRecordButton.Visibility =
                    hardware.IsArchived ? Visibility.Visible : Visibility.Collapsed;

            HardwareDetailsViewTitle.Text = GetHardwareDisplayName(hardware);

            GameDetailsRatingHeaderPanel.Visibility = Visibility.Collapsed;

            HardwareDetailsMainTitle.Text =
                currentLanguage == "pl" ? "Informacje o sprzęcie" : "Hardware information";

            HardwareDetailsConditionTitle.Text =
                currentLanguage == "pl" ? "Stan sprzętu / zestawu" : "Hardware / set condition";

            HardwareDetailsSideTitle.Text =
                currentLanguage == "pl" ? "Zakup i notatki" : "Purchase and notes";

            HardwareDetailsViewText.Text = "";
            HardwareDetailsConditionText.Text = "";
            HardwareDetailsSideText.Text = "";

            ApplyCardOpacity(hardware);

            CardOpacityButton.Content =
                $"{(currentLanguage == "pl" ? "Przezroczystość" : "Opacity")} {hardware.CardOpacity}% ▼";

            ApplyRecordBackground();
            UpdatePageBackgroundButtonText();

            HardwarePhotosPlaceholderText.Text =
                currentLanguage == "pl"
                    ? "Zdjęcia sprzętu"
                    : "Hardware photos";

            // =========================
            // INFORMACJE O SPRZĘCIE
            // =========================

            HardwareDetailsViewText.Text =
                $"{(currentLanguage == "pl" ? "Producent" : "Manufacturer")}: {hardware.Manufacturer}";

            if (!string.IsNullOrWhiteSpace(hardware.Family))
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Rodzina" : "Family")}: {hardware.Family}";
            }

            if (!string.IsNullOrWhiteSpace(hardware.Platform))
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Platforma" : "Platform")}: {hardware.Platform}";
            }

            if (!string.IsNullOrWhiteSpace(hardware.Model))
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Model" : "Model")}: {hardware.Model}";
            }

            if (!string.IsNullOrWhiteSpace(hardware.Revision))
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Rewizja" : "Revision")}: {hardware.Revision}";
            }

            if (!string.IsNullOrWhiteSpace(hardware.BoardRevision) &&
                hardware.BoardRevision != "Wybierz" &&
                hardware.BoardRevision != "Select")
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Rewizja płyty" : "Board revision")}: {hardware.BoardRevision}";
            }

            if (!string.IsNullOrWhiteSpace(hardware.Region))
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Region" : "Region")}: {hardware.Region}";
            }

            if (!string.IsNullOrWhiteSpace(hardware.Color) &&
                hardware.Color != "Wybierz" &&
                hardware.Color != "Select" &&
                hardware.Color != "Aqua Blue")
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Kolor" : "Color")}: {hardware.Color}";
            }

            if (!string.IsNullOrWhiteSpace(hardware.SpecialEdition) &&
                hardware.SpecialEdition != "Brak" &&
                hardware.SpecialEdition != "None")
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Edycja specjalna" : "Special edition")}: {hardware.SpecialEdition}";
            }

            if (!string.IsNullOrWhiteSpace(hardware.SerialNumber))
            {
                HardwareDetailsViewText.Text +=
                    $"\n{(currentLanguage == "pl" ? "Numer seryjny" : "Serial number")}: {hardware.SerialNumber}";
            }

            // =========================
            // STAN SPRZĘTU / ZESTAWU
            // =========================

            if (!string.IsNullOrWhiteSpace(hardware.Condition) &&
                    hardware.Condition != "Bardzo dobry" &&
                    hardware.Condition != "Very Good")
            {
                HardwareDetailsConditionText.Text +=
                    $"{(currentLanguage == "pl" ? "Stan" : "Condition")}: " +
                    $"{ConditionDatabase.Translate(hardware.Condition, currentLanguage)}";
            }

            List<string> contents = new();

            if (hardware.HasBox)
            {
                string boxText = currentLanguage == "pl" ? "Pudełko" : "Box";

                if (!string.IsNullOrWhiteSpace(hardware.BoxCondition) &&
                    hardware.BoxCondition != "Wybierz stan pudełka" &&
                    hardware.BoxCondition != "Select box condition")
                {
                    boxText += $" ({ConditionDatabase.Translate(hardware.BoxCondition, currentLanguage)})";
                }

                contents.Add(boxText);
            }

            if (hardware.HasManual)
                contents.Add(currentLanguage == "pl" ? "Instrukcja" : "Manual");

            if (hardware.HasInserts)
                contents.Add(currentLanguage == "pl" ? "Wkładki / ulotki" : "Inserts / leaflets");

            if (hardware.HasPowerSupply)
                contents.Add(currentLanguage == "pl" ? "Zasilacz" : "Power supply");

            if (hardware.HasCable)
                contents.Add(currentLanguage == "pl" ? "Kabel" : "Cable");

            if (hardware.HasController)
                contents.Add(currentLanguage == "pl" ? "Kontroler" : "Controller");

            if (hardware.HasMemoryCard)
                contents.Add(currentLanguage == "pl" ? "Karta pamięci" : "Memory card");

            if (contents.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(HardwareDetailsConditionText.Text))
                    HardwareDetailsConditionText.Text += "\n\n";

                HardwareDetailsConditionText.Text +=
                    currentLanguage == "pl"
                        ? "Zawartość zestawu:"
                        : "Set contents:";

                foreach (string item in contents)
                    HardwareDetailsConditionText.Text += $"\n- {item}";
            }

            // =========================
            // PRAWA STRONA — ZAKUP I NOTATKI
            // =========================

            if (hardware.PurchaseDate.HasValue)
            {
                HardwareDetailsSideText.Text +=
                    $"{(currentLanguage == "pl" ? "Data zakupu" : "Purchase date")}: {hardware.PurchaseDate.Value:yyyy-MM-dd}";
            }

            if (hardware.PurchasePrice.HasValue)
            {
                if (!string.IsNullOrWhiteSpace(HardwareDetailsSideText.Text))
                    HardwareDetailsSideText.Text += "\n";

                HardwareDetailsSideText.Text +=
                    $"{(currentLanguage == "pl" ? "Cena zakupu" : "Purchase price")}: {hardware.PurchasePrice.Value:0.00} {hardware.PurchaseCurrency}";
            }

            if (!string.IsNullOrWhiteSpace(hardware.Notes))
            {
                if (!string.IsNullOrWhiteSpace(HardwareDetailsSideText.Text))
                    HardwareDetailsSideText.Text += "\n\n";

                HardwareDetailsSideText.Text +=
                    $"{(currentLanguage == "pl" ? "Notatki" : "Notes")}:\n{hardware.Notes}";
            }

            // =========================
            // HISTORIA SERWISOWA
            // =========================

            if (hardware.ServiceHistory.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(HardwareDetailsSideText.Text))
                    HardwareDetailsSideText.Text += "\n\n";

                HardwareDetailsSideText.Text +=
                    currentLanguage == "pl"
                        ? "Historia serwisowa:"
                        : "Service history:";

                foreach (var service in hardware.ServiceHistory
                             .OrderByDescending(x => x.ServiceDate))
                {
                    string dateText = service.ServiceDate.HasValue
                        ? service.ServiceDate.Value.ToString("yyyy-MM-dd")
                        : "-";

                    string costText = service.Cost.HasValue
                        ? service.Cost.Value.ToString("0.00")
                        : "-";

                    HardwareDetailsSideText.Text +=
                        $"\n\n{dateText}" +
                        $"\n{ServiceTypeDatabase.Translate(service.ServiceType, currentLanguage)}" +
                        $"\n{costText}";

                    if (!string.IsNullOrWhiteSpace(service.CustomNote))
                        HardwareDetailsSideText.Text +=
                            $"\n{service.CustomNote}";
                }
            }

            HardwareDetailsConditionTitle.Visibility =
                !string.IsNullOrWhiteSpace(HardwareDetailsConditionText.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            HardwareDetailsConditionText.Visibility =
                !string.IsNullOrWhiteSpace(HardwareDetailsConditionText.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            HardwareDetailsSideTitle.Visibility =
                !string.IsNullOrWhiteSpace(HardwareDetailsSideText.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            HardwareDetailsSideText.Visibility =
                !string.IsNullOrWhiteSpace(HardwareDetailsSideText.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            LoadHardwarePhotosPreview(hardware);
        }
        private readonly List<string> SupportedCurrencies = new()
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
        private void ApplyAccessoryCardOpacity(Accessory accessory)
        {
            double opacity = accessory.CardOpacity / 100.0;

            AccessoryPhotosBorder.Opacity = opacity;
            AccessoryDetailsContentBorder.Opacity = opacity;
            AccessoryExtraDetailsBorder.Opacity = opacity;
            AccessoryActionsBorder.Opacity = opacity;
            AccessoryDetailsTitle.Opacity = opacity;
        }
        private void GamePhotosButton_Click(object sender, RoutedEventArgs e)
        {
            GamePhotoGalleryWindow window =
                new GamePhotoGalleryWindow(
                    currentGamePhotos,
                    currentGameFrontCover,
                    currentGameBackCover,
                    currentLanguage);

            window.Owner = this;

            if (window.ShowDialog() != true)
                return;

            currentGamePhotos = window.Photos;
            currentGameFrontCover = window.FrontCoverPhoto;
            currentGameBackCover = window.BackCoverPhoto;

            int photosCount = currentGamePhotos?.Count ?? 0;

            if (currentGameFrontCover != null)
                photosCount++;

            if (currentGameBackCover != null)
                photosCount++;

            GamePhotosButton.Content =
                currentLanguage == "pl"
                    ? $"Zdjęcia / okładki ({photosCount})"
                    : $"Photos / covers ({photosCount})";
        }
        private void UpdateGameRatingStars(double rating)
        {
            GameRatingStarsPanel.Children.Clear();

            Color accent =
                ((SolidColorBrush)FindResource("AppAccentBrush")).Color;

            Brush filledBrush = new LinearGradientBrush(
                Shift(accent, +0.25),
                Shift(accent, -0.15),
                90);

            Brush emptyBrush = new SolidColorBrush(
                Color.FromArgb(55, 255, 255, 255));

            for (int i = 1; i <= 10; i++)
            {
                double fillAmount;

                if (rating >= i)
                    fillAmount = 1.0;
                else if (rating >= i - 0.5)
                    fillAmount = 0.5;
                else
                    fillAmount = 0.0;

                Grid starGrid = new Grid
                {
                    Width = 30,
                    Height = 34,
                    Margin = new Thickness(4, 0, 4, 0)
                };

                TextBlock emptyStar = new TextBlock
                {
                    Text = "★",
                    FontSize = 30,
                    FontWeight = FontWeights.Bold,
                    Foreground = emptyBrush,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                TextBlock filledStar = new TextBlock
                {
                    Text = "★",
                    FontSize = 30,
                    FontWeight = FontWeights.Bold,
                    Foreground = filledBrush,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                if (fillAmount < 1.0)
                {
                    filledStar.Clip = new RectangleGeometry(
                        new Rect(0, 0, 25 * fillAmount, 34));
                }

                starGrid.Children.Add(emptyStar);

                if (fillAmount > 0)
                    starGrid.Children.Add(filledStar);

                GameRatingStarsPanel.Children.Add(starGrid);
            }
        }
        private void GameUserRatingCombo_SelectionChanged(
       object sender,
       SelectionChangedEventArgs e)
        {
            if (GameUserRatingCombo.SelectedIndex <= 0)
            {
                UpdateGameRatingStars(0);
                return;
            }

            if (double.TryParse(
                GameUserRatingCombo.SelectedItem?.ToString(),
                out double rating))
            {
                UpdateGameRatingStars(rating);
            }
        }
        private void LoadHardwarePhotosPreview(Hardware hardware)
        {
            HardwarePhotosPanel.Children.Clear();

            if (hardware.Photos == null || hardware.Photos.Count == 0)
            {
                HardwarePhotosScrollViewer.Visibility = Visibility.Collapsed;
                HardwareNoPhotoPanel.Visibility = Visibility.Visible;
                return;
            }

            HardwareNoPhotoPanel.Visibility = Visibility.Collapsed;
            HardwarePhotosScrollViewer.Visibility = Visibility.Visible;

            foreach (CollectionPhoto photo in hardware.Photos)
            {
                if (string.IsNullOrWhiteSpace(photo.OriginalPath) ||
                    !File.Exists(photo.OriginalPath))
                    continue;

                Border photoBorder = new Border
                {
                    Width = 170,
                    Height = 170,
                    Margin = new Thickness(0, 0, 10, 0),
                    CornerRadius = new CornerRadius(8),
                    Background = (Brush)FindResource("PreviewBoxBrush"),
                    BorderBrush = photo.IsMain
                        ? Brushes.Gold
                        : (Brush)FindResource("AppBorderBrush"),
                    BorderThickness = photo.IsMain
                        ? new Thickness(2)
                        : new Thickness(1)
                };

                Grid grid = new Grid();

                Image image = new Image
                {
                    Source = LoadImageWithoutLock(photo.OriginalPath, 260),
                    Stretch = Stretch.UniformToFill
                };

                grid.Children.Add(image);

                if (photo.IsMain)
                {
                    Border badge = new Border
                    {
                        Background = Brushes.Gold,
                        CornerRadius = new CornerRadius(4),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(6),
                        Padding = new Thickness(6, 2, 6, 2)
                    };

                    badge.Child = new TextBlock
                    {
                        Text = "★",
                        Foreground = Brushes.Black,
                        FontWeight = FontWeights.Bold
                    };

                    grid.Children.Add(badge);
                }

                photoBorder.Child = grid;

                photoBorder.Cursor = Cursors.Hand;

                photoBorder.MouseLeftButtonUp += (s, e) =>
                {
                    PhotoPreviewWindow window = new PhotoPreviewWindow(
                        photo.OriginalPath,
                        currentLanguage)
                    {
                        Owner = this
                    };

                    window.ShowDialog();
                };

                HardwarePhotosPanel.Children.Add(photoBorder);
            }

            if (HardwarePhotosPanel.Children.Count == 0)
            {
                HardwarePhotosScrollViewer.Visibility = Visibility.Collapsed;
                HardwareNoPhotoPanel.Visibility = Visibility.Visible;
            }
        }
        private void LoadPcPhotosPreview(PcSystem pc)
        {
            PcPhotosPanel.Children.Clear();

            if (pc.Photos == null || pc.Photos.Count == 0)
            {
                PcPhotosScrollViewer.Visibility = Visibility.Collapsed;
                PcNoPhotoPanel.Visibility = Visibility.Visible;
                return;
            }

            PcNoPhotoPanel.Visibility = Visibility.Collapsed;
            PcPhotosScrollViewer.Visibility = Visibility.Visible;

            foreach (CollectionPhoto photo in pc.Photos)
            {
                if (string.IsNullOrWhiteSpace(photo.OriginalPath) ||
                    !File.Exists(photo.OriginalPath))
                    continue;

                Border photoBorder = new Border
                {
                    Width = 170,
                    Height = 170,
                    Margin = new Thickness(0, 0, 10, 0),
                    CornerRadius = new CornerRadius(8),
                    Background = (Brush)FindResource("PreviewBoxBrush"),
                    BorderBrush = photo.IsMain
                        ? Brushes.Gold
                        : (Brush)FindResource("AppBorderBrush"),
                    BorderThickness = photo.IsMain
                        ? new Thickness(2)
                        : new Thickness(1)
                };

                Grid grid = new Grid();

                Image image = new Image
                {
                    Source = LoadImageWithoutLock(photo.OriginalPath, 260),
                    Stretch = Stretch.UniformToFill
                };

                grid.Children.Add(image);

                if (photo.IsMain)
                {
                    Border badge = new Border
                    {
                        Background = Brushes.Gold,
                        CornerRadius = new CornerRadius(4),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(6),
                        Padding = new Thickness(6, 2, 6, 2)
                    };

                    badge.Child = new TextBlock
                    {
                        Text = "★",
                        Foreground = Brushes.Black,
                        FontWeight = FontWeights.Bold
                    };

                    grid.Children.Add(badge);
                }

                photoBorder.Child = grid;

                photoBorder.Cursor = Cursors.Hand;

                photoBorder.MouseLeftButtonUp += (s, e) =>
                {
                    PhotoPreviewWindow window = new PhotoPreviewWindow(
                        photo.OriginalPath,
                        currentLanguage)
                    {
                        Owner = this
                    };

                    window.ShowDialog();
                };

                PcPhotosPanel.Children.Add(photoBorder);
            }

            if (PcPhotosPanel.Children.Count == 0)
            {
                PcPhotosScrollViewer.Visibility = Visibility.Collapsed;
                PcNoPhotoPanel.Visibility = Visibility.Visible;
            }
        }

        private BitmapImage LoadImageWithoutLock(
        string path,
        int decodePixelWidth = 0)
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
        private (double Width, double Height) GetGamePhotoPreviewSize(
     CollectionPhoto photo,
     bool isCover)
        {
            if (isCover)
                return (140, 210);

            const double targetHeight = 210;
            const double minWidth = 140;
            const double maxWidth = 320;

            try
            {
                BitmapImage bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.UriSource = new Uri(photo.OriginalPath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                double ratio =
                    (double)bitmap.PixelWidth /
                    bitmap.PixelHeight;

                double width = targetHeight * ratio;

                width = Math.Max(minWidth, width);
                width = Math.Min(maxWidth, width);

                return (width, targetHeight);
            }
            catch
            {
                return (210, targetHeight);
            }
        }

        private TreeViewItem GetArchiveRoot()
        {
            // archiveRootItem może wskazywać na stary element, który został usunięty z TreeView
            // podczas czyszczenia pustych gałęzi. Dlatego najpierw szukamy realnego Archiwum
            // w CollectionTree, a dopiero potem tworzymy nowe.
            foreach (object item in CollectionTree.Items)
            {
                if (item is TreeViewItem treeItem &&
                    treeItem.Tag is TreeNodeData node &&
                    node.NodeType == "ArchiveRoot")
                {
                    archiveRootItem = treeItem;
                    archiveRootItem.Header = currentLanguage == "pl"
                        ? "Archiwum"
                        : "Archive";

                    return archiveRootItem;
                }
            }

            archiveRootItem = new TreeViewItem
            {
                Header = currentLanguage == "pl"
                    ? "Archiwum"
                    : "Archive",

                Tag = new TreeNodeData
                {
                    NodeType = "ArchiveRoot",
                    BackgroundGroup = "Default"
                }
            };

            CollectionTree.Items.Add(archiveRootItem);

            return archiveRootItem;
        }
        private void RefreshStats()
        {
            int pcCount = collection.PcSystems.Count;

            int consoleCount = collection.HardwareItems
                .Count(x => !IsAccessoryHardware(x));

            int accessoryCount =
                collection.Accessories.Count +
                collection.HardwareItems.Count(IsAccessoryHardware);

            LeftStatsTitle.Text = T("Collection");

            LeftPcCount.Text =
                currentLanguage == "pl"
                    ? $"Ilość komputerów: {pcCount}"
                    : $"Computers: {pcCount}";

            LeftConsoleCount.Text =
                currentLanguage == "pl"
                    ? $"Ilość konsol: {consoleCount}"
                    : $"Consoles: {consoleCount}";

            LeftAccessoryCount.Text =
                currentLanguage == "pl"
                    ? $"Ilość akcesoriów: {accessoryCount}"
                    : $"Accessories: {accessoryCount}";

            HomePageButton.Content =
                currentLanguage == "pl"
                    ? "Strona główna"
                    : "Home";
        }

        // =========================
        // BACKGROUND
        // =========================

        private void UpdateBackgroundForCurrentState()
        {
            string fileName = GetBackgroundFileForState();
            SetBackground(fileName);
        }

        private string GetBackgroundFileForState()
        {
            if (currentPlatformGroup == "Default")
            {
                return currentTheme switch
                {
                    "Modern" => "modern_bg.png",
                    "PlayStation" => "ps_background.png",
                    "Xbox" => "xbox_bg.png",
                    "Nintendo" => "nintendo_bg.png",
                    "SEGA" => "sega_bg.png",
                    "Atari" => "atari_bg.png",
                    "Commodore" => "commodore_bg.png",
                    _ => "default_bg.png"
                };
            }

            return currentPlatformGroup switch
            {
                "Modern" => IsColorThemeFor("Modern")
                    ? "modern_bg.png"
                    : "modern_graphite_bg.png",

                "Sony" => IsColorThemeFor("PlayStation")
                    ? "playstation_bg.png"
                    : "playstation_graphite_bg.png",

                "Microsoft" => IsColorThemeFor("Xbox")
                    ? "xbox_bg.png"
                    : "xbox_graphite_bg.png",

                "Nintendo" => IsColorThemeFor("Nintendo")
                    ? "nintendo_bg.png"
                    : "nintendo_graphite_bg.png",

                "SEGA" => IsColorThemeFor("SEGA")
                    ? "sega_bg.png"
                    : "sega_graphite_bg.png",

                "Atari" => IsColorThemeFor("Atari")
                    ? "atari_bg.png"
                    : "atari_graphite_bg.png",

                "Commodore" => IsColorThemeFor("Commodore")
                    ? "commodore_bg.png"
                    : "commodore_graphite_bg.png",

                "Amiga" or "Amiga (Commodore)" => currentTheme == "Default"
                    ? "amiga_bg.png"
                    : "amiga_graphite_bg.png",

                "PC" => currentTheme == "Default"
                    ? "pc_bg.png"
                    : "pc_graphite_bg.png",

                _ => currentTheme == "Default"
                    ? "default_bg.png"
                    : "default_graphite_bg.png"
            };
        }
        private void HardwareManufacturerCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadHardwareTypesForSelectedManufacturer();
        }

        private void HardwareTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadHardwarePlatformsForSelectedType();
        }
        private void ClearSelectedRecordForNonRecordView()
        {
            selectedGameItem = null;
            selectedHardwareItem = null;
            selectedPcItem = null;
            selectedAccessoryItem = null;

            currentPlatformGroup = "Default";

            UpdateBackgroundForCurrentState();
            UpdatePageBackgroundButtonText();
        }
        private void LoadGameFamiliesForSelectedManufacturer()
        {
            if (GameFamilyCombo == null)
                return;

            GameFamilyCombo.Items.Clear();
            GamePlatformCombo.Items.Clear();

            string manufacturer =
                GameManufacturerCombo.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(manufacturer))
                return;

            if (manufacturer == "PC")
            {
                GameFamilyLabel.Visibility = Visibility.Collapsed;
                GameFamilyCombo.Visibility = Visibility.Collapsed;

                LoadGamePlatformsForSelectedFamily();

                return;
            }

            GameFamilyLabel.Visibility = Visibility.Visible;
            GameFamilyCombo.Visibility = Visibility.Visible;

            if (manufacturer == "Apple")
            {
                GameFamilyCombo.Items.Add("Mac");
                GameFamilyCombo.SelectedIndex = 0;
                return;
            }

            var families = ConsolePlatformDatabase
                .GetFamilies()
                .Where(x => x.Manufacturer == manufacturer)
                .Select(x => x.Name)
                .ToList();

            var customFamilies = collection.HardwareItems
                .Where(x => x.Manufacturer == manufacturer)
                .Select(x => x.Family)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            families = families
                .Concat(customFamilies)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            foreach (string family in families)
                GameFamilyCombo.Items.Add(family);

            if (GameFamilyCombo.Items.Count > 0)
                GameFamilyCombo.SelectedIndex = 0;
        }
        private void LoadGamePlatformsForSelectedFamily()
        {
            if (GamePlatformCombo == null)
                return;

            GamePlatformCombo.Items.Clear();

            string manufacturer =
                GameManufacturerCombo.SelectedItem?.ToString() ?? "";

            string family =
                GameFamilyCombo.SelectedItem?.ToString() ?? "";

            if (manufacturer == "PC")
            {
                GamePlatformCombo.Items.Add(
                    currentLanguage == "pl" ? "Wybierz" : "Select");

                GamePlatformCombo.Items.Add("Windows");
                GamePlatformCombo.Items.Add("DOS");
                GamePlatformCombo.Items.Add("Linux");
                GamePlatformCombo.Items.Add(currentLanguage == "pl" ? "Inne" : "Other");

                GamePlatformCombo.SelectedIndex = 0;
                return;
            }

            if (manufacturer == "Apple")
            {
                GamePlatformCombo.Items.Add(
                    currentLanguage == "pl" ? "Wybierz" : "Select");

                GamePlatformCombo.Items.Add("Classic Mac OS");
                GamePlatformCombo.Items.Add("Mac OS X");
                GamePlatformCombo.Items.Add("macOS");
                GamePlatformCombo.Items.Add(currentLanguage == "pl" ? "Inne" : "Other");

                GamePlatformCombo.SelectedIndex = 0;
                return;
            }

            var platforms = ConsolePlatformDatabase
                .GetPlatforms()
                .Where(x =>
                    x.Manufacturer == manufacturer &&
                    x.Family == family)
                .Select(x => NormalizePlatformForForm(x.Name))
                .ToList();

            var customPlatforms = collection.HardwareItems
                .Where(x =>
                    x.Manufacturer == manufacturer &&
                    x.Family == family)
                .Select(x => x.Platform)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            platforms = platforms
                .Concat(customPlatforms)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            GamePlatformCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            foreach (string platform in platforms)
                GamePlatformCombo.Items.Add(platform);

            GamePlatformCombo.SelectedIndex = 0;
        }
        private void LoadHardwareTypesForSelectedManufacturer()
        {
            if (HardwareTypeCombo == null)
                return;

            HardwareTypeCombo.Items.Clear();
            HardwarePlatformCombo.Items.Clear();

            string manufacturer = HardwareManufacturerCombo.SelectedItem?.ToString() ?? "";

            if (manufacturer == (currentLanguage == "pl" ? "Inny" : "Other"))
            {
                LoadCustomHardwareTypes();
                ApplyHardwareDetailsManualInputMode();
                return;
            }

            GameFamilyLabel.Visibility = Visibility.Visible;
            GameFamilyCombo.Visibility = Visibility.Visible;

            var families = ConsolePlatformDatabase
                .GetFamilies()
                .Where(x => x.Manufacturer == manufacturer)
                .Select(x => x.Name)
                .Distinct()
                .ToList();

            foreach (string family in families)
                HardwareTypeCombo.Items.Add(
                    TranslateHardwareTypeForDisplay(manufacturer, family));

            HardwareTypeCombo.SelectedIndex = 0;
        }
        private void LoadCustomHardwareTypes()
        {
            HardwareTypeCombo.Items.Clear();

            if (currentLanguage == "pl")
            {
                HardwareTypeCombo.Items.Add("Konsole");
                HardwareTypeCombo.Items.Add("Konsole przenośne");
                HardwareTypeCombo.Items.Add("Komputery");
                HardwareTypeCombo.Items.Add("Dodatki");
                HardwareTypeCombo.Items.Add("Multimedia");
                HardwareTypeCombo.Items.Add("Developer Hardware");
                HardwareTypeCombo.Items.Add("Inne");
            }
            else
            {
                HardwareTypeCombo.Items.Add("Consoles");
                HardwareTypeCombo.Items.Add("Handhelds");
                HardwareTypeCombo.Items.Add("Computers");
                HardwareTypeCombo.Items.Add("Add-ons");
                HardwareTypeCombo.Items.Add("Multimedia");
                HardwareTypeCombo.Items.Add("Developer Hardware");
                HardwareTypeCombo.Items.Add("Other");
            }

            HardwareTypeCombo.SelectedIndex = 0;
        }
        private void LoadHardwarePlatformsForSelectedType()
        {
            if (HardwarePlatformCombo == null)
                return;

            HardwarePlatformCombo.Items.Clear();

            string manufacturer = HardwareManufacturerCombo.SelectedItem?.ToString() ?? "";

            if (manufacturer == (currentLanguage == "pl" ? "Inny" : "Other"))
            {
                HardwarePlatformCombo.Visibility = Visibility.Collapsed;
                HardwarePlatformTextBox.Visibility = Visibility.Visible;
                HardwarePlatformLabel.Visibility = Visibility.Visible;

                HardwarePlatformTextBox.Clear();

                return;
            }

            string type = TranslateHardwareTypeBackToKey(
                manufacturer,
                HardwareTypeCombo.SelectedItem?.ToString() ?? "");

            var platforms = ConsolePlatformDatabase
                .GetPlatforms()
                .Where(x => x.Manufacturer == manufacturer && x.Family == type)
                .Select(x => NormalizePlatformForForm(x.Name))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            foreach (string platform in platforms)
                HardwarePlatformCombo.Items.Add(platform);

            if (HardwarePlatformCombo.Items.Count > 0)
            {
                HardwarePlatformCombo.SelectedIndex = 0;
                LoadHardwareDetailsForSelectedPlatform();
            }
        }
        private void LoadAccessoryTypes()
        {
            HardwarePlatformCombo.Items.Clear();

            string[] accessories = currentLanguage == "pl"
                ? new[]
                {
            "Pad / kontroler",
            "Memory Card",
            "Kamera",
            "Pilot",
            "Zasilacz",
            "Kabel AV / HDMI / RF",
            "Adapter",
            "Kierownica",
            "Mikrofon",
            "Stacja dokująca",
            "Inne akcesorium"
                }
                : new[]
                {
            "Pad / controller",
            "Memory Card",
            "Camera",
            "Remote control",
            "Power supply",
            "AV / HDMI / RF cable",
            "Adapter",
            "Steering wheel",
            "Microphone",
            "Docking station",
            "Other accessory"
                };

            foreach (string accessory in accessories)
                HardwarePlatformCombo.Items.Add(accessory);

            HardwarePlatformCombo.SelectedIndex = 0;
        }
        private bool IsColorThemeFor(string themeName)
        {
            return currentTheme == "Default" || currentTheme == themeName;
        }

        private void LoadHardwareConditionDefaults()
        {
            HardwareConditionCombo.Items.Clear();

            HardwareConditionCombo.Items.Add(
                currentLanguage == "pl"
                    ? "Wybierz stan sprzętu"
                    : "Select hardware condition");

            HardwareConditionCombo.Items.Add(currentLanguage == "pl" ? "Nowy" : "New");
            HardwareConditionCombo.Items.Add(currentLanguage == "pl" ? "Idealny" : "Mint");
            HardwareConditionCombo.Items.Add(currentLanguage == "pl" ? "Bardzo dobry" : "Very good");
            HardwareConditionCombo.Items.Add(currentLanguage == "pl" ? "Dobry" : "Good");
            HardwareConditionCombo.Items.Add(currentLanguage == "pl" ? "Dostateczny" : "Acceptable");
            HardwareConditionCombo.Items.Add(currentLanguage == "pl" ? "Słaby" : "Poor");
            HardwareConditionCombo.Items.Add(currentLanguage == "pl" ? "Uszkodzony" : "Damaged");

            HardwareConditionCombo.SelectedIndex = 0;
        }

        private void LoadBoxConditionDefaults()
        {
            HardwareBoxConditionCombo.Items.Clear();

            HardwareBoxConditionCombo.Items.Add(
                currentLanguage == "pl"
                    ? "Wybierz stan pudełka"
                    : "Select box condition");

            HardwareBoxConditionCombo.Items.Add(currentLanguage == "pl" ? "Nowy" : "New");
            HardwareBoxConditionCombo.Items.Add(currentLanguage == "pl" ? "Idealny" : "Mint");
            HardwareBoxConditionCombo.Items.Add(currentLanguage == "pl" ? "Bardzo dobry" : "Very good");
            HardwareBoxConditionCombo.Items.Add(currentLanguage == "pl" ? "Dobry" : "Good");
            HardwareBoxConditionCombo.Items.Add(currentLanguage == "pl" ? "Dostateczny" : "Acceptable");
            HardwareBoxConditionCombo.Items.Add(currentLanguage == "pl" ? "Słaby" : "Poor");
            HardwareBoxConditionCombo.Items.Add(currentLanguage == "pl" ? "Uszkodzony" : "Damaged");

            HardwareBoxConditionCombo.SelectedIndex = 0;
        }
        private void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            if (sender is not TreeViewItem item)
                return;

            CollapseChildTreeItems(item);

            e.Handled = false;
        }

        private void CollapseChildTreeItems(TreeViewItem parent)
        {
            foreach (object child in parent.Items)
            {
                if (child is TreeViewItem childItem)
                {
                    childItem.IsExpanded = false;
                    CollapseChildTreeItems(childItem);
                }
            }
        }

        private string TranslateHardwareTypeForDisplay(string manufacturer, string type)
        {
            if (currentLanguage != "pl")
                return type;

            return manufacturer switch
            {
                "Atari" => type switch
                {
                    "Atari Consoles" => "Konsole",
                    "Atari Handhelds" => "Konsole przenośne",
                    "Modern Atari" => "Współczesne Atari",
                    _ => type
                },

                "SEGA" => type switch
                {
                    "SEGA Consoles" => "Konsole",
                    "SEGA Handhelds" => "Konsole przenośne",
                    "SEGA Add-ons" => "Dodatki",
                    "SEGA Computers" => "Komputery",
                    _ => type
                },

                "Nintendo" => type switch
                {
                    "Home Consoles" => "Konsole",
                    "Handhelds" => "Konsole przenośne",
                    _ => type
                },

                _ => type
            };
        }

        private string TranslateHardwareTypeBackToKey(string manufacturer, string displayType)
        {
            if (currentLanguage != "pl")
                return displayType;

            return manufacturer switch
            {
                "Atari" => displayType switch
                {
                    "Konsole" => "Atari Consoles",
                    "Konsole przenośne" => "Atari Handhelds",
                    "Współczesne Atari" => "Modern Atari",
                    _ => displayType
                },

                "SEGA" => displayType switch
                {
                    "Konsole" => "SEGA Consoles",
                    "Konsole przenośne" => "SEGA Handhelds",
                    "Dodatki" => "SEGA Add-ons",
                    "Komputery" => "SEGA Computers",
                    _ => displayType
                },

                "Nintendo" => displayType switch
                {
                    "Konsole" => "Home Consoles",
                    "Konsole przenośne" => "Handhelds",
                    _ => displayType
                },

                _ => displayType
            };
        }

        private string TranslateHardwareTypeBackToKey(string displayType)
        {
            if (currentLanguage == "pl")
            {
                return displayType switch
                {
                    "Konsole" => "Atari Consoles",
                    "Konsole przenośne" => "Atari Handhelds",
                    "Współczesne Atari" => "Modern Atari",
                    _ => displayType
                };
            }

            return displayType;
        }
        private void LoadHardwareDetailsForSelectedPlatform()
        {
            string manufacturer =
                HardwareManufacturerCombo.SelectedItem?.ToString() ?? "";

            string platform =
                HardwarePlatformCombo.SelectedItem?.ToString() ?? "";

            LoadModels(manufacturer, platform);

            LoadBoardRevisions(manufacturer, platform);
            LoadColors(manufacturer, platform);
            LoadEditions(manufacturer, platform);

            LoadRevisionsForSelectedModelVariant();

            HardwareColorCombo.IsEnabled = true;

            ApplyHardwareDetailsManualInputMode();
        }

        private void ApplyHardwareDetailsManualInputMode()
        {
            SetComboOrTextVisibility(
                HardwareModelLabel,
                HardwareModelCombo,
                HardwareModelTextBox,
                HardwareModelCombo.Items.Count > 0,
                true);

            SetComboOrTextVisibility(
                HardwareRevisionLabel,
                HardwareRevisionCombo,
                HardwareRevisionTextBox,
                HardwareRevisionCombo.Items.Count > 0,
                true);

            SetComboOrTextVisibility(
                HardwareBoardRevisionLabel,
                HardwareBoardRevisionCombo,
                HardwareBoardRevisionTextBox,
                HardwareBoardRevisionCombo.Items.Count > 0,
                true);

            SetComboOrTextVisibility(
                HardwareRegionLabel,
                HardwareRegionCombo,
                HardwareRegionTextBox,
                HardwareRegionCombo.Items.Count > 0,
                true);

            SetComboOrTextVisibility(
                HardwareColorLabel,
                HardwareColorCombo,
                HardwareColorTextBox,
                HardwareColorCombo.Items.Count > 0,
                true);

            SetComboOrTextVisibility(
                HardwareEditionLabel,
                HardwareEditionCombo,
                HardwareEditionTextBox,
                HardwareEditionCombo.Items.Count > 0,
                true);
        }
        private void LoadModels(string manufacturer, string platform)
        {
            HardwareModelCombo.Items.Clear();

            currentHardwareModels = HardwareModelDatabase
                .GetModels()
                .Where(x =>
                    NormalizeManufacturerForHardwareModels(x.Manufacturer) == NormalizeManufacturerForHardwareModels(manufacturer) &&
                    NormalizePlatformForForm(x.Platform) == platform)
                .ToList();

            var modelVariants = currentHardwareModels
                .Select(x => GetModelVariantName(x))
                .Distinct()
                .OrderBy(x => GetModelSortOrder(x))
                .ThenBy(x => x)
                .ToList();

            if (modelVariants.Count > 0)
            {
                HardwareModelCombo.Items.Add(
                    currentLanguage == "pl" ? "Wybierz" : "Select");

                foreach (string model in modelVariants)
                {
                    string displayModel =
                        currentLanguage == "pl" && model == "Other / Custom"
                            ? "Niestandardowy"
                            : model;

                    HardwareModelCombo.Items.Add(displayModel);
                }

                HardwareModelCombo.SelectedIndex = 0;
            }
        }
        private string NormalizeManufacturerForHardwareModels(string manufacturer)
        {
            return manufacturer switch
            {
                "Amiga (Commodore)" => "Amiga",
                _ => manufacturer
            };
        }
        private void SetFieldVisibility(TextBlock label, Control control, bool visible)
        {
            Visibility visibility = visible ? Visibility.Visible : Visibility.Collapsed;

            label.Visibility = visibility;
            control.Visibility = visibility;
        }
        private bool IsSelectValue(string value)
        {
            return value == "Wybierz" ||
                   value == "Select" ||
                   value == "Brak" ||
                   value == "None";
        }

        private string GetComboOrTextValue(ComboBox combo, TextBox textBox)
        {
            if (combo.Visibility == Visibility.Visible)
            {
                string value = combo.Text?.Trim() ?? "";
                return IsSelectValue(value) ? "" : value;
            }

            return textBox.Text?.Trim() ?? "";
        }
        private void SetComboOrTextVisibility(
             TextBlock label,
             ComboBox combo,
             TextBox textBox,
             bool hasComboItems,
             bool showTextBoxWhenEmpty = true)
        {
            bool showTextBox =
                !hasComboItems && showTextBoxWhenEmpty;

            label.Visibility =
                hasComboItems || showTextBox
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            combo.Visibility =
                hasComboItems
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            textBox.Visibility =
                showTextBox
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
        private bool IsOtherCustomValue(string value)
        {
            return value == "Other / Custom" ||
                   value == "Niestandardowy" ||
                   value == "Other";
        }
        private void LoadRevisions(string manufacturer, string platform)
        {
            HardwareRevisionCombo.Items.Clear();
        }
        private void LoadBoardRevisions(
        string manufacturer,
        string platform)
        {
            HardwareBoardRevisionCombo.Items.Clear();

            var revisions = HardwareBoardRevisionDatabase
                .GetBoardRevisions()
                .Where(x =>
                    x.Manufacturer == manufacturer &&
                    x.Platform == platform)
                .Select(x => x.RevisionName)
                .Distinct()
                .OrderBy(x => x);

            var revisionList = revisions
    .Where(x => !string.IsNullOrWhiteSpace(x))
    .ToList();

            bool hasItems = revisionList.Count > 0;

            if (hasItems)
            {
                HardwareBoardRevisionCombo.Items.Add(
                    currentLanguage == "pl" ? "Wybierz" : "Select");

                foreach (var revision in revisionList)
                    HardwareBoardRevisionCombo.Items.Add(revision);

                HardwareBoardRevisionCombo.SelectedIndex = 0;
            }

            SetComboOrTextVisibility(
                HardwareBoardRevisionLabel,
                HardwareBoardRevisionCombo,
                HardwareBoardRevisionTextBox,
                hasItems,
                true);
        }
        private void LoadRegions(string manufacturer, string platform)
        {
            HardwareRegionCombo.Items.Clear();
        }
        private void LoadColors(string manufacturer, string platform)
        {
            HardwareColorCombo.Items.Clear();

            var colors = HardwareColorDatabase
                .GetColors()
                .Where(x =>
                    x.Manufacturer == manufacturer &&
                    x.Platform == platform)
                .Select(x => x.ColorName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            bool hasColors = colors.Count > 0;

            if (hasColors)
            {
                HardwareColorCombo.Items.Add(
                    currentLanguage == "pl" ? "Wybierz" : "Select");

                foreach (var color in colors)
                    HardwareColorCombo.Items.Add(color);

                HardwareColorCombo.SelectedIndex = 0;
            }

            SetComboOrTextVisibility(
                HardwareColorLabel,
                HardwareColorCombo,
                HardwareColorTextBox,
                hasColors,
                true);
        }
        private void HardwareModelCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedModel =
                HardwareModelCombo.SelectedItem?.ToString() ?? "";

            if (IsOtherCustomValue(selectedModel))
            {
                SetComboOrTextVisibility(
                    HardwareRevisionLabel,
                    HardwareRevisionCombo,
                    HardwareRevisionTextBox,
                    false,
                    true);

                SetComboOrTextVisibility(
                    HardwareBoardRevisionLabel,
                    HardwareBoardRevisionCombo,
                    HardwareBoardRevisionTextBox,
                    false,
                    true);

                SetComboOrTextVisibility(
                    HardwareRegionLabel,
                    HardwareRegionCombo,
                    HardwareRegionTextBox,
                    false,
                    true);

                SetComboOrTextVisibility(
                    HardwareColorLabel,
                    HardwareColorCombo,
                    HardwareColorTextBox,
                    false,
                    true);

                SetComboOrTextVisibility(
                    HardwareEditionLabel,
                    HardwareEditionCombo,
                    HardwareEditionTextBox,
                    false,
                    true);

                return;
            }

            LoadRevisionsForSelectedModelVariant();
        }
        private void ClearHardwareDetailsTextBoxes()
        {
            HardwareModelTextBox.Text = "";
            HardwareRevisionTextBox.Text = "";
            HardwareBoardRevisionTextBox.Text = "";
            HardwareRegionTextBox.Text = "";
            HardwareColorTextBox.Text = "";
            HardwareEditionTextBox.Text = "";
            HardwareSerialTextBox.Text = "";
        }
        private void LoadRevisionsForSelectedModelVariant()
        {
            HardwareRevisionCombo.Items.Clear();
            HardwareRegionCombo.Items.Clear();

            string selectedVariant =
                HardwareModelCombo.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(selectedVariant) ||
                selectedVariant == "Wybierz" ||
                selectedVariant == "Select")
                return;

            var revisions = currentHardwareModels
                .Where(x => GetModelVariantName(x) == selectedVariant)
                .Select(x => x.ModelCode)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            bool hasRevisions = revisions.Count > 0;

            if (hasRevisions)
            {
                HardwareRevisionCombo.Items.Add(
                    currentLanguage == "pl" ? "Wybierz" : "Select");

                foreach (var revision in revisions)
                    HardwareRevisionCombo.Items.Add(revision);

                HardwareRevisionCombo.SelectedIndex = 0;
            }

            SetComboOrTextVisibility(
                HardwareRevisionLabel,
                HardwareRevisionCombo,
                HardwareRevisionTextBox,
                hasRevisions,
                true);

            LoadRegionForSelectedRevision();
        }
        private void HardwareRevisionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadRegionForSelectedRevision();
        }
        private void LoadRegionForSelectedRevision()
        {
            HardwareRegionCombo.Items.Clear();

            string selectedModel =
                HardwareModelCombo.SelectedItem?.ToString() ?? "";

            string selectedRevision =
                HardwareRevisionCombo.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(selectedModel) ||
                string.IsNullOrWhiteSpace(selectedRevision) ||
                selectedModel == "Wybierz" ||
                selectedModel == "Select" ||
                selectedRevision == "Wybierz" ||
                selectedRevision == "Select")
            {
                SetComboOrTextVisibility(
                    HardwareRegionLabel,
                    HardwareRegionCombo,
                    HardwareRegionTextBox,
                    false,
                    true);

                return;
            }

            var regions = currentHardwareModels
                .Where(x =>
                    GetModelVariantName(x) == selectedModel &&
                    x.ModelCode == selectedRevision)
                .Select(x => x.Region)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            bool hasRegions = regions.Count > 0;

            if (hasRegions)
            {
                HardwareRegionCombo.Items.Add(
                    currentLanguage == "pl" ? "Wybierz" : "Select");

                foreach (var region in regions)
                    HardwareRegionCombo.Items.Add(region);

                HardwareRegionCombo.SelectedIndex = 0;
            }

            SetComboOrTextVisibility(
                HardwareRegionLabel,
                HardwareRegionCombo,
                HardwareRegionTextBox,
                hasRegions,
                true);
        }
        private void LoadEditions(string manufacturer, string platform)
        {
            HardwareEditionCombo.Items.Clear();

            var editions = HardwareEditionDatabase
                .GetEditions()
                .Where(x =>
                    x.Manufacturer == manufacturer &&
                    x.Platform == platform)
                .Select(x => x.EditionName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            bool hasEditions = editions.Count > 0;

            if (hasEditions)
            {
                HardwareEditionCombo.Items.Add(
                    currentLanguage == "pl" ? "Brak" : "None");

                foreach (var edition in editions)
                    HardwareEditionCombo.Items.Add(edition);

                HardwareEditionCombo.SelectedIndex = 0;
            }

            SetComboOrTextVisibility(
                HardwareEditionLabel,
                HardwareEditionCombo,
                HardwareEditionTextBox,
                hasEditions,
                true);
        }
        private void HardwareEditionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedEdition =
                HardwareEditionCombo.SelectedItem?.ToString() ?? "";

            string noneText =
                currentLanguage == "pl" ? "Brak" : "None";

            if (string.IsNullOrWhiteSpace(selectedEdition) ||
                selectedEdition == noneText)
            {
                HardwareColorCombo.IsEnabled = true;
                return;
            }

            HardwareColorCombo.Items.Clear();
            HardwareColorCombo.Items.Add(
                currentLanguage == "pl"
                    ? "Edycja kolekcjonerska"
                    : "Collector edition");

            HardwareColorCombo.SelectedIndex = 0;
            HardwareColorCombo.IsEnabled = false;
        }
        private int GetModelSortOrder(string name)
        {
            string n = (name ?? "").Trim().ToLowerInvariant();

            // Other / Custom zawsze na końcu
            if (n.Contains("other") || n.Contains("custom"))
                return 9999;

            // PlayStation
            if (n == "playstation") return 10;

            if (n == "playstation 3 fat" || n == "playstation 3") return 30;
            if (n == "playstation 3 slim") return 31;
            if (n == "playstation 3 superslim" || n == "playstation 3 super slim") return 32;

            if (n == "playstation 4 fat" || n == "playstation 4") return 40;
            if (n == "playstation 4 slim") return 41;
            if (n == "playstation 4 pro") return 42;

            if (n == "playstation 5 fat" || n == "playstation 5") return 50;
            if (n == "playstation 5 digital") return 51;
            if (n == "playstation 5 slim") return 52;
            if (n == "playstation 5 slim digital") return 53;
            if (n == "playstation 5 pro") return 54;

            // Xbox
            if (n == "xbox") return 100;

            if (n == "xbox 360 arcade fat") return 110;
            if (n == "xbox 360 core fat") return 111;
            if (n == "xbox 360 premium/pro fat" || n == "xbox 360 premium fat" || n == "xbox 360 pro fat") return 112;
            if (n == "xbox 360e" || n == "xbox 360 e") return 113;
            if (n == "xbox 360 s slim" || n == "xbox 360 s" || n == "xbox 360 slim") return 114;

            if (n == "xbox one") return 120;
            if (n == "xbox one s") return 121;
            if (n == "xbox one x") return 122;

            if (n == "xbox series s 512gb") return 130;
            if (n == "xbox series s 1tb") return 131;

            if (n == "xbox series x 1tb") return 140;
            if (n == "xbox series x digital 1tb") return 141;
            if (n == "xbox series x 2tb galaxy black") return 142;

            // Nintendo
            if (n == "nintendo switch") return 200;
            if (n == "nintendo switch lite") return 201;
            if (n == "nintendo switch oled") return 202;
            if (n == "nintendo switch 2") return 203;

            return 5000;
        }

        private void SetBackground(string fileName)
        {
            ThemeBackground.Stretch = Stretch.Fill;

            try
            {
                ThemeBackground.Source = new BitmapImage(
                    new Uri($"Assets/Backgrounds/{fileName}", UriKind.Relative));
            }
            catch
            {
            }
        }

        // =========================
        // WINDOW BUTTONS
        // =========================

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void HardwarePlatformCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadHardwareDetailsForSelectedPlatform();
        }
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            bool close = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "Czy chcesz zamknąć aplikację HGC?\n\nNiezapisane zmiany zostaną utracone."
                    : "Do you want to close HGC?\n\nUnsaved changes will be lost.",
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!close)
                return;

            Close();
        }
        private void SaveCollectionChanges()
        {
            HgcStorageService.SaveCollection(collection);
        }
        private void ClearCollection()
        {
            bool confirm = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "To usunie całą kolekcję: gry, sprzęt, PC, akcesoria oraz archiwum.\n\nOPERACJA JEST NIEODWRACALNA.\n\nCzy na pewno chcesz kontynuować?"
                    : "This will remove the entire collection: games, hardware, PC, accessories and archive.\n\nTHIS OPERATION CANNOT BE UNDONE.\n\nAre you sure you want to continue?",
                "HGC",
                HgcMessageBoxType.Warning,
                currentLanguage);

            if (!confirm)
                return;

            // Dane
            collection.Games.Clear();
            collection.HardwareItems.Clear();
            collection.PcSystems.Clear();
            collection.Accessories.Clear();

            collection.ArchivedGames.Clear();
            collection.ArchivedHardwareItems.Clear();
            collection.ArchivedPcSystems.Clear();
            collection.ArchivedAccessories.Clear();

            // Zaznaczone rekordy
            selectedGameItem = null;
            selectedHardwareItem = null;
            selectedPcItem = null;
            selectedAccessoryItem = null;

            // Edytowane rekordy
            editingGameItem = null;
            editingHardwareItem = null;
            editingPcItem = null;
            editingAccessoryItem = null;

            // Reset filtrów
            currentGameFilter = GameFilterMode.All;
            currentGameGenre = "";
            currentPlatformGroup = "Default";
            isGamesViewActive = false;

            // Wyczyść drzewa
            CollectionTree.Items.Clear();
            GamesTree.Items.Clear();

            // Odbuduj pusty interfejs
            BuildGamesPanel();
            RefreshStats();

            UpdateBackgroundForCurrentState();

            DeleteHgcMediaFolder("Photos");
            DeleteHgcMediaFolder("Backgrounds");
            DeleteHgcMediaFolder("Thumbnails");

            SaveCollectionChanges();

            ShowDefaultRightPanel();

            HgcMessageBox.Show(
                this,
                currentLanguage == "pl"
                    ? "Kolekcja została wyczyszczona."
                    : "Collection has been cleared.",
                "HGC",
                HgcMessageBoxType.Information,
                currentLanguage);
        }
        private void DeleteHgcMediaFolder(string folderName)
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "HGC",
                folderName);

            try
            {
                if (Directory.Exists(folder))
                    Directory.Delete(folder, true);

                Directory.CreateDirectory(folder);
            }
            catch (Exception ex)
            {
                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? $"Nie udało się wyczyścić folderu {folderName}:\n\n{ex.Message}"
                        : $"Failed to clear {folderName} folder:\n\n{ex.Message}",
                    "HGC",
                    HgcMessageBoxType.Warning,
                    currentLanguage);
            }
        }

        private void MenuClearCollection_Click(object sender, RoutedEventArgs e)
        {
            ClearCollection();
        }

        private void MenuRestartApplication_Click(object sender, RoutedEventArgs e)
        {
            bool restart = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "Czy chcesz ponownie uruchomić aplikację HGC?\n\nNiezapisane zmiany zostaną utracone."
                    : "Do you want to restart HGC?\n\nUnsaved changes will be lost.",
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!restart)
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = Environment.ProcessPath!,
                Arguments = "--no-splash",
                UseShellExecute = true
            });

            Application.Current.Shutdown();
        }

        private void MenuExitApplication_Click(object sender, RoutedEventArgs e)
        {
            bool close = HgcMessageBox.ShowYesNo(
                this,
                currentLanguage == "pl"
                    ? "Czy chcesz zamknąć aplikację HGC?\n\nNiezapisane zmiany zostaną utracone."
                    : "Do you want to close HGC?\n\nUnsaved changes will be lost.",
                "HGC",
                HgcMessageBoxType.Question,
                currentLanguage);

            if (!close)
                return;

            Application.Current.Shutdown();
        }
        private void SaveSettingsChanges()
        {
            HgcSettingsService.SaveSettings(settings);
        }
        private void RefreshSortingMenuChecks()
        {
            MenuCategoryOrderHAG.IsChecked = collectionCategoryOrder == "Hardware,Accessories,Games";
            MenuCategoryOrderHGA.IsChecked = collectionCategoryOrder == "Hardware,Games,Accessories";
            MenuCategoryOrderAHG.IsChecked = collectionCategoryOrder == "Accessories,Hardware,Games";
            MenuCategoryOrderAGH.IsChecked = collectionCategoryOrder == "Accessories,Games,Hardware";
            MenuCategoryOrderGHA.IsChecked = collectionCategoryOrder == "Games,Hardware,Accessories";
            MenuCategoryOrderGAH.IsChecked = collectionCategoryOrder == "Games,Accessories,Hardware";

            MenuShowHardwareCategory.IsChecked = showHardwareCategory;
            MenuShowAccessoriesCategory.IsChecked = showAccessoriesCategory;
            MenuShowGamesCategory.IsChecked = showGamesCategory;

            MenuCategoryLinesAuto.IsChecked = collectionCategoryLinesAuto;
            MenuCategoryLines1.IsChecked = !collectionCategoryLinesAuto && collectionCategoryLines == 1;
            MenuCategoryLines2.IsChecked = !collectionCategoryLinesAuto && collectionCategoryLines == 2;
            MenuCategoryLines3.IsChecked = !collectionCategoryLinesAuto && collectionCategoryLines == 3;
            MenuCategoryLines4.IsChecked = !collectionCategoryLinesAuto && collectionCategoryLines == 4;

            MenuTileSizeSmall.IsChecked = collectionTileSize == "Small";
            MenuTileSizeMedium.IsChecked = collectionTileSize == "Medium";
            MenuTileSizeLarge.IsChecked = collectionTileSize == "Large";

            MenuSortAZ.IsChecked = collectionSortMode == "AZ";
            MenuSortZA.IsChecked = collectionSortMode == "ZA";
            MenuSortAdded.IsChecked = collectionSortMode == "Added";

            MenuItemsPerPageAuto.IsChecked = collectionItemsPerPageMode == "Auto";
            MenuItemsPerPage50.IsChecked = collectionItemsPerPageMode != "Auto" && collectionItemsPerPage == 50;
            MenuItemsPerPage60.IsChecked = collectionItemsPerPageMode != "Auto" && collectionItemsPerPage == 60;
            MenuItemsPerPage75.IsChecked = collectionItemsPerPageMode != "Auto" && collectionItemsPerPage == 75;
            MenuItemsPerPage100.IsChecked = collectionItemsPerPageMode != "Auto" && collectionItemsPerPage == 100;

            MenuRecentlyAddedShow.IsChecked = showRecentlyAddedPanel;
            MenuRecentlyAddedHide.IsChecked = !showRecentlyAddedPanel;
        }
        private BitmapImage LoadBackgroundImageWithoutLock(string path)
        {
            BitmapImage image = new BitmapImage();

            image.BeginInit();
            image.UriSource = new Uri(path, UriKind.Absolute);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();

            return image;
        }
     
        private void MenuCreateBackup_Click(object sender, RoutedEventArgs e)
        {
            HgcBackupService.CreateBackup(
                this,
                HgcStorageService.GetDataFolder(),
                currentLanguage);
        }

        private void MenuRestoreBackup_Click(object sender, RoutedEventArgs e)
        {
            bool restored = HgcBackupService.RestoreBackup(
                this,
                HgcStorageService.GetDataFolder(),
                currentLanguage);

            if (!restored)
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = Environment.ProcessPath!,
                Arguments = "--no-splash",
                UseShellExecute = true
            });

            Application.Current.Shutdown();
        }
        private void RecentlyAddedFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (recentlyAddedGame == null)
                return;

            recentlyAddedGame.IsFavorite = !recentlyAddedGame.IsFavorite;

            UpdateRecentlyAddedIcons();

            RecentlyAddedFavoriteButton.ToolTip =
                recentlyAddedGame.IsFavorite
                    ? (currentLanguage == "pl" ? "Usuń z ulubionych" : "Remove from favorites")
                    : (currentLanguage == "pl" ? "Dodaj do ulubionych" : "Add to favorites");

            RecentlyAddedFavoriteIcon.Source = new BitmapImage(
                new Uri(
                    recentlyAddedGame.IsFavorite
                        ? "/Assets/Icons/favorite.png"
                        : "/Assets/Icons/favorite_empty.png",
                    UriKind.Relative));

            RebuildCollectionTree();
            BuildGamesPanel();
            RefreshStats();

            ShowDefaultRightPanel();

            SaveCollectionChanges();
        }
        private void RecentlyAddedDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (recentlyAddedGame == null)
                return;

            selectedGameItem = recentlyAddedGame;
            selectedHardwareItem = null;

            DeleteRecordButton_Click(sender, e);
        }
        private void RecentlyAddedEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (recentlyAddedGame == null)
                return;

            selectedGameItem = recentlyAddedGame;
            selectedHardwareItem = null;

            EditRecordButton_Click(sender, e);
        }
        private void UpdateRecentlyAddedIcons()
        {
            string suffix =
                currentTheme == "Commodore" ||
                currentTheme == "Atari" ||
                currentTheme == "PlayStation" ||
                currentTheme == "Nintendo" ||
                currentTheme == "Xbox" ||
                currentTheme == "Modern"
                    ? "_white"
                    : "";

            RecentlyAddedDeleteIcon.Source =
                new BitmapImage(new Uri($"/Assets/Icons/trash{suffix}.png", UriKind.Relative));

            RecentlyAddedEditIcon.Source =
                new BitmapImage(new Uri($"/Assets/Icons/edit{suffix}.png", UriKind.Relative));

            RecentlyAddedPreviewIcon.Source =
                new BitmapImage(new Uri($"/Assets/Icons/view{suffix}.png", UriKind.Relative));

            RecentlyAddedFavoriteIcon.Source =
                new BitmapImage(new Uri(
                    recentlyAddedGame != null && recentlyAddedGame.IsFavorite
                        ? $"/Assets/Icons/favorite{suffix}.png"
                        : $"/Assets/Icons/favorite_empty{suffix}.png",
                    UriKind.Relative));
        }
        private string GetTileDisplayName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "";

            name = name.Trim();

            if (name.Length <= 40)
                return name;

            return name.Substring(0, 40).TrimEnd() + "...";
        }    
        private bool ValidateNameLength(string name)
        {
            if (name.Trim().Length <= 50)
                return true;

            HgcMessageBox.Show(
                this,
                currentLanguage == "pl"
                   ? "Nazwa może zawierać maksymalnie 50 znaków."
                   : "The name can contain a maximum of 50 characters.",
                "HGC",
                HgcMessageBoxType.Warning,
                currentLanguage);

            return false;
        }
        private void RefreshServiceHistoryList()
        {
            ServiceHistoryListBox.Items.Clear();

            if (selectedHardwareItem == null)
                return;

            foreach (ServiceEntry entry in selectedHardwareItem.ServiceHistory)
            {
                string date = entry.ServiceDate.HasValue
                    ? entry.ServiceDate.Value.ToString("yyyy-MM-dd")
                    : "";

                string cost = entry.Cost.HasValue
                    ? $"{entry.Cost.Value:0.00} {entry.CostCurrency}"
                    : "";

                ServiceHistoryListBox.Items.Add(
                   $"{date}  •  {ServiceTypeDatabase.Translate(entry.ServiceType, currentLanguage)}  •  {cost}");
            }
        }
        private void ServiceHistoryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedHardwareItem == null)
                return;

            if (ServiceHistoryListBox.SelectedIndex < 0)
                return;

            if (ServiceHistoryListBox.SelectedIndex >= selectedHardwareItem.ServiceHistory.Count)
                return;

            editingServiceEntry =
                selectedHardwareItem.ServiceHistory[ServiceHistoryListBox.SelectedIndex];

            isEditingService = true;

            ServiceDatePicker.SelectedDate = editingServiceEntry.ServiceDate;

            ServiceTypeCombo.Text =
                ServiceTypeDatabase.Translate(editingServiceEntry.ServiceType, currentLanguage);

            ServiceNotesTextBox.Text = editingServiceEntry.CustomNote;

            ServiceCostTextBox.Text =
                editingServiceEntry.Cost.HasValue
                    ? editingServiceEntry.Cost.Value.ToString("0.00")
                    : "";

            ServiceCostCurrencyCombo.SelectedItem =
                string.IsNullOrWhiteSpace(editingServiceEntry.CostCurrency)
                    ? defaultCurrency
                    : editingServiceEntry.CostCurrency;

            SaveServiceButton.Content =
                currentLanguage == "pl"
                    ? "Zapisz zmiany"
                    : "Save changes";
        }
        private void DeleteServiceEntryButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedHardwareItem == null)
                return;

            if (ServiceHistoryListBox.SelectedIndex < 0)
                return;

            if (ServiceHistoryListBox.SelectedIndex >= selectedHardwareItem.ServiceHistory.Count)
                return;

            selectedHardwareItem.ServiceHistory.RemoveAt(ServiceHistoryListBox.SelectedIndex);

            editingServiceEntry = null;
            isEditingService = false;

            ServiceDatePicker.SelectedDate = DateTime.Today;
            ServiceTypeCombo.SelectedIndex = 0;
            ServiceCostTextBox.Text = "";
            ServiceCostCurrencyCombo.SelectedItem = defaultCurrency;
            ServiceNotesTextBox.Text = "";

            SaveServiceButton.Content =
                currentLanguage == "pl"
                    ? "Dodaj wpis"
                    : "Add entry";

            RefreshServiceHistoryList();
            ShowHardwareDetails(selectedHardwareItem);
            RefreshStats();
            SaveCollectionChanges();
        }
        private void LeftCollectionStatsPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ShowHardwareAccessoriesOverview();
        }

        private void ShowHardwareAccessoriesOverview()
        {
            isGamesViewActive = false;
            currentGameGenre = "";
            currentGameFilter = GameFilterMode.All;

            collectionCurrentPage = 1;

            HideRightPanels();

            CollectionTilesPanel.Visibility = Visibility.Visible;
            CollectionTilesSectionsPanel.Children.Clear();
            currentCollectionTilesWrap = null;

            RecentlyAddedBorder.Visibility = Visibility.Collapsed;

            CollectionTilesTitle.Text =
                currentLanguage == "pl"
                    ? "Sprzęt i akcesoria"
                    : "Hardware and accessories";

            CollectionTilesTitle.Visibility = Visibility.Visible;

            if (collection.PcSystems.Count > 0)
            {
                AddTilesSectionTitle(currentLanguage == "pl" ? "Komputery" : "Computers");

                foreach (PcSystem pc in SortPcItems(collection.PcSystems))
                    AddPcTile(pc);
            }

            var hardwareItems = SortHardwareItems(
                collection.HardwareItems.Where(x => !IsAccessoryHardware(x)));

            if (hardwareItems.Count > 0)
            {
                AddTilesSectionTitle(currentLanguage == "pl" ? "Sprzęt" : "Hardware");

                foreach (Hardware hardware in hardwareItems)
                    AddHardwareTile(hardware);
            }

            var accessoryHardwareItems = SortHardwareItems(
                collection.HardwareItems.Where(IsAccessoryHardware));

            var accessoryItems = SortAccessoryItems(collection.Accessories);

            if (accessoryHardwareItems.Count > 0 || accessoryItems.Count > 0)
            {
                AddTilesSectionTitle(currentLanguage == "pl" ? "Akcesoria" : "Accessories");

                foreach (Hardware accessoryHardware in accessoryHardwareItems)
                    AddHardwareTile(accessoryHardware);

                foreach (Accessory accessory in accessoryItems)
                    AddAccessoryTile(accessory);
            }

            CollectionPaginationBar.Visibility = Visibility.Collapsed;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                CollectionTilesScrollViewer.ScrollToTop();
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }
        private string NormalizeGenreForSave(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre))
                return "";

            return GenreDatabase.TranslateGenre(genre.Trim(), "pl");
        }
        private string NormalizeConditionForSave(string condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
                return "";

            return ConditionDatabase.Translate(condition.Trim(), "pl");
        }
        private ImageSource GetCachedTileImage(string path, int decodeSize)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return null;

            string cacheKey = $"{path}|{decodeSize}";

            if (tileImageCache.TryGetValue(cacheKey, out ImageSource cachedImage))
                return cachedImage;

            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.DecodePixelWidth = decodeSize;
            bitmap.UriSource = new Uri(path, UriKind.Absolute);
            bitmap.EndInit();

            bitmap.Freeze();

            tileImageCache[cacheKey] = bitmap;

            return bitmap;
        }
    }
}