using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace HGC
{
    public partial class PdfGeneratingWindow : Window
    {
        public PdfGeneratingWindow(string currentLanguage)
        {
            InitializeComponent();

            GeneratingText.Text = currentLanguage == "pl"
                ? "Tworzenie PDF"
                : "Creating PDF";

            Loaded += PdfGeneratingWindow_Loaded;
        }

        private void PdfGeneratingWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            DoubleAnimation fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(350)
            };

            BeginAnimation(
                OpacityProperty,
                fadeIn);

            DoubleAnimation floatAnimation = new DoubleAnimation
            {
                From = 0,
                To = -2,
                Duration = TimeSpan.FromMilliseconds(1000),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            LogoTranslate.BeginAnimation(
                TranslateTransform.YProperty,
                floatAnimation);

            ColorAnimation textAnimation = new ColorAnimation
            {
                From = Colors.White,
                To = Color.FromRgb(165, 165, 165),
                Duration = TimeSpan.FromMilliseconds(1000),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            TextBrush.BeginAnimation(
                SolidColorBrush.ColorProperty,
                textAnimation);
        }
    }
}