using HGC.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HGC
{
    public partial class PcCoolingWindow : Window
    {
        private readonly string currentLanguage;

        internal PcCooling? ResultCooling { get; private set; }

        public PcCoolingWindow(string language)
        {
            InitializeComponent();

            currentLanguage = language;

            ApplyLanguage();
            LoadCombos();

            StandardCoolingRadio.IsChecked = true;
            UpdateCoolingModePanels();
            ResetCpuAirDetails();

        }
        internal PcCoolingWindow(
        string language,
        PcCooling cooling)
        : this(language)
        {
            LoadCoolingForEdit(cooling);
        }

        private void ApplyLanguage()
        {
            WindowTitleText.Text =
                currentLanguage == "pl" ? "Dodaj chłodzenie" : "Add cooling";

            CoolingModeTitle.Text =
                currentLanguage == "pl" ? "Typ chłodzenia" : "Cooling type";

            StandardCoolingRadio.Content =
                currentLanguage == "pl" ? "Standardowe chłodzenie" : "Standard Cooling";

            CustomWaterLoopRadio.Content =
                currentLanguage == "pl" ? "Własne chłodzenie wodne" : "Custom Water Cooling";

            StandardCoolingTitle.Text =
                currentLanguage == "pl" ? "Standardowe chłodzenie" : "Standard Cooling";

            CustomWaterLoopTitle.Text =
                currentLanguage == "pl" ? "Własne chłodzenie wodne" : "Custom Water Cooling";

            CpuCoolingTitle.Text =
                currentLanguage == "pl" ? "Chłodzenie CPU" : "CPU Cooling";

            CpuCoolingTypeLabel.Text =
                currentLanguage == "pl" ? "Typ chłodzenia" : "Cooling type";

            CpuOtherCoolingTypeLabel.Text =
                currentLanguage == "pl" ? "Opis typu chłodzenia" : "Cooling type description";

            HasRadiatorDetailsCheckBox.Content =
                currentLanguage == "pl" ? "Dodaj dane radiatora" : "Add radiator details";

            RadiatorTypeLabel.Text =
                currentLanguage == "pl" ? "Rodzaj radiatora" : "Radiator Type";

            OtherRadiatorTypeLabel.Text =
                currentLanguage == "pl" ? "Opis radiatora" : "Radiator Description";

            RadiatorPrimaryColorLabel.Text =
                currentLanguage == "pl" ? "Kolor główny radiatora" : "Radiator Primary Color";

            RadiatorSecondaryColorLabel.Text =
                currentLanguage == "pl" ? "Kolor dodatkowy radiatora" : "Radiator Secondary Color";

            HasHeatpipeCountCheckBox.Content =
                currentLanguage == "pl" ? "Podaj liczbę heatpipe" : "Specify heatpipe count";

            HeatpipeCountLabel.Text =
                currentLanguage == "pl" ? "Liczba heatpipe" : "Heatpipe count";

            HasPeltierModuleCheckBox.Content =
                currentLanguage == "pl" ? "Zastosowano ogniwo Peltiera" : "Has Peltier module";

            PeltierModelLabel.Text =
                currentLanguage == "pl" ? "Model ogniwa Peltiera" : "Peltier module model";

            HasFanDetailsCheckBox.Content =
                currentLanguage == "pl" ? "Dodaj wentylatory" : "Add fans";

            FanCountLabel.Text =
                currentLanguage == "pl" ? "Liczba wentylatorów" : "Fan count";

            GpuCoolingTitle.Text =
                currentLanguage == "pl" ? "Chłodzenie GPU" : "GPU Cooling";

            GpuCoolingTypeLabel.Text =
                currentLanguage == "pl" ? "Typ chłodzenia" : "Cooling type";

            GpuStockTypeLabel.Text =
                currentLanguage == "pl" ? "Rodzaj stock" : "Stock type";

            GpuCustomDescriptionLabel.Text =
                currentLanguage == "pl" ? "Opis chłodzenia" : "Cooling description";

            CancelButton.Content =
                currentLanguage == "pl" ? "Anuluj" : "Cancel";

            SaveButton.Content =
                currentLanguage == "pl" ? "Zapisz" : "Save";

            MotherboardCoolingTitle.Text =
                currentLanguage == "pl" ? "Chłodzenie płyty głównej" : "Motherboard Cooling";

            MotherboardCoolingTypeLabel.Text =
                currentLanguage == "pl"
                    ? "Typ chłodzenia"
                    : "Cooling type";

            MotherboardStockTypeLabel.Text =
                currentLanguage == "pl"
                    ? "Rodzaj stock"
                    : "Stock type";

            MotherboardCustomDescriptionLabel.Text =
                currentLanguage == "pl"
                    ? "Opis chłodzenia"
                    : "Cooling description";

            RamCoolingTitle.Text =
                currentLanguage == "pl" ? "Chłodzenie RAM" : "RAM Cooling";

            RamCoolingTypeLabel.Text =
                currentLanguage == "pl" ? "Typ chłodzenia" : "Cooling type";

            RamStockTypeLabel.Text =
                currentLanguage == "pl" ? "Rodzaj stock" : "Stock type";

            RamCustomDescriptionLabel.Text =
                currentLanguage == "pl" ? "Opis chłodzenia" : "Cooling description";

            StorageCoolingTitle.Text =
                currentLanguage == "pl"
                    ? "Chłodzenie dysków"
                    : "Storage Cooling";

            StorageCoolingTypeLabel.Text =
                currentLanguage == "pl"
                    ? "Typ chłodzenia"
                    : "Cooling type";

            StorageStockTypeLabel.Text =
                currentLanguage == "pl"
                    ? "Rodzaj stock"
                    : "Stock type";

            StorageCustomDescriptionLabel.Text =
                currentLanguage == "pl"
                    ? "Opis chłodzenia"
                    : "Cooling description";

            PsuCoolingTitle.Text =
                currentLanguage == "pl" ? "Chłodzenie zasilacza" : "PSU Cooling";

            PsuCoolingTypeLabel.Text =
                currentLanguage == "pl" ? "Typ chłodzenia" : "Cooling type";

            PsuStockTypeLabel.Text =
                currentLanguage == "pl" ? "Rodzaj stock" : "Stock type";

            PsuHasFanDetailsCheckBox.Content =
                currentLanguage == "pl" ? "Dodaj dane wentylatora" : "Add fan details";

            PsuFanCountLabel.Text =
                currentLanguage == "pl" ? "Liczba wentylatorów" : "Fan count";

            PsuFanSizeLabel.Text =
                currentLanguage == "pl" ? "Rozmiar wentylatora" : "Fan size";

            PsuFanPrimaryColorLabel.Text =
                currentLanguage == "pl" ? "Kolor główny" : "Primary color";

            PsuFanSecondaryColorLabel.Text =
                currentLanguage == "pl" ? "Kolor dodatkowy" : "Secondary color";

            PsuFanRgbLabel.Text = "RGB";

            PsuFanReverseCheckBox.Content =
                currentLanguage == "pl" ? "Odwrócony ciąg" : "Reverse airflow";

            PsuCustomDescriptionLabel.Text =
                currentLanguage == "pl" ? "Opis chłodzenia" : "Cooling description";

            CaseCoolingTitle.Text =
                currentLanguage == "pl"
                    ? "Chłodzenie obudowy"
                    : "Case Cooling";

            CaseFrontCheckBox.Content =
                currentLanguage == "pl"
                    ? "Przód"
                    : "Front";

            CaseLeftCheckBox.Content =
                currentLanguage == "pl"
                    ? "Lewy bok"
                    : "Left Side";

            CaseRightCheckBox.Content =
                currentLanguage == "pl"
                    ? "Prawy bok"
                    : "Right Side";

            CaseTopCheckBox.Content =
                currentLanguage == "pl"
                    ? "Góra"
                    : "Top";

            CaseBottomCheckBox.Content =
                currentLanguage == "pl"
                    ? "Dół"
                    : "Bottom";

            CaseRearCheckBox.Content =
                currentLanguage == "pl"
                    ? "Tył"
                    : "Rear";

            CaseFrontFanCountLabel.Text =
                currentLanguage == "pl" ? "Liczba wentylatorów" : "Fan count";

            CaseLeftFanCountLabel.Text =
                currentLanguage == "pl" ? "Liczba wentylatorów" : "Fan count";

            CaseRightFanCountLabel.Text =
                currentLanguage == "pl" ? "Liczba wentylatorów" : "Fan count";

            CaseTopFanCountLabel.Text =
                currentLanguage == "pl" ? "Liczba wentylatorów" : "Fan count";

            CaseBottomFanCountLabel.Text =
                currentLanguage == "pl" ? "Liczba wentylatorów" : "Fan count";

            CaseRearFanCountLabel.Text =
                currentLanguage == "pl" ? "Liczba wentylatorów" : "Fan count";

            WaterSecondaryCoolingTitle.Text =
                currentLanguage == "pl" ? "Pozostałe komponenty" : "Other components";

            WaterRamCoolingLabel.Text = "RAM";

            WaterStorageCoolingLabel.Text =
                currentLanguage == "pl" ? "Dyski" : "Storage";

            WaterPsuCoolingLabel.Text =
                currentLanguage == "pl" ? "Zasilacz" : "PSU";

            // ================= CUSTOM WATER COOLING - WATER BLOCKS =================

            WaterBlocksTitle.Text =
                currentLanguage == "pl"
                    ? "Bloki wodne"
                    : "Water blocks";

            WaterCpuBlockCheckBox.Content =
                currentLanguage == "pl"
                    ? "Blok CPU"
                    : "CPU Block";

            WaterGpuBlockCheckBox.Content =
                currentLanguage == "pl"
                    ? "Blok GPU"
                    : "GPU Block";

            WaterMotherboardBlockCheckBox.Content =
                currentLanguage == "pl"
                    ? "Blok płyty głównej"
                    : "Motherboard Block";

            WaterCpuBlockFittingsCheckBox.Content =
                currentLanguage == "pl"
                    ? "Dodaj złączki"
                    : "Add fittings";

            WaterGpuBlockFittingsCheckBox.Content =
                currentLanguage == "pl"
                    ? "Dodaj złączki"
                    : "Add fittings";

            WaterMotherboardBlockFittingsCheckBox.Content =
                currentLanguage == "pl"
                    ? "Dodaj złączki"
                    : "Add fittings";

            WaterCpuBlockManufacturerLabel.Text =
    currentLanguage == "pl" ? "Producent" : "Manufacturer";

            WaterCpuBlockModelLabel.Text =
                currentLanguage == "pl" ? "Model" : "Model";

            WaterCpuBlockFittingsManufacturerLabel.Text =
                currentLanguage == "pl" ? "Producent złączek" : "Fittings manufacturer";

            WaterCpuBlockFittingsModelLabel.Text =
                currentLanguage == "pl" ? "Model złączek" : "Fittings model";

            WaterCpuBlockFittingsCountLabel.Text =
                currentLanguage == "pl" ? "Ilość" : "Quantity";

            WaterGpuBlockManufacturerLabel.Text =
                currentLanguage == "pl" ? "Producent" : "Manufacturer";

            WaterGpuBlockModelLabel.Text =
                currentLanguage == "pl" ? "Model" : "Model";

            WaterGpuBlockFittingsManufacturerLabel.Text =
                currentLanguage == "pl" ? "Producent złączek" : "Fittings manufacturer";

            WaterGpuBlockFittingsModelLabel.Text =
                currentLanguage == "pl" ? "Model złączek" : "Fittings model";

            WaterGpuBlockFittingsCountLabel.Text =
                currentLanguage == "pl" ? "Ilość" : "Quantity";

            WaterMotherboardBlockManufacturerLabel.Text =
                currentLanguage == "pl" ? "Producent" : "Manufacturer";

            WaterMotherboardBlockModelLabel.Text =
                currentLanguage == "pl" ? "Model" : "Model";

            WaterMotherboardBlockFittingsManufacturerLabel.Text =
                currentLanguage == "pl" ? "Producent złączek" : "Fittings manufacturer";

            WaterMotherboardBlockFittingsModelLabel.Text =
                currentLanguage == "pl" ? "Model złączek" : "Fittings model";

            WaterMotherboardBlockFittingsCountLabel.Text =
                currentLanguage == "pl" ? "Ilość" : "Quantity";

            WaterRamBlockManufacturerLabel.Text =
                currentLanguage == "pl" ? "Producent bloku" : "Block manufacturer";

            WaterRamBlockModelLabel.Text =
                currentLanguage == "pl" ? "Model bloku" : "Block model";

            WaterStorageBlockManufacturerLabel.Text =
                currentLanguage == "pl" ? "Producent bloku" : "Block manufacturer";

            WaterStorageBlockModelLabel.Text =
                currentLanguage == "pl" ? "Model bloku" : "Block model";

            WaterPsuBlockManufacturerLabel.Text =
                currentLanguage == "pl" ? "Producent bloku" : "Block manufacturer";

            WaterPsuBlockModelLabel.Text =
                currentLanguage == "pl" ? "Model bloku" : "Block model";

            HasAioNotesCheckBox.Content =
                currentLanguage == "pl" ? "Dodaj informacje" : "Add information";

            AioNotesLabel.Text =
                currentLanguage == "pl" ? "Informacje" : "Information";
        }
        private void LoadCoolingForEdit(PcCooling cooling)
        {
            if (cooling == null)
                return;

            if (cooling.CoolingMode == "CustomWaterLoop")
            {
                CustomWaterLoopRadio.IsChecked = true;
            }
            else
            {
                StandardCoolingRadio.IsChecked = true;
            }

            UpdateCoolingModePanels();

            // ================= STANDARD =================

            SetCpuCoolingType(cooling.StandardCooling.CpuCooling.CoolingType);
            CpuOtherCoolingTypeTextBox.Text =
                cooling.StandardCooling.CpuCooling.OtherCoolingType ?? "";

            if (cooling.StandardCooling.CpuCooling.CoolingType == "Air")
            {
                CpuAirPanel.Visibility = Visibility.Visible;

                var air = cooling.StandardCooling.CpuCooling.AirCooling;

                bool hasRadiator =
                    !string.IsNullOrWhiteSpace(air.RadiatorType) ||
                    !string.IsNullOrWhiteSpace(air.RadiatorPrimaryColor) ||
                    !string.IsNullOrWhiteSpace(air.RadiatorSecondaryColor);

                HasRadiatorDetailsCheckBox.IsChecked = hasRadiator;
                RadiatorDetailsPanel.Visibility =
                    hasRadiator ? Visibility.Visible : Visibility.Collapsed;

                if (hasRadiator)
                {
                    SetRadiatorType(air.RadiatorType);
                    OtherRadiatorTypeTextBox.Text = air.OtherRadiatorType ?? "";
                    RadiatorPrimaryColorTextBox.Text = air.RadiatorPrimaryColor ?? "";
                    RadiatorSecondaryColorTextBox.Text = air.RadiatorSecondaryColor ?? "";
                }

                HasHeatpipeCountCheckBox.IsChecked = air.HasHeatpipeCount;
                HeatpipeCountPanel.Visibility =
                    air.HasHeatpipeCount ? Visibility.Visible : Visibility.Collapsed;

                if (air.HeatpipeCount != null)
                    HeatpipeCountCombo.Text = air.HeatpipeCount.Value.ToString();

                HasPeltierModuleCheckBox.IsChecked = air.HasPeltierModule;
                PeltierModulePanel.Visibility =
                    air.HasPeltierModule ? Visibility.Visible : Visibility.Collapsed;

                PeltierModelTextBox.Text = air.PeltierModel ?? "";

                HasFanDetailsCheckBox.IsChecked = air.Fans.HasFanDetails;
                FanDetailsPanel.Visibility =
                    air.Fans.HasFanDetails ? Visibility.Visible : Visibility.Collapsed;

                if (air.Fans.HasFanDetails)
                {
                    if (air.Fans.FanCount != null)
                        FanCountCombo.Text = air.Fans.FanCount.Value.ToString();

                    BuildFanEditors();
                    LoadFanSetIntoContainer(
                        FansContainer,
                        "Fan",
                        air.Fans.Fans);
                }
            }

            SetGpuCoolingType(cooling.StandardCooling.GpuCooling.CoolingType);
            SetGpuStockType(cooling.StandardCooling.GpuCooling.StockType);
            GpuCustomDescriptionTextBox.Text =
                cooling.StandardCooling.GpuCooling.CustomDescription ?? "";

            SetMotherboardCoolingType(cooling.StandardCooling.MotherboardCooling.CoolingType);
            SetMotherboardStockType(cooling.StandardCooling.MotherboardCooling.StockType);
            MotherboardCustomDescriptionTextBox.Text =
                cooling.StandardCooling.MotherboardCooling.CustomDescription ?? "";

            SetRamCoolingType(cooling.StandardCooling.RamCooling.CoolingType);
            SetRamStockType(cooling.StandardCooling.RamCooling.StockType);
            RamCustomDescriptionTextBox.Text =
                cooling.StandardCooling.RamCooling.CustomDescription ?? "";

            SetStorageCoolingType(cooling.StandardCooling.StorageCooling.CoolingType);
            SetStorageStockType(cooling.StandardCooling.StorageCooling.StockType);
            StorageCustomDescriptionTextBox.Text =
                cooling.StandardCooling.StorageCooling.CustomDescription ?? "";

            SetPsuCoolingType(cooling.StandardCooling.PsuCooling.CoolingType);
            SetPsuStockType(cooling.StandardCooling.PsuCooling.StockType);
            PsuCustomDescriptionTextBox.Text =
                cooling.StandardCooling.PsuCooling.CustomDescription ?? "";

            PsuHasFanDetailsCheckBox.IsChecked =
                cooling.StandardCooling.PsuCooling.Fans.HasFanDetails;

            PsuFanDetailsPanel.Visibility =
                cooling.StandardCooling.PsuCooling.Fans.HasFanDetails
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (cooling.StandardCooling.CpuCooling.CoolingType == "AIO")
            {
                CpuAioPanel.Visibility = Visibility.Visible;

                var aio = cooling.StandardCooling.CpuCooling.AioCooling;

                HasAioDetailsCheckBox.IsChecked = aio.HasAioDetails;
                AioDetailsPanel.Visibility =
                    aio.HasAioDetails ? Visibility.Visible : Visibility.Collapsed;

                AioManufacturerTextBox.Text = aio.Manufacturer ?? "";
                AioModelTextBox.Text = aio.Model ?? "";

                bool hasNotes =
                    !string.IsNullOrWhiteSpace(aio.Notes);

                HasAioNotesCheckBox.IsChecked = hasNotes;
                AioNotesPanel.Visibility =
                    hasNotes ? Visibility.Visible : Visibility.Collapsed;

                AioNotesTextBox.Text = aio.Notes ?? "";
            }

            if (cooling.StandardCooling.PsuCooling.Fans.HasFanDetails)
            {
                if (cooling.StandardCooling.PsuCooling.Fans.FanCount != null)
                    PsuFanCountCombo.Text =
                        cooling.StandardCooling.PsuCooling.Fans.FanCount.Value.ToString();

                PcCoolingFan? psuFan =
                    cooling.StandardCooling.PsuCooling.Fans.Fans.FirstOrDefault();

                if (psuFan != null)
                {
                    SetPsuFanSize(psuFan.Size);
                    PsuFanPrimaryColorTextBox.Text = psuFan.PrimaryColor ?? "";
                    PsuFanSecondaryColorTextBox.Text = psuFan.SecondaryColor ?? "";
                    SetPsuFanRgb(psuFan.RgbType);
                    PsuFanReverseCheckBox.IsChecked = psuFan.IsReverseFan;
                }
            }

            LoadCaseCoolingForEdit(cooling.StandardCooling.CaseCooling);

            // ================= CUSTOM WATER LOOP =================

            LoadWaterBlockForEdit(
                cooling.CustomWaterLoop.CpuBlock,
                WaterCpuBlockCheckBox,
                WaterCpuBlockPanel,
                WaterCpuBlockManufacturerTextBox,
                WaterCpuBlockModelTextBox);

            LoadWaterBlockForEdit(
                cooling.CustomWaterLoop.GpuBlock,
                WaterGpuBlockCheckBox,
                WaterGpuBlockPanel,
                WaterGpuBlockManufacturerTextBox,
                WaterGpuBlockModelTextBox);

            LoadWaterBlockForEdit(
                cooling.CustomWaterLoop.MotherboardBlock,
                WaterMotherboardBlockCheckBox,
                WaterMotherboardBlockPanel,
                WaterMotherboardBlockManufacturerTextBox,
                WaterMotherboardBlockModelTextBox);

            LoadSimpleWaterBlockForEdit(
                cooling.CustomWaterLoop.RamBlock,
                WaterRamCoolingModeCombo,
                WaterRamCoolingPanel,
                WaterRamBlockManufacturerTextBox,
                WaterRamBlockModelTextBox);

            LoadSimpleWaterBlockForEdit(
                cooling.CustomWaterLoop.StorageBlock,
                WaterStorageCoolingModeCombo,
                WaterStorageCoolingPanel,
                WaterStorageBlockManufacturerTextBox,
                WaterStorageBlockModelTextBox);

            LoadSimpleWaterBlockForEdit(
                cooling.CustomWaterLoop.PsuBlock,
                WaterPsuCoolingModeCombo,
                WaterPsuCoolingPanel,
                WaterPsuBlockManufacturerTextBox,
                WaterPsuBlockModelTextBox);
        }
        private void SelectComboText(ComboBox combo, string text)
        {
            if (combo == null || string.IsNullOrWhiteSpace(text))
                return;

            foreach (object item in combo.Items)
            {
                if (item?.ToString() == text)
                {
                    combo.SelectedItem = item;
                    return;
                }
            }
        }

        private void SetCpuCoolingType(string key)
        {
            SelectComboText(CpuCoolingTypeCombo, key switch
            {
                "NotSpecified" => currentLanguage == "pl" ? "Nie określono" : "Not specified",
                "AirBoxOem" => currentLanguage == "pl" ? "Powietrzne Box / OEM" : "Air Box / OEM",
                "Air" => currentLanguage == "pl" ? "Powietrzne" : "Air",
                "AIO" => "AIO",
                "Passive" => currentLanguage == "pl" ? "Pasywne" : "Passive",
                "Peltier" => "Peltier",
                "LN2" => "LN2",
                "Other" => currentLanguage == "pl" ? "Inne" : "Other",
                _ => ""
            });
        }

        private void SetRadiatorType(string key)
        {
            SelectComboText(RadiatorTypeCombo, key switch
            {
                "NotSpecified" => currentLanguage == "pl" ? "Nie określono" : "Not specified",
                "BoxOem" => "Box / OEM",
                "Tower" => currentLanguage == "pl" ? "Wieżowy" : "Tower",
                "DualTower" => currentLanguage == "pl" ? "Podwójna wieża" : "Dual Tower",
                "TripleTower" => currentLanguage == "pl" ? "Potrójna wieża" : "Triple Tower",
                "TopFlow" => currentLanguage == "pl" ? "Górny nawiew" : "Top Flow",
                "LowProfile" => currentLanguage == "pl" ? "Niski profil" : "Low Profile",
                "SlotCooler" => currentLanguage == "pl" ? "Slotowy" : "Slot Cooler",
                "Passive" => currentLanguage == "pl" ? "Pasywny" : "Passive",
                "Other" => currentLanguage == "pl" ? "Inny" : "Other",
                _ => ""
            });
        }

        private void SetGpuCoolingType(string key)
        {
            SelectComboText(GpuCoolingTypeCombo, CoolingTypeText(key));
        }

        private void SetMotherboardCoolingType(string key)
        {
            SelectComboText(MotherboardCoolingTypeCombo, CoolingTypeText(key));
        }

        private void SetRamCoolingType(string key)
        {
            SelectComboText(RamCoolingTypeCombo, CoolingTypeText(key));
        }

        private void SetStorageCoolingType(string key)
        {
            SelectComboText(StorageCoolingTypeCombo, CoolingTypeText(key));
        }

        private void SetPsuCoolingType(string key)
        {
            SelectComboText(PsuCoolingTypeCombo, CoolingTypeText(key));
        }

        private string CoolingTypeText(string key)
        {
            return key switch
            {
                "NotSpecified" => currentLanguage == "pl" ? "Nie określono" : "Not specified",
                "Stock" => "Stock",
                "Custom" => "Custom",
                _ => ""
            };
        }

        private void SetGpuStockType(string key)
        {
            SelectComboText(GpuStockTypeCombo, key switch
            {
                "NotSpecified" => currentLanguage == "pl" ? "Nie określono" : "Not specified",
                "Air" => currentLanguage == "pl" ? "Powietrzne" : "Air",
                "Passive" => currentLanguage == "pl" ? "Pasywne" : "Passive",
                "HybridAio" => currentLanguage == "pl" ? "Hybrydowe AIO" : "Hybrid AIO",
                _ => ""
            });
        }

        private void SetMotherboardStockType(string key)
        {
            SelectComboText(MotherboardStockTypeCombo, key switch
            {
                "NotSpecified" => currentLanguage == "pl" ? "Nie określono" : "Not specified",
                "Air" => currentLanguage == "pl" ? "Powietrzne" : "Air",
                "Passive" => currentLanguage == "pl" ? "Pasywne" : "Passive",
                _ => ""
            });
        }

        private void SetRamStockType(string key)
        {
            SelectComboText(RamStockTypeCombo, SimpleStockTypeText(key));
        }

        private void SetStorageStockType(string key)
        {
            SelectComboText(StorageStockTypeCombo, SimpleStockTypeText(key));
        }

        private void SetPsuStockType(string key)
        {
            SelectComboText(PsuStockTypeCombo, SimpleStockTypeText(key));
        }

        private string SimpleStockTypeText(string key)
        {
            return key switch
            {
                "NotSpecified" => currentLanguage == "pl" ? "Nie określono" : "Not specified",
                "None" => currentLanguage == "pl" ? "Brak" : "None",
                "Passive" => currentLanguage == "pl" ? "Pasywne" : "Passive",
                "Active" => currentLanguage == "pl" ? "Aktywne" : "Active",
                _ => ""
            };
        }

        private void SetPsuFanSize(string key)
        {
            SelectComboText(PsuFanSizeCombo, key switch
            {
                "40" => "40 mm",
                "60" => "60 mm",
                "80" => "80 mm",
                "92" => "92 mm",
                "120" => "120 mm",
                "135" => "135 mm",
                "140" => "140 mm",
                "Other" => currentLanguage == "pl" ? "Inny" : "Other",
                _ => currentLanguage == "pl" ? "Nie określono" : "Not specified"
            });
        }

        private void SetPsuFanRgb(string key)
        {
            SelectComboText(PsuFanRgbCombo, RgbText(key));
        }

        private string RgbText(string key)
        {
            return key switch
            {
                "NoRgb" => currentLanguage == "pl" ? "Brak RGB" : "No RGB",
                "RGB" => "RGB",
                "ARGB" => "ARGB",
                "FixedLed" => currentLanguage == "pl" ? "Stały LED" : "Fixed LED",
                "Other" => currentLanguage == "pl" ? "Inny" : "Other",
                _ => currentLanguage == "pl" ? "Brak RGB" : "No RGB"
            };
        }

        private string FanSizeText(string key)
        {
            return key switch
            {
                "80" => "80 mm",
                "92" => "92 mm",
                "100" => "100 mm",
                "120" => "120 mm",
                "135" => "135 mm",
                "140" => "140 mm",
                "180" => "180 mm",
                "200" => "200 mm",
                "Other" => currentLanguage == "pl" ? "Inny" : "Other",
                _ => currentLanguage == "pl" ? "Nie określono" : "Not specified"
            };
        }

        private void LoadFanSetIntoContainer(
            StackPanel container,
            string prefix,
            List<PcCoolingFan> fans)
        {
            for (int i = 0; i < fans.Count; i++)
            {
                int index = i + 1;
                PcCoolingFan fan = fans[i];

                ComboBox? sizeCombo =
                    FindVisualChildByName<ComboBox>(
                        container,
                        $"{prefix}SizeCombo{index}");

                TextBox? primaryColorTextBox =
                    FindVisualChildByName<TextBox>(
                        container,
                        $"{prefix}PrimaryColorTextBox{index}");

                TextBox? secondaryColorTextBox =
                    FindVisualChildByName<TextBox>(
                        container,
                        $"{prefix}SecondaryColorTextBox{index}");

                ComboBox? rgbCombo =
                    FindVisualChildByName<ComboBox>(
                        container,
                        $"{prefix}RgbCombo{index}");

                CheckBox? reverseCheckBox =
                    FindVisualChildByName<CheckBox>(
                        container,
                        $"{prefix}ReverseCheckBox{index}");

                if (sizeCombo != null)
                    SelectComboText(sizeCombo, FanSizeText(fan.Size));

                if (primaryColorTextBox != null)
                    primaryColorTextBox.Text = fan.PrimaryColor ?? "";

                if (secondaryColorTextBox != null)
                    secondaryColorTextBox.Text = fan.SecondaryColor ?? "";

                if (rgbCombo != null)
                    SelectComboText(rgbCombo, RgbText(fan.RgbType));

                if (reverseCheckBox != null)
                    reverseCheckBox.IsChecked = fan.IsReverseFan;
            }
        }

        private void LoadCaseCoolingForEdit(PcCaseCooling caseCooling)
        {
            if (caseCooling == null || caseCooling.FanGroups.Count == 0)
                return;

            HasCaseCoolingCheckBox.IsChecked = true;
            CaseCoolingDetailsPanel.Visibility = Visibility.Visible;

            foreach (PcCaseFanGroup group in caseCooling.FanGroups)
            {
                switch (group.Location)
                {
                    case "Front":
                        LoadCaseFanGroupForEdit(group, CaseFrontCheckBox, CaseFrontPanel, CaseFrontFanCountCombo, CaseFrontFansContainer, "CaseFront");
                        break;

                    case "LeftSide":
                        LoadCaseFanGroupForEdit(group, CaseLeftCheckBox, CaseLeftPanel, CaseLeftFanCountCombo, CaseLeftFansContainer, "CaseLeftSide");
                        break;

                    case "RightSide":
                        LoadCaseFanGroupForEdit(group, CaseRightCheckBox, CaseRightPanel, CaseRightFanCountCombo, CaseRightFansContainer, "CaseRightSide");
                        break;

                    case "Top":
                        LoadCaseFanGroupForEdit(group, CaseTopCheckBox, CaseTopPanel, CaseTopFanCountCombo, CaseTopFansContainer, "CaseTop");
                        break;

                    case "Bottom":
                        LoadCaseFanGroupForEdit(group, CaseBottomCheckBox, CaseBottomPanel, CaseBottomFanCountCombo, CaseBottomFansContainer, "CaseBottom");
                        break;

                    case "Rear":
                        LoadCaseFanGroupForEdit(group, CaseRearCheckBox, CaseRearPanel, CaseRearFanCountCombo, CaseRearFansContainer, "CaseRear");
                        break;
                }
            }
        }

        private void LoadCaseFanGroupForEdit(
            PcCaseFanGroup group,
            CheckBox checkBox,
            StackPanel panel,
            ComboBox fanCountCombo,
            StackPanel container,
            string prefix)
        {
            checkBox.IsChecked = true;
            panel.Visibility = Visibility.Visible;

            fanCountCombo.Text = group.Fans.Count.ToString();

            BuildCaseFanEditors(
                container,
                fanCountCombo,
                group.Location,
                group.Location,
                group.Location);

            LoadFanSetIntoContainer(container, prefix, group.Fans);
        }

        private void LoadWaterBlockForEdit(
            PcWaterLoopComponent component,
            CheckBox checkBox,
            StackPanel panel,
            TextBox manufacturerTextBox,
            TextBox modelTextBox)
        {
            if (component == null || !component.IsUsed)
                return;

            checkBox.IsChecked = true;
            panel.Visibility = Visibility.Visible;

            manufacturerTextBox.Text = component.Manufacturer ?? "";
            modelTextBox.Text = component.Model ?? "";
        }

        private void LoadSimpleWaterBlockForEdit(
            PcWaterLoopComponent component,
            ComboBox modeCombo,
            StackPanel panel,
            TextBox manufacturerTextBox,
            TextBox modelTextBox)
        {
            if (component == null || !component.IsUsed)
                return;

            modeCombo.SelectedIndex = 1;
            panel.Visibility = Visibility.Visible;

            manufacturerTextBox.Text = component.Manufacturer ?? "";
            modelTextBox.Text = component.Model ?? "";
        }
        private void LoadCombos()
        {
            LoadCpuCoolingTypes();
            LoadRadiatorTypes();
            LoadHeatpipeCounts();
            LoadFanCounts();
            LoadGpuCoolingTypes();
            LoadGpuStockTypes();
            LoadMotherboardCoolingTypes();
            LoadMotherboardStockTypes();
            LoadRamCoolingTypes();
            LoadRamStockTypes();
            LoadStorageCoolingTypes();
            LoadStorageStockTypes();
            LoadPsuCoolingTypes();
            LoadPsuStockTypes();
            LoadPsuFanCounts();
            LoadPsuFanSizes();
            LoadPsuFanRgbTypes();
            LoadCaseCoolingCombos();
            LoadWaterCoolingModes();
            LoadWaterFittingsCounts();
        }
        private void LoadWaterCoolingModes()
        {
            LoadWaterCoolingModeComboByName("WaterRamCoolingModeCombo");
            LoadWaterCoolingModeComboByName("WaterStorageCoolingModeCombo");
            LoadWaterCoolingModeComboByName("WaterPsuCoolingModeCombo");
        }

        private void LoadWaterCoolingModeComboByName(string comboName)
        {
            if (FindName(comboName) is not ComboBox combo)
                return;

            combo.Items.Clear();

            combo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");
            combo.Items.Add(currentLanguage == "pl" ? "Korzysta z chłodzenia wodnego" : "Uses water cooling");
            combo.Items.Add(currentLanguage == "pl" ? "Standardowe chłodzenie" : "Standard cooling");

            combo.SelectedIndex = 0;
        }
        private void LoadWaterFittingsCounts()
        {
            LoadWaterFittingsCountCombo(WaterCpuBlockFittingsCountCombo);
            LoadWaterFittingsCountCombo(WaterGpuBlockFittingsCountCombo);
            LoadWaterFittingsCountCombo(WaterMotherboardBlockFittingsCountCombo);
        }

        private void LoadWaterFittingsCountCombo(ComboBox combo)
        {
            if (combo == null)
                return;

            combo.Items.Clear();

            for (int i = 1; i <= 20; i++)
                combo.Items.Add(i.ToString());

            combo.SelectedIndex = 0;
        }
        private void WaterBlockFittingsCheckBox_Checked(
        object sender,
        RoutedEventArgs e)
        {
            WaterCpuBlockFittingsPanel.Visibility =
                WaterCpuBlockFittingsCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            WaterGpuBlockFittingsPanel.Visibility =
                WaterGpuBlockFittingsCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            WaterMotherboardBlockFittingsPanel.Visibility =
                WaterMotherboardBlockFittingsCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
        private void WaterCoolingModeCombo_SelectionChanged(
    object sender,
    SelectionChangedEventArgs e)
        {
            if (WaterRamCoolingPanel != null &&
                WaterRamCoolingModeCombo != null)
            {
                WaterRamCoolingPanel.Visibility =
                    WaterRamCoolingModeCombo.SelectedIndex == 1
                        ? Visibility.Visible
                        : Visibility.Collapsed;
            }

            if (WaterStorageCoolingPanel != null &&
                WaterStorageCoolingModeCombo != null)
            {
                WaterStorageCoolingPanel.Visibility =
                    WaterStorageCoolingModeCombo.SelectedIndex == 1
                        ? Visibility.Visible
                        : Visibility.Collapsed;
            }

            if (WaterPsuCoolingPanel != null &&
                WaterPsuCoolingModeCombo != null)
            {
                WaterPsuCoolingPanel.Visibility =
                    WaterPsuCoolingModeCombo.SelectedIndex == 1
                        ? Visibility.Visible
                        : Visibility.Collapsed;
            }
        }

        private void LoadWaterCoolingModeCombo(ComboBox combo)
        {
            combo.Items.Clear();

            combo.Items.Add(currentLanguage == "pl" ? "Wybierz" : "Select");
            combo.Items.Add(currentLanguage == "pl" ? "Korzysta z chłodzenia wodnego" : "Uses Water Cooling");
            combo.Items.Add(currentLanguage == "pl" ? "Standardowe chłodzenie" : "Standard Cooling");

            combo.SelectedIndex = 0;
        }
        private void LoadCaseCoolingCombos()
        {
            LoadCaseFanCountCombo(CaseFrontFanCountCombo);
            LoadCaseFanCountCombo(CaseLeftFanCountCombo);
            LoadCaseFanCountCombo(CaseRightFanCountCombo);
            LoadCaseFanCountCombo(CaseTopFanCountCombo);
            LoadCaseFanCountCombo(CaseBottomFanCountCombo);
            LoadCaseFanCountCombo(CaseRearFanCountCombo);
        }
        private void CaseLeftFanCountCombo_SelectionChanged(
    object sender,
    SelectionChangedEventArgs e)
        {
            BuildCaseFanEditors(
                CaseLeftFansContainer,
                CaseLeftFanCountCombo,
                "LeftSide",
                "Lewy bok",
                "Left Side");
        }

        private void CaseRightFanCountCombo_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            BuildCaseFanEditors(
                CaseRightFansContainer,
                CaseRightFanCountCombo,
                "RightSide",
                "Prawy bok",
                "Right Side");
        }

        private void CaseTopFanCountCombo_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            BuildCaseFanEditors(
                CaseTopFansContainer,
                CaseTopFanCountCombo,
                "Top",
                "Góra",
                "Top");
        }

        private void CaseBottomFanCountCombo_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            BuildCaseFanEditors(
                CaseBottomFansContainer,
                CaseBottomFanCountCombo,
                "Bottom",
                "Dół",
                "Bottom");
        }

        private void CaseRearFanCountCombo_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            BuildCaseFanEditors(
                CaseRearFansContainer,
                CaseRearFanCountCombo,
                "Rear",
                "Tył",
                "Rear");
        }
        private void LoadCaseFanCountCombo(ComboBox combo)
        {
            if (combo == null)
                return;

            combo.Items.Clear();

            combo.Items.Add(
                currentLanguage == "pl"
                    ? "Wybierz"
                    : "Select");

            combo.Items.Add("1");
            combo.Items.Add("2");
            combo.Items.Add("3");
            combo.Items.Add("4");

            combo.SelectedIndex = 0;
        }
        private void CaseFrontFanCountCombo_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
        {
            BuildCaseFrontFanEditors();
        }
        private void BuildCaseFrontFanEditors()
        {
            BuildCaseFanEditors(
                CaseFrontFansContainer,
                CaseFrontFanCountCombo,
                "Front",
                "Front",
                "Front");
        }
        private void BuildCaseFanEditors(
            StackPanel container,
            ComboBox fanCountCombo,
            string locationKey,
            string locationPl,
            string locationEn)
        {
            if (container == null)
                return;

            container.Children.Clear();

            string selectedCount =
            fanCountCombo.SelectedItem?.ToString() ?? "";

            if (!int.TryParse(selectedCount, out int fanCount))
            {
                return;
            }

            for (int i = 1; i <= fanCount; i++)
            {
                Border border = new Border
                {
                    BorderBrush = Brushes.DimGray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 0, 0, 10)
                };

                StackPanel panel = new StackPanel();

                panel.Children.Add(new TextBlock
                {
                    Text = currentLanguage == "pl"
                        ? $"Wentylator {locationPl} #{i}"
                        : $"{locationEn} Fan #{i}",
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                panel.Children.Add(new TextBlock
                {
                    Text = currentLanguage == "pl" ? "Rozmiar" : "Size",
                    Foreground = Brushes.White
                });

                ComboBox sizeCombo = new ComboBox
                {
                    Name = $"Case{locationKey}FanSizeCombo{i}",
                    Height = 30,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 4, 0, 10)
                };

                sizeCombo.Items.Add(currentLanguage == "pl" ? "Nie określono" : "Not specified");
                sizeCombo.Items.Add("80 mm");
                sizeCombo.Items.Add("92 mm");
                sizeCombo.Items.Add("100 mm");
                sizeCombo.Items.Add("120 mm");
                sizeCombo.Items.Add("140 mm");
                sizeCombo.Items.Add("180 mm");
                sizeCombo.Items.Add("200 mm");
                sizeCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");
                sizeCombo.SelectedIndex = 0;

                panel.Children.Add(sizeCombo);

                panel.Children.Add(new TextBlock
                {
                    Text = currentLanguage == "pl" ? "Kolor główny" : "Primary Color",
                    Foreground = Brushes.White
                });

                TextBox primaryColorTextBox = new TextBox
                {
                    Name = $"Case{locationKey}FanPrimaryColorTextBox{i}",
                    Height = 30,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 4, 0, 10)
                };

                panel.Children.Add(primaryColorTextBox);

                panel.Children.Add(new TextBlock
                {
                    Text = currentLanguage == "pl" ? "Kolor dodatkowy" : "Secondary Color",
                    Foreground = Brushes.White
                });

                TextBox secondaryColorTextBox = new TextBox
                {
                    Name = $"Case{locationKey}FanSecondaryColorTextBox{i}",
                    Height = 30,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 4, 0, 10)
                };

                panel.Children.Add(secondaryColorTextBox);

                panel.Children.Add(new TextBlock
                {
                    Text = "RGB",
                    Foreground = Brushes.White
                });

                ComboBox rgbCombo = new ComboBox
                {
                    Name = $"Case{locationKey}FanRgbCombo{i}",
                    Height = 30,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 4, 0, 10)
                };

                rgbCombo.Items.Add(currentLanguage == "pl" ? "Brak RGB" : "No RGB");
                rgbCombo.Items.Add("RGB");
                rgbCombo.Items.Add("ARGB");
                rgbCombo.Items.Add(currentLanguage == "pl" ? "Stały LED" : "Fixed LED");
                rgbCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");
                rgbCombo.SelectedIndex = 0;

                panel.Children.Add(rgbCombo);

                CheckBox reverseCheckBox = new CheckBox
                {
                    Name = $"Case{locationKey}FanReverseCheckBox{i}",
                    Content = currentLanguage == "pl"
                     ? "Odwrócony ciąg"
                     : "Reverse Fan",
                    Foreground = Brushes.White
                };

                panel.Children.Add(reverseCheckBox);

                border.Child = panel;
                container.Children.Add(border);
            }
        }
        private void LoadPsuCoolingTypes()
        {
            PsuCoolingTypeCombo.Items.Clear();

            PsuCoolingTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Nie określono" : "Not specified");

            PsuCoolingTypeCombo.Items.Add("Stock");
            PsuCoolingTypeCombo.Items.Add("Custom");

            PsuCoolingTypeCombo.SelectedIndex = 0;
        }

        private void LoadPsuStockTypes()
        {
            PsuStockTypeCombo.Items.Clear();

            PsuStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Nie określono" : "Not specified");

            PsuStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Brak" : "None");

            PsuStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Pasywne" : "Passive");

            PsuStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Aktywne" : "Active");

            PsuStockTypeCombo.SelectedIndex = 0;
        }

        private void LoadPsuFanCounts()
        {
            PsuFanCountCombo.Items.Clear();

            PsuFanCountCombo.Items.Add("1");
            PsuFanCountCombo.Items.Add("2");

            PsuFanCountCombo.SelectedIndex = 0;
        }

        private void LoadPsuFanSizes()
        {
            PsuFanSizeCombo.Items.Clear();

            PsuFanSizeCombo.Items.Add(
                currentLanguage == "pl" ? "Nie określono" : "Not specified");

            PsuFanSizeCombo.Items.Add("40 mm");
            PsuFanSizeCombo.Items.Add("60 mm");
            PsuFanSizeCombo.Items.Add("80 mm");
            PsuFanSizeCombo.Items.Add("92 mm");
            PsuFanSizeCombo.Items.Add("120 mm");
            PsuFanSizeCombo.Items.Add("135 mm");
            PsuFanSizeCombo.Items.Add("140 mm");

            PsuFanSizeCombo.Items.Add(
                currentLanguage == "pl" ? "Inny" : "Other");

            PsuFanSizeCombo.SelectedIndex = 0;
        }
        private void PsuCoolingTypeCombo_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
        {
            string selected =
                PsuCoolingTypeCombo.SelectedItem?.ToString() ?? "";

            bool isStock =
                selected == "Stock";

            bool isCustom =
                selected == "Custom";

            PsuStockPanel.Visibility =
                isStock ? Visibility.Visible : Visibility.Collapsed;

            PsuCustomPanel.Visibility =
                isCustom ? Visibility.Visible : Visibility.Collapsed;

            if (!isStock)
            {
                PsuStockTypeCombo.SelectedIndex = 0;
                PsuHasFanDetailsCheckBox.IsChecked = false;
                PsuFanDetailsPanel.Visibility = Visibility.Collapsed;
            }

            if (!isCustom)
                PsuCustomDescriptionTextBox.Text = "";
        }

        private void PsuHasFanDetailsCheckBox_Checked(
            object sender,
            RoutedEventArgs e)
        {
            PsuFanDetailsPanel.Visibility =
                PsuHasFanDetailsCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (PsuHasFanDetailsCheckBox.IsChecked != true)
            {
                PsuFanCountCombo.SelectedIndex = 0;
                PsuFanSizeCombo.SelectedIndex = 0;
                PsuFanPrimaryColorTextBox.Text = "";
                PsuFanSecondaryColorTextBox.Text = "";
                PsuFanRgbCombo.SelectedIndex = 0;
                PsuFanReverseCheckBox.IsChecked = false;
            }
        }
        private void LoadPsuFanRgbTypes()
        {
            PsuFanRgbCombo.Items.Clear();

            PsuFanRgbCombo.Items.Add(
                currentLanguage == "pl" ? "Brak RGB" : "No RGB");

            PsuFanRgbCombo.Items.Add("RGB");
            PsuFanRgbCombo.Items.Add("ARGB");

            PsuFanRgbCombo.Items.Add(
                currentLanguage == "pl" ? "Stały LED" : "Fixed LED");

            PsuFanRgbCombo.Items.Add(
                currentLanguage == "pl" ? "Inny" : "Other");

            PsuFanRgbCombo.SelectedIndex = 0;
        }
        private void WaterBlockCheckBox_Checked(
        object sender,
        RoutedEventArgs e)
        {
            WaterCpuBlockPanel.Visibility =
                WaterCpuBlockCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            WaterGpuBlockPanel.Visibility =
                WaterGpuBlockCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            WaterMotherboardBlockPanel.Visibility =
                WaterMotherboardBlockCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
        private void LoadStorageCoolingTypes()
        {
            StorageCoolingTypeCombo.Items.Clear();

            StorageCoolingTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Nie określono" : "Not specified");

            StorageCoolingTypeCombo.Items.Add("Stock");
            StorageCoolingTypeCombo.Items.Add("Custom");

            StorageCoolingTypeCombo.SelectedIndex = 0;
        }
        private void StorageCoolingTypeCombo_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
        {
            string selected =
                StorageCoolingTypeCombo.SelectedItem?.ToString() ?? "";

            bool isStock =
                selected == "Stock";

            bool isCustom =
                selected == "Custom";

            StorageStockPanel.Visibility =
                isStock ? Visibility.Visible : Visibility.Collapsed;

            StorageCustomPanel.Visibility =
                isCustom ? Visibility.Visible : Visibility.Collapsed;

            if (!isStock)
                StorageStockTypeCombo.SelectedIndex = 0;

            if (!isCustom)
                StorageCustomDescriptionTextBox.Text = "";
        }
        private void LoadStorageStockTypes()
        {
            StorageStockTypeCombo.Items.Clear();

            StorageStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Nie określono" : "Not specified");

            StorageStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Brak" : "None");

            StorageStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Pasywne" : "Passive");

            StorageStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Aktywne" : "Active");

            StorageStockTypeCombo.SelectedIndex = 0;
        }
        private void LoadRamCoolingTypes()
        {
            RamCoolingTypeCombo.Items.Clear();

            RamCoolingTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Nie określono" : "Not specified");

            RamCoolingTypeCombo.Items.Add("Stock");
            RamCoolingTypeCombo.Items.Add("Custom");

            RamCoolingTypeCombo.SelectedIndex = 0;
        }
        private void RamCoolingTypeCombo_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
        {
            string selected =
                RamCoolingTypeCombo.SelectedItem?.ToString() ?? "";

            bool isStock =
                selected == "Stock";

            bool isCustom =
                selected == "Custom";

            RamStockPanel.Visibility =
                isStock ? Visibility.Visible : Visibility.Collapsed;

            RamCustomPanel.Visibility =
                isCustom ? Visibility.Visible : Visibility.Collapsed;

            if (!isStock)
                RamStockTypeCombo.SelectedIndex = 0;

            if (!isCustom)
                RamCustomDescriptionTextBox.Text = "";
        }
        private void LoadRamStockTypes()
        {
            RamStockTypeCombo.Items.Clear();

            RamStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Nie określono" : "Not specified");

            RamStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Brak" : "None");

            RamStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Pasywne" : "Passive");

            RamStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Aktywne" : "Active");

            RamStockTypeCombo.SelectedIndex = 0;
        }
        private void LoadMotherboardCoolingTypes()
        {
            MotherboardCoolingTypeCombo.Items.Clear();

            MotherboardCoolingTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Nie określono" : "Not specified");

            MotherboardCoolingTypeCombo.Items.Add("Stock");
            MotherboardCoolingTypeCombo.Items.Add("Custom");

            MotherboardCoolingTypeCombo.SelectedIndex = 0;
        }
        private void MotherboardCoolingTypeCombo_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
        {
            string selected =
                MotherboardCoolingTypeCombo.SelectedItem?.ToString() ?? "";

            bool isStock =
                selected == "Stock";

            bool isCustom =
                selected == "Custom";

            MotherboardStockPanel.Visibility =
                isStock ? Visibility.Visible : Visibility.Collapsed;

            MotherboardCustomPanel.Visibility =
                isCustom ? Visibility.Visible : Visibility.Collapsed;

            if (!isStock)
                MotherboardStockTypeCombo.SelectedIndex = 0;

            if (!isCustom)
                MotherboardCustomDescriptionTextBox.Text = "";
        }

        private void LoadMotherboardStockTypes()
        {
            MotherboardStockTypeCombo.Items.Clear();

            MotherboardStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Nie określono" : "Not specified");

            MotherboardStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Powietrzne" : "Air");

            MotherboardStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Pasywne" : "Passive");

            MotherboardStockTypeCombo.SelectedIndex = 0;
        }
        private void LoadGpuCoolingTypes()
        {
            GpuCoolingTypeCombo.Items.Clear();

            GpuCoolingTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Nie określono" : "Not specified");

            GpuCoolingTypeCombo.Items.Add("Stock");
            GpuCoolingTypeCombo.Items.Add("Custom");

            GpuCoolingTypeCombo.SelectedIndex = 0;
        }

        private void LoadGpuStockTypes()
        {
            GpuStockTypeCombo.Items.Clear();

            GpuStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Nie określono" : "Not specified");

            GpuStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Powietrzne" : "Air");

            GpuStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Pasywne" : "Passive");

            GpuStockTypeCombo.Items.Add(
                currentLanguage == "pl" ? "Hybrydowe AIO" : "Hybrid AIO");

            GpuStockTypeCombo.SelectedIndex = 0;
        }
        private void GpuCoolingTypeCombo_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
        {
            string selected =
                GpuCoolingTypeCombo.SelectedItem?.ToString() ?? "";

            bool isStock =
                selected == "Stock";

            bool isCustom =
                selected == "Custom";

            GpuStockPanel.Visibility =
                isStock ? Visibility.Visible : Visibility.Collapsed;

            GpuCustomPanel.Visibility =
                isCustom ? Visibility.Visible : Visibility.Collapsed;

            if (!isStock)
                GpuStockTypeCombo.SelectedIndex = 0;

            if (!isCustom)
                GpuCustomDescriptionTextBox.Text = "";
        }

        private void LoadCpuCoolingTypes()
        {
            CpuCoolingTypeCombo.Items.Clear();

            CpuCoolingTypeCombo.Items.Add(currentLanguage == "pl" ? "Nie określono" : "Not specified");
            CpuCoolingTypeCombo.Items.Add(currentLanguage == "pl" ? "Powietrzne Box / OEM" : "Air Box / OEM");
            CpuCoolingTypeCombo.Items.Add(currentLanguage == "pl" ? "Powietrzne" : "Air");
            CpuCoolingTypeCombo.Items.Add("AIO");
            CpuCoolingTypeCombo.Items.Add(currentLanguage == "pl" ? "Pasywne" : "Passive");
            CpuCoolingTypeCombo.Items.Add("Peltier");
            CpuCoolingTypeCombo.Items.Add("LN2");
            CpuCoolingTypeCombo.Items.Add(currentLanguage == "pl" ? "Inne" : "Other");

            CpuCoolingTypeCombo.SelectedIndex = 0;
        }

        private void LoadRadiatorTypes()
        {
            RadiatorTypeCombo.Items.Clear();

            RadiatorTypeCombo.Items.Add(currentLanguage == "pl" ? "Nie określono" : "Not specified");
            RadiatorTypeCombo.Items.Add("Box / OEM");
            RadiatorTypeCombo.Items.Add(currentLanguage == "pl" ? "Wieżowy" : "Tower");
            RadiatorTypeCombo.Items.Add(currentLanguage == "pl" ? "Podwójna wieża" : "Dual Tower");
            RadiatorTypeCombo.Items.Add(currentLanguage == "pl" ? "Potrójna wieża" : "Triple Tower");
            RadiatorTypeCombo.Items.Add(currentLanguage == "pl" ? "Górny nawiew" : "Top Flow");
            RadiatorTypeCombo.Items.Add(currentLanguage == "pl" ? "Niski profil" : "Low Profile");
            RadiatorTypeCombo.Items.Add(currentLanguage == "pl" ? "Slotowy" : "Slot Cooler");
            RadiatorTypeCombo.Items.Add(currentLanguage == "pl" ? "Pasywny" : "Passive");
            RadiatorTypeCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");

            RadiatorTypeCombo.SelectedIndex = 0;
        }

        private void LoadHeatpipeCounts()
        {
            HeatpipeCountCombo.Items.Clear();

            for (int i = 1; i <= 10; i++)
                HeatpipeCountCombo.Items.Add(i.ToString());

            HeatpipeCountCombo.SelectedIndex = 0;
        }

        private void LoadFanCounts()
        {
            FanCountCombo.Items.Clear();

            FanCountCombo.Items.Add("1");
            FanCountCombo.Items.Add("2");
            FanCountCombo.Items.Add("3");
            FanCountCombo.Items.Add("4");

            FanCountCombo.SelectedIndex = 0;
        }

        private void CoolingModeRadio_Checked(object sender, RoutedEventArgs e)
        {
            UpdateCoolingModePanels();
        }

        private void UpdateCoolingModePanels()
        {
            if (StandardCoolingPanel == null ||
                CustomWaterLoopPanel == null)
                return;

            StandardCoolingPanel.Visibility =
                StandardCoolingRadio.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            CustomWaterLoopPanel.Visibility =
                CustomWaterLoopRadio.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void CpuCoolingTypeCombo_SelectionChanged(
     object sender,
     SelectionChangedEventArgs e)
        {
            UpdateCpuCoolingTypeFields();

            string selected =
                CpuCoolingTypeCombo.SelectedItem?.ToString() ?? "";

            bool isAir =
                selected == "Powietrzne" ||
                selected == "Air";

            bool isAio =
                selected == "AIO";

            CpuAirPanel.Visibility =
                isAir ? Visibility.Visible : Visibility.Collapsed;

            CpuAioPanel.Visibility =
                isAio ? Visibility.Visible : Visibility.Collapsed;

            if (!isAir)
                ResetCpuAirDetails();

            if (!isAio)
                ResetCpuAioDetails();
        }

        private void UpdateCpuCoolingTypeFields()
        {
            if (CpuOtherCoolingTypeLabel == null ||
                CpuOtherCoolingTypeTextBox == null)
                return;

            string selected =
                CpuCoolingTypeCombo.SelectedItem?.ToString() ?? "";

            bool isOther =
                selected == "Inne" ||
                selected == "Other";

            CpuOtherCoolingTypeLabel.Visibility =
                isOther ? Visibility.Visible : Visibility.Collapsed;

            CpuOtherCoolingTypeTextBox.Visibility =
                isOther ? Visibility.Visible : Visibility.Collapsed;

            if (!isOther)
                CpuOtherCoolingTypeTextBox.Text = "";
        }

        private void HasRadiatorDetailsCheckBox_Checked(
            object sender,
            RoutedEventArgs e)
        {
            RadiatorDetailsPanel.Visibility =
                HasRadiatorDetailsCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (HasRadiatorDetailsCheckBox.IsChecked != true)
                ResetRadiatorDetails();
        }

        private void RadiatorTypeCombo_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (OtherRadiatorTypeLabel == null ||
                OtherRadiatorTypeTextBox == null)
                return;

            string selected =
                RadiatorTypeCombo.SelectedItem?.ToString() ?? "";

            bool isOther =
                selected == "Inny" ||
                selected == "Other";

            OtherRadiatorTypeLabel.Visibility =
                isOther ? Visibility.Visible : Visibility.Collapsed;

            OtherRadiatorTypeTextBox.Visibility =
                isOther ? Visibility.Visible : Visibility.Collapsed;

            if (!isOther)
                OtherRadiatorTypeTextBox.Text = "";
        }

        private void HasHeatpipeCountCheckBox_Checked(
            object sender,
            RoutedEventArgs e)
        {
            HeatpipeCountPanel.Visibility =
                HasHeatpipeCountCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (HasHeatpipeCountCheckBox.IsChecked != true)
                HeatpipeCountCombo.SelectedIndex = 0;
        }

        private void HasPeltierModuleCheckBox_Checked(
            object sender,
            RoutedEventArgs e)
        {
            PeltierModulePanel.Visibility =
                HasPeltierModuleCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (HasPeltierModuleCheckBox.IsChecked != true)
                PeltierModelTextBox.Text = "";
        }

        private void HasFanDetailsCheckBox_Checked(
            object sender,
            RoutedEventArgs e)
        {
            FanDetailsPanel.Visibility =
                HasFanDetailsCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (HasFanDetailsCheckBox.IsChecked == true)
                BuildFanEditors();
            else
                ResetFanDetails();
        }

        private void FanCountCombo_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (HasFanDetailsCheckBox != null &&
                HasFanDetailsCheckBox.IsChecked == true)
            {
                BuildFanEditors();
            }
        }

        private void BuildFanEditors()
        {
            if (FansContainer == null)
                return;

            FansContainer.Children.Clear();

            if (!int.TryParse(
                    FanCountCombo.SelectedItem?.ToString(),
                    out int fanCount))
                return;

            for (int i = 1; i <= fanCount; i++)
            {
                Border border = new Border
                {
                    BorderBrush = Brushes.DimGray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 0, 0, 10)
                };

                StackPanel panel = new StackPanel();

                panel.Children.Add(new TextBlock
                {
                    Text = currentLanguage == "pl"
                        ? $"Wentylator #{i}"
                        : $"Fan #{i}",
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                panel.Children.Add(new TextBlock
                {
                    Text = currentLanguage == "pl"
                        ? "Rozmiar wentylatora"
                        : "Fan size",
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 6)
                });

                ComboBox sizeCombo = new ComboBox
                {
                    Name = $"FanSizeCombo{i}",
                    Height = 30,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Tag = i,
                    Margin = new Thickness(0, 0, 0, 10)
                };

                sizeCombo.Items.Add(currentLanguage == "pl" ? "Nie określono" : "Not specified");
                sizeCombo.Items.Add("80 mm");
                sizeCombo.Items.Add("92 mm");
                sizeCombo.Items.Add("100 mm");
                sizeCombo.Items.Add("120 mm");
                sizeCombo.Items.Add("135 mm");
                sizeCombo.Items.Add("140 mm");
                sizeCombo.Items.Add("180 mm");
                sizeCombo.Items.Add("200 mm");
                sizeCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");
                sizeCombo.SelectedIndex = 0;

                panel.Children.Add(sizeCombo);

                panel.Children.Add(new TextBlock
                {
                    Text = currentLanguage == "pl"
                        ? "Kolor główny"
                        : "Primary color",
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 6)
                });

                panel.Children.Add(new TextBox
                {
                    Name = $"FanPrimaryColorTextBox{i}",
                    Height = 30,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                panel.Children.Add(new TextBlock
                {
                    Text = currentLanguage == "pl"
                        ? "Kolor dodatkowy"
                        : "Secondary color",
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 6)
                });

                panel.Children.Add(new TextBox
                {
                    Name = $"FanSecondaryColorTextBox{i}",
                    Height = 30,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                panel.Children.Add(new TextBlock
                {
                    Text = "RGB",
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 6)
                });

                ComboBox rgbCombo = new ComboBox
                {
                    Name = $"FanRgbCombo{i}",
                    Height = 30,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 10)
                };

                rgbCombo.Items.Add(currentLanguage == "pl" ? "Brak RGB" : "No RGB");
                rgbCombo.Items.Add("RGB");
                rgbCombo.Items.Add("ARGB");
                rgbCombo.Items.Add(currentLanguage == "pl" ? "Stały LED" : "Fixed LED");
                rgbCombo.Items.Add(currentLanguage == "pl" ? "Inny" : "Other");
                rgbCombo.SelectedIndex = 0;

                panel.Children.Add(rgbCombo);

                panel.Children.Add(new CheckBox
                {
                    Name = $"FanReverseCheckBox{i}",
                    Content = currentLanguage == "pl"
                        ? "Odwrócony ciąg"
                        : "Reverse airflow",
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 0)
                });

                border.Child = panel;
                FansContainer.Children.Add(border);
            }
        }

        private void ResetCpuAirDetails()
        {
            if (CpuAirPanel == null)
                return;

            HasRadiatorDetailsCheckBox.IsChecked = false;
            HasHeatpipeCountCheckBox.IsChecked = false;
            HasPeltierModuleCheckBox.IsChecked = false;
            HasFanDetailsCheckBox.IsChecked = false;

            RadiatorDetailsPanel.Visibility = Visibility.Collapsed;
            HeatpipeCountPanel.Visibility = Visibility.Collapsed;
            PeltierModulePanel.Visibility = Visibility.Collapsed;
            FanDetailsPanel.Visibility = Visibility.Collapsed;

            ResetRadiatorDetails();

            HeatpipeCountCombo.SelectedIndex = 0;
            PeltierModelTextBox.Text = "";

            ResetFanDetails();
        }

        private void ResetCpuAioDetails()
        {
            if (CpuAioPanel == null)
                return;

            HasAioDetailsCheckBox.IsChecked = false;
            AioDetailsPanel.Visibility = Visibility.Collapsed;

            AioManufacturerTextBox.Text = "";
            AioModelTextBox.Text = "";

            HasAioNotesCheckBox.IsChecked = false;
            AioNotesPanel.Visibility = Visibility.Collapsed;
            AioNotesTextBox.Text = "";
        }
   
        private void ResetRadiatorDetails()
        {
            RadiatorTypeCombo.SelectedIndex = 0;
            OtherRadiatorTypeTextBox.Text = "";
            RadiatorPrimaryColorTextBox.Text = "";
            RadiatorSecondaryColorTextBox.Text = "";

            OtherRadiatorTypeLabel.Visibility = Visibility.Collapsed;
            OtherRadiatorTypeTextBox.Visibility = Visibility.Collapsed;
        }

        private void ResetFanDetails()
        {
            FanCountCombo.SelectedIndex = 0;
            FansContainer.Children.Clear();
        }

        private void CancelButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void HasAioDetailsCheckBox_Checked(
    object sender,
    RoutedEventArgs e)
        {
            AioDetailsPanel.Visibility =
                HasAioDetailsCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (HasAioDetailsCheckBox.IsChecked != true)
            {
                AioManufacturerTextBox.Text = "";
                AioModelTextBox.Text = "";

                HasAioNotesCheckBox.IsChecked = false;
                AioNotesPanel.Visibility = Visibility.Collapsed;
                AioNotesTextBox.Text = "";
            }
        }

        private void HasAioNotesCheckBox_Checked(
            object sender,
            RoutedEventArgs e)
        {
            AioNotesPanel.Visibility =
                HasAioNotesCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (HasAioNotesCheckBox.IsChecked != true)
                AioNotesTextBox.Text = "";
        }
        private void SaveButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            PcCooling cooling = new PcCooling();

            cooling.CoolingMode =
                CustomWaterLoopRadio.IsChecked == true
                    ? "CustomWaterLoop"
                    : "StandardCooling";

            cooling.StandardCooling.CpuCooling.CoolingType =
                GetCpuCoolingTypeKey();

            cooling.StandardCooling.CpuCooling.OtherCoolingType =
                CpuOtherCoolingTypeTextBox.Text.Trim();

            bool isAir =
                cooling.StandardCooling.CpuCooling.CoolingType == "Air";

            if (isAir)
            {
                SaveCpuAirCooling(cooling);
            }

            bool isAio =
                cooling.StandardCooling.CpuCooling.CoolingType == "AIO";

            if (isAio)
            {
                SaveCpuAioCooling(cooling);
            }

            cooling.StandardCooling.GpuCooling.CoolingType =
                GetGpuCoolingTypeKey();

            cooling.StandardCooling.GpuCooling.StockType =
                GetGpuStockTypeKey();

            cooling.StandardCooling.GpuCooling.CustomDescription =
                GpuCustomDescriptionTextBox.Text.Trim();

            cooling.StandardCooling.MotherboardCooling.CoolingType =
                GetMotherboardCoolingTypeKey();

            cooling.StandardCooling.MotherboardCooling.StockType =
                GetMotherboardStockTypeKey();

            cooling.StandardCooling.MotherboardCooling.CustomDescription =
                MotherboardCustomDescriptionTextBox.Text.Trim();

            cooling.StandardCooling.RamCooling.CoolingType =
                GetRamCoolingTypeKey();

            cooling.StandardCooling.RamCooling.StockType =
                GetRamStockTypeKey();

            cooling.StandardCooling.RamCooling.CustomDescription =
                RamCustomDescriptionTextBox.Text.Trim();

            cooling.StandardCooling.StorageCooling.CoolingType =
                GetStorageCoolingTypeKey();

            cooling.StandardCooling.StorageCooling.StockType =
                GetStorageStockTypeKey();

            cooling.StandardCooling.StorageCooling.CustomDescription =
                StorageCustomDescriptionTextBox.Text.Trim();

            cooling.StandardCooling.PsuCooling.CoolingType =
    GetPsuCoolingTypeKey();

            cooling.StandardCooling.PsuCooling.StockType =
                GetPsuStockTypeKey();

            cooling.StandardCooling.PsuCooling.CustomDescription =
                PsuCustomDescriptionTextBox.Text.Trim();

            cooling.StandardCooling.PsuCooling.Fans.HasFanDetails =
                PsuHasFanDetailsCheckBox.IsChecked == true;

            cooling.StandardCooling.PsuCooling.Fans.Fans.Clear();

            if (cooling.StandardCooling.PsuCooling.Fans.HasFanDetails)
            {
                PcCoolingFan fan = new PcCoolingFan();

                fan.Size = GetPsuFanSizeKey();
                fan.PrimaryColor = PsuFanPrimaryColorTextBox.Text.Trim();
                fan.SecondaryColor = PsuFanSecondaryColorTextBox.Text.Trim();
                fan.RgbType = GetPsuFanRgbKey();
                fan.IsReverseFan = PsuFanReverseCheckBox.IsChecked == true;

                cooling.StandardCooling.PsuCooling.Fans.FanCount =
                    int.TryParse(PsuFanCountCombo.Text, out int psuFanCount)
                        ? psuFanCount
                        : null;

                cooling.StandardCooling.PsuCooling.Fans.Fans.Add(fan);
            }
            else
            {
                cooling.StandardCooling.PsuCooling.Fans.FanCount = null;
            }

            SaveCaseCooling(cooling);

            ResultCooling = cooling;

            DialogResult = true;
            Close();
        }
        private void SaveCaseCooling(PcCooling cooling)
        {
            cooling.StandardCooling.CaseCooling.FanGroups.Clear();

            if (HasCaseCoolingCheckBox.IsChecked != true)
                return;

            SaveCaseFanGroup(
                cooling,
                CaseFrontCheckBox,
                CaseFrontFanCountCombo,
                CaseFrontFansContainer,
                "Front");

            SaveCaseFanGroup(
                cooling,
                CaseLeftCheckBox,
                CaseLeftFanCountCombo,
                CaseLeftFansContainer,
                "LeftSide");

            SaveCaseFanGroup(
                cooling,
                CaseRightCheckBox,
                CaseRightFanCountCombo,
                CaseRightFansContainer,
                "RightSide");

            SaveCaseFanGroup(
                cooling,
                CaseTopCheckBox,
                CaseTopFanCountCombo,
                CaseTopFansContainer,
                "Top");

            SaveCaseFanGroup(
                cooling,
                CaseBottomCheckBox,
                CaseBottomFanCountCombo,
                CaseBottomFansContainer,
                "Bottom");

            SaveCaseFanGroup(
                cooling,
                CaseRearCheckBox,
                CaseRearFanCountCombo,
                CaseRearFansContainer,
                "Rear");
        }
        private void SaveCaseFanGroup(
    PcCooling cooling,
    CheckBox locationCheckBox,
    ComboBox fanCountCombo,
    StackPanel fansContainer,
    string locationKey)
        {
            if (locationCheckBox.IsChecked != true)
                return;

            string selectedCount =
                fanCountCombo.SelectedItem?.ToString() ?? "";

            if (!int.TryParse(selectedCount, out int fanCount))
                return;

            PcCaseFanGroup group = new PcCaseFanGroup
            {
                Location = locationKey
            };

            for (int i = 1; i <= fanCount; i++)
            {
                ComboBox? sizeCombo =
                    FindVisualChildByName<ComboBox>(
                        fansContainer,
                        $"Case{locationKey}FanSizeCombo{i}");

                TextBox? primaryColorTextBox =
                    FindVisualChildByName<TextBox>(
                        fansContainer,
                        $"Case{locationKey}FanPrimaryColorTextBox{i}");

                TextBox? secondaryColorTextBox =
                    FindVisualChildByName<TextBox>(
                        fansContainer,
                        $"Case{locationKey}FanSecondaryColorTextBox{i}");

                ComboBox? rgbCombo =
                    FindVisualChildByName<ComboBox>(
                        fansContainer,
                        $"Case{locationKey}FanRgbCombo{i}");

                CheckBox? reverseCheckBox =
                    FindVisualChildByName<CheckBox>(
                        fansContainer,
                        $"Case{locationKey}FanReverseCheckBox{i}");

                PcCoolingFan fan = new PcCoolingFan
                {
                    Size = GetFanSizeKey(sizeCombo?.Text ?? ""),
                    PrimaryColor = primaryColorTextBox?.Text.Trim() ?? "",
                    SecondaryColor = secondaryColorTextBox?.Text.Trim() ?? "",
                    RgbType = GetFanRgbKey(rgbCombo?.Text ?? ""),
                    IsReverseFan = reverseCheckBox?.IsChecked == true
                };

                group.Fans.Add(fan);
            }

            cooling.StandardCooling.CaseCooling.FanGroups.Add(group);
        }
        private string GetPsuCoolingTypeKey()
        {
            string selected =
                PsuCoolingTypeCombo.SelectedItem?.ToString() ?? "";

            return selected switch
            {
                "Nie określono" => "NotSpecified",
                "Not specified" => "NotSpecified",
                "Stock" => "Stock",
                "Custom" => "Custom",
                _ => ""
            };
        }

        private string GetPsuStockTypeKey()
        {
            string selected =
                PsuStockTypeCombo.SelectedItem?.ToString() ?? "";

            return selected switch
            {
                "Nie określono" => "NotSpecified",
                "Not specified" => "NotSpecified",
                "Brak" => "None",
                "None" => "None",
                "Pasywne" => "Passive",
                "Passive" => "Passive",
                "Aktywne" => "Active",
                "Active" => "Active",
                _ => ""
            };
        }

        private string GetPsuFanSizeKey()
        {
            string selected =
                PsuFanSizeCombo.SelectedItem?.ToString() ?? "";

            return selected switch
            {
                "Nie określono" => "",
                "Not specified" => "",
                "40 mm" => "40",
                "60 mm" => "60",
                "80 mm" => "80",
                "92 mm" => "92",
                "120 mm" => "120",
                "135 mm" => "135",
                "140 mm" => "140",
                "Inny" => "Other",
                "Other" => "Other",
                _ => ""
            };
        }
        private void CaseLocationCheckBox_Checked(
        object sender,
        RoutedEventArgs e)
        {
            CaseFrontPanel.Visibility =
                CaseFrontCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            CaseLeftPanel.Visibility =
                CaseLeftCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            CaseRightPanel.Visibility =
                CaseRightCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            CaseTopPanel.Visibility =
                CaseTopCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            CaseBottomPanel.Visibility =
                CaseBottomCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            CaseRearPanel.Visibility =
                CaseRearCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
        private string GetPsuFanRgbKey()
        {
            string selected =
                PsuFanRgbCombo.SelectedItem?.ToString() ?? "";

            return selected switch
            {
                "Brak RGB" => "NoRgb",
                "No RGB" => "NoRgb",
                "RGB" => "RGB",
                "ARGB" => "ARGB",
                "Stały LED" => "FixedLed",
                "Fixed LED" => "FixedLed",
                "Inny" => "Other",
                "Other" => "Other",
                _ => ""
            };
        }
        private void SaveCpuAioCooling(PcCooling cooling)
        {
            var aio = cooling.StandardCooling.CpuCooling.AioCooling;

            aio.HasAioDetails =
                HasAioDetailsCheckBox.IsChecked == true;

            if (aio.HasAioDetails)
            {
                aio.Manufacturer = AioManufacturerTextBox.Text.Trim();
                aio.Model = AioModelTextBox.Text.Trim();
                aio.Notes =
                    HasAioNotesCheckBox.IsChecked == true
                        ? AioNotesTextBox.Text.Trim()
                        : "";
            }
            else
            {
                aio.Manufacturer = "";
                aio.Model = "";
                aio.Notes = "";
            }
        }
        private string GetStorageCoolingTypeKey()
        {
            string selected =
                StorageCoolingTypeCombo.SelectedItem?.ToString() ?? "";

            return selected switch
            {
                "Nie określono" => "NotSpecified",
                "Not specified" => "NotSpecified",

                "Stock" => "Stock",
                "Custom" => "Custom",

                _ => ""
            };
        }

        private string GetStorageStockTypeKey()
        {
            string selected =
                StorageStockTypeCombo.SelectedItem?.ToString() ?? "";

            return selected switch
            {
                "Nie określono" => "NotSpecified",
                "Not specified" => "NotSpecified",

                "Brak" => "None",
                "None" => "None",

                "Pasywne" => "Passive",
                "Passive" => "Passive",

                "Aktywne" => "Active",
                "Active" => "Active",

                _ => ""
            };
        }
        private string GetRamCoolingTypeKey()
        {
            string selected =
                RamCoolingTypeCombo.SelectedItem?.ToString() ?? "";

            return selected switch
            {
                "Nie określono" => "NotSpecified",
                "Not specified" => "NotSpecified",

                "Stock" => "Stock",
                "Custom" => "Custom",

                _ => ""
            };
        }

        private string GetRamStockTypeKey()
        {
            string selected =
                RamStockTypeCombo.SelectedItem?.ToString() ?? "";

            return selected switch
            {
                "Nie określono" => "NotSpecified",
                "Not specified" => "NotSpecified",

                "Brak" => "None",
                "None" => "None",

                "Pasywne" => "Passive",
                "Passive" => "Passive",

                "Aktywne" => "Active",
                "Active" => "Active",

                _ => ""
            };
        }
        private string GetMotherboardCoolingTypeKey()
        {
            string selected =
                MotherboardCoolingTypeCombo.SelectedItem?.ToString() ?? "";

            return selected switch
            {
                "Nie określono" => "NotSpecified",
                "Not specified" => "NotSpecified",

                "Stock" => "Stock",
                "Custom" => "Custom",

                _ => ""
            };
        }
        private string GetMotherboardStockTypeKey()
        {
            string selected =
                MotherboardStockTypeCombo.SelectedItem?.ToString() ?? "";

            return selected switch
            {
                "Nie określono" => "NotSpecified",
                "Not specified" => "NotSpecified",

                "Powietrzne" => "Air",
                "Air" => "Air",

                "Pasywne" => "Passive",
                "Passive" => "Passive",

                _ => ""
            };
        }
        private string GetGpuCoolingTypeKey()
        {
            string selected =
                GpuCoolingTypeCombo.SelectedItem?.ToString() ?? "";

            return selected switch
            {
                "Nie określono" => "NotSpecified",
                "Not specified" => "NotSpecified",
                "Stock" => "Stock",
                "Custom" => "Custom",
                _ => ""
            };
        }

        private string GetGpuStockTypeKey()
        {
            string selected =
                GpuStockTypeCombo.SelectedItem?.ToString() ?? "";

            return selected switch
            {
                "Nie określono" => "NotSpecified",
                "Not specified" => "NotSpecified",

                "Powietrzne" => "Air",
                "Air" => "Air",

                "Pasywne" => "Passive",
                "Passive" => "Passive",

                "Hybrydowe AIO" => "HybridAio",
                "Hybrid AIO" => "HybridAio",

                _ => ""
            };
        }
        private T? FindVisualChildByName<T>(
        DependencyObject parent,
        string name)
        where T : FrameworkElement
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count; i++)
            {
                DependencyObject child =
                    VisualTreeHelper.GetChild(parent, i);

                if (child is T element &&
                    element.Name == name)
                {
                    return element;
                }

                T? result =
                    FindVisualChildByName<T>(child, name);

                if (result != null)
                    return result;
            }

            return null;
        }

        private void SaveCpuAirCooling(PcCooling cooling)
        {
            var air =
                cooling.StandardCooling.CpuCooling.AirCooling;

            if (HasRadiatorDetailsCheckBox.IsChecked == true)
            {
                air.RadiatorType = GetRadiatorTypeKey();
                air.OtherRadiatorType = OtherRadiatorTypeTextBox.Text.Trim();
                air.RadiatorPrimaryColor = RadiatorPrimaryColorTextBox.Text.Trim();
                air.RadiatorSecondaryColor = RadiatorSecondaryColorTextBox.Text.Trim();
            }

            air.HasHeatpipeCount =
                HasHeatpipeCountCheckBox.IsChecked == true;

            if (air.HasHeatpipeCount &&
                int.TryParse(HeatpipeCountCombo.Text, out int heatpipeCount))
            {
                air.HeatpipeCount = heatpipeCount;
            }
            else
            {
                air.HeatpipeCount = null;
            }

            air.HasPeltierModule =
                HasPeltierModuleCheckBox.IsChecked == true;

            air.PeltierModel =
                air.HasPeltierModule
                    ? PeltierModelTextBox.Text.Trim()
                    : "";

            air.Fans.HasFanDetails =
    HasFanDetailsCheckBox.IsChecked == true;

            air.Fans.Fans.Clear();

            if (air.Fans.HasFanDetails &&
                int.TryParse(FanCountCombo.Text, out int fanCount))
            {
                air.Fans.FanCount = fanCount;

                for (int i = 1; i <= fanCount; i++)
                {
                    ComboBox? sizeCombo =
                        FindVisualChildByName<ComboBox>(
                            FansContainer,
                            $"FanSizeCombo{i}");

                    TextBox? primaryColorTextBox =
                        FindVisualChildByName<TextBox>(
                            FansContainer,
                            $"FanPrimaryColorTextBox{i}");

                    TextBox? secondaryColorTextBox =
                        FindVisualChildByName<TextBox>(
                            FansContainer,
                            $"FanSecondaryColorTextBox{i}");

                    ComboBox? rgbCombo =
                        FindVisualChildByName<ComboBox>(
                            FansContainer,
                            $"FanRgbCombo{i}");

                    CheckBox? reverseCheckBox =
                        FindVisualChildByName<CheckBox>(
                            FansContainer,
                            $"FanReverseCheckBox{i}");

                    PcCoolingFan fan = new PcCoolingFan();

                    fan.Size =
                        GetFanSizeKey(sizeCombo?.Text ?? "");

                    fan.PrimaryColor =
                        primaryColorTextBox?.Text.Trim() ?? "";

                    fan.SecondaryColor =
                        secondaryColorTextBox?.Text.Trim() ?? "";

                    fan.RgbType =
                        GetFanRgbKey(rgbCombo?.Text ?? "");

                    fan.IsReverseFan =
                        reverseCheckBox?.IsChecked == true;

                    air.Fans.Fans.Add(fan);
                }
            }
            else
            {
                air.Fans.FanCount = null;
            }
        }

        private string GetCpuCoolingTypeKey()
        {
            string selected =
                CpuCoolingTypeCombo.SelectedItem?.ToString() ?? "";

            return selected switch
            {
                "Nie określono" => "NotSpecified",
                "Not specified" => "NotSpecified",

                "Powietrzne Box / OEM" => "AirBoxOem",
                "Air Box / OEM" => "AirBoxOem",

                "Powietrzne" => "Air",
                "Air" => "Air",

                "AIO" => "AIO",

                "Pasywne" => "Passive",
                "Passive" => "Passive",

                "Peltier" => "Peltier",

                "LN2" => "LN2",

                "Inne" => "Other",
                "Other" => "Other",

                _ => ""
            };
        }

        private string GetRadiatorTypeKey()
        {
            string selected =
                RadiatorTypeCombo.SelectedItem?.ToString() ?? "";

            return selected switch
            {
                "Nie określono" => "NotSpecified",
                "Not specified" => "NotSpecified",

                "Box / OEM" => "BoxOem",

                "Wieżowy" => "Tower",
                "Tower" => "Tower",

                "Podwójna wieża" => "DualTower",
                "Dual Tower" => "DualTower",

                "Potrójna wieża" => "TripleTower",
                "Triple Tower" => "TripleTower",

                "Górny nawiew" => "TopFlow",
                "Top Flow" => "TopFlow",

                "Niski profil" => "LowProfile",
                "Low Profile" => "LowProfile",

                "Slotowy" => "SlotCooler",
                "Slot Cooler" => "SlotCooler",

                "Pasywny" => "Passive",
                "Passive" => "Passive",

                "Inny" => "Other",
                "Other" => "Other",

                _ => ""
            };
        }

        private string GetFanSizeKey(string selected)
        {
            return selected switch
            {
                "Nie określono" => "",
                "Not specified" => "",

                "80 mm" => "80",
                "92 mm" => "92",
                "100 mm" => "100",
                "120 mm" => "120",
                "135 mm" => "135",
                "140 mm" => "140",
                "180 mm" => "180",
                "200 mm" => "200",

                "Inny" => "Other",
                "Other" => "Other",

                _ => ""
            };
        }

        private string GetFanRgbKey(string selected)
        {
            return selected switch
            {
                "Brak RGB" => "NoRgb",
                "No RGB" => "NoRgb",

                "RGB" => "RGB",
                "ARGB" => "ARGB",

                "Stały LED" => "FixedLed",
                "Fixed LED" => "FixedLed",

                "Inny" => "Other",
                "Other" => "Other",

                _ => ""
            };
        }
        private void HasCaseCoolingCheckBox_Checked(
        object sender,
        RoutedEventArgs e)
        {
            if (CaseCoolingDetailsPanel == null)
                return;

            CaseCoolingDetailsPanel.Visibility =
                HasCaseCoolingCheckBox.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            if (HasCaseCoolingCheckBox.IsChecked != true)
            {
                CaseFrontCheckBox.IsChecked = false;
                CaseLeftCheckBox.IsChecked = false;
                CaseRightCheckBox.IsChecked = false;
                CaseTopCheckBox.IsChecked = false;
                CaseBottomCheckBox.IsChecked = false;
                CaseRearCheckBox.IsChecked = false;
            }
        }
    }
}