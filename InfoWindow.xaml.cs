using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace HGC
{
    public enum InfoPage
    {
        About,
        Rawg,
        Copyright,
        Support
    }

    public partial class InfoWindow : Window
    {
        private readonly string currentLanguage;

        public InfoWindow(InfoPage page, string language = "pl")
        {
            InitializeComponent();

            currentLanguage = language;

            LoadPage(page);
        }

        private void LoadPage(InfoPage page)
        {
            SupportButton.Visibility = Visibility.Collapsed;
            RawgButton.Visibility = Visibility.Collapsed;

            switch (page)
            {
                case InfoPage.About:
                    InfoTitleText.Text = currentLanguage == "pl"
                        ? "O programie"
                        : "About";

                    InfoContentText.Text = currentLanguage == "pl"

     ? "Hajper Games Collection to darmowa aplikacja desktopowa przeznaczona do kompleksowego zarządzania prywatną kolekcją gier, sprzętu, komputerów oraz akcesoriów.\r\n\r\nProgram umożliwia tworzenie szczegółowej bazy kolekcji z wykorzystaniem rozbudowanych kart elementów zawierających między innymi opisy, dane techniczne, informacje o stanie, historię serwisową, galerie zdjęć, okładki, własne tła oraz dodatkowe informacje definiowane przez użytkownika.\r\n\r\nHGC oferuje rozbudowane funkcje wyszukiwania i filtrowania, moduł statystyk z wykresami i analizą kolekcji, system motywów, regulację przezroczystości interfejsu oraz eksport szczegółowych raportów do formatów PDF i TXT. Aplikacja umożliwia również pobieranie grafik gier z wykorzystaniem interfejsu API serwisu RAWG.\r\n\r\nProgram rozwijany jest samodzielnie przez hajperAPP jako niezależny projekt hobbystyczny. Powstał z pasji do gier, kolekcjonowania oraz programowania, a jego głównym celem jest stworzenie nowoczesnego, intuicyjnego i stale rozwijanego narzędzia dla kolekcjonerów.\r\n\r\nPodczas projektowania i rozwoju aplikacji wykorzystywane są również nowoczesne narzędzia oparte na sztucznej inteligencji, wspomagające analizę, projektowanie interfejsu użytkownika, implementację oraz testowanie wybranych elementów programu. Ostateczna architektura aplikacji, zastosowane rozwiązania oraz wszystkie decyzje projektowe pozostają wynikiem autorskiej pracy.\r\n\r\nHGC nie zawiera reklam, nie wymaga tworzenia konta użytkownika i nie wykonuje żadnych operacji sieciowych bez wyraźnej inicjatywy użytkownika."

     : "Hajper Games Collection is a free desktop application designed for comprehensive management of a private collection of games, hardware, computers, and accessories.\r\n\r\nThe application allows users to build a detailed collection database using extensive item records that can include descriptions, technical specifications, condition information, service history, photo galleries, front and back covers, custom backgrounds, and additional user-defined information.\r\n\r\nHGC provides advanced search and filtering capabilities, a comprehensive statistics module with charts and collection analysis, multiple visual themes, interface transparency settings, and detailed report export to PDF and TXT formats. The application also supports downloading game artwork through the RAWG API.\r\n\r\nThe software is independently developed by hajperAPP as a hobby project. It was created out of a passion for gaming, collecting, and software development, with the goal of providing collectors with a modern, intuitive, and continuously evolving collection management tool.\r\n\r\nModern artificial intelligence tools are also used during the design and development process to assist with analysis, user interface design, implementation, and testing of selected application components. The overall application architecture, implemented solutions, and all design decisions remain the result of independent development and original authorial work.\r\n\r\nHGC contains no advertisements, does not require user registration, and performs no online operations without the user's explicit action.";
                   
                    break;

                case InfoPage.Rawg:
                    InfoTitleText.Text = "RAWG";

                    InfoContentText.Text = currentLanguage == "pl"
                        ? "Hajper Games Collection wykorzystuje publiczny interfejs API serwisu RAWG do ręcznego wyszukiwania informacji o grach oraz pobierania grafik na wyraźne żądanie użytkownika.\r\n\r\nIntegracja z RAWG ma na celu ułatwienie uzupełniania kolekcji poprzez szybkie odnalezienie odpowiedniej gry i dodanie grafiki bez konieczności ręcznego wyszukiwania jej w Internecie.\r\n\r\nProgram nie wykonuje automatycznych zapytań do serwisu RAWG. Połączenie z API następuje wyłącznie po wybraniu odpowiedniej opcji przez użytkownika, a pobierane są jedynie informacje niezbędne do wykonania wybranego działania.\r\n\r\nHajper Games Collection nie przechowuje danych logowania do serwisu RAWG, nie wysyła informacji o prywatnej kolekcji użytkownika ani nie udostępnia żadnych danych osobowych.\r\n\r\nWszystkie dane, grafiki, znaki towarowe oraz pozostałe materiały udostępniane przez serwis RAWG pozostają własnością ich prawnych właścicieli. Hajper Games Collection nie jest oficjalnie powiązany, sponsorowany ani wspierany przez serwis RAWG.\r\n\r\nSerdeczne podziękowania dla zespołu RAWG za udostępnienie publicznego API, które umożliwia tworzenie funkcji wzbogacających doświadczenie użytkowników aplikacji HGC."
                        : "Hajper Games Collection uses the public RAWG API to manually search for game information and download artwork only at the user's explicit request.\r\n\r\nThe RAWG integration is designed to simplify the process of completing a collection by allowing users to quickly find a game and add artwork without manually searching the web.\r\n\r\nThe application does not perform automatic requests to the RAWG service. Communication with the API takes place only after the user selects the appropriate option, and only the information required to complete the requested action is retrieved.\r\n\r\nHajper Games Collection does not store RAWG account credentials, does not transmit information about the user's private collection, and does not share any personal data.\r\n\r\nAll data, images, trademarks, and other materials provided through the RAWG service remain the property of their respective owners. Hajper Games Collection is not affiliated with, sponsored by, or endorsed by RAWG.\r\n\r\nSpecial thanks to the RAWG team for providing a public API that makes it possible to build features that enhance the overall user experience within HGC.";

                    RawgButton.Visibility = Visibility.Visible;

                    break;


                case InfoPage.Copyright:
                    InfoTitleText.Text = currentLanguage == "pl"
                        ? "Prawa autorskie"
                        : "Copyright";

                    InfoContentText.Text = currentLanguage == "pl"
                        ? "Hajper Games Collection (HGC) jest autorskim oprogramowaniem rozwijanym przez hajperAPP. O ile nie wskazano inaczej, wszelkie prawa autorskie do kodu źródłowego, projektu interfejsu użytkownika, dokumentacji oraz innych oryginalnych elementów programu należą do autora aplikacji.\r\n\r\nW programie mogą być wykorzystywane nazwy handlowe, znaki towarowe, oznaczenia produktów, nazwy producentów, platform, konsol, komputerów, akcesoriów oraz gier wyłącznie w celach identyfikacyjnych i informacyjnych. Wszelkie prawa do tych oznaczeń pozostają własnością ich odpowiednich właścicieli.\r\n\r\nHajper Games Collection nie jest powiązany, sponsorowany, licencjonowany, autoryzowany ani wspierany przez żadnego producenta sprzętu, wydawcę gier, właściciela znaków towarowych ani operatora platformy.\r\n\r\nAutor aplikacji nie ma wpływu na treści dodawane przez użytkowników, w tym opisy, zdjęcia, grafiki, okładki, tła oraz inne materiały przechowywane w programie. Użytkownik ponosi pełną odpowiedzialność za legalność, pochodzenie oraz sposób wykorzystania materiałów dodawanych do swojej kolekcji.\r\n\r\nWszelkie prawa do danych, grafik oraz innych materiałów pobieranych z zewnętrznych źródeł pozostają własnością ich odpowiednich właścicieli i są wykorzystywane zgodnie z obowiązującymi warunkami korzystania z tych usług oraz ich licencjami."
                        : "Hajper Games Collection (HGC) is proprietary software developed by hajperAPP. Unless otherwise stated, all copyrights to the source code, user interface design, documentation, and other original components of the application belong to the author.\r\n\r\nThe application may use trade names, trademarks, product names, manufacturer names, platform names, console names, computer names, accessory names, and game titles solely for identification and informational purposes. All rights to such names and marks remain the property of their respective owners.\r\n\r\nHajper Games Collection is not affiliated with, sponsored by, licensed by, authorized by, or endorsed by any hardware manufacturer, game publisher, trademark owner, or platform operator.\r\n\r\nThe application author has no control over the content added by users, including descriptions, photographs, images, covers, backgrounds, or any other materials stored within the application. Users are solely responsible for ensuring the legality, origin, and appropriate use of all content added to their collections.\r\n\r\nAll rights to data, images, and other materials obtained from third-party sources remain the property of their respective owners and are used in accordance with the applicable terms of service and licensing conditions.";
                                        break;

                case InfoPage.Support:
                    InfoTitleText.Text = currentLanguage == "pl"
                        ? "Postaw kawę"
                        : "Buy me a coffee";

                    InfoContentText.Text = currentLanguage == "pl"
                        ? "hajperAPP to moja niewielka, niezależna inicjatywa, w ramach której rozwijam aplikacje tworzone z pasji do technologii, gier oraz programowania.\r\n\r\nHajper Games Collection jest rozwijany samodzielnie, po godzinach i w wolnym czasie, z dużym naciskiem na jakość, funkcjonalność oraz ciągły rozwój. Pomysł na stworzenie projektu narodził się między innymi podczas rekonwalescencji po kontuzji, kiedy długie tygodnie spędzone w domu stały się okazją do nauki oraz realizacji pomysłów odkładanych od dłuższego czasu.\r\n\r\nJeżeli HGC okazał się dla Ciebie przydatnym narzędziem i chciałbyś wesprzeć dalszy rozwój projektu, możesz postawić mi symboliczną kawę.\r\n\r\nKażde wsparcie pomaga poświęcić więcej czasu na rozwój aplikacji, dodawanie nowych funkcji, poprawianie błędów oraz tworzenie kolejnych projektów.\r\n\r\nDziękuję za korzystanie z Hajper Games Collection oraz za każdą formę wsparcia."
                        : "hajperAPP is my small independent initiative focused on developing applications inspired by a passion for technology, gaming, and software development.\r\n\r\nHajper Games Collection is developed independently in my free time, with a strong focus on quality, functionality, and continuous improvement. The idea for the project began during my recovery from an injury, when several weeks at home became an opportunity to learn new technologies and finally bring long-planned ideas to life.\r\n\r\nIf HGC has been useful to you and you would like to support its future development, you are welcome to buy me a symbolic coffee.\r\n\r\nEvery contribution helps me dedicate more time to improving the application, adding new features, fixing bugs, and developing future projects.\r\n\r\nThank you for using Hajper Games Collection and for every bit of support.";

                    SupportButton.Visibility = Visibility.Visible;
                    break;
            }

            CloseButton.Content = currentLanguage == "pl" ? "Zamknij" : "Close";
            SupportButton.Content = currentLanguage == "pl" ? "Postaw kawę" : "Buy me a coffee";
            RawgButton.Content = currentLanguage == "pl"
                ? "Otwórz RAWG"
                : "Open RAWG";
        }
        private void RawgButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://rawg.io",
                UseShellExecute = true
            });
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SupportButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://buycoffee.to/hajperapp",
                UseShellExecute = true
            });
        }
    }
}