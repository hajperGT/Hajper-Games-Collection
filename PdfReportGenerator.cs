using Hajper_Game_Collection.Data;
using HGC.Models;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System.IO;
using System.Windows;
using System.Windows.Resources;

namespace HGC
{
    internal static class PdfReportGenerator
    {
        #region Layout

        private const double PageMargin = 40;
        private const double CardPadding = 16;
        private const double CardSpacing = 18;
        private const double CardRadius = 10;
        private const double ShadowOffset = 2;
        private const double AccentWidth = 6;

        private const double HeaderHeight = 36;
        private const double FooterHeight = 28;

        #endregion

        #region Colors

        private static readonly XColor PageColor =
            XColor.FromArgb(244, 245, 247);

        private static readonly XColor CardColor =
            XColor.FromArgb(250, 251, 253);

        private static readonly XColor BorderColor =
            XColor.FromArgb(205, 210, 218);

        private static readonly XColor ShadowColor =
            XColor.FromArgb(230, 232, 236);

        private static readonly XColor AccentColor =
            XColor.FromArgb(32, 126, 230);

        private static readonly XColor TextColor =
            XColor.FromArgb(35, 35, 35);

        private static readonly XColor SecondaryTextColor =
            XColor.FromArgb(115, 115, 115);

        #endregion

        #region Brushes

        private static readonly XBrush CardBrush =
            new XSolidBrush(CardColor);

        private static readonly XBrush ShadowBrush =
            new XSolidBrush(ShadowColor);

        private static readonly XBrush AccentBrush =
            new XSolidBrush(AccentColor);

        private static readonly XBrush TextBrush =
            new XSolidBrush(TextColor);

        private static readonly XBrush SecondaryBrush =
            new XSolidBrush(SecondaryTextColor);

        private static readonly XPen BorderPen =
            new XPen(BorderColor, 1);

        private const string PdfLogoFile =
            @"Assets\hgcmainico.png";

        private const string PdfBackgroundFile =
            @"Assets\Icons\ps_addaccessories.png";

        #endregion
        private static PdfDocument document = null!;
        private static PdfPage page = null!;
        private static XGraphics? gfx;

        private static double pageWidth;
        private static double pageHeight;
        private static double margin;
        private static double contentWidth;
        private static double currentY;
        private static bool numberPages;
        private static string currentLanguage = "pl";
        private static CollectionDatabase currentCollection = null!;
        private static string currentDefaultCurrency = "";

        internal static void Generate(
            CollectionDatabase collection,
            string defaultCurrency,
            string currentLanguage,
            PdfExportOptionsWindow options,
            string filePath)
        {
            if (GlobalFontSettings.FontResolver == null)
            {
                GlobalFontSettings.FontResolver = new HgcPdfFontResolver();
            }

            PdfReportGenerator.currentLanguage = currentLanguage;
            numberPages = options.NumberPages;
            currentCollection = collection;
            currentDefaultCurrency = defaultCurrency;

            document = new PdfDocument();
            document.Info.Title = "Hajper Games Collection";

            margin = 42;

            AddPage();

            if (options.IncludeTitlePage)
                DrawTitlePage();

            if (options.IncludeStatistics)
            {
                DrawStatisticsSummaryPage();
                DrawDetailedStatisticsPage();
                DrawPlatformManufacturerStatisticsPage();
            }

            if (options.IncludeHardware ||
                options.IncludePc ||
                options.IncludeGames ||
                options.IncludeAccessories ||
                options.IncludeService)
            {
                DrawCollectionItemsPages(options);
            }

            if (options.IncludeStatistics)
            {
                DrawHighlightsPage();
            }

            gfx?.Dispose();
            gfx = null;

            DrawAllFooters();

            document.Save(filePath);
            document.Close();
        }
        private static void DrawCollectionItemsPages(
        PdfExportOptionsWindow options)
        {
            AddPage();

            DrawSectionHeader(T(
                "Wykaz kolekcji",
                "Collection list"));

            XFont introFont = new XFont("Segoe UI", 10.5, XFontStyleEx.Regular);

            gfx!.DrawString(
                T(
                    "Szczegółowy wykaz elementów zapisanych w kolekcji.",
                    "Detailed list of items stored in the collection."),
                introFont,
                SecondaryBrush,
                new XRect(margin, currentY - 8, contentWidth, 20),
                XStringFormats.TopLeft);

            currentY += 30;

            if (options.IncludeHardware)
                DrawHardwareListSection(options);

            if (options.IncludePc)
                DrawPcListSection(options);

            if (options.IncludeGames)
                DrawGamesListSection(options);

            if (options.IncludeAccessories)
                DrawAccessoriesListSection(options);

            DrawServiceHistorySection(options);
        }
        private static void DrawServiceHistorySection(
        PdfExportOptionsWindow options)
        {
            bool hasHardwareService = currentCollection.HardwareItems
                .Any(h => h.ServiceHistory.Count > 0);

            bool hasPcService = currentCollection.PcSystems
                .Any(p => p.ServiceHistory.Count > 0);

            if (!hasHardwareService && !hasPcService)
                return;

            EnsureSpaceForSection(170);
            DrawSectionHeader(T("Historia serwisu", "Service history"));

            foreach (Hardware hardware in currentCollection.HardwareItems
                .Where(h => h.ServiceHistory.Count > 0))
            {
                string title = !string.IsNullOrWhiteSpace(hardware.CustomName)
                    ? hardware.CustomName
                    : !string.IsNullOrWhiteSpace(hardware.Model)
                        ? hardware.Model
                        : !string.IsNullOrWhiteSpace(hardware.Platform)
                            ? hardware.Platform
                            : T("Sprzęt", "Hardware");

                string subtitle = string.Join(" • ",
                    new[]
                    {
                hardware.Manufacturer,
                hardware.Platform
                    }.Where(x => !string.IsNullOrWhiteSpace(x)));

                DrawServiceDeviceCard(
                    @"Assets\Icons\PDF\service.png",
                    title,
                    subtitle,
                    hardware.ServiceHistory
                        .OrderByDescending(s => s.ServiceDate)
                        .ToList());
            }

            foreach (PcSystem pc in currentCollection.PcSystems
                .Where(p => p.ServiceHistory.Count > 0))
            {
                string title = !string.IsNullOrWhiteSpace(pc.Name)
                    ? pc.Name
                    : !string.IsNullOrWhiteSpace(pc.Model)
                        ? pc.Model
                        : T("Komputer", "Computer");

                string subtitle = string.Join(" • ",
                    new[]
                    {
                pc.Manufacturer,
                pc.ComputerType,
                pc.OperatingSystem
                    }.Where(x => !string.IsNullOrWhiteSpace(x)));

                DrawServiceDeviceCard(
                    @"Assets\Icons\PDF\service.png",
                    title,
                    subtitle,
                    pc.ServiceHistory
                        .OrderByDescending(s => s.ServiceDate)
                        .ToList());
            }
        }
        private static void DrawPcListSection(
        PdfExportOptionsWindow options)
        {
            if (currentCollection.PcSystems.Count == 0)
                return;

            EnsureSpaceForSection(170);
            DrawSectionHeader(T("Komputery", "Computers"));

            foreach (PcSystem pc in currentCollection.PcSystems)
            {
                string title = !string.IsNullOrWhiteSpace(pc.Name)
                    ? pc.Name
                    : !string.IsNullOrWhiteSpace(pc.Model)
                        ? pc.Model
                        : T("Komputer", "Computer");

                string subtitle = string.Join(" • ",
                    new[]
                    {
                pc.Manufacturer,
                pc.ComputerType,
                pc.OperatingSystem
                    }.Where(x => !string.IsNullOrWhiteSpace(x)));

                List<CollectionField> fields = new()
                {
                    new(T("Typ", "Type"), pc.ComputerType),
                    new(T("Przenośny", "Portable"), pc.PortableType),
                    new(T("Producent", "Manufacturer"), pc.Manufacturer),
                    new(T("Model", "Model"), pc.Model),
                    new(T("System", "System"), pc.OperatingSystem),
                    new(T("Dodano", "Added"), pc.DateAdded.ToString("dd.MM.yyyy"))
                };

                if (pc.PurchasePrice is > 0)
                {
                    fields.Add(new CollectionField(
                        T("Cena", "Price"),
                        $"{pc.PurchasePrice.Value:N2} {pc.PurchaseCurrency}"));
                }

                string? imagePath = pc.Photos
                    .FirstOrDefault(p => p.IsMain)?.OriginalPath
                    ?? pc.Photos.FirstOrDefault()?.OriginalPath;

                DrawCollectionCard(
                    @"Assets\Icons\PDF\pc.png",
                    title,
                    subtitle,
                    imagePath,
                    fields,
                    options,
                    margin,
                    currentY,
                    contentWidth);
            }
        }
        private static void EnsureSpaceForSection(double firstCardHeight)
        {
            const double sectionHeaderHeight = 40;

            if (currentY + sectionHeaderHeight + firstCardHeight > pageHeight - margin)
            {
                AddPage();
            }
        }
        private static void DrawGamesListSection(
        PdfExportOptionsWindow options)
        {
            if (currentCollection.Games.Count == 0)
                return;

            EnsureSpaceForSection(170);
            DrawSectionHeader(T("Gry", "Games"));

            double columnGap = 18;
            double rowGap = 16;
            double cardWidth = (contentWidth - columnGap) / 2;
            double leftX = margin;
            double rightX = margin + cardWidth + columnGap;

            List<Game> games = currentCollection.Games.ToList();

            for (int i = 0; i < games.Count; i += 2)
            {
                Game leftGame = games[i];
                Game? rightGame = i + 1 < games.Count ? games[i + 1] : null;

                double leftCardHeight = GetGamePdfCardHeight(leftGame, options);
                double rightCardHeight = rightGame != null
                    ? GetGamePdfCardHeight(rightGame, options)
                    : leftCardHeight;

                double cardHeight = Math.Max(leftCardHeight, rightCardHeight);

                double rowHeight = cardHeight + rowGap;

                CheckPageBreak(rowHeight);

                double rowY = currentY;

                DrawGamePdfCard(leftGame, leftX, rowY, cardWidth, options);

                if (rightGame != null)
                    DrawGamePdfCard(rightGame, rightX, rowY, cardWidth, options);

                currentY = rowY + cardHeight + rowGap;
            }
        }
        private static double GetGamePdfCardHeight(
    Game game,
    PdfExportOptionsWindow options)
        {
            int fieldCount = 6;

            if (game.PurchasePrice is > 0)
                fieldCount++;

            return GetCollectionCardHeight(fieldCount, options);
        }
        private static void DrawGamePdfCard(
            Game game,
            double x,
            double y,
            double width,
            PdfExportOptionsWindow options)
        {
            string subtitle = string.Join(" • ",
                new[]
                {
            game.Manufacturer,
            game.Platform
                }.Where(v => !string.IsNullOrWhiteSpace(v)));

            List<CollectionField> fields = new()
    {
        new(T("Platforma", "Platform"), game.Platform),
        new(T("Gatunek", "Genre"), GenreDatabase.TranslateGenre(game.Genre, currentLanguage)),
        new(T("Wydawca", "Publisher"), game.Publisher),
        new(T("Region", "Region"), game.Region),
        new(T("Stan", "Condition"),
                ConditionDatabase.Translate(game.Condition, currentLanguage)),
        new(T("Dodano", "Added"), game.DateAdded.ToString("dd.MM.yyyy"))
    };

            if (game.PurchasePrice is > 0)
            {
                fields.Add(new CollectionField(
                    T("Cena", "Price"),
                    $"{game.PurchasePrice.Value:N2} {game.PurchaseCurrency}"));
            }

            string? imagePath =
                game.FrontCoverPhoto?.OriginalPath
                ?? game.Photos.FirstOrDefault(p => p.IsMain)?.OriginalPath
                ?? game.Photos.FirstOrDefault()?.OriginalPath;

            DrawCollectionCard(
                @"Assets\Icons\PDF\gamepad.png",
                game.Title,
                subtitle,
                imagePath,
                fields,
                options,
                x,
                y,
                width,
                false);
        }

