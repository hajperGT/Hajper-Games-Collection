using System;
using System.Windows;
using HGC.Models;

namespace HGC
{
    public partial class PcGpuWindow : Window
    {
        public PcGpu? ResultGpu { get; private set; }

        private readonly string currentLanguage;
        private readonly PcGpu? editingGpu;

        public PcGpuWindow(string language)
        {
            InitializeComponent();
            currentLanguage = language;

            ApplyLanguage();
            LoadInterfaceTypes();
        }

        public PcGpuWindow(string language, PcGpu gpu)
            : this(language)
        {
            editingGpu = gpu;

            GpuManufacturerTextBox.Text = gpu.GpuManufacturer;
            GpuModelTextBox.Text = gpu.Model;
            GpuCardManufacturerTextBox.Text = gpu.CardManufacturer;

            GpuHasSerialCheckBox.IsChecked = gpu.HasSerialNumber;
            GpuSerialTextBox.Text = gpu.SerialNumber;

            GpuVramAmountTextBox.Text = gpu.VramAmount;
            GpuVramTypeTextBox.Text = gpu.VramType;
            GpuVramBusTextBox.Text = gpu.VramBusWidth;
            GpuVramClockTextBox.Text = gpu.VramClock;
            GpuCoreClockTextBox.Text = gpu.CoreClock;

            GpuHasOverclockCheckBox.IsChecked = gpu.HasOverclock;
            GpuOverclockCoreTextBox.Text = gpu.OverclockCoreClock;
            GpuOverclockMemoryTextBox.Text = gpu.OverclockMemoryClock;
            GpuPowerLimitTextBox.Text = gpu.PowerLimit;

            GpuPurchaseDatePicker.SelectedDate = gpu.PurchaseDate;
            GpuPurchasePriceTextBox.Text = gpu.PurchasePrice?.ToString() ?? "";

            GpuInterfaceTypeCombo.Text = gpu.InterfaceType;
            LoadInterfaceVersions();
            GpuInterfaceVersionCombo.Text = gpu.InterfaceVersion;

            GpuHasCoreDetailsCheckBox.IsChecked = gpu.HasCoreDetails;

            GpuRopsTextBox.Text = gpu.Rops;
            GpuTmusTextBox.Text = gpu.Tmus;
            GpuVertexShadersTextBox.Text = gpu.VertexShaders;
            GpuPixelShadersTextBox.Text = gpu.PixelShaders;
            GpuUnifiedShadersTextBox.Text = gpu.UnifiedShaders;
            GpuCudaCoresTextBox.Text = gpu.CudaCores;
            GpuStreamProcessorsTextBox.Text = gpu.StreamProcessors;
            GpuRtUnitsTextBox.Text = gpu.RtUnits;
            GpuTensorUnitsTextBox.Text = gpu.TensorUnits;
            GpuOtherCoreDetailsTextBox.Text = gpu.OtherCoreDetails;
        }

        private void ApplyLanguage()
        {
            WindowTitleText.Text = currentLanguage == "pl" ? "Dodaj kartę graficzną" : "Add graphics card";

            GpuBasicTitle.Text = currentLanguage == "pl" ? "Podstawowe informacje" : "Basic information";
            GpuManufacturerLabel.Text = currentLanguage == "pl" ? "Producent GPU" : "GPU manufacturer";
            GpuModelLabel.Text = "Model";
            GpuCardManufacturerLabel.Text = currentLanguage == "pl" ? "Producent karty" : "Card manufacturer";

            GpuHasSerialCheckBox.Content = currentLanguage == "pl" ? "Dodaj numer seryjny" : "Add serial number";
            GpuSerialLabel.Text = currentLanguage == "pl" ? "Numer seryjny" : "Serial number";

            GpuDetailsTitle.Text = currentLanguage == "pl" ? "Szczegóły" : "Details";
            GpuVramAmountLabel.Text = currentLanguage == "pl" ? "Ilość pamięci VRAM" : "VRAM amount";
            GpuVramTypeLabel.Text = currentLanguage == "pl" ? "Rodzaj pamięci VRAM" : "VRAM type";
            GpuVramBusLabel.Text = currentLanguage == "pl" ? "Szyna pamięci VRAM" : "VRAM bus width";
            GpuVramClockLabel.Text = currentLanguage == "pl" ? "Taktowanie pamięci VRAM" : "VRAM clock";
            GpuCoreClockLabel.Text = currentLanguage == "pl" ? "Taktowanie rdzenia" : "Core clock";

            GpuHasOverclockCheckBox.Content = currentLanguage == "pl" ? "Dodaj podkręcenie" : "Add overclock";
            GpuOverclockCoreLabel.Text = currentLanguage == "pl" ? "Aktualne taktowanie rdzenia" : "Current core clock";
            GpuOverclockMemoryLabel.Text = currentLanguage == "pl" ? "Aktualne taktowanie pamięci" : "Current memory clock";
            GpuPowerLimitLabel.Text = currentLanguage == "pl" ? "Aktualny limit mocy" : "Current power limit";

            GpuPurchaseTitle.Text = currentLanguage == "pl" ? "Zakup" : "Purchase";
            GpuPurchaseDateLabel.Text = currentLanguage == "pl" ? "Data zakupu" : "Purchase date";
            GpuPurchasePriceLabel.Text = currentLanguage == "pl" ? "Cena zakupu" : "Purchase price";

            CancelButton.Content = currentLanguage == "pl" ? "Anuluj" : "Cancel";
            SaveButton.Content = currentLanguage == "pl" ? "Zapisz" : "Save";
        }

        private void GpuHasSerialCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            GpuSerialPanel.Visibility =
                GpuHasSerialCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void GpuHasOverclockCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            GpuOverclockPanel.Visibility =
                GpuHasOverclockCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
        private void GpuInterfaceTypeCombo_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LoadInterfaceVersions();
        }

        private void GpuHasCoreDetailsCheckBox_Checked(
            object sender,
            RoutedEventArgs e)
        {
            GpuCoreDetailsPanel.Visibility =
                GpuHasCoreDetailsCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void LoadInterfaceTypes()
        {
            GpuInterfaceTypeCombo.Items.Clear();

            GpuInterfaceTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            GpuInterfaceTypeCombo.Items.Add("PCI");
            GpuInterfaceTypeCombo.Items.Add("PCI-E");
            GpuInterfaceTypeCombo.Items.Add("AGP");
            GpuInterfaceTypeCombo.Items.Add("ISA");

            GpuInterfaceTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Inny" : "Other");

            GpuInterfaceTypeCombo.SelectedIndex = 0;
        }

        private void LoadInterfaceVersions()
        {
            string selected =
                GpuInterfaceTypeCombo.SelectedItem?.ToString() ?? "";

            GpuInterfaceVersionCombo.Items.Clear();

            bool showVersion =
                selected == "PCI-E" ||
                selected == "AGP";

            GpuInterfaceVersionLabel.Visibility =
                showVersion ? Visibility.Visible : Visibility.Collapsed;

            GpuInterfaceVersionCombo.Visibility =
                showVersion ? Visibility.Visible : Visibility.Collapsed;

            if (!showVersion)
                return;

            GpuInterfaceVersionCombo.Items.Add(
                currentLanguage == "pl" ? "Wybierz" : "Select");

            if (selected == "PCI-E")
            {
                GpuInterfaceVersionCombo.Items.Add("PCI-E 1.0");
                GpuInterfaceVersionCombo.Items.Add("PCI-E 2.0");
                GpuInterfaceVersionCombo.Items.Add("PCI-E 3.0");
                GpuInterfaceVersionCombo.Items.Add("PCI-E 4.0");
                GpuInterfaceVersionCombo.Items.Add("PCI-E 5.0");
                GpuInterfaceVersionCombo.Items.Add("PCI-E 6.0");
            }

            if (selected == "AGP")
            {
                GpuInterfaceVersionCombo.Items.Add("AGP x1");
                GpuInterfaceVersionCombo.Items.Add("AGP x2");
                GpuInterfaceVersionCombo.Items.Add("AGP x4");
                GpuInterfaceVersionCombo.Items.Add("AGP x8");
            }

            GpuInterfaceVersionCombo.SelectedIndex = 0;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(GpuModelTextBox.Text))
            {
                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? "Podaj model karty graficznej."
                        : "Enter graphics card model.",
                    "HGC",
                    HgcMessageBoxType.Information,
                    currentLanguage);

                return;
            }

