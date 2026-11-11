using backend.Models;
using frontend;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace videojatek
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //különleges lista ami magától frissül ha változik az adatbázis
        public ObservableCollection<Game> Games { get; set; } = new ObservableCollection<Game>();
        public ObservableCollection<string> Categories { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Platforms { get; set; } = new ObservableCollection<string>();
        private List<Game> _allGames = new();

        public MainWindow()
        {
            InitializeComponent();

            // Összekötjük a Window-t saját magával, hogy a DataBinding működjön
            DataContext = this;
            //betölti az összes játékot , comboxot és kategóriát az adatbázisból
            LoadDataFromApiAsync(); 
        }

        private void Jatekkezelo(object sender, RoutedEventArgs e)
        {
            HomeContentGrid.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new Uri("Jatekkezelo.xaml", UriKind.Relative));
        }

        private async void Jatekgyujtemeny(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = null;
            MainFrame.Visibility = Visibility.Collapsed;
            HomeContentGrid.Visibility = Visibility.Visible;
            await LoadDataFromApiAsync();
        }

        private void JatekosokEsErtekelesek(object sender, RoutedEventArgs e)
        {
            HomeContentGrid.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new Uri("PlayerReviewPage.xaml", UriKind.Relative));
        }

        private void Frissites(object sender, RoutedEventArgs e)
        {
            Keresosav.Clear();
            Platform_valaszto.SelectedIndex = 0;
            Kategoria_valaszto.SelectedIndex = 0;
        }

        private void Kilepes(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // 3 óra a kukába és még nem jó , lehet megtanulok repülni a tetőröl
        
        private void UltimateSearchBarAndOptionFilttering()
        {
            
            if (Keresosav == null || Platform_valaszto == null || Kategoria_valaszto == null) return;

            
            string searchText = Keresosav.Text?.Trim().ToLower() ?? string.Empty;
            string selectedPlatform = Platform_valaszto.SelectedItem?.ToString() ?? "Minden platform";
            string selectedCategory = Kategoria_valaszto.SelectedItem?.ToString() ?? "Minden műfaj";

            
            // A gyűjteményt az API-ból kapjuk; a WPF nem csatlakozik közvetlenül az adatbázishoz.
            var filteredGames = _allGames.Where(g =>
            {
                // Keresősáv szűrés (ha a név tartalmazza a keresett szöveget)
                bool matchesSearch = string.IsNullOrEmpty(searchText) || (g.Name != null && g.Name.ToLower().Contains(searchText));

                // Platform szűrés
                bool matchesPlatform = selectedPlatform == "Minden platform" || g.Platform == selectedPlatform;

                // Műfaj szűrés
                bool matchesCategory = selectedCategory == "Minden műfaj" || g.Categories == selectedCategory;

                return matchesSearch && matchesPlatform && matchesCategory;
            }).ToList();

            //  képernyőn lévő ObservableCollection-t frisítjk
            Games.Clear();
            foreach (var game in filteredGames)
            {
                Games.Add(game);
            }

        }

        private void Platform_valaszto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UltimateSearchBarAndOptionFilttering();
        }

        private void Keresosav_TextChanged(object sender, TextChangedEventArgs e)
        {
            UltimateSearchBarAndOptionFilttering();
        }

        private void Kategoria_valaszto_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            UltimateSearchBarAndOptionFilttering();
        }



        private async Task LoadDataFromApiAsync()
        {
            try
            {
                var games = await GameApiClient.GetGamesAsync();
                _allGames = games;
                UltimateSearchBarAndOptionFilttering();

                var platforms = await GameApiClient.GetPlatformsAsync();
                Platforms.Clear();
                Platforms.Add("Minden platform");
                foreach (var p in platforms) Platforms.Add(p);

                var categories = await GameApiClient.GetCategoriesAsync();
                Categories.Clear();
                Categories.Add("Minden műfaj");
                foreach (var c in categories) Categories.Add(c);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba az adatok betöltésekor: " + ex.Message);
            }
        }


        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await LoadDataFromApiAsync();
        }



    }
}