        private static void DrawAccessoriesListSection(
     PdfExportOptionsWindow options)
        {
            if (currentCollection.Accessories.Count == 0)
                return;

            EnsureSpaceForSection(170);
            DrawSectionHeader(T("Akcesoria", "Accessories"));

            double columnGap = 18;
            double rowGap = 16;

            double cardWidth = (contentWidth - columnGap) / 2;
            double leftX = margin;
            double rightX = margin + cardWidth + columnGap;

            List<Accessory> accessories = currentCollection.Accessories.ToList();

            for (int i = 0; i < accessories.Count; i += 2)
            {
                Accessory leftAccessory = accessories[i];
                Accessory? rightAccessory = i + 1 < accessories.Count ? accessories[i + 1] : null;

                double leftCardHeight = GetAccessoryPdfCardHeight(leftAccessory, options);
                double rightCardHeight = rightAccessory != null
                    ? GetAccessoryPdfCardHeight(rightAccessory, options)
                    : leftCardHeight;

                double cardHeight = Math.Max(leftCardHeight, rightCardHeight);
                double rowHeight = cardHeight + rowGap;

                CheckPageBreak(rowHeight);

                double rowY = currentY;

                DrawAccessoryPdfCard(leftAccessory, leftX, rowY, cardWidth, options);

                if (rightAccessory != null)
                    DrawAccessoryPdfCard(rightAccessory, rightX, rowY, cardWidth, options);

                currentY = rowY + cardHeight + rowGap;
            }
        }
        private static double GetAccessoryPdfCardHeight(
            Accessory accessory,
            PdfExportOptionsWindow options)
        {
            int fieldCount = 5;

            if (accessory.PurchasePrice is > 0)
                fieldCount++;

            return GetCollectionCardHeight(fieldCount, options);
        }
        private static void DrawAccessoryPdfCard(
    Accessory accessory,
    double x,
    double y,
    double width,
    PdfExportOptionsWindow options)
        {
            string title = !string.IsNullOrWhiteSpace(accessory.Model)
                ? accessory.Model
                : !string.IsNullOrWhiteSpace(accessory.AccessoryType)
                    ? accessory.AccessoryType
                    : T("Akcesorium", "Accessory");

            string subtitle = string.Join(" • ",
                new[]
                {
            accessory.Manufacturer,
            accessory.AccessoryType
                }.Where(v => !string.IsNullOrWhiteSpace(v)));

            List<CollectionField> fields = new()
    {
        new(T("Typ", "Type"), accessory.AccessoryType),
        new(T("Producent", "Manufacturer"), accessory.Manufacturer),
        new(T("Model", "Model"), accessory.Model),
        new(T("Stan", "Condition"),
               ConditionDatabase.Translate(accessory.Condition, currentLanguage)),
        new(T("Dodano", "Added"), accessory.DateAdded.ToString("dd.MM.yyyy"))
    };

            if (accessory.PurchasePrice is > 0)
            {
                fields.Add(new CollectionField(
                    T("Cena", "Price"),
                    $"{accessory.PurchasePrice.Value:N2} {accessory.PurchaseCurrency}"));
            }

            string? imagePath = accessory.Photos
                .FirstOrDefault(p => p.IsMain)?.OriginalPath
                ?? accessory.Photos.FirstOrDefault()?.OriginalPath;

            DrawCollectionCard(
                @"Assets\Icons\PDF\accessories.png",
                title,
                subtitle,
                imagePath,
                fields,
                options,
                x,
                y,
                width,
                false);
        }
        private static void DrawHardwareListSection(
        PdfExportOptionsWindow options)
        {
            if (currentCollection.HardwareItems.Count == 0)
                return;

            EnsureSpaceForSection(170);
            DrawSectionHeader(T("Sprzęt", "Hardware"));

            foreach (Hardware hardware in currentCollection.HardwareItems)
            {
                string title = !string.IsNullOrWhiteSpace(hardware.CustomName)
                    ? hardware.CustomName
                    : !string.IsNullOrWhiteSpace(hardware.Model)
                        ? hardware.Model
                        : !string.IsNullOrWhiteSpace(hardware.Platform)
                            ? hardware.Platform
                            : T("Sprzęt", "Hardware");

                string subtitle = string.Join(" • ",
                    new[]
                    {
                hardware.Manufacturer,
                hardware.Platform
                    }.Where(x => !string.IsNullOrWhiteSpace(x)));

                List<CollectionField> fields = new()
        {
            new(T("Typ", "Type"), hardware.HardwareType),
            new(T("Model", "Model"), hardware.Model),
            new(T("Wariant", "Variant"), hardware.Variant),
            new(T("Region", "Region"), hardware.Region),
            new(T("Stan", "Condition"),
                 ConditionDatabase.Translate(hardware.Condition, currentLanguage)),
            new(T("Dodano", "Added"), hardware.DateAdded.ToString("dd.MM.yyyy"))
        };

                if (hardware.PurchasePrice is > 0)
                {
                    fields.Add(new CollectionField(
                        T("Cena", "Price"),
                        $"{hardware.PurchasePrice.Value:N2} {hardware.PurchaseCurrency}"));
                }

                string? imagePath = hardware.Photos
                    .FirstOrDefault(p => p.IsMain)?.OriginalPath
                    ?? hardware.Photos.FirstOrDefault()?.OriginalPath;

                DrawCollectionCard(
                    @"Assets\Icons\PDF\pc.png",
                    title,
                    subtitle,
                    imagePath,
                    fields,
                    options,
                    margin,
                    currentY,
                    contentWidth);
            }
        }
        private sealed class CollectionField
        {
            public string Label { get; }
            public string Value { get; }

            public CollectionField(string label, string value)
            {
                Label = label;
                Value = value;
            }
        }
        private static double GetCollectionCardHeight(
        int fieldCount,
        PdfExportOptionsWindow options)
        {
            double headerHeight = 48;
            double lineHeight = 15;
            double bottomPadding = 8;

            double textHeight =
                headerHeight +
                fieldCount * lineHeight +
                bottomPadding;

            if (!options.IncludePhotos)
                return textHeight;

            double photoHeight = options.PhotoSize switch
            {
                PdfPhotoSize.Small => 70,
                PdfPhotoSize.Medium => 100,
                PdfPhotoSize.Large => 130,
                _ => 70
            };

            return Math.Max(
                textHeight,
                photoHeight + 24);
        }
        private static double GetServiceCardHeight(int serviceCount)
        {
            double headerHeight = 52;
            double entryHeight = 62;
            double bottomPadding = 10;

            return headerHeight +
                   serviceCount * entryHeight +
                   bottomPadding;
        }
        private static void DrawServiceDeviceCard(
        string iconPath,
        string title,
        string subtitle,
        List<ServiceEntry> entries)
        {
            double height = GetServiceCardHeight(entries.Count);

            CheckPageBreak(height + 12);

            double y = currentY;

            DrawCard(
                margin,
                y,
                contentWidth,
                height);

            DrawImageIfExists(
                iconPath,
                margin + 14,
                y + 12,
                26,
                26);

            XFont titleFont = new XFont("Segoe UI", 12, XFontStyleEx.Bold);
            XFont subtitleFont = new XFont("Segoe UI", 9.5, XFontStyleEx.Regular);

            gfx!.DrawString(
                title,
                titleFont,
                TextBrush,
                new XRect(margin + 50, y + 12, contentWidth - 70, 18),
                XStringFormats.TopLeft);

            gfx.DrawString(
                subtitle,
                subtitleFont,
                SecondaryBrush,
                new XRect(margin + 50, y + 30, contentWidth - 70, 16),
                XStringFormats.TopLeft);

            XFont dateFont = new XFont("Segoe UI", 9.5, XFontStyleEx.Bold);
            XFont typeFont = new XFont("Segoe UI", 10.2, XFontStyleEx.Bold);
            XFont normalFont = new XFont("Segoe UI", 9.3, XFontStyleEx.Regular);

            double entryY = y + 58;

            foreach (ServiceEntry entry in entries)
            {
                string dateText = entry.ServiceDate.HasValue
                    ? entry.ServiceDate.Value.ToString("dd.MM.yyyy")
                    : T("Brak daty", "No date");

                gfx.DrawString(
                    dateText,
                    dateFont,
                    SecondaryBrush,
                    new XRect(margin + 18, entryY, contentWidth - 36, 14),
                    XStringFormats.TopLeft);

                if (!string.IsNullOrWhiteSpace(entry.ServiceType))
                {
                    gfx.DrawString(
                        ServiceTypeDatabase.Translate(entry.ServiceType, currentLanguage), typeFont,
                        TextBrush,
                        new XRect(margin + 18, entryY + 16, contentWidth - 36, 15),
                        XStringFormats.TopLeft);
                }

                if (entry.Cost is > 0)
                {
                    gfx.DrawString(
                        $"{entry.Cost.Value:N2} {entry.CostCurrency}",
                        normalFont,
                        TextBrush,
                        new XRect(margin + 18, entryY + 34, contentWidth - 36, 14),
                        XStringFormats.TopLeft);
                }

                if (!string.IsNullOrWhiteSpace(entry.CustomNote))
                {
                    gfx.DrawString(
                        entry.CustomNote,
                        normalFont,
                        SecondaryBrush,
                        new XRect(margin + 150, entryY + 34, contentWidth - 168, 14),
                        XStringFormats.TopLeft);
                }

                entryY += 62;
            }

            currentY = y + height + 12;
        }
        private static void DrawImageUniformToFill(
            XGraphics gfx,
            string imagePath,
            double x,
            double y,
            double width,
            double height)
        {
            if (string.IsNullOrWhiteSpace(imagePath) ||
                !File.Exists(imagePath))
                return;

            try
            {
                using XImage image = XImage.FromFile(imagePath);

                double imageRatio = image.PixelWidth / (double)image.PixelHeight;
                double targetRatio = width / height;

                double drawWidth;
                double drawHeight;

                if (imageRatio > targetRatio)
                {
                    drawHeight = height;
                    drawWidth = height * imageRatio;
                }
                else
                {
                    drawWidth = width;
                    drawHeight = width / imageRatio;
                }

                double drawX = x + (width - drawWidth) / 2.0;
                double drawY = y + (height - drawHeight) / 2.0;

                gfx.DrawImage(image, drawX, drawY, drawWidth, drawHeight);
            }
            catch (Exception ex)
            {
                #if DEBUG
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine("HGC PDF - Image skipped");
                System.Diagnostics.Debug.WriteLine($"File: {imagePath}");
                System.Diagnostics.Debug.WriteLine(ex);
                System.Diagnostics.Debug.WriteLine("========================================");
                #endif
            }
        }
        private static void DrawCollectionCard(
     string iconPath,
     string title,
     string subtitle,
     string? imagePath,
     List<CollectionField> fields,
     PdfExportOptionsWindow options,
     double x,
     double y,
     double width,
     bool manageFlow = true)
        {
            double height = GetCollectionCardHeight(fields.Count, options);

            if (manageFlow)
            {
                CheckPageBreak(height + 12);
                y = currentY;
            }

            DrawCard(x, y, width, height);

            DrawImageIfExists(
                iconPath,
                x + 10,
                y + 10,
                26,
                26);

            XFont titleFont = new XFont("Segoe UI", 12, XFontStyleEx.Bold);
            XFont subtitleFont = new XFont("Segoe UI", 9.5, XFontStyleEx.Regular);
            XFont labelFont = new XFont("Segoe UI", 9.3, XFontStyleEx.Bold);
            XFont valueFont = new XFont("Segoe UI", 9.0, XFontStyleEx.Regular);

            double headerTextX = x + 46;
            double headerTextW = width - 58;

            gfx!.DrawString(
                TrimToWidth(title, titleFont, headerTextW),
                titleFont,
                TextBrush,
                new XRect(headerTextX, y + 12, headerTextW, 18),
                XStringFormats.TopLeft);

            if (!string.IsNullOrWhiteSpace(subtitle))
            {
                gfx.DrawString(
                    TrimToWidth(subtitle, subtitleFont, headerTextW),
                    subtitleFont,
                    SecondaryBrush,
                    new XRect(headerTextX, y + 30, headerTextW, 16),
                    XStringFormats.TopLeft);
            }

            double textX = x + 50;
            double textW = width - 64;

            double imageWidth = 0;
            double imageHeight = 0;

            bool isGameCard =
                iconPath.Contains("gamepad", StringComparison.OrdinalIgnoreCase);

            if (options.IncludePhotos && !string.IsNullOrWhiteSpace(imagePath))
            {
                if (isGameCard)
                {
                    imageWidth = 58;
                    imageHeight = 82;
                }
                else
                {
                    imageWidth = 70;
                    imageHeight = 70;
                }

                double imageY = y + 50;

                if (isGameCard)
                {
                    int visibleFieldCount = fields.Count(f => !string.IsNullOrWhiteSpace(f.Value));
                    double fieldsHeight = visibleFieldCount * 15;

                    imageY = y + 52 + (fieldsHeight - imageHeight) / 2.0;
                }

                DrawImageUniformToFill(
                    gfx,
                    imagePath,
                    x + 16,
                    imageY,
                    imageWidth,
                    imageHeight);

                const double imageLeftMargin = 16;
                const double imageTextSpacing = 24;

                textX = x + imageLeftMargin + imageWidth + imageTextSpacing;
                textW = width - imageWidth - imageLeftMargin - imageTextSpacing - 20;
            }

            double fieldY = y + 52;

            double labelW = 50;
            double valueX = textX + labelW + 12;

            foreach (CollectionField field in fields)
            {
                if (string.IsNullOrWhiteSpace(field.Value))
                    continue;

                gfx.DrawString(
                    field.Label,
                    labelFont,
                    SecondaryBrush,
                    new XRect(textX, fieldY, labelW, 14),
                    XStringFormats.TopLeft);

                gfx.DrawString(
                     TrimToWidth(field.Value, valueFont, textW - labelW - 8),
                     valueFont,
                     SecondaryBrush,
                     new XRect(valueX, fieldY, textW - labelW - 8, 14),
                     XStringFormats.TopLeft);

                fieldY += 15;
            }

            if (manageFlow)
                currentY = y + height + 12;
        }
        private static string TrimToWidth(
            string text,
            XFont font,
            double maxWidth)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            if (gfx!.MeasureString(text, font).Width <= maxWidth)
                return text;

