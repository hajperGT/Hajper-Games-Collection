using System;
using System.Windows;
using HGC.Models;

namespace HGC
{
    public partial class PcCpuWindow : Window
    {
        public PcCpu? ResultCpu { get; private set; }

        private readonly PcCpu? editingCpu;

        private readonly string currentLanguage;

        public PcCpuWindow(string language)
        {
            InitializeComponent();

            currentLanguage = language;

            ApplyLanguage();
            LoadManufacturers();
        }

        public PcCpuWindow(
        string language,
        PcCpu cpu)
        : this(language)
        {
            editingCpu = cpu; 
            CpuManufacturerCombo.Text = cpu.Manufacturer;
            CpuModelTextBox.Text = cpu.Model;

            CpuHasSerialCheckBox.IsChecked =
                cpu.HasSerialNumber;

            CpuSerialTextBox.Text =
                cpu.SerialNumber;

            CpuHasOverclockCheckBox.IsChecked =
                cpu.HasOverclock;

            CpuClockTextBox.Text =
                cpu.CurrentClock;

            CpuVoltageTextBox.Text =
                cpu.CurrentVoltage;

            CpuPurchaseDatePicker.SelectedDate =
                cpu.PurchaseDate;

            CpuPurchasePriceTextBox.Text =
                cpu.PurchasePrice?.ToString() ?? "";
        }

        private void ApplyLanguage()
        {
            WindowTitleText.Text =
                currentLanguage == "pl"
                    ? "Dodaj procesor"
                    : "Add CPU";

            CpuBasicTitle.Text =
                currentLanguage == "pl"
                    ? "Podstawowe informacje"
                    : "Basic information";

            CpuManufacturerLabel.Text =
                currentLanguage == "pl"
                    ? "Producent"
                    : "Manufacturer";

            CpuModelLabel.Text =
                currentLanguage == "pl"
                    ? "Model"
                    : "Model";

            CpuHasSerialCheckBox.Content =
                currentLanguage == "pl"
                    ? "Dodaj numer seryjny"
                    : "Add serial number";

            CpuSerialLabel.Text =
                currentLanguage == "pl"
                    ? "Numer seryjny"
                    : "Serial number";

            CpuHasOverclockCheckBox.Content =
                currentLanguage == "pl"
                    ? "Dodaj podkręcenie"
                    : "Add overclock";

            CpuClockLabel.Text =
                currentLanguage == "pl"
                    ? "Aktualny zegar"
                    : "Current clock";

            CpuVoltageLabel.Text =
                currentLanguage == "pl"
                    ? "Aktualne napięcie"
                    : "Current voltage";

            CpuPurchaseTitle.Text =
                currentLanguage == "pl"
                    ? "Zakup"
                    : "Purchase";

            CpuPurchaseDateLabel.Text =
                currentLanguage == "pl"
                    ? "Data zakupu"
                    : "Purchase date";

            CpuPurchasePriceLabel.Text =
                currentLanguage == "pl"
                    ? "Cena zakupu"
                    : "Purchase price";

            CancelButton.Content =
                currentLanguage == "pl"
                    ? "Anuluj"
                    : "Cancel";

            SaveButton.Content =
                currentLanguage == "pl"
                    ? "Zapisz"
                    : "Save";
        }

        private void LoadManufacturers()
        {
            CpuManufacturerCombo.Items.Clear();

            CpuManufacturerCombo.Items.Add("Intel");
            CpuManufacturerCombo.Items.Add("AMD");
            CpuManufacturerCombo.Items.Add("IBM");
            CpuManufacturerCombo.Items.Add("Cyrix");
            CpuManufacturerCombo.Items.Add("VIA");
            CpuManufacturerCombo.Items.Add("Motorola");
            CpuManufacturerCombo.Items.Add(
                currentLanguage == "pl" ? "Inny" : "Other");

            CpuManufacturerCombo.SelectedIndex = 0;
        }

        private void CpuHasSerialCheckBox_Checked(
            object sender,
            RoutedEventArgs e)
        {
            CpuSerialPanel.Visibility =
                CpuHasSerialCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void CpuHasOverclockCheckBox_Checked(
            object sender,
            RoutedEventArgs e)
        {
            CpuOverclockPanel.Visibility =
                CpuHasOverclockCheckBox.IsChecked == true
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
            if (string.IsNullOrWhiteSpace(CpuModelTextBox.Text))
            {
                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? "Podaj model procesora."
                        : "Enter CPU model.",
                    "HGC",
                    HgcMessageBoxType.Information,
                    currentLanguage);

                return;
            }

            PcCpu cpu = editingCpu ?? new PcCpu();

            cpu.Manufacturer =
                CpuManufacturerCombo.Text;

            cpu.Model =
                CpuModelTextBox.Text.Trim();

            cpu.HasSerialNumber =
                CpuHasSerialCheckBox.IsChecked == true;

            cpu.SerialNumber =
                CpuHasSerialCheckBox.IsChecked == true
                    ? CpuSerialTextBox.Text.Trim()
                    : "";

            cpu.HasOverclock =
                CpuHasOverclockCheckBox.IsChecked == true;

            cpu.CurrentClock =
                CpuHasOverclockCheckBox.IsChecked == true
                    ? CpuClockTextBox.Text.Trim()
                    : "";

            cpu.CurrentVoltage =
                CpuHasOverclockCheckBox.IsChecked == true
                    ? CpuVoltageTextBox.Text.Trim()
                    : "";

            cpu.PurchaseDate =
                CpuPurchaseDatePicker.SelectedDate;

            if (decimal.TryParse(
                    CpuPurchasePriceTextBox.Text,
                    out decimal price))
            {
                cpu.PurchasePrice = price;
            }

            ResultCpu = cpu;

            DialogResult = true;
            Close();
        }
    }
}