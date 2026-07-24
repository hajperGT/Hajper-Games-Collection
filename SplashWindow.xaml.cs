using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace HGC
{
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();

            Loaded += SplashWindow_Loaded;
        }

        private void SplashWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            DoubleAnimation fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(500)
            };

            BeginAnimation(OpacityProperty, fadeIn);
        }

        public void FadeOut(Action onCompleted)
        {
            DoubleAnimation fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(500)
            };

            fadeOut.Completed += (s, e) => onCompleted?.Invoke();

            BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}