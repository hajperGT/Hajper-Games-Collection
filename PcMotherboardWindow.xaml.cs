using System;
using System.Windows;
using HGC.Models;

namespace HGC
{
    public partial class PcMotherboardWindow : Window
    {
        public PcMotherboard? ResultMotherboard { get; private set; }

        private readonly string currentLanguage;
        private readonly PcMotherboard? editingMotherboard;

        public PcMotherboardWindow(string language)
        {
            InitializeComponent();

            currentLanguage = language;

            ApplyLanguage();            
        }

        public PcMotherboardWindow(
            string language,
            PcMotherboard motherboard)
            : this(language)
        {
            editingMotherboard = motherboard;

            BoardManufacturerTextBox.Text = motherboard.Manufacturer;
            BoardModelTextBox.Text = motherboard.Model;

            BoardHasChipsetCheckBox.IsChecked = motherboard.HasChipset;
            BoardChipsetTextBox.Text = motherboard.Chipset;

            BoardHasBiosCheckBox.IsChecked = motherboard.HasBiosVersion;
            BoardBiosTextBox.Text = motherboard.BiosVersion;

            BoardHasRevisionCheckBox.IsChecked = motherboard.HasRevision;
            BoardRevisionTextBox.Text = motherboard.Revision;

            BoardHasSerialCheckBox.IsChecked = motherboard.HasSerialNumber;
            BoardSerialTextBox.Text = motherboard.SerialNumber;

            BoardPurchaseDatePicker.SelectedDate = motherboard.PurchaseDate;

            BoardPurchasePriceTextBox.Text =
                motherboard.PurchasePrice?.ToString() ?? "";
        }

        private void ApplyLanguage()
        {
            WindowTitleText.Text =
                currentLanguage == "pl"
                    ? "Dodaj płytę główną"
                    : "Add motherboard";

            BoardBasicTitle.Text =
                currentLanguage == "pl"
                    ? "Podstawowe informacje"
                    : "Basic information";

            BoardManufacturerLabel.Text =
                currentLanguage == "pl" ? "Producent" : "Manufacturer";

            BoardModelLabel.Text =
                currentLanguage == "pl" ? "Model" : "Model";

            BoardHasChipsetCheckBox.Content =
                currentLanguage == "pl" ? "Dodaj chipset" : "Add chipset";

            BoardChipsetLabel.Text =
                currentLanguage == "pl" ? "Chipset" : "Chipset";

            BoardHasBiosCheckBox.Content =
                currentLanguage == "pl" ? "Dodaj wersję BIOS" : "Add BIOS version";

            BoardBiosLabel.Text =
                currentLanguage == "pl" ? "Wersja BIOS" : "BIOS version";

            BoardHasRevisionCheckBox.Content =
                currentLanguage == "pl" ? "Dodaj rewizję" : "Add revision";

            BoardRevisionLabel.Text =
                currentLanguage == "pl" ? "Rewizja" : "Revision";

            BoardHasSerialCheckBox.Content =
                currentLanguage == "pl" ? "Dodaj numer seryjny" : "Add serial number";

            BoardSerialLabel.Text =
                currentLanguage == "pl" ? "Numer seryjny" : "Serial number";

            BoardPurchaseTitle.Text =
                currentLanguage == "pl" ? "Zakup" : "Purchase";

            BoardPurchaseDateLabel.Text =
                currentLanguage == "pl" ? "Data zakupu" : "Purchase date";

            BoardPurchasePriceLabel.Text =
                currentLanguage == "pl" ? "Cena zakupu" : "Purchase price";

            CancelButton.Content =
                currentLanguage == "pl" ? "Anuluj" : "Cancel";

            SaveButton.Content =
                currentLanguage == "pl" ? "Zapisz" : "Save";
        }
              

        private void BoardHasChipsetCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            BoardChipsetPanel.Visibility =
                BoardHasChipsetCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void BoardHasBiosCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            BoardBiosPanel.Visibility =
                BoardHasBiosCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void BoardHasRevisionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            BoardRevisionPanel.Visibility =
                BoardHasRevisionCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void BoardHasSerialCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            BoardSerialPanel.Visibility =
                BoardHasSerialCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BoardModelTextBox.Text))
            {
                HgcMessageBox.Show(
                    this,
                    currentLanguage == "pl"
                        ? "Podaj model płyty głównej."
                        : "Enter motherboard model.",
                    "HGC",
                    HgcMessageBoxType.Information,
                    currentLanguage);

                return;
            }

            PcMotherboard motherboard =
                editingMotherboard ?? new PcMotherboard();

            motherboard.Manufacturer = BoardManufacturerTextBox.Text.Trim();
            motherboard.Model = BoardModelTextBox.Text.Trim();

            motherboard.HasChipset =
                BoardHasChipsetCheckBox.IsChecked == true;

            motherboard.Chipset =
                BoardHasChipsetCheckBox.IsChecked == true
                    ? BoardChipsetTextBox.Text.Trim()
                    : "";

            motherboard.HasBiosVersion =
                BoardHasBiosCheckBox.IsChecked == true;

            motherboard.BiosVersion =
                BoardHasBiosCheckBox.IsChecked == true
                    ? BoardBiosTextBox.Text.Trim()
                    : "";

            motherboard.HasRevision =
                BoardHasRevisionCheckBox.IsChecked == true;

            motherboard.Revision =
                BoardHasRevisionCheckBox.IsChecked == true
                    ? BoardRevisionTextBox.Text.Trim()
                    : "";

            motherboard.HasSerialNumber =
                BoardHasSerialCheckBox.IsChecked == true;

            motherboard.SerialNumber =
                BoardHasSerialCheckBox.IsChecked == true
                    ? BoardSerialTextBox.Text.Trim()
                    : "";

            motherboard.PurchaseDate =
                BoardPurchaseDatePicker.SelectedDate;

            if (decimal.TryParse(
                    BoardPurchasePriceTextBox.Text,
                    out decimal price))
            {
                motherboard.PurchasePrice = price;
            }
            else
            {
                motherboard.PurchasePrice = null;
            }

            ResultMotherboard = motherboard;

            DialogResult = true;
            Close();
        }
    }
}