using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Input;

namespace HGC
{
    public enum HgcMessageBoxType
    {
        Information,
        Warning,
        Error,
        Question
    }

    public enum HgcMessageBoxButtons
    {
        OK,
        YesNo,
        YesNoCancel
    }

    public enum HgcMessageBoxResult
    {
        OK,
        Yes,
        No,
        Cancel
    }

    public partial class HgcMessageBox : Window
    {
        private readonly HgcMessageBoxButtons buttons;

        public HgcMessageBox(
            string message,
            string title,
            HgcMessageBoxType type,
            HgcMessageBoxButtons buttons,
            string language = "pl")
        {
            InitializeComponent();

            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            this.buttons = buttons;

            Title = title;
            TitleText.Text = title;
            MessageText.Text = message;

            ApplyType(type);
            ApplyButtons(buttons, language);
            KeyDown += HgcMessageBox_KeyDown;
        }
        private void HgcMessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (buttons == HgcMessageBoxButtons.OK)
                    OkButton_Click(OkButton, new RoutedEventArgs());
                else
                    YesButton_Click(YesButton, new RoutedEventArgs());

                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                if (buttons == HgcMessageBoxButtons.YesNo)
                    NoButton_Click(NoButton, new RoutedEventArgs());
                else
                    OkButton_Click(OkButton, new RoutedEventArgs());

                e.Handled = true;
            }
        }
        public HgcMessageBoxResult Result { get; private set; } = HgcMessageBoxResult.Cancel;

        private void ApplyType(HgcMessageBoxType type)
        {
            switch (type)
            {
                case HgcMessageBoxType.Warning:
                    IconText.Text = "!";
                    IconCircle.Background = new SolidColorBrush(Color.FromRgb(214, 143, 32));
                    break;

                case HgcMessageBoxType.Error:
                    IconText.Text = "X";
                    IconCircle.Background = new SolidColorBrush(Color.FromRgb(179, 38, 30));
                    IconText.Foreground = Brushes.White;
                    break;

                case HgcMessageBoxType.Question:
                    IconText.Text = "?";
                    IconCircle.Background = (Brush)FindResource("AppAccentBrush");
                    break;

                default:
                    IconText.Text = "i";
                    IconCircle.Background = (Brush)FindResource("AppAccentBrush");
                    break;
            }
        }

        private void ApplyButtons(
            HgcMessageBoxButtons buttons,
            string language)
        {
            bool isPolish = language == "pl";

            OkButton.Content = "OK";
            YesButton.Content = isPolish ? "Tak" : "Yes";
            NoButton.Content = isPolish ? "Nie" : "No";

            if (buttons == HgcMessageBoxButtons.YesNo)
            {
                OkButton.Visibility = Visibility.Collapsed;
                YesButton.Visibility = Visibility.Visible;
                NoButton.Visibility = Visibility.Visible;
            }
            else if (buttons == HgcMessageBoxButtons.YesNoCancel)
            {
                OkButton.Content = isPolish ? "Anuluj" : "Cancel";

                OkButton.Visibility = Visibility.Visible;
                YesButton.Visibility = Visibility.Visible;
                NoButton.Visibility = Visibility.Visible;
            }
            else
            {
                OkButton.Visibility = Visibility.Visible;
                YesButton.Visibility = Visibility.Collapsed;
                NoButton.Visibility = Visibility.Collapsed;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = buttons == HgcMessageBoxButtons.YesNoCancel
                ? HgcMessageBoxResult.Cancel
                : HgcMessageBoxResult.OK;

            DialogResult = true;
            Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = HgcMessageBoxResult.Yes;
            DialogResult = true;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = HgcMessageBoxResult.No;
            DialogResult = true;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Result = HgcMessageBoxResult.Cancel;
            DialogResult = false;
            Close();
        }
        public static HgcMessageBoxResult ShowYesNoCancel(
        Window owner,
        string message,
        string title = "HGC",
        HgcMessageBoxType type = HgcMessageBoxType.Question,
        string language = "pl")
        {
            HgcMessageBox box = new HgcMessageBox(
                message,
                title,
                type,
                HgcMessageBoxButtons.YesNoCancel,
                language);

            box.Owner = owner;
            box.ShowDialog();

            return box.Result;
        }

        public static void Show(
            Window owner,
            string message,
            string title = "HGC",
            HgcMessageBoxType type = HgcMessageBoxType.Information,
            string language = "pl")
        {
            HgcMessageBox box = new HgcMessageBox(
                message,
                title,
                type,
                HgcMessageBoxButtons.OK,
                language);

            box.Owner = owner;
            box.ShowDialog();
        }

        public static bool ShowYesNo(
        Window owner,
        string message,
        string title = "HGC",
        HgcMessageBoxType type = HgcMessageBoxType.Question,
        string language = "pl")
        {
            HgcMessageBox box = new HgcMessageBox(
                message,
                title,
                type,
                HgcMessageBoxButtons.YesNo,
                language);

            box.Owner = owner;
            box.ShowDialog();

            return box.Result == HgcMessageBoxResult.Yes;
        }
    }
}