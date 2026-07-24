using System;
using System.Windows;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;

namespace HGC
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            RenderOptions.ProcessRenderMode =
                System.Windows.Interop.RenderMode.SoftwareOnly;

            base.OnStartup(e);

            bool noSplash = e.Args.Any(x =>
                x.Equals("--no-splash", StringComparison.OrdinalIgnoreCase));

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            if (noSplash)
            {
                MainWindow mainWindow = new MainWindow();

                Current.MainWindow = mainWindow;
                ShutdownMode = ShutdownMode.OnMainWindowClose;

                mainWindow.Show();
                mainWindow.Activate();

                return;
            }

            SplashWindow splash = new SplashWindow();
            splash.Show();
            splash.Activate();
            splash.UpdateLayout();

            Dispatcher.Invoke(
                DispatcherPriority.Render,
                new Action(() => { })
            );

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };

            timer.Tick += (s, args) =>
            {
                timer.Stop();

                splash.FadeOut(() =>
                {
                    MainWindow mainWindow = new MainWindow();

                    Current.MainWindow = mainWindow;
                    ShutdownMode = ShutdownMode.OnMainWindowClose;

                    mainWindow.Show();
                    mainWindow.Activate();

                    splash.Close();
                });
            };

            timer.Start();
        }
    }
}