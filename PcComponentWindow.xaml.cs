using HGC.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace HGC
{
    public partial class PcComponentWindow : Window
    {
        private readonly string currentLanguage;
        private readonly string componentType;
        private readonly object? editComponent;
        private readonly bool isEditMode;

        public object? ResultComponent { get; private set; }

        public PcComponentWindow(
            string language,
            string type)
        {
            InitializeComponent();

            currentLanguage = language;
            componentType = type;

            ApplyLanguage();
            LoadCombos();
            ShowComponentPanel();
        }
        public PcComponentWindow(
        string language,
        string type,
        object component)
        : this(language, type)
        {
            editComponent = component;
            isEditMode = true;

            LoadComponentData();
        }
        private void ApplyLanguage()
        {
            WindowTitleText.Text = componentType switch
            {
                "RAM" => currentLanguage == "pl"
                    ? (isEditMode ? "Edytuj RAM" : "Dodaj RAM")
                    : (isEditMode ? "Edit RAM" : "Add RAM"),

                "Storage" => currentLanguage == "pl"
                    ? (isEditMode ? "Edytuj dysk" : "Dodaj dysk")
                    : (isEditMode ? "Edit drive" : "Add drive"),

                "PSU" => currentLanguage == "pl"
                    ? (isEditMode ? "Edytuj zasilacz" : "Dodaj zasilacz")
                    : (isEditMode ? "Edit PSU" : "Add PSU"),

                "Case" => currentLanguage == "pl"
                    ? (isEditMode ? "Edytuj obudowę" : "Dodaj obudowę")
                    : (isEditMode ? "Edit case" : "Add case"),

                _ => currentLanguage == "pl"
                    ? "Komponent"
                    : "Component"
            };

            BasicTitle.Text =
                currentLanguage == "pl" ? "Podstawowe informacje" : "Basic information";

            ManufacturerLabel.Text =
                currentLanguage == "pl" ? "Producent" : "Manufacturer";

            ModelLabel.Text =
                currentLanguage == "pl" ? "Model" : "Model";

            PurchaseTitle.Text =
                currentLanguage == "pl" ? "Zakup" : "Purchase";

            PurchaseDateLabel.Text =
                currentLanguage == "pl" ? "Data zakupu" : "Purchase date";

            PurchasePriceLabel.Text =
                currentLanguage == "pl" ? "Cena zakupu" : "Purchase price";

            CancelButton.Content =
                currentLanguage == "pl" ? "Anuluj" : "Cancel";

            SaveButton.Content =
                currentLanguage == "pl" ? "Zapisz" : "Save";
        }
        private void LoadComponentData()
        {
            if (editComponent is PcRam ram)
            {
                ManufacturerTextBox.Text = ram.Manufacturer;
                ModelTextBox.Text = ram.Model;
                RamCapacityTextBox.Text = ram.TotalCapacity;
                RamTypeCombo.Text = ram.MemoryType;
                RamClockTextBox.Text = ram.Clock;
                PurchaseDatePicker.SelectedDate = ram.PurchaseDate;
                PurchasePriceTextBox.Text = ram.PurchasePrice?.ToString();
            }
            else if (editComponent is PcStorage storage)
            {
                ManufacturerTextBox.Text = storage.Manufacturer;
                ModelTextBox.Text = storage.Model;
                StorageCapacityTextBox.Text = storage.Capacity;
                StorageDriveTypeCombo.Text = storage.DriveType;
                StorageInterfaceCombo.Text = storage.Interface;
                PurchaseDatePicker.SelectedDate = storage.PurchaseDate;
                PurchasePriceTextBox.Text = storage.PurchasePrice?.ToString();
            }
            else if (editComponent is PcPowerSupply psu)
            {
                ManufacturerTextBox.Text = psu.Manufacturer;
                ModelTextBox.Text = psu.Model;
                PsuPowerTextBox.Text = psu.Power;
                PsuFormFactorCombo.Text = psu.FormFactor;
                PurchaseDatePicker.SelectedDate = psu.PurchaseDate;
                PurchasePriceTextBox.Text = psu.PurchasePrice?.ToString();
            }
            else if (editComponent is PcCase pcCase)
            {
                ManufacturerTextBox.Text = pcCase.Manufacturer;
                ModelTextBox.Text = pcCase.Model;
                CaseTypeCombo.Text = pcCase.CaseType;
                PurchaseDatePicker.SelectedDate = pcCase.PurchaseDate;
                PurchasePriceTextBox.Text = pcCase.PurchasePrice?.ToString();
            }
        }
        private void LoadCombos()
        {
            LoadRamCombos();
            LoadStorageCombos();
            LoadPsuCombos();
            LoadCaseCombos();
        }

        private void ShowComponentPanel()
        {
            RamPanel.Visibility = Visibility.Collapsed;
            RamTimingsPanel.Visibility = Visibility.Collapsed;
            RamOverclockPanel.Visibility = Visibility.Collapsed;
            StoragePanel.Visibility = Visibility.Collapsed;
            PsuPanel.Visibility = Visibility.Collapsed;
            CasePanel.Visibility = Visibility.Collapsed;

            if (componentType == "RAM")
            {
                RamPanel.Visibility = Visibility.Visible;
                RamTimingsPanel.Visibility = Visibility.Visible;
                RamOverclockPanel.Visibility = Visibility.Visible;
            }

            if (componentType == "Storage")
                StoragePanel.Visibility = Visibility.Visible;

            if (componentType == "PSU")
                PsuPanel.Visibility = Visibility.Visible;

            if (componentType == "Case")
                CasePanel.Visibility = Visibility.Visible;
        }

        private void LoadRamCombos()
        {
            RamTypeCombo.Items.Clear();

            RamTypeCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");

            RamTypeCombo.Items.Add("FPM");
            RamTypeCombo.Items.Add("EDO");
            RamTypeCombo.Items.Add("SDRAM");

            RamTypeCombo.Items.Add("DDR");
            RamTypeCombo.Items.Add("DDR2");
            RamTypeCombo.Items.Add("DDR3");
            RamTypeCombo.Items.Add("DDR4");
            RamTypeCombo.Items.Add("DDR5");

            RamTypeCombo.Items.Add("RDRAM");

            RamProfileCombo.Items.Clear();
            RamProfileCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");
            RamProfileCombo.Items.Add("JEDEC");
            RamProfileCombo.Items.Add("XMP");
            RamProfileCombo.Items.Add("EXPO");
            RamProfileCombo.Items.Add("Manual");

            RamTypeCombo.SelectedIndex = 0;
            RamProfileCombo.SelectedIndex = 0;
        }

        private void LoadStorageCombos()
        {
            StorageDriveTypeCombo.Items.Clear();
            StorageDriveTypeCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");
            StorageDriveTypeCombo.Items.Add("HDD");
            StorageDriveTypeCombo.Items.Add("SSD");
            StorageDriveTypeCombo.Items.Add("SSHD");
            StorageDriveTypeCombo.Items.Add("CF");
            StorageDriveTypeCombo.Items.Add("SD");
            StorageDriveTypeCombo.Items.Add("MicroSD");
            StorageDriveTypeCombo.Items.Add("ZIP");
            StorageDriveTypeCombo.Items.Add("MO");
            StorageDriveTypeCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");
            StorageDriveTypeCombo.SelectedIndex = 0;

            StorageInterfaceCombo.Items.Clear();
            StorageInterfaceCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");
            StorageInterfaceCombo.Items.Add("IDE");
            StorageInterfaceCombo.Items.Add("EIDE");
            StorageInterfaceCombo.Items.Add("SATA");
            StorageInterfaceCombo.Items.Add("mSATA");
            StorageInterfaceCombo.Items.Add("M.2 SATA");
            StorageInterfaceCombo.Items.Add("M.2 NVMe");
            StorageInterfaceCombo.Items.Add("U.2");
            StorageInterfaceCombo.Items.Add("SCSI");
            StorageInterfaceCombo.Items.Add("SAS");
            StorageInterfaceCombo.Items.Add("USB");
            StorageInterfaceCombo.Items.Add("FireWire");
            StorageInterfaceCombo.Items.Add("PCI");
            StorageInterfaceCombo.Items.Add("PCI-E");
            StorageInterfaceCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");
            StorageInterfaceCombo.SelectedIndex = 0;

            StorageFormFactorCombo.Items.Clear();
            StorageFormFactorCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");
            StorageFormFactorCombo.Items.Add("1.8\"");
            StorageFormFactorCombo.Items.Add("2.5\"");
            StorageFormFactorCombo.Items.Add("3.5\"");
            StorageFormFactorCombo.Items.Add("M.2 2230");
            StorageFormFactorCombo.Items.Add("M.2 2242");
            StorageFormFactorCombo.Items.Add("M.2 2260");
            StorageFormFactorCombo.Items.Add("M.2 2280");
            StorageFormFactorCombo.Items.Add("M.2 22110");
            StorageFormFactorCombo.Items.Add("CF");
            StorageFormFactorCombo.Items.Add("SD");
            StorageFormFactorCombo.Items.Add("MicroSD");
            StorageFormFactorCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");
            StorageFormFactorCombo.SelectedIndex = 0;
        }

        private void LoadPsuCombos()
        {
            PsuFormFactorCombo.Items.Clear();
            PsuFormFactorCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");
            PsuFormFactorCombo.Items.Add("AT");
            PsuFormFactorCombo.Items.Add("ATX");
            PsuFormFactorCombo.Items.Add("SFX");
            PsuFormFactorCombo.Items.Add("SFX-L");
            PsuFormFactorCombo.Items.Add("TFX");
            PsuFormFactorCombo.Items.Add("Flex ATX");
            PsuFormFactorCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");
            PsuFormFactorCombo.SelectedIndex = 0;

            PsuStandardCombo.Items.Clear();
            PsuStandardCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");
            PsuStandardCombo.Items.Add("AT");
            PsuStandardCombo.Items.Add("ATX");
            PsuStandardCombo.Items.Add("ATX 2.0");
            PsuStandardCombo.Items.Add("ATX 2.01");
            PsuStandardCombo.Items.Add("ATX 2.1");
            PsuStandardCombo.Items.Add("ATX 2.2");
            PsuStandardCombo.Items.Add("ATX 2.3");
            PsuStandardCombo.Items.Add("ATX 2.4");
            PsuStandardCombo.Items.Add("ATX 3.0");
            PsuStandardCombo.Items.Add("ATX 3.1");
            PsuStandardCombo.Items.Add(currentLanguage == "pl" ? "Inna" : "Other");
            PsuStandardCombo.SelectedIndex = 0;

            PsuCertificateCombo.Items.Clear();
            PsuCertificateCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");
            PsuCertificateCombo.Items.Add("80 Plus");
            PsuCertificateCombo.Items.Add("80 Plus Bronze");
            PsuCertificateCombo.Items.Add("80 Plus Silver");
            PsuCertificateCombo.Items.Add("80 Plus Gold");
            PsuCertificateCombo.Items.Add("80 Plus Platinum");
            PsuCertificateCombo.Items.Add("80 Plus Titanium");
            PsuCertificateCombo.Items.Add("Cybenetics");
            PsuCertificateCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");
            PsuCertificateCombo.SelectedIndex = 0;

            PsuModularityCombo.Items.Clear();
            PsuModularityCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");
            PsuModularityCombo.Items.Add(currentLanguage == "pl" ? "Niemodularny" : "Non-modular");
            PsuModularityCombo.Items.Add(currentLanguage == "pl" ? "Półmodularny" : "Semi-modular");
            PsuModularityCombo.Items.Add(currentLanguage == "pl" ? "Modularny" : "Modular");
            PsuModularityCombo.SelectedIndex = 0;
        }

        private void LoadCaseCombos()
        {
            CaseTypeCombo.Items.Clear();
            CaseTypeCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");
            CaseTypeCombo.Items.Add("Desktop");
            CaseTypeCombo.Items.Add("Mini Tower");
            CaseTypeCombo.Items.Add("Midi Tower");
            CaseTypeCombo.Items.Add("Full Tower");
            CaseTypeCombo.Items.Add("Mini-ITX");
            CaseTypeCombo.Items.Add("Micro-ATX");
            CaseTypeCombo.Items.Add("ATX");
            CaseTypeCombo.Items.Add("E-ATX");
            CaseTypeCombo.Items.Add("HTPC");
            CaseTypeCombo.Items.Add("Rack 1U");
            CaseTypeCombo.Items.Add("Rack 2U");
            CaseTypeCombo.Items.Add("Rack 3U");
            CaseTypeCombo.Items.Add("Rack 4U");
            CaseTypeCombo.Items.Add("Open Bench");
            CaseTypeCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");
            CaseTypeCombo.SelectedIndex = 0;

            CaseColorCombo.Items.Clear();
            CaseColorCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");
            CaseColorCombo.Items.Add(currentLanguage == "pl" ? "Czarny" : "Black");
            CaseColorCombo.Items.Add(currentLanguage == "pl" ? "Biały" : "White");
            CaseColorCombo.Items.Add(currentLanguage == "pl" ? "Srebrny" : "Silver");
            CaseColorCombo.Items.Add(currentLanguage == "pl" ? "Szary" : "Gray");
            CaseColorCombo.Items.Add(currentLanguage == "pl" ? "Czerwony" : "Red");
            CaseColorCombo.Items.Add(currentLanguage == "pl" ? "Niebieski" : "Blue");
            CaseColorCombo.Items.Add(currentLanguage == "pl" ? "Zielony" : "Green");
            CaseColorCombo.Items.Add(currentLanguage == "pl" ? "Żółty" : "Yellow");
            CaseColorCombo.Items.Add(currentLanguage == "pl" ? "Pomarańczowy" : "Orange");
            CaseColorCombo.Items.Add(currentLanguage == "pl" ? "Fioletowy" : "Purple");
            CaseColorCombo.Items.Add(currentLanguage == "pl" ? "Różowy" : "Pink");
            CaseColorCombo.Items.Add(currentLanguage == "pl" ? "Przezroczysty" : "Transparent");
            CaseColorCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");
            CaseColorCombo.SelectedIndex = 0;

            CaseMaxMotherboardCombo.Items.Clear();
            CaseMaxMotherboardCombo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");
            CaseMaxMotherboardCombo.Items.Add("Mini-ITX");
            CaseMaxMotherboardCombo.Items.Add("mATX");
            CaseMaxMotherboardCombo.Items.Add("ATX");
            CaseMaxMotherboardCombo.Items.Add("E-ATX");
            CaseMaxMotherboardCombo.Items.Add("SSI-EEB");
            CaseMaxMotherboardCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");
            CaseMaxMotherboardCombo.SelectedIndex = 0;
        }

        private bool IsSelectedValue(string value)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                   value != "Wybierz" &&
                   value != "Select" &&
                   value != "Inny" &&
                   value != "Other" &&
                   value != "Inna";
        }

        private void RamHasOverclockCheckBox_Checked(
            object sender,
            RoutedEventArgs e)
        {
            RamOverclockFieldsPanel.Visibility =
                RamHasOverclockCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void PsuHasRailsCheckBox_Checked(
            object sender,
            RoutedEventArgs e)
        {
            PsuRailsPanel.Visibility =
                PsuHasRailsCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void CancelButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ModelTextBox.Text))
            {
                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? "Podaj model komponentu."
                        : "Enter component model.",
                    "HGC",
                    HgcMessageBoxType.Information,
                    currentLanguage);

                return;
            }

            ResultComponent = componentType switch
            {
                "RAM" => CreateRam(),
                "Storage" => CreateStorage(),
                "PSU" => CreatePowerSupply(),
                "Case" => CreateCase(),
                _ => null
            };

            DialogResult = true;
            Close();
        }
        private void WaterCoolingModeCombo_SelectionChanged(
         object sender,
         SelectionChangedEventArgs e)
        {
            SetWaterCoolingPanelVisibility(
                "WaterRamCoolingModeCombo",
                "WaterRamCoolingPanel");

            SetWaterCoolingPanelVisibility(
                "WaterStorageCoolingModeCombo",
                "WaterStorageCoolingPanel");

            SetWaterCoolingPanelVisibility(
                "WaterPsuCoolingModeCombo",
                "WaterPsuCoolingPanel");
        }

        private void SetWaterCoolingPanelVisibility(
        string comboName,
        string panelName)
        {
            if (FindName(comboName) is not ComboBox combo)
                return;

            if (FindName(panelName) is not StackPanel panel)
                return;

            panel.Visibility =
                combo.SelectedIndex == 1
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
        private PcRam CreateRam()
        {
            PcRam ram = new PcRam();

            ram.Manufacturer = ManufacturerTextBox.Text.Trim();
            ram.Model = ModelTextBox.Text.Trim();

            if (int.TryParse(RamModuleCountTextBox.Text, out int modules))
                ram.ModuleCount = modules;

            ram.TotalCapacity = RamCapacityTextBox.Text.Trim();
            ram.MemoryType = IsSelectedValue(RamTypeCombo.Text) ? RamTypeCombo.Text : "";
            ram.IsEcc = RamEccCheckBox.IsChecked == true;
            ram.IsRegistered = RamRegisteredCheckBox.IsChecked == true;
            ram.Profile = IsSelectedValue(RamProfileCombo.Text) ? RamProfileCombo.Text : "";

            ram.Clock = RamClockTextBox.Text.Trim();
            ram.Voltage = RamVoltageTextBox.Text.Trim();

            ram.CL = RamCLTextBox.Text.Trim();
            ram.TRCD = RamTRCDTextBox.Text.Trim();
            ram.TRP = RamTRPTextBox.Text.Trim();
            ram.TRAS = RamTRASTextBox.Text.Trim();
            ram.TRC = RamTRCTextBox.Text.Trim();
            ram.CommandRate = RamCommandRateTextBox.Text.Trim();

            ram.TRFC = RamTRFCTextBox.Text.Trim();
            ram.TREFI = RamTREFITextBox.Text.Trim();
            ram.TFAW = RamTFAWTextBox.Text.Trim();
            ram.TRRDS = RamTRRDSTextBox.Text.Trim();
            ram.TRRDL = RamTRRDLTextBox.Text.Trim();
            ram.TWTRS = RamTWTRSTextBox.Text.Trim();
            ram.TWTRL = RamTWTRLTextBox.Text.Trim();
            ram.TCWL = RamTCWLTextBox.Text.Trim();

            ram.HasOverclock = RamHasOverclockCheckBox.IsChecked == true;

            ram.OcClock = ram.HasOverclock ? RamOcClockTextBox.Text.Trim() : "";
            ram.OcVoltage = ram.HasOverclock ? RamOcVoltageTextBox.Text.Trim() : "";
            ram.OcCL = ram.HasOverclock ? RamOcCLTextBox.Text.Trim() : "";
            ram.OcTRCD = ram.HasOverclock ? RamOcTRCDTextBox.Text.Trim() : "";
            ram.OcTRP = ram.HasOverclock ? RamOcTRPTextBox.Text.Trim() : "";
            ram.OcTRAS = ram.HasOverclock ? RamOcTRASTextBox.Text.Trim() : "";

            ram.PurchaseDate = PurchaseDatePicker.SelectedDate;

            if (decimal.TryParse(PurchasePriceTextBox.Text, out decimal price))
                ram.PurchasePrice = price;

            return ram;
        }

        private PcStorage CreateStorage()
        {
            PcStorage storage = new PcStorage();

            storage.Manufacturer = ManufacturerTextBox.Text.Trim();
            storage.Model = ModelTextBox.Text.Trim();

            storage.Capacity = StorageCapacityTextBox.Text.Trim();
            storage.DriveType = IsSelectedValue(StorageDriveTypeCombo.Text) ? StorageDriveTypeCombo.Text : "";
            storage.Interface = IsSelectedValue(StorageInterfaceCombo.Text) ? StorageInterfaceCombo.Text : "";
            storage.FormFactor = IsSelectedValue(StorageFormFactorCombo.Text) ? StorageFormFactorCombo.Text : "";

            storage.IsBootDrive = StorageBootDriveCheckBox.IsChecked == true;
            storage.Rpm = StorageRpmTextBox.Text.Trim();
            storage.Cache = StorageCacheTextBox.Text.Trim();
            storage.Controller = StorageControllerTextBox.Text.Trim();
            storage.NandType = StorageNandTextBox.Text.Trim();
            storage.HasDramCache = StorageDramCacheCheckBox.IsChecked == true;

            storage.PurchaseDate = PurchaseDatePicker.SelectedDate;

            if (decimal.TryParse(PurchasePriceTextBox.Text, out decimal price))
                storage.PurchasePrice = price;

            return storage;
        }

        private PcPowerSupply CreatePowerSupply()
        {
            PcPowerSupply psu = new PcPowerSupply();

            psu.Manufacturer = ManufacturerTextBox.Text.Trim();
            psu.Model = ModelTextBox.Text.Trim();

            psu.Power = PsuPowerTextBox.Text.Trim();
            psu.FormFactor = IsSelectedValue(PsuFormFactorCombo.Text) ? PsuFormFactorCombo.Text : "";
            psu.StandardVersion = IsSelectedValue(PsuStandardCombo.Text) ? PsuStandardCombo.Text : "";
            psu.Certificate = IsSelectedValue(PsuCertificateCombo.Text) ? PsuCertificateCombo.Text : "";
            psu.Modularity = IsSelectedValue(PsuModularityCombo.Text) ? PsuModularityCombo.Text : "";

            psu.HasOcp = PsuOcpCheckBox.IsChecked == true;
            psu.HasOvp = PsuOvpCheckBox.IsChecked == true;
            psu.HasUvp = PsuUvpCheckBox.IsChecked == true;
            psu.HasOpp = PsuOppCheckBox.IsChecked == true;
            psu.HasOtp = PsuOtpCheckBox.IsChecked == true;
            psu.HasScp = PsuScpCheckBox.IsChecked == true;
            psu.HasSip = PsuSipCheckBox.IsChecked == true;
            psu.HasNlo = PsuNloCheckBox.IsChecked == true;

            psu.HasPowerRails = PsuHasRailsCheckBox.IsChecked == true;

            psu.Rail12V1 = psu.HasPowerRails ? PsuRail12V1TextBox.Text.Trim() : "";
            psu.Rail12V2 = psu.HasPowerRails ? PsuRail12V2TextBox.Text.Trim() : "";
            psu.Rail12V3 = psu.HasPowerRails ? PsuRail12V3TextBox.Text.Trim() : "";
            psu.Rail12V4 = psu.HasPowerRails ? PsuRail12V4TextBox.Text.Trim() : "";
            psu.Rail5V = psu.HasPowerRails ? PsuRail5VTextBox.Text.Trim() : "";
            psu.Rail33V = psu.HasPowerRails ? PsuRail33VTextBox.Text.Trim() : "";

            psu.PurchaseDate = PurchaseDatePicker.SelectedDate;

            if (decimal.TryParse(PurchasePriceTextBox.Text, out decimal price))
                psu.PurchasePrice = price;

            return psu;
        }

        private PcCase CreateCase()
        {
            PcCase pcCase = new PcCase();

            pcCase.Manufacturer = ManufacturerTextBox.Text.Trim();
            pcCase.Model = ModelTextBox.Text.Trim();

            pcCase.CaseType = IsSelectedValue(CaseTypeCombo.Text) ? CaseTypeCombo.Text : "";
            pcCase.Color = IsSelectedValue(CaseColorCombo.Text) ? CaseColorCombo.Text : "";

            pcCase.HasSideWindow = CaseSideWindowCheckBox.IsChecked == true;
            pcCase.HasTemperedGlass = CaseTemperedGlassCheckBox.IsChecked == true;
            pcCase.HasRgb = CaseRgbCheckBox.IsChecked == true;
            pcCase.HasVerticalGpuMount = CaseVerticalGpuCheckBox.IsChecked == true;
            pcCase.HasHotSwap = CaseHotSwapCheckBox.IsChecked == true;
            pcCase.HasSoundDamping = CaseSoundDampingCheckBox.IsChecked == true;

            pcCase.MaxMotherboardSize = IsSelectedValue(CaseMaxMotherboardCombo.Text) ? CaseMaxMotherboardCombo.Text : "";
            pcCase.MaxGpuLength = CaseMaxGpuLengthTextBox.Text.Trim();
            pcCase.MaxCpuCoolerHeight = CaseMaxCpuCoolerHeightTextBox.Text.Trim();

            pcCase.PurchaseDate = PurchaseDatePicker.SelectedDate;

            if (decimal.TryParse(PurchasePriceTextBox.Text, out decimal price))
                pcCase.PurchasePrice = price;

            return pcCase;
        }
    }
}