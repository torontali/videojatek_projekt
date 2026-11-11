using backend.Models;
using frontend;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace videojatek
{
    public partial class PlayerReviewPage : Page
    {
        private readonly ObservableCollection<player> _players = new();   //azert hasznalok ObservableCollection-t, hogy a listak automatikusan frissuljenek, ha valtozik az adat
        private readonly ObservableCollection<Game> _games = new();
        private readonly ObservableCollection<Rewiev> _reviews = new();

        public PlayerReviewPage()
        {
            InitializeComponent();
            PlayersList.ItemsSource = _players;
            PlayerSelect.ItemsSource = _players;
            GameSelect.ItemsSource = _games;
            ReviewsList.ItemsSource = _reviews;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) 
        { 
            await ReloadAsync(); 
        }

        private async Task ReloadAsync()
        {
            try
            {
                var players = await GameApiClient.GetPlayersAsync();
                var games = await GameApiClient.GetGamesAsync();
                var reviews = await GameApiClient.GetReviewsAsync();
                var statistics = await GameApiClient.GetReviewStatisticsAsync();

                _players.Clear();
                foreach (var item in players)
                {
                    _players.Add(item);               //azert kell a Clear mert ha nem lenne minden egyes betoltesnel ujra belekerulnenek a regi adatok
                }
                _games.Clear();
                foreach (var item in games)
                {
                    _games.Add(item);
                }
                _reviews.Clear(); foreach (var item in reviews)
                {
                    _reviews.Add(item);
                }
                if (PlayerSelect.SelectedItem is null && _players.Count > 0)      //alapbol az elso elem van kivalasztva
                {
                    PlayerSelect.SelectedIndex = 0;
                }
                if (GameSelect.SelectedItem is null && _games.Count > 0)
                {
                    GameSelect.SelectedIndex = 0;
                }

                StatisticsText.Text = statistics is null? "Nincs elérhető statisztika.": $"Értékelések: {statistics.ReviewCount}   |   Értékelt játékok: {statistics.ReviewedGames}   |   Átlagpontszám: {(statistics.AveragePoints.HasValue ? statistics.AveragePoints.Value.ToString("0.00") : "nincs adat")}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nem sikerült betölteni az adatokat. Ellenőrizd, hogy a backend és a MySQL fut-e.\n\n" + ex.Message);
            }
        }

        private async void AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameBox.Text) || string.IsNullOrWhiteSpace(EmailBox.Text))
            {
                MessageBox.Show("A felhasználónév és az e-mail kötelező."); return;
            }
            if (!int.TryParse(LevelBox.Text, out int level) || level < 1)             //egesz szamma alakit ha tud
            {
                MessageBox.Show("A szint legalább 1 legyen."); return;
            }
            try
            {
                await GameApiClient.AddPlayerAsync(new player { Username = UsernameBox.Text.Trim(), Email = EmailBox.Text.Trim(), Level = level });
                UsernameBox.Clear(); EmailBox.Clear(); LevelBox.Text = "1";
                await ReloadAsync();
                MessageBox.Show("A játékos mentése sikerült.");
            }
            catch (Exception ex) { MessageBox.Show("Nem sikerült menteni a játékost.\n\n" + ex.Message); }
        }

        private async void DeletePlayer_Click(object sender, RoutedEventArgs e)
        {
            if (PlayersList.SelectedItem is not player selected) 
            { 
                MessageBox.Show("Válassz ki egy játékost."); 
                return; 
            }
            if (MessageBox.Show($"Biztosan törlöd ezt a játékost: {selected.Username}? A hozzá tartozó értékelések is törlődnek.", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;   //ha nem re kattuintunk aakor braklel
            try 
            { 
                await GameApiClient.DeletePlayerAsync(selected.Id);
                await ReloadAsync(); 
            }
            catch (Exception ex) 
            { 
                MessageBox.Show("Nem sikerült törölni a játékost.\n\n" + ex.Message); 
            }
        }

        private async void AddReview_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerSelect.SelectedItem is not player selectedPlayer || GameSelect.SelectedItem is not Game selectedGame)
            {
                MessageBox.Show("Válassz játékost és játékot."); return;
            }
            if (PointsSelect.SelectedItem is not ComboBoxItem selectedPoints || !int.TryParse(selectedPoints.Content?.ToString(), out int points))
            {
                MessageBox.Show("Válassz pontszámot 1 és 5 között."); return;
            }
            try
            {
                await GameApiClient.AddReviewAsync(new Rewiev { PlayerId = selectedPlayer.Id, GameId = selectedGame.Id, Points = points, Comment = CommentBox.Text.Trim() });
                CommentBox.Clear(); await ReloadAsync(); MessageBox.Show("Az értékelés mentése sikerült.");
            }
            catch (Exception ex) { MessageBox.Show("Nem sikerült menteni az értékelést.\n\n" + ex.Message); }
        }

        private async void DeleteReview_Click(object sender, RoutedEventArgs e)
        {
            if (ReviewsList.SelectedItem is not Rewiev selected) 
            { 
                MessageBox.Show("Válassz ki egy értékelést."); 
                return; 
            }
            if (MessageBox.Show("Biztosan törlöd a kijelölt értékelést?", "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try 
            { 
                await GameApiClient.DeleteReviewAsync(selected.Id); 
                await ReloadAsync(); 
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Nem sikerült törölni az értékelést.\n\n" + ex.Message); 
            }
        }

        private async void PlayersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlayersList.SelectedItem is not player selected) 
            { 
                PlayerGamesText.Text = "Válassz ki egy játékost az értékelt játékaihoz."; 
                return; 
            }
            try
            {
                var games = await GameApiClient.GetRatedGamesAsync(selected.Id);
                PlayerGamesText.Text = games.Count == 0? "Ennek a játékosnak még nincs értékelése." : "Értékelt játékok: " + string.Join(", ", games.Select(g => $"{g.GameName} ({g.Points}/5)"));
            }
            catch (Exception ex) 
            { 
                PlayerGamesText.Text = "Az értékelt játékok lekérése sikertelen: " + ex.Message; 
            }
        }
    }
}