            string result = text;

            while (result.Length > 3 &&
                   gfx.MeasureString(result + "...", font).Width > maxWidth)
            {
                result = result[..^1];
            }

            return result + "...";
        }
        private static void DrawPlatformManufacturerStatisticsPage()
        {
            AddPage();

            DrawSectionHeader(T(
                "Platformy i producenci",
                "Platforms and manufacturers"));

            XFont introFont = new XFont("Segoe UI", 10.5, XFontStyleEx.Regular);

            gfx!.DrawString(
                T(
                    "Zestawienie gier, platform, producentów oraz wydatków.",
                    "Summary of games, platforms, manufacturers and expenses."),
                introFont,
                SecondaryBrush,
                new XRect(margin, currentY - 8, contentWidth, 20),
                XStringFormats.TopLeft);

            currentY += 30;

            double columnGap = 18;
            double rowGap = 12;

            double columnWidth = (contentWidth - columnGap) / 2;

            double leftX = margin;
            double rightX = margin + columnWidth + columnGap;

            double minimumCardHeight = 140;

            void DrawRow(
                string leftIcon,
                string leftTitle,
                List<StatLine> leftLines,
                string rightIcon,
                string rightTitle,
                List<StatLine> rightLines)
            {
                double rowHeight = GetRowHeight(
                    leftLines,
                    rightLines,
                    minimumCardHeight);

                if (currentY + rowHeight > pageHeight - margin - FooterHeight - 20)
                {
                    AddPage();

                    DrawSectionHeader(T(
                        "Platformy i producenci",
                        "Platforms and manufacturers"));
                }

                DrawStatisticSectionCard(
                    leftIcon,
                    leftTitle,
                    leftLines,
                    leftX,
                    currentY,
                    columnWidth,
                    rowHeight);

                DrawStatisticSectionCard(
                    rightIcon,
                    rightTitle,
                    rightLines,
                    rightX,
                    currentY,
                    columnWidth,
                    rowHeight);

                currentY += rowHeight + rowGap;
            }

            DrawRow(
                @"Assets\Icons\PDF\gamepad.png",
                T("Gry według platform", "Games by platform"),
                BuildPlatformLines(),

                @"Assets\Icons\PDF\value.png",
                T("Wydatki według platform", "Platform expenses"),
                BuildPlatformValueLines());

            DrawRow(
                @"Assets\Icons\PDF\pc.png",
                T("Producenci według liczby", "Manufacturers by count"),
                BuildManufacturerLines(),

                @"Assets\Icons\PDF\value.png",
                T("Wydatki według producentów", "Manufacturer expenses"),
                BuildManufacturerValueLines());

            var serviceEntries = currentCollection.HardwareItems
    .SelectMany(h => h.ServiceHistory)
    .Concat(currentCollection.PcSystems.SelectMany(p => p.ServiceHistory))
    .ToList();

            int serviceCount = serviceEntries.Count;

            decimal serviceTotal = serviceEntries.Sum(s =>
                ConvertCurrency(
                    s.Cost ?? 0m,
                    s.CostCurrency,
                    currentDefaultCurrency));

            decimal averageServiceCost =
                serviceCount == 0
                    ? 0m
                    : serviceTotal / serviceCount;

            decimal maxServiceCost =
                serviceEntries.Count == 0
                    ? 0m
                    : serviceEntries.Max(s =>
                        ConvertCurrency(
                            s.Cost ?? 0m,
                            s.CostCurrency,
                            currentDefaultCurrency));

            List<StatLine> serviceLines = new()
{
    new(T($"Wpisy serwisowe: {serviceCount}",
          $"Service entries: {serviceCount}"), true),

    new(T($"Łączny koszt: {serviceTotal:N2} {currentDefaultCurrency}",
          $"Total cost: {serviceTotal:N2} {currentDefaultCurrency}")),

    new(T($"Średni koszt: {averageServiceCost:N2} {currentDefaultCurrency}",
          $"Average cost: {averageServiceCost:N2} {currentDefaultCurrency}")),

    new(T($"Najdroższy serwis: {maxServiceCost:N2} {currentDefaultCurrency}",
          $"Most expensive service: {maxServiceCost:N2} {currentDefaultCurrency}"))
};

            List<StatLine> highlightLines = GetHighlights()
                .Take(8)
                .Select(h => new StatLine(h.Text))
                .ToList();

            double lastRowHeight = GetRowHeight(
                serviceLines,
                highlightLines,
                minimumCardHeight);

            if (currentY + lastRowHeight > pageHeight - margin - FooterHeight - 20)
            {
                AddPage();

                DrawSectionHeader(T(
                    "Platformy i producenci",
                    "Platforms and manufacturers"));
            }

            DrawStatisticSectionCard(
                @"Assets\Icons\PDF\service.png",
                T("Serwis", "Service"),
                serviceLines,
                leftX,
                currentY,
                columnWidth,
                lastRowHeight);

            DrawHighlightsCard(
                rightX,
                currentY,
                columnWidth,
                lastRowHeight);

            currentY += lastRowHeight + rowGap;
        }
        private static void DrawDetailedStatisticsPage()
        {
            AddPage();

            DrawSectionHeader(T(
                "Szczegółowa struktura kolekcji",
                "Detailed collection structure"));

            XFont introFont = new XFont("Segoe UI", 10.5, XFontStyleEx.Regular);

            gfx!.DrawString(
                T(
                    "Rozbicie kolekcji według głównych kategorii, typów i gatunków.",
                    "Breakdown of the collection by main categories, types and genres."),
                introFont,
                SecondaryBrush,
                new XRect(margin, currentY - 8, contentWidth, 20),
                XStringFormats.TopLeft);

            currentY += 30;



            double columnGap = 18;
            double rowGap = 12;
            double columnWidth = (contentWidth - columnGap) / 2;

            double leftX = margin;
            double rightX = margin + columnWidth + columnGap;

            double minimumCardHeight = 140;

            void DrawRow(
                string leftIcon,
                string leftTitle,
                List<StatLine> leftLines,
                string rightIcon,
                string rightTitle,
                List<StatLine> rightLines)
            {
                double rowHeight = GetRowHeight(
                    leftLines,
                    rightLines,
                    minimumCardHeight);

                if (currentY + rowHeight > pageHeight - margin - FooterHeight - 20)
                {
                    AddPage();

                    DrawSectionHeader(T(
                        "Szczegółowa struktura kolekcji",
                        "Detailed collection structure"));
                }

                DrawStatisticSectionCard(
                    leftIcon,
                    leftTitle,
                    leftLines,
                    leftX,
                    currentY,
                    columnWidth,
                    rowHeight);

                DrawStatisticSectionCard(
                    rightIcon,
                    rightTitle,
                    rightLines,
                    rightX,
                    currentY,
                    columnWidth,
                    rowHeight);

                currentY += rowHeight + rowGap;
            }

            List<StatLine> gameLines = new()
                {
                    new(T($"Wszystkie gry: {currentCollection.Games.Count}",
                          $"All games: {currentCollection.Games.Count}"), true),

                    new(T($"Ukończone: {currentCollection.Games.Count(g => g.IsCompleted)}",
                          $"Completed: {currentCollection.Games.Count(g => g.IsCompleted)}")),

                    new(T($"Nieukończone: {currentCollection.Games.Count(g => !g.IsCompleted && !g.IsPlanned)}",
                          $"Not completed: {currentCollection.Games.Count(g => !g.IsCompleted && !g.IsPlanned)}")),

                    new(T($"Planowane: {currentCollection.Games.Count(g => g.IsPlanned)}",
                          $"Planned: {currentCollection.Games.Count(g => g.IsPlanned)}")),

                    new(T($"Ulubione: {currentCollection.Games.Count(g => g.IsFavorite)}",
                          $"Favorites: {currentCollection.Games.Count(g => g.IsFavorite)}"))
                };

            List<StatLine> genreLines = currentCollection.Games
                .GroupBy(g =>
                    string.IsNullOrWhiteSpace(g.Genre)
                        ? T("Niesklasyfikowane", "Unclassified")
                        : g.Genre.Trim())
                .OrderByDescending(g => g.Count())
                .Select(g => new StatLine(
                    $"{GenreDatabase.TranslateGenre(g.Key, currentLanguage)}: {g.Count()}"))
                .ToList();

            DrawRow(
                @"Assets\Icons\PDF\gamepad.png",
                T("Gry", "Games"),
                gameLines,
                @"Assets\Icons\PDF\gamepad.png",
                T("Gry według gatunku", "Games by genre"),
                genreLines);

            int consolesCount = currentCollection.HardwareItems.Count;
            int computersCount = currentCollection.PcSystems.Count;

            List<StatLine> hardwareLines = new()
                {
                    new(T($"Łącznie: {consolesCount + computersCount}",
                          $"Total: {consolesCount + computersCount}"), true),

                    new(T($"Konsole / sprzęt: {consolesCount}",
                          $"Consoles / hardware: {consolesCount}")),

                    new(T($"Komputery: {computersCount}",
                          $"Computers: {computersCount}")),

                    new(T($"Zarchiwizowane: {currentCollection.ArchivedHardwareItems.Count + currentCollection.ArchivedPcSystems.Count}",
                          $"Archived: {currentCollection.ArchivedHardwareItems.Count + currentCollection.ArchivedPcSystems.Count}"))
                };

                        List<StatLine> hardwareTypeLines = new()
                {
                    new(T($"Konsole / sprzęt: {consolesCount}",
                          $"Consoles / hardware: {consolesCount}")),

                    new(T($"Komputery: {computersCount}",
                          $"Computers: {computersCount}"))
                };

                        DrawRow(
                            @"Assets\Icons\PDF\pc.png",
                            T("Sprzęt", "Hardware"),
                            hardwareLines,
                            @"Assets\Icons\PDF\pc.png",
                            T("Sprzęt według typu", "Hardware by type"),
                            hardwareTypeLines);

                        int accessoriesCount = currentCollection.Accessories.Count;

                        List<StatLine> accessoryLines = new()
                {
                    new(T($"Łącznie: {accessoriesCount}",
                          $"Total: {accessoriesCount}"), true),

                    new(T($"Aktywne: {accessoriesCount}",
                          $"Active: {accessoriesCount}")),

                    new(T($"Zarchiwizowane: {currentCollection.ArchivedAccessories.Count}",
                          $"Archived: {currentCollection.ArchivedAccessories.Count}"))
                };

            List<StatLine> accessoryTypeLines = currentCollection.Accessories
                .GroupBy(a =>
                    string.IsNullOrWhiteSpace(a.AccessoryType)
                        ? T("Niesklasyfikowane", "Unclassified")
                        : a.AccessoryType.Trim())
                .OrderByDescending(a => a.Count())
                .Select(a => new StatLine($"{a.Key}: {a.Count()}"))
                .ToList();

            DrawRow(
                @"Assets\Icons\PDF\accessories.png",
                T("Akcesoria", "Accessories"),
                accessoryLines,
                @"Assets\Icons\PDF\accessories.png",
                T("Akcesoria według typu", "Accessories by type"),
                accessoryTypeLines);                    
        }
        private static double GetStatisticSectionCardHeight(
        IEnumerable<StatLine> lines)
        {
            int lineCount = lines.Count();

            double headerHeight = 44;
            double lineHeight = 13;
            double bottomPadding = 16;

            return headerHeight + lineCount * lineHeight + bottomPadding;
        }

        private static double GetRowHeight(
            IEnumerable<StatLine> leftLines,
            IEnumerable<StatLine> rightLines,
            double minimumHeight)
        {
            double leftHeight = GetStatisticSectionCardHeight(leftLines);
            double rightHeight = GetStatisticSectionCardHeight(rightLines);

            return Math.Max(
                minimumHeight,
                Math.Max(leftHeight, rightHeight));
        }
        private static string T(string pl, string en)
        {
            return currentLanguage == "pl" ? pl : en;
        }
        private sealed class ChartSlice
        {
            public string Name { get; set; } = "";
            public decimal Value { get; set; }
            public double Percentage { get; set; }
        }
        private static string GetAccessoryReportManufacturer(Accessory accessory)
        {
            if (!string.IsNullOrWhiteSpace(accessory.AssignedManufacturer))
                return accessory.AssignedManufacturer.Trim();

            if (!string.IsNullOrWhiteSpace(accessory.Manufacturer))
                return accessory.Manufacturer.Trim();

            return T("Niesklasyfikowane", "Unclassified");
        }
        private sealed class HighlightItem
        {
            public string Text { get; }

            public HighlightItem(string text)
            {
                Text = text;
            }
        }
        private static List<HighlightItem> GetHighlights()
        {
            List<HighlightItem> items = new();

            // Najczęstszy gatunek
            var topGenre = currentCollection.Games
                .Where(g => !string.IsNullOrWhiteSpace(g.Genre))
                .GroupBy(g => g.Genre.Trim())
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            if (topGenre != null)
            {
                items.Add(new HighlightItem(
                    T($"Najwięcej gier: {topGenre.Key} ({topGenre.Count()})",
                      $"Most games: {topGenre.Key} ({topGenre.Count()})")));
            }

            // Najczęstsza platforma
            var topPlatform = currentCollection.Games
                .Where(g => !string.IsNullOrWhiteSpace(g.Platform))
                .GroupBy(g => g.Platform.Trim())
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            if (topPlatform != null)
            {
                items.Add(new HighlightItem(
                    T($"Najwięcej gier na: {topPlatform.Key} ({topPlatform.Count()})",
                      $"Most games for: {topPlatform.Key} ({topPlatform.Count()})")));
            }

            // Najczęstszy producent
            var topManufacturer = currentCollection.Games
                .Where(g => !string.IsNullOrWhiteSpace(g.Manufacturer))
                .GroupBy(g => g.Manufacturer.Trim())
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            if (topManufacturer != null)
            {
                items.Add(new HighlightItem(
                    T($"Najczęstszy producent: {topManufacturer.Key} ({topManufacturer.Count()})",
                      $"Top manufacturer: {topManufacturer.Key} ({topManufacturer.Count()})")));
            }

            // Najdroższy zakup
            var mostExpensivePurchase =
                 currentCollection.Games
                     .Select(g => new
                     {
                         Name = g.Title,
                         Price = g.PurchasePrice,
                         Currency = g.PurchaseCurrency
                     })
                 .Concat(currentCollection.HardwareItems.Select(h => new
                 {
                     Name = !string.IsNullOrWhiteSpace(h.CustomName)
                         ? h.CustomName
                         : !string.IsNullOrWhiteSpace(h.Model)
                             ? h.Model
                             : !string.IsNullOrWhiteSpace(h.Platform)
                                 ? h.Platform
                                 : h.Manufacturer,
                     Price = h.PurchasePrice,
                     Currency = h.PurchaseCurrency
                 }))
                 .Concat(currentCollection.PcSystems.Select(p => new
                 {
                     Name = !string.IsNullOrWhiteSpace(p.Name)
                         ? p.Name
                         : !string.IsNullOrWhiteSpace(p.Model)
                             ? p.Model
                             : p.Manufacturer,
                     Price = p.PurchasePrice,
                     Currency = p.PurchaseCurrency
                 }))
                 .Concat(currentCollection.Accessories.Select(a => new
                 {
                     Name = !string.IsNullOrWhiteSpace(a.Model)
                         ? a.Model
                         : !string.IsNullOrWhiteSpace(a.AccessoryType)
                             ? a.AccessoryType
                             : a.Manufacturer,
                     Price = a.PurchasePrice,
                     Currency = a.PurchaseCurrency
                 }))
                 .Where(x => x.Price.HasValue && x.Price.Value > 0)
                 .OrderByDescending(x => ConvertCurrency(
                     x.Price!.Value,
                     x.Currency,
                     currentDefaultCurrency))
                 .FirstOrDefault();

            // Najwięcej akcesoriów
            var topAccessoryType = currentCollection.Accessories
            .Where(a => !string.IsNullOrWhiteSpace(a.AccessoryType))
            .GroupBy(a => a.AccessoryType.Trim())
            .OrderByDescending(a => a.Count())
            .FirstOrDefault();

            if (topAccessoryType != null)
            {
                items.Add(new HighlightItem(
                    T($"Najwięcej akcesoriów: {topAccessoryType.Key} ({topAccessoryType.Count()})",
                      $"Most accessories: {topAccessoryType.Key} ({topAccessoryType.Count()})")));
            }
            // Najdroższy serwis
            var mostExpensiveService =
                currentCollection.HardwareItems
                    .SelectMany(h => h.ServiceHistory.Select(s => new
                    {
                        Cost = s.Cost,
                        Currency = s.CostCurrency
                    }))
                .Concat(currentCollection.PcSystems.SelectMany(p => p.ServiceHistory.Select(s => new
                {
                    Cost = s.Cost,
                    Currency = s.CostCurrency
                })))
                .Where(x => x.Cost.HasValue && x.Cost.Value > 0)
                .OrderByDescending(x => ConvertCurrency(
                    x.Cost!.Value,
                    x.Currency,
                    currentDefaultCurrency))
                .FirstOrDefault();

            if (mostExpensiveService != null)
            {
                decimal convertedCost = ConvertCurrency(
                    mostExpensiveService.Cost!.Value,
                    mostExpensiveService.Currency,
                    currentDefaultCurrency);

                items.Add(new HighlightItem(
                    T($"Najdroższy serwis: {convertedCost:N2} {currentDefaultCurrency}",
                      $"Most expensive service: {convertedCost:N2} {currentDefaultCurrency}")));
            }

            if (mostExpensivePurchase != null)
            {
                decimal convertedPrice = ConvertCurrency(
                    mostExpensivePurchase.Price!.Value,
                    mostExpensivePurchase.Currency,
                    currentDefaultCurrency);

                items.Add(new HighlightItem(
                    T($"Najdroższy zakup:\n{mostExpensivePurchase.Name} ({convertedPrice:N2} {currentDefaultCurrency})",
                    $"Most expensive purchase:\n{mostExpensivePurchase.Name} ({convertedPrice:N2} {currentDefaultCurrency})")));
            }

            return items;
        }
        private static List<StatLine> BuildPlatformLines()
        {
            Dictionary<string, int> platformCounts = currentCollection.Games
                .GroupBy(g =>
                    string.IsNullOrWhiteSpace(g.Platform)
                        ? T("Niesklasyfikowane", "Unclassified")
                        : g.Platform.Trim())
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count());

            return LimitLinesWithOther(
            platformCounts
                .Select(p => new StatLine($"{p.Key}: {p.Value}")),
            15,
            T("Pozostałe", "Other"));
        }
        private static List<StatLine> BuildPlatformValueLines()
        {
            Dictionary<string, decimal> platformTotals = new();

            foreach (Game game in currentCollection.Games)
            {
                if (!game.PurchasePrice.HasValue || game.PurchasePrice.Value <= 0)
                    continue;

                string platform = string.IsNullOrWhiteSpace(game.Platform)
                    ? T("Niesklasyfikowane", "Unclassified")
                    : game.Platform.Trim();

                decimal value = ConvertCurrency(
                    game.PurchasePrice.Value,
                    game.PurchaseCurrency,
                    currentDefaultCurrency);

                if (!platformTotals.ContainsKey(platform))
                    platformTotals[platform] = 0m;

                platformTotals[platform] += value;
            }

            return LimitValueLinesWithOther(
                platformTotals.OrderByDescending(p => p.Value),
                15,
                T("Pozostałe", "Other"));
        }
        private static List<StatLine> BuildManufacturerLines()
        {
            Dictionary<string, int> manufacturerCounts = new();

            void AddManufacturer(string? manufacturer)
            {
                string name = string.IsNullOrWhiteSpace(manufacturer)
                    ? T("Niesklasyfikowane", "Unclassified")
                    : manufacturer.Trim();

                if (!manufacturerCounts.ContainsKey(name))
                    manufacturerCounts[name] = 0;

                manufacturerCounts[name]++;
            }

            foreach (Game game in currentCollection.Games)
                AddManufacturer(game.Manufacturer);

            foreach (Hardware hardware in currentCollection.HardwareItems)
                AddManufacturer(hardware.Manufacturer);

            foreach (PcSystem pc in currentCollection.PcSystems)
                AddManufacturer(string.IsNullOrWhiteSpace(pc.Manufacturer) ? "PC" : pc.Manufacturer);

            foreach (Accessory accessory in currentCollection.Accessories)
                AddManufacturer(GetAccessoryReportManufacturer(accessory));

            return LimitLinesWithOther(
                manufacturerCounts
                    .OrderByDescending(m => m.Value)
                    .Select(m => new StatLine($"{m.Key}: {m.Value}")),
                15,
                T("Pozostałe", "Other"));
        }
        private static List<StatLine> BuildManufacturerValueLines()
        {
            Dictionary<string, decimal> manufacturerTotals = new();

            void AddValue(string? manufacturer, decimal? price, string currency)
            {
                if (!price.HasValue || price.Value <= 0)
                    return;

                string name = string.IsNullOrWhiteSpace(manufacturer)
                    ? T("Niesklasyfikowane", "Unclassified")
                    : manufacturer.Trim();

                decimal value = ConvertCurrency(
                    price.Value,
                    currency,
                    currentDefaultCurrency);

                if (!manufacturerTotals.ContainsKey(name))
                    manufacturerTotals[name] = 0m;

                manufacturerTotals[name] += value;
            }

            foreach (Game game in currentCollection.Games)
                AddValue(game.Manufacturer, game.PurchasePrice, game.PurchaseCurrency);

            foreach (Hardware hardware in currentCollection.HardwareItems)
                AddValue(hardware.Manufacturer, hardware.PurchasePrice, hardware.PurchaseCurrency);

            foreach (PcSystem pc in currentCollection.PcSystems)
                AddValue(
                    string.IsNullOrWhiteSpace(pc.Manufacturer) ? "PC" : pc.Manufacturer,
                    pc.PurchasePrice,
                    pc.PurchaseCurrency);

            foreach (Accessory accessory in currentCollection.Accessories)
                AddValue(
                    GetAccessoryReportManufacturer(accessory),
                    accessory.PurchasePrice,
                    accessory.PurchaseCurrency);

            return LimitValueLinesWithOther(
                    manufacturerTotals.OrderByDescending(m => m.Value),
                    15,
                    T("Pozostałe", "Other"));
        }
        private static List<StatLine> LimitValueLinesWithOther(
        IEnumerable<KeyValuePair<string, decimal>> values,
        int maxVisible,
        string otherLabel)
        {
            List<KeyValuePair<string, decimal>> list = values.ToList();

            if (list.Count <= maxVisible)
            {
                return list
                    .Select(v => new StatLine($"{v.Key}: {v.Value:N2} {currentDefaultCurrency}"))
                    .ToList();
            }

            List<StatLine> result = list
                .Take(maxVisible)
                .Select(v => new StatLine($"{v.Key}: {v.Value:N2} {currentDefaultCurrency}"))
                .ToList();

            decimal otherTotal = list
                .Skip(maxVisible)
                .Sum(v => v.Value);

            result.Add(new StatLine(
                $"{otherLabel}: {otherTotal:N2} {currentDefaultCurrency}",
                true));

            return result;
        }
        private static List<StatLine> LimitLinesWithOther(
        IEnumerable<StatLine> lines,
        int maxVisible,
        string otherLabel)
        {
            List<StatLine> list = lines.ToList();

            if (list.Count <= maxVisible)
                return list;

            List<StatLine> visible = list
                .Take(maxVisible)
                .ToList();

            int remainingCount = list.Count - maxVisible;

            visible.Add(new StatLine(
                T($"{otherLabel}: {remainingCount}",
                  $"Other: {remainingCount}"),
                true));

            return visible;
        }
        private static void DrawHighlightsCard(
             double x,
             double y,
             double width,
             double height)
        {
            XFont titleFont = new XFont("Segoe UI", 12, XFontStyleEx.Bold);
            XFont lineFont = new XFont("Segoe UI", 10.2, XFontStyleEx.Regular);

            List<HighlightItem> highlights = GetHighlights();

            double lineHeight = 13;
            double paddingBottom = 20;
            double lineY = y + 44;

            int visibleLines = highlights
                .Take(8)
                .SelectMany(item => item.Text.Replace("\r", "").Split('\n'))
                .Count(line => !string.IsNullOrWhiteSpace(line));

            double neededHeight = 44 + (visibleLines * lineHeight) + paddingBottom;

            height = Math.Max(height, neededHeight);

            DrawCard(x, y, width, height);

            DrawImageIfExists(
                @"Assets\Icons\PDF\star.png",
                x + 12,
                y + 10,
                30,
                30);

            gfx!.DrawString(
                T("Najciekawsze informacje", "Highlights"),
                titleFont,
                TextBrush,
                new XRect(x + 50, y + 15, width - 62, 18),
                XStringFormats.TopLeft);

            foreach (HighlightItem item in highlights.Take(8))
            {
                foreach (string line in item.Text.Replace("\r", "").Split('\n'))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    gfx.DrawString(
                        line,
                        lineFont,
                        SecondaryBrush,
                        new XRect(x + 16, lineY, width - 32, lineHeight),
                        XStringFormats.TopLeft);

                    lineY += lineHeight;
                }

                lineY += 2;
            }
        }    
        private static double DrawWrappedText(
            string text,
            XFont font,
            XBrush brush,
            double x,
            double y,
            double maxWidth,
            double lineHeight)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            string[] words = text.Split(' ');
            string currentLine = "";
            double currentY = y;

            foreach (string word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine)
                    ? word
                    : currentLine + " " + word;

                if (gfx!.MeasureString(testLine, font).Width <= maxWidth)
                {
                    currentLine = testLine;
                }
                else
                {
                    gfx.DrawString(
                        currentLine,
                        font,
                        brush,
                        new XPoint(x, currentY));

                    currentY += lineHeight;
                    currentLine = word;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                gfx!.DrawString(
                    currentLine,
                    font,
                    brush,
                    new XPoint(x, currentY));

                currentY += lineHeight;
            }

            return currentY - y;
        }
        private static List<ChartSlice> GetManufacturerExpenseChartData()
        {
            Dictionary<string, decimal> totals = new();

            void AddValue(string? manufacturer, decimal? price, string currency)
            {
                if (!price.HasValue || price.Value <= 0)
                    return;

                string name;

                if (string.IsNullOrWhiteSpace(manufacturer))
                {
                    name = T("Niesklasyfikowane", "Unclassified");
                }
                else
                {
                    name = manufacturer.Trim();
                }

                decimal value = ConvertCurrency(
                    price.Value,
                    currency,
                    currentDefaultCurrency);

                if (!totals.ContainsKey(name))
                    totals[name] = 0m;

                totals[name] += value;
            }

            foreach (Game game in currentCollection.Games)
                AddValue(game.Manufacturer, game.PurchasePrice, game.PurchaseCurrency);

            foreach (Hardware hardware in currentCollection.HardwareItems)
                AddValue(hardware.Manufacturer, hardware.PurchasePrice, hardware.PurchaseCurrency);

            foreach (PcSystem pc in currentCollection.PcSystems)
                AddValue("PC", pc.PurchasePrice, pc.PurchaseCurrency);

            foreach (Accessory accessory in currentCollection.Accessories)
                AddValue(accessory.Manufacturer, accessory.PurchasePrice, accessory.PurchaseCurrency);

            decimal grandTotal = totals.Values.Sum();

            return totals
                .OrderByDescending(t => t.Value)
                .Select(t => new ChartSlice
                {
                    Name = t.Key,
                    Value = t.Value,
                    Percentage = grandTotal == 0
                        ? 0
                        : (double)(t.Value / grandTotal * 100m)
                })
                .ToList();
        }
        private static void DrawDonutChart(
            List<ChartSlice> slices,
            double centerX,
            double centerY,
            double outerRadius,
            double innerRadius)
                {
                    if (slices.Count == 0 || slices.Sum(s => s.Value) <= 0)
                        return;

                    XColor[] colors =
                    {
                XColor.FromArgb(32, 126, 230),
                XColor.FromArgb(64, 170, 245),
                XColor.FromArgb(91, 192, 190),
                XColor.FromArgb(110, 145, 235),
                XColor.FromArgb(150, 130, 220),
                XColor.FromArgb(90, 180, 120),
                XColor.FromArgb(230, 170, 80),
                XColor.FromArgb(210, 110, 110),
                XColor.FromArgb(140, 160, 180),
                XColor.FromArgb(120, 120, 150)
            };

            double startAngle = -90;
            double total = (double)slices.Sum(s => s.Value);

            for (int i = 0; i < slices.Count; i++)
            {
                ChartSlice slice = slices[i];

                double sweepAngle = total == 0
                    ? 0
                    : (double)slice.Value / total * 360.0;

                XBrush brush = new XSolidBrush(colors[i % colors.Length]);

                gfx!.DrawPie(
                    brush,
                    centerX - outerRadius,
                    centerY - outerRadius,
                    outerRadius * 2,
                    outerRadius * 2,
                    startAngle,
                    sweepAngle);

                startAngle += sweepAngle;
            }

            gfx!.DrawEllipse(
                CardBrush,
                centerX - innerRadius,
                centerY - innerRadius,
                innerRadius * 2,
                innerRadius * 2);
        }
        private static void DrawManufacturerExpenseChartCard(
            double x,
            double y,
            double width,
            double height)
        {
            List<ChartSlice> slices = GetManufacturerExpenseChartData();

            DrawCard(x, y, width, height);

            XFont titleFont = new XFont("Segoe UI", 13, XFontStyleEx.Bold);
            XFont centerValueFont = new XFont("Segoe UI", 17, XFontStyleEx.Bold);
            XFont centerLabelFont = new XFont("Segoe UI", 9.5, XFontStyleEx.Regular);
            XFont legendFont = new XFont("Segoe UI", 9.5, XFontStyleEx.Regular);
            XFont legendBoldFont = new XFont("Segoe UI", 9.5, XFontStyleEx.Bold);

            gfx!.DrawString(
                T("Wydatki według producenta", "Expenses by manufacturer"),
                titleFont,
                TextBrush,
                new XRect(x + 22, y + 16, width - 44, 20),
                XStringFormats.TopLeft);

            if (slices.Count == 0 || slices.Sum(s => s.Value) <= 0)
            {
                gfx.DrawString(
                    T("Brak danych o wydatkach.", "No expense data available."),
                    legendFont,
                    SecondaryBrush,
                    new XRect(x + 22, y + 52, width - 44, 20),
                    XStringFormats.TopLeft);

                return;
            }

            XColor[] colors =
            {
        XColor.FromArgb(32, 126, 230),
        XColor.FromArgb(64, 170, 245),
        XColor.FromArgb(91, 192, 190),
        XColor.FromArgb(110, 145, 235),
        XColor.FromArgb(150, 130, 220),
        XColor.FromArgb(90, 180, 120),
        XColor.FromArgb(230, 170, 80),
        XColor.FromArgb(210, 110, 110),
        XColor.FromArgb(140, 160, 180),
        XColor.FromArgb(120, 120, 150)
        };

            double chartCenterX = x + 125;
            double chartCenterY = y + 125;

            DrawDonutChart(
                slices,
                chartCenterX,
                chartCenterY,
                72,
                42);

            decimal totalValue = slices.Sum(s => s.Value);

            gfx.DrawString(
                $"{totalValue:N0}",
                centerValueFont,
                TextBrush,
                new XRect(chartCenterX - 45, chartCenterY - 15, 90, 22),
                XStringFormats.TopCenter);

            gfx.DrawString(
                T("Łącznie", "Total"),
                centerLabelFont,
                SecondaryBrush,
                new XRect(chartCenterX - 45, chartCenterY + 6, 90, 16),
                XStringFormats.TopCenter);

            double legendX = x + 240;
            double legendY = y + 52;
            double lineHeight = 18;

            for (int i = 0; i < slices.Count; i++)
            {
                ChartSlice slice = slices[i];
                XBrush colorBrush = new XSolidBrush(colors[i % colors.Length]);

                gfx.DrawEllipse(
                    colorBrush,
                    legendX,
                    legendY + 4,
                    8,
                    8);

                gfx.DrawString(
                    slice.Name,
                    legendFont,
                    TextBrush,
                    new XRect(legendX + 16, legendY, 120, 14),
                    XStringFormats.TopLeft);

                gfx.DrawString(
                    $"{slice.Value:N0} {currentDefaultCurrency}",
                    legendBoldFont,
                    TextBrush,
                    new XRect(legendX + 105, legendY, 85, 14),
                    XStringFormats.TopRight);

                gfx.DrawString(
                    $"{slice.Percentage:0.0}%",
                    legendFont,
                    SecondaryBrush,
                    new XRect(legendX + 195, legendY, 45, 14),
                    XStringFormats.TopRight);

                legendY += lineHeight;

                if (legendY > y + height - 24)
                    break;
            }
        }
        private static bool DrawImageIfExists(
             string relativePath,
             double x,
             double y,
             double width,
             double height)
        {
            string fullPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                relativePath);

            if (!File.Exists(fullPath))
                return false;

            using XImage image = XImage.FromFile(fullPath);

            gfx!.DrawImage(
                image,
                x,
                y,
                width,
                height);

            return true;
        }

        private static void DrawFramedImageIfExists(
        string relativePath,
        double x,
        double y,
        double width,
        double height)
        {
            string fullPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                relativePath);

            if (!File.Exists(fullPath))
                return;

            double padding = 7;
            double shadowOffset = 3;
            double radius = 9;

            double frameX = x;
            double frameY = y;
            double frameW = width + padding * 2;
            double frameH = height + padding * 2;

            gfx!.DrawRoundedRectangle(
                new XSolidBrush(XColor.FromArgb(222, 226, 234)),
                new XRect(
                    frameX + shadowOffset,
                    frameY + shadowOffset,
                    frameW,
                    frameH),
                new XSize(radius, radius));

            gfx.DrawRoundedRectangle(
                new XPen(XColor.FromArgb(215, 222, 232), 0.8),
                XBrushes.White,
                new XRect(
                    frameX,
                    frameY,
                    frameW,
                    frameH),
                new XSize(radius, radius));

            using XImage image = XImage.FromFile(fullPath);

            gfx.DrawImage(
                image,
                frameX + padding,
                frameY + padding,
                width,
                height);
        }

        private static void AddPage()
        {
            gfx?.Dispose();
            gfx = null;

            page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;

            pageWidth = page.Width;
            pageHeight = page.Height;
            contentWidth = pageWidth - margin * 2;
            currentY = margin;

            gfx = XGraphics.FromPdfPage(page);

            DrawPageChrome();
        }

        private static void CheckPageBreak(double requiredHeight)
        {
            if (currentY + requiredHeight <= pageHeight - margin - 30)
                return;

            AddPage();
        }
        private static void DrawPageChrome()
        {
            gfx!.DrawRectangle(
                XBrushes.White,
                new XRect(0, 0, pageWidth, pageHeight));

            DrawSideGradient();

            gfx.DrawRectangle(
                AccentBrush,
                new XRect(0, 0, pageWidth, 18));
        }
        private static void DrawSideGradient()
        {
            int steps = 36;
            double gradientWidth = pageWidth * 0.33;
            double stepWidth = gradientWidth / steps;

            for (int i = 0; i < steps; i++)
            {
                double t = (double)i / (steps - 1);

                int r = (int)(210 + (255 - 210) * t);
                int g = (int)(220 + (255 - 220) * t);
                int b = 255;

                XBrush brush = new XSolidBrush(XColor.FromArgb(r, g, b));

                gfx!.DrawRectangle(
                    brush,
                    new XRect(i * stepWidth, 0, stepWidth + 1, pageHeight));

                gfx.DrawRectangle(
                    brush,
                    new XRect(pageWidth - ((i + 1) * stepWidth), 0, stepWidth + 1, pageHeight));
            }
        }
        private static void DrawTitlePage()
        {
            XFont titleFont = new XFont("Segoe UI", 42, XFontStyleEx.Bold);
            XFont subtitleFont = new XFont("Segoe UI", 17, XFontStyleEx.Bold);
            XFont boldFont = new XFont("Segoe UI", 12.5, XFontStyleEx.Bold);
            XFont normalFont = new XFont("Segoe UI", 11.5, XFontStyleEx.Regular);

            double left = 54;

            DrawImageIfExists(
                PdfLogoFile,
                left,
                40,
                80,
                80);

            double titleY = 140;

            gfx.DrawRectangle(
             AccentBrush,
             new XRect(
                 left,
                 titleY + 13,
                 14,
                 34));

            gfx.DrawString(
                "Hajper Games Collection",
                titleFont,
                TextBrush,
                new XRect(left + 15, titleY, pageWidth - left - 112, 42),
                XStringFormats.TopLeft);

            gfx.DrawString(
                T("Raport kolekcji", "Collection report"),
                subtitleFont,
                SecondaryBrush,
                new XRect(left + 16, titleY + 54, pageWidth - left - 112, 28),
                XStringFormats.TopLeft);

            double cardX = left + 32;
            double cardY = 280;
            double cardW = 405;
            double cardH = 86;

            DrawCard(cardX, cardY, cardW, cardH);

            DrawImageIfExists(
                @"Assets\icons\PDF\calendar.png",
                cardX + 22,
                cardY + 22,
                42,
                42);

            gfx.DrawString(
                T(
                    $"Wygenerowano: {DateTime.Now:dd.MM.yyyy HH:mm:ss}",
                    $"Generated: {DateTime.Now:dd.MM.yyyy HH:mm:ss}"),
                boldFont,
                TextBrush,
               new XRect(cardX + 16, cardY + 22, cardW - 16, 22),
                XStringFormats.TopCenter);

            gfx.DrawString(
                T(
                    "Raport wygenerowany przez Hajper Games Collection.",
                    "Report generated by Hajper Games Collection."),
                normalFont,
                TextBrush,
               new XRect(cardX + 16, cardY + 48, cardW - 16, 20),
                XStringFormats.TopCenter);

            DrawImageIfExists(
                PdfBackgroundFile,
                pageWidth - 440,
                pageHeight - 370,
                420,
                350);

            currentY = pageHeight - margin - FooterHeight;
        }


        private static void DrawSectionHeader(string title)
        {
            CheckPageBreak(50);

            XFont font = new XFont("Segoe UI", 18, XFontStyleEx.Bold);

            gfx!.DrawRectangle(
                AccentBrush,
                new XRect(margin, currentY, 6, 26));

            gfx!.DrawString(
                title.ToUpper(),
                font,
                TextBrush,
                new XRect(margin + 14, currentY - 1, contentWidth - 14, 30),
                XStringFormats.TopLeft);

            currentY += 40;
        }

        private static void DrawText(string text)
        {
            CheckPageBreak(22);

            XFont font = new XFont("Segoe UI", 10.5, XFontStyleEx.Regular);

            gfx!.DrawString(
                text,
                font,
                TextBrush,
                new XRect(margin, currentY, contentWidth, 18),
                XStringFormats.TopLeft);

            currentY += 18;
        }
        private static void DrawCard(
        double x,
        double y,
        double width,
        double height)
        {
            // Cień
            gfx!.DrawRoundedRectangle(
                new XSolidBrush(XColor.FromArgb(45, 0, 0, 0)),
                new XRect(
                    x + ShadowOffset,
                    y + ShadowOffset,
                    width,
                    height),
                new XSize(CardRadius, CardRadius));

            // Karta
            gfx.DrawRoundedRectangle(
                BorderPen,
                CardBrush,
                new XRect(
                    x,
                    y,
                    width,
                    height),
                new XSize(CardRadius, CardRadius));

        }

        private static void DrawAllFooters()
        {
            if (!numberPages)
                return;

            int totalPages = document.PageCount;

            for (int i = 0; i < totalPages; i++)
            {
                PdfPage footerPage = document.Pages[i];

                using XGraphics footerGfx = XGraphics.FromPdfPage(
                    footerPage,
                    XGraphicsPdfPageOptions.Append);

                XFont footerFont = new XFont("Segoe UI", 8.5, XFontStyleEx.Regular);

                string text = T(
                    $"Strona {i + 1} / {totalPages}",
                    $"Page {i + 1} / {totalPages}");

                footerGfx.DrawRectangle(
                    AccentBrush,
                    new XRect(margin, footerPage.Height - 44, footerPage.Width - margin * 2, 1.2));

                footerGfx.DrawString(
                    text,
                    footerFont,
                    SecondaryBrush,
                    new XRect(margin, footerPage.Height - 34, footerPage.Width - margin * 2, 14),
                    XStringFormats.TopCenter);
            }
        }
        private static void DrawStatisticsSummaryPage()
        {
            AddPage();

            DrawSectionHeader(T("Statystyki", "Statistics"));

            XFont introFont = new XFont("Segoe UI", 10.5, XFontStyleEx.Regular);

            gfx!.DrawString(
                T(
                    "Najważniejsze informacje o kolekcji zapisanej w Hajper Games Collection.",
                    "Key information about the collection stored in Hajper Games Collection."),
                introFont,
                SecondaryBrush,
                new XRect(margin, currentY - 8, contentWidth, 20),
                XStringFormats.TopLeft);

            currentY += 24;

            XFont summaryTitleFont = new XFont("Segoe UI", 15, XFontStyleEx.Bold);

            gfx!.DrawString(
                T("Podsumowanie kolekcji", "Collection summary"),
                summaryTitleFont,
                TextBrush,
                new XRect(margin, currentY + 4, contentWidth, 22),
                XStringFormats.TopLeft);

            currentY += 36;

            int gamesCount = currentCollection.Games.Count;
            int hardwareCount = currentCollection.HardwareItems.Count +
                                currentCollection.PcSystems.Count;
            int accessoriesCount = currentCollection.Accessories.Count;
            int serviceCount = GetServiceCount();

            double cardY = currentY + 10;
            double smallCardW = (contentWidth - 18) / 2;
            double smallCardH = 86;

            DrawDashboardStatCard(
                @"Assets\Icons\PDF\gamepad.png",
                T("Gry", "Games"),
                gamesCount.ToString(),
                margin,
                cardY,
                smallCardW,
                smallCardH);

            DrawDashboardStatCard(
                @"Assets\Icons\PDF\pc.png",
                T("Sprzęt", "Hardware"),
                hardwareCount.ToString(),
                margin + smallCardW + 18,
                cardY,
                smallCardW,
                smallCardH);

            DrawDashboardStatCard(
                @"Assets\Icons\PDF\accessories.png",
                T("Akcesoria", "Accessories"),
                accessoriesCount.ToString(),
                margin,
                cardY + smallCardH + 18,
                smallCardW,
                smallCardH);

            DrawDashboardStatCard(
                @"Assets\Icons\PDF\service.png",
                T("Serwis", "Service"),
                serviceCount.ToString(),
                margin + smallCardW + 18,
                cardY + smallCardH + 18,
                smallCardW,
                smallCardH);

            currentY = cardY + smallCardH * 2 + 36;

            DrawTotalValueCard(
                margin,
                currentY,
                contentWidth,
                110);

            currentY += 128;

            DrawManufacturerExpenseChartCard(
                margin,
                currentY,
                contentWidth,
                250);

            currentY += 268;
        }
        private static void DrawDashboardStatCard(
            string iconPath,
            string title,
            string value,
            double x,
            double y,
            double width,
            double height)
        {
            DrawCard(x, y, width, height);

            DrawImageIfExists(
                iconPath,
                x + 18,
                y + 20,
                42,
                42);

            XFont titleFont = new XFont("Segoe UI", 10.5, XFontStyleEx.Bold);
            XFont valueFont = new XFont("Segoe UI", 22, XFontStyleEx.Bold);

            gfx!.DrawString(
                title,
                titleFont,
                SecondaryBrush,
                new XRect(x + 72, y + 18, width - 88, 18),
                XStringFormats.TopLeft);

            gfx.DrawString(
                value,
                valueFont,
                TextBrush,
                new XRect(x + 72, y + 38, width - 88, 32),
                XStringFormats.TopLeft);
        }
        private static int GetServiceCount()
        {
            int hardwareServices = currentCollection.HardwareItems
                .Sum(h => h.ServiceHistory.Count);

            int pcServices = currentCollection.PcSystems
                .Sum(p => p.ServiceHistory.Count);

            return hardwareServices + pcServices;
        }
        private static decimal ConvertCurrency(
    decimal amount,
    string fromCurrency,
    string toCurrency)
        {
            if (string.IsNullOrWhiteSpace(fromCurrency))
                fromCurrency = currentDefaultCurrency;

            if (string.IsNullOrWhiteSpace(toCurrency))
                toCurrency = currentDefaultCurrency;

            fromCurrency = fromCurrency.Trim().ToUpper();
            toCurrency = toCurrency.Trim().ToUpper();

            if (fromCurrency == toCurrency)
                return amount;

            decimal fromRate = GetCurrencyRate(fromCurrency);
            decimal toRate = GetCurrencyRate(toCurrency);

            decimal amountInBase = amount * fromRate;

            return amountInBase / toRate;
        }

        private static decimal GetCurrencyRate(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                return 1m;

            currency = currency.Trim().ToUpper();

            if (currency == currentDefaultCurrency)
                return 1m;

            CurrencyRate? rate = currentCollection.CurrencyRates
                .FirstOrDefault(r => r.Currency == currency);

            return rate?.Rate ?? 1m;
        }
        private static decimal GetTotalValue<T>(
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
                    currentDefaultCurrency);
            }

            return total;
        }

        private static decimal GetTotalCollectionValue()
        {
            decimal gamesTotalValue = GetTotalValue(
                currentCollection.Games,
                g => g.PurchasePrice,
                g => g.PurchaseCurrency);

            decimal hardwareTotalValue = GetTotalValue(
                currentCollection.HardwareItems,
                h => h.PurchasePrice,
                h => h.PurchaseCurrency);

            decimal pcTotalValue = GetTotalValue(
                currentCollection.PcSystems,
                p => p.PurchasePrice,
                p => p.PurchaseCurrency);

            decimal accessoriesTotalValue = GetTotalValue(
                currentCollection.Accessories,
                a => a.PurchasePrice,
                a => a.PurchaseCurrency);

            decimal serviceTotalValue =
                currentCollection.HardwareItems
                    .SelectMany(h => h.ServiceHistory)
                    .Sum(s => ConvertCurrency(
                        s.Cost ?? 0m,
                        s.CostCurrency,
                        currentDefaultCurrency))
                +
                currentCollection.PcSystems
                    .SelectMany(p => p.ServiceHistory)
                    .Sum(s => ConvertCurrency(
                        s.Cost ?? 0m,
                        s.CostCurrency,
                        currentDefaultCurrency));

            return gamesTotalValue +
                   hardwareTotalValue +
                   pcTotalValue +
                   accessoriesTotalValue;
        }
        private static void AddOriginalCurrencyValue(
    Dictionary<string, decimal> totals,
    decimal? price,
    string currency)
        {
            if (!price.HasValue)
                return;

            string key = string.IsNullOrWhiteSpace(currency)
                ? currentDefaultCurrency
                : currency.Trim().ToUpper();

            if (!totals.ContainsKey(key))
                totals[key] = 0m;

            totals[key] += price.Value;
        }

        private static Dictionary<string, decimal> GetOriginalCurrencyTotals()
        {
            Dictionary<string, decimal> totals = new();

            foreach (Game game in currentCollection.Games)
                AddOriginalCurrencyValue(totals, game.PurchasePrice, game.PurchaseCurrency);

            foreach (Hardware hardware in currentCollection.HardwareItems)
                AddOriginalCurrencyValue(totals, hardware.PurchasePrice, hardware.PurchaseCurrency);

            foreach (PcSystem pc in currentCollection.PcSystems)
                AddOriginalCurrencyValue(totals, pc.PurchasePrice, pc.PurchaseCurrency);

            foreach (Accessory accessory in currentCollection.Accessories)
                AddOriginalCurrencyValue(totals, accessory.PurchasePrice, accessory.PurchaseCurrency);

            foreach (Hardware hardware in currentCollection.HardwareItems)
            {
                foreach (ServiceEntry service in hardware.ServiceHistory)
                    AddOriginalCurrencyValue(totals, service.Cost, service.CostCurrency);
            }

            foreach (PcSystem pc in currentCollection.PcSystems)
            {
                foreach (ServiceEntry service in pc.ServiceHistory)
                    AddOriginalCurrencyValue(totals, service.Cost, service.CostCurrency);
            }

            return totals;
        }
        private static void DrawTotalValueCard(
            double x,
            double y,
            double width,
            double height)
        {
            decimal totalValue = GetTotalCollectionValue();

            Dictionary<string, decimal> originalCurrencyTotals =
                GetOriginalCurrencyTotals();

            DrawCard(x, y, width, height);

            DrawImageIfExists(
                @"Assets\Icons\PDF\value.png",
                x + 22,
                y + 28,
                48,
                48);

            XFont titleFont = new XFont("Segoe UI", 12, XFontStyleEx.Bold);
            XFont valueFont = new XFont("Segoe UI", 24, XFontStyleEx.Bold);
            XFont smallFont = new XFont("Segoe UI", 9.5, XFontStyleEx.Regular);

            gfx!.DrawString(
                T("Łączna wartość kolekcji", "Total collection value"),
                titleFont,
                SecondaryBrush,
                new XRect(x + 88, y + 22, width - 110, 20),
                XStringFormats.TopLeft);

            gfx.DrawString(
                $"{totalValue:N2} {currentDefaultCurrency}",
                valueFont,
                TextBrush,
                new XRect(x + 88, y + 45, width * 0.48, 34),
                XStringFormats.TopLeft);

            if (originalCurrencyTotals.Count <= 1)
                return;

            double rightX = x + width * 0.66;
            double lineY = y + 28;

            foreach (var item in originalCurrencyTotals.OrderBy(v => v.Key))
            {
                gfx.DrawString(
                    $"{item.Key}: {item.Value:N2}",
                    smallFont,
                    SecondaryBrush,
                    new XRect(rightX, lineY, width - (rightX - x) - 18, 14),
                    XStringFormats.TopLeft);

                lineY += 15;
            }
        }
        private static void DrawHighlightsPage()
        {
            AddPage();

            DrawSectionHeader(T(
                "Najciekawsze informacje",
                "Collection highlights"));

            XFont introFont = new XFont("Segoe UI", 10.5, XFontStyleEx.Regular);

            gfx!.DrawString(
                T(
                    "Najciekawsze informacje i rekordy zapisanej kolekcji.",
                    "The most interesting information and records from your collection."),
                introFont,
                SecondaryBrush,
                new XRect(margin, currentY - 8, contentWidth, 20),
                XStringFormats.TopLeft);

            currentY += 30;

            DrawHighlightsSummaryCard();
        }
        private sealed class HighlightLine
        {
            public string IconPath { get; }
            public string Title { get; }
            public string Value { get; }

            public HighlightLine(
                string iconPath,
                string title,
                string value)
            {
                IconPath = iconPath;
                Title = title;
                Value = value;
            }
        }
        private static List<HighlightLine> GetHighlightLines()
        {
            List<HighlightLine> lines = new();

            string Money(decimal value) =>
                $"{value:N2} {currentDefaultCurrency}";

            void Add(string icon, string title, string value)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    lines.Add(new HighlightLine(icon, title, value));
            }

            var allPurchases = new List<(string Name, decimal Price, string Currency)>();

            allPurchases.AddRange(currentCollection.Games
                .Where(g => g.PurchasePrice is > 0)
                .Select(g => (g.Title, g.PurchasePrice!.Value, g.PurchaseCurrency)));

            allPurchases.AddRange(currentCollection.HardwareItems
                .Where(h => h.PurchasePrice is > 0)
                .Select(h => (
                    !string.IsNullOrWhiteSpace(h.CustomName) ? h.CustomName :
                    !string.IsNullOrWhiteSpace(h.Model) ? h.Model :
                    !string.IsNullOrWhiteSpace(h.Platform) ? h.Platform :
                    T("Sprzęt", "Hardware"),
                    h.PurchasePrice!.Value,
                    h.PurchaseCurrency)));

            allPurchases.AddRange(currentCollection.PcSystems
                .Where(p => p.PurchasePrice is > 0)
                .Select(p => (
                    !string.IsNullOrWhiteSpace(p.Name) ? p.Name :
                    !string.IsNullOrWhiteSpace(p.Model) ? p.Model :
                    T("Komputer", "Computer"),
                    p.PurchasePrice!.Value,
                    p.PurchaseCurrency)));

            allPurchases.AddRange(currentCollection.Accessories
                .Where(a => a.PurchasePrice is > 0)
                .Select(a => (
                    !string.IsNullOrWhiteSpace(a.Model) ? a.Model :
                    !string.IsNullOrWhiteSpace(a.AccessoryType) ? a.AccessoryType :
                    T("Akcesorium", "Accessory"),
                    a.PurchasePrice!.Value,
                    a.PurchaseCurrency)));

            var mostExpensiveItem = allPurchases
                .OrderByDescending(x => ConvertCurrency(x.Price, x.Currency, currentDefaultCurrency))
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(mostExpensiveItem.Name))
            {
                Add(
                    @"Assets\Icons\PDF\value.png",
                    T("Najdroższy element", "Most expensive item"),
                    $"{mostExpensiveItem.Name} • {Money(ConvertCurrency(mostExpensiveItem.Price, mostExpensiveItem.Currency, currentDefaultCurrency))}");
            }

            var mostExpensiveGame = currentCollection.Games
                .Where(g => g.PurchasePrice is > 0)
                .OrderByDescending(g => ConvertCurrency(g.PurchasePrice!.Value, g.PurchaseCurrency, currentDefaultCurrency))
                .FirstOrDefault();

            if (mostExpensiveGame != null)
            {
                Add(
                    @"Assets\Icons\PDF\gamepad.png",
                    T("Najdroższa gra", "Most expensive game"),
                    $"{mostExpensiveGame.Title} • {Money(ConvertCurrency(mostExpensiveGame.PurchasePrice!.Value, mostExpensiveGame.PurchaseCurrency, currentDefaultCurrency))}");
            }

            var highestRatedGame = currentCollection.Games
                .Where(g => g.UserRating.HasValue)
                .OrderByDescending(g => g.UserRating)
                .FirstOrDefault();

            if (highestRatedGame != null)
            {
                Add(
                    @"Assets\Icons\PDF\star.png",
                    T("Najwyżej oceniona gra", "Highest rated game"),
                    $"{highestRatedGame.Title} • {highestRatedGame.UserRating:0.#} / 10");
            }

            var mostExpensiveHardware = currentCollection.HardwareItems
                .Where(h => h.PurchasePrice is > 0)
                .OrderByDescending(h => ConvertCurrency(h.PurchasePrice!.Value, h.PurchaseCurrency, currentDefaultCurrency))
                .FirstOrDefault();

            if (mostExpensiveHardware != null)
            {
                string name =
                    !string.IsNullOrWhiteSpace(mostExpensiveHardware.CustomName) ? mostExpensiveHardware.CustomName :
                    !string.IsNullOrWhiteSpace(mostExpensiveHardware.Model) ? mostExpensiveHardware.Model :
                    !string.IsNullOrWhiteSpace(mostExpensiveHardware.Platform) ? mostExpensiveHardware.Platform :
                    T("Sprzęt", "Hardware");

                Add(
                    @"Assets\Icons\PDF\pc.png",
                    T("Najdroższy sprzęt", "Most expensive hardware"),
                    $"{name} • {Money(ConvertCurrency(mostExpensiveHardware.PurchasePrice!.Value, mostExpensiveHardware.PurchaseCurrency, currentDefaultCurrency))}");
            }

            var mostExpensiveAccessory = currentCollection.Accessories
                .Where(a => a.PurchasePrice is > 0)
                .OrderByDescending(a => ConvertCurrency(a.PurchasePrice!.Value, a.PurchaseCurrency, currentDefaultCurrency))
                .FirstOrDefault();

            if (mostExpensiveAccessory != null)
            {
                string name =
                    !string.IsNullOrWhiteSpace(mostExpensiveAccessory.Model) ? mostExpensiveAccessory.Model :
                    !string.IsNullOrWhiteSpace(mostExpensiveAccessory.AccessoryType) ? mostExpensiveAccessory.AccessoryType :
                    T("Akcesorium", "Accessory");

                Add(
                    @"Assets\Icons\PDF\accessories.png",
                    T("Najdroższe akcesorium", "Most expensive accessory"),
                    $"{name} • {Money(ConvertCurrency(mostExpensiveAccessory.PurchasePrice!.Value, mostExpensiveAccessory.PurchaseCurrency, currentDefaultCurrency))}");
            }

            var serviceEntries = currentCollection.HardwareItems
                .SelectMany(h => h.ServiceHistory.Select(s => new
                {
                    Device = !string.IsNullOrWhiteSpace(h.CustomName) ? h.CustomName :
                             !string.IsNullOrWhiteSpace(h.Model) ? h.Model :
                             !string.IsNullOrWhiteSpace(h.Platform) ? h.Platform :
                             T("Sprzęt", "Hardware"),
                    Entry = s
                }))
                .Concat(currentCollection.PcSystems.SelectMany(p => p.ServiceHistory.Select(s => new
                {
                    Device = !string.IsNullOrWhiteSpace(p.Name) ? p.Name :
                             !string.IsNullOrWhiteSpace(p.Model) ? p.Model :
                             T("Komputer", "Computer"),
                    Entry = s
                })))
                .ToList();

            var mostExpensiveService = serviceEntries
                .Where(x => x.Entry.Cost is > 0)
                .OrderByDescending(x => ConvertCurrency(x.Entry.Cost!.Value, x.Entry.CostCurrency, currentDefaultCurrency))
                .FirstOrDefault();

            if (mostExpensiveService != null)
            {
                Add(
                    @"Assets\Icons\PDF\service.png",
                    T("Najdroższy serwis", "Most expensive service"),
                    $"{mostExpensiveService.Device} • {Money(ConvertCurrency(mostExpensiveService.Entry.Cost!.Value, mostExpensiveService.Entry.CostCurrency, currentDefaultCurrency))}");
            }

            var lastAdded = currentCollection.Games.Select(g => (Name: g.Title, Date: g.DateAdded))
                .Concat(currentCollection.HardwareItems.Select(h => (
                    !string.IsNullOrWhiteSpace(h.CustomName) ? h.CustomName :
                    !string.IsNullOrWhiteSpace(h.Model) ? h.Model :
                    !string.IsNullOrWhiteSpace(h.Platform) ? h.Platform :
                    T("Sprzęt", "Hardware"),
                    h.DateAdded)))
                .Concat(currentCollection.PcSystems.Select(p => (
                    !string.IsNullOrWhiteSpace(p.Name) ? p.Name :
                    !string.IsNullOrWhiteSpace(p.Model) ? p.Model :
                    T("Komputer", "Computer"),
                    p.DateAdded)))
                .Concat(currentCollection.Accessories.Select(a => (
                    !string.IsNullOrWhiteSpace(a.Model) ? a.Model :
                    !string.IsNullOrWhiteSpace(a.AccessoryType) ? a.AccessoryType :
                    T("Akcesorium", "Accessory"),
                    a.DateAdded)))
                .OrderByDescending(x => x.Item2)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(lastAdded.Item1))
            {
                Add(
                    @"Assets\Icons\PDF\calendar.png",
                    T("Ostatnio dodany element", "Last added item"),
                    $"{lastAdded.Item1} • {lastAdded.Item2:dd.MM.yyyy}");
            }

            var topPlatform = currentCollection.Games
                .Where(g => !string.IsNullOrWhiteSpace(g.Platform))
                .GroupBy(g => g.Platform.Trim())
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            if (topPlatform != null)
            {
                Add(
                    @"Assets\Icons\PDF\gamepad.png",
                    T("Najpopularniejsza platforma", "Most popular platform"),
                    $"{topPlatform.Key} • {topPlatform.Count()}");
            }

            int photoCount =
                currentCollection.Games.Sum(g => g.Photos.Count) +
                currentCollection.Games.Count(g => g.FrontCoverPhoto != null) +
                currentCollection.Games.Count(g => g.BackCoverPhoto != null) +
                currentCollection.HardwareItems.Sum(h => h.Photos.Count) +
                currentCollection.PcSystems.Sum(p => p.Photos.Count) +
                currentCollection.Accessories.Sum(a => a.Photos.Count);

            Add(
                @"Assets\Icons\nophoto.png",
                T("Zdjęcia w kolekcji", "Photos in collection"),
                photoCount.ToString());

            return lines.Take(9).ToList();
        }
        private static void DrawHighlightsSummaryCard()
        {
            List<HighlightLine> lines = GetHighlightLines();

            double cardX = margin;
            double cardY = currentY;
            double cardW = contentWidth;
            double cardH = 440;

            CheckPageBreak(cardH + 12);

            cardY = currentY;

            DrawCard(cardX, cardY, cardW, cardH);

            XFont itemTitleFont = new XFont("Segoe UI", 10.8, XFontStyleEx.Bold);
            XFont itemValueFont = new XFont("Segoe UI", 10.2, XFontStyleEx.Regular);

            double rowY = cardY + 24;
            double rowH = 45;

            foreach (HighlightLine line in lines.Take(9))
            {
                DrawImageIfExists(
                    line.IconPath,
                    cardX + 28,
                    rowY,
                    28,
                    28);

                gfx!.DrawString(
                    line.Title,
                    itemTitleFont,
                    TextBrush,
                    new XRect(cardX + 76, rowY + 3, 195, 16),
                    XStringFormats.TopLeft);

                gfx.DrawString(
                    TrimToWidth(line.Value, itemValueFont, cardW - 285),
                    itemValueFont,
                    SecondaryBrush,
                    new XRect(cardX + 285, rowY + 3, cardW - 315, 18),
                    XStringFormats.TopLeft);

                gfx.DrawLine(
                    new XPen(BorderColor, 0.7),
                    cardX + 76,
                    rowY + rowH - 6,
                    cardX + cardW - 24,
                    rowY + rowH - 6);

                rowY += rowH;
            }

            currentY = cardY + cardH + 12;
        }
        private static Dictionary<string, decimal> GetPlatformPurchaseTotals()
        {
            Dictionary<string, decimal> totals = new();

            foreach (Game game in currentCollection.Games)
            {
                decimal price = ConvertCurrency(
                    game.PurchasePrice ?? 0m,
                    game.PurchaseCurrency,
                    currentDefaultCurrency);

                string platform;

                if (string.IsNullOrWhiteSpace(game.Platform))
                {
                    platform = T("Niesklasyfikowane", "Unclassified");
                }
                else
                {
                    platform = game.Platform.Trim();
                }

                if (!totals.ContainsKey(platform))
                    totals[platform] = 0m;

                totals[platform] += price;
            }

            return totals;
        }
        private static void DrawStatisticSectionCard(
            string iconPath,
            string title,
            IEnumerable<StatLine> lines,
            double x,
            double y,
            double width,
            double height)
        {
            DrawCard(x, y, width, height);

            DrawImageIfExists(
                iconPath,
                x + 12,
                y + 10,
                30,
                30);

            XFont titleFont = new XFont("Segoe UI", 12, XFontStyleEx.Bold);
            XFont lineFont = new XFont("Segoe UI", 10.2, XFontStyleEx.Regular);
            XFont boldLineFont = new XFont("Segoe UI", 10.2, XFontStyleEx.Bold);

            gfx!.DrawString(
                title,
                titleFont,
                TextBrush,
                new XRect(x + 50, y + 15, width - 62, 18),
                XStringFormats.TopLeft);

            double lineY = y + 44;

            foreach (StatLine line in lines)
            {
                gfx.DrawString(
                    line.Text,
                    line.IsBold ? boldLineFont : lineFont,
                    line.IsBold ? TextBrush : SecondaryBrush,
                    new XRect(x + 16, lineY, width - 32, 15),
                    XStringFormats.TopLeft);

                lineY += 13;
            }
        }

    }
}