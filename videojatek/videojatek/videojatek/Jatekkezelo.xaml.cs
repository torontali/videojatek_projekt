using frontend;
using System.Windows;
using System.Windows.Controls;

namespace videojatek
{
    public partial class Jatekkezelo : Page
    {
        public Jatekkezelo() 
        {
            InitializeComponent();
        } 

        private async void Hozzaadas_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Jatek_neve.Text))
            {
                MessageBox.Show("A játék neve nem lehet üres.", "Hiányzó adat");
                return;
            }

            Hozzaadas_gomb.IsEnabled = false;              //azert tiltjuk le hogy fix ne adja hozza tobbszor a fh
            try
            {
                await GameApiClient.AddGameAsync(new GameRequest
                {
                    Name = Jatek_neve.Text.Trim(),
                    Company = Ceg_neve.Text.Trim(),
                    ReleaseDate = Megjelenes_eve.Text.Trim(),
                    Platform = Platform.Text.Trim(),
                    Type = Mufaj.Text.Trim(),
                    CoverImage = Borito_url.Text.Trim()
                });

                MessageBox.Show("A játék sikeresen hozzáadva!");
                Jatek_neve.Clear();                                 //toroljuk hogy a kovi hozzaadasoknal ne legyenek ott regi adatok
                Ceg_neve.Clear();
                Megjelenes_eve.Clear();
                Platform.Clear();
                Mufaj.Clear();
                Borito_url.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nem sikerült hozzáadni a játékot. Ellenőrizd, hogy fut-e a backend és a MySQL.\n\n" + ex.Message);
            }
            finally
            {
                Hozzaadas_gomb.IsEnabled = true;
            }
        }
    }
}