            PcGpu gpu = editingGpu ?? new PcGpu();

            gpu.GpuManufacturer = GpuManufacturerTextBox.Text.Trim();
            gpu.Model = GpuModelTextBox.Text.Trim();
            gpu.CardManufacturer = GpuCardManufacturerTextBox.Text.Trim();

            gpu.HasSerialNumber = GpuHasSerialCheckBox.IsChecked == true;
            gpu.SerialNumber = gpu.HasSerialNumber ? GpuSerialTextBox.Text.Trim() : "";

            gpu.VramAmount = GpuVramAmountTextBox.Text.Trim();
            gpu.VramType = GpuVramTypeTextBox.Text.Trim();
            gpu.VramBusWidth = GpuVramBusTextBox.Text.Trim();
            gpu.VramClock = GpuVramClockTextBox.Text.Trim();
            gpu.CoreClock = GpuCoreClockTextBox.Text.Trim();

            gpu.HasOverclock = GpuHasOverclockCheckBox.IsChecked == true;
            gpu.OverclockCoreClock = gpu.HasOverclock ? GpuOverclockCoreTextBox.Text.Trim() : "";
            gpu.OverclockMemoryClock = gpu.HasOverclock ? GpuOverclockMemoryTextBox.Text.Trim() : "";
            gpu.PowerLimit = gpu.HasOverclock ? GpuPowerLimitTextBox.Text.Trim() : "";

            gpu.InterfaceType =
            IsSelectedValue(GpuInterfaceTypeCombo.Text)
            ? GpuInterfaceTypeCombo.Text
            : "";

            gpu.InterfaceVersion =
                GpuInterfaceVersionCombo.Visibility == Visibility.Visible &&
                IsSelectedValue(GpuInterfaceVersionCombo.Text)
                    ? GpuInterfaceVersionCombo.Text
                    : "";

            gpu.HasCoreDetails =
                GpuHasCoreDetailsCheckBox.IsChecked == true;

            gpu.Rops = gpu.HasCoreDetails ? GpuRopsTextBox.Text.Trim() : "";
            gpu.Tmus = gpu.HasCoreDetails ? GpuTmusTextBox.Text.Trim() : "";
            gpu.VertexShaders = gpu.HasCoreDetails ? GpuVertexShadersTextBox.Text.Trim() : "";
            gpu.PixelShaders = gpu.HasCoreDetails ? GpuPixelShadersTextBox.Text.Trim() : "";
            gpu.UnifiedShaders = gpu.HasCoreDetails ? GpuUnifiedShadersTextBox.Text.Trim() : "";
            gpu.CudaCores = gpu.HasCoreDetails ? GpuCudaCoresTextBox.Text.Trim() : "";
            gpu.StreamProcessors = gpu.HasCoreDetails ? GpuStreamProcessorsTextBox.Text.Trim() : "";
            gpu.RtUnits = gpu.HasCoreDetails ? GpuRtUnitsTextBox.Text.Trim() : "";
            gpu.TensorUnits = gpu.HasCoreDetails ? GpuTensorUnitsTextBox.Text.Trim() : "";
            gpu.OtherCoreDetails = gpu.HasCoreDetails ? GpuOtherCoreDetailsTextBox.Text.Trim() : "";

            gpu.PurchaseDate = GpuPurchaseDatePicker.SelectedDate;

            if (decimal.TryParse(GpuPurchasePriceTextBox.Text, out decimal price))
                gpu.PurchasePrice = price;
            else
                gpu.PurchasePrice = null;

            ResultGpu = gpu;

            DialogResult = true;
            Close();
        }
        private bool IsSelectedValue(string value)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                   value != "Wybierz" &&
                   value != "Select";
        }
    }
}