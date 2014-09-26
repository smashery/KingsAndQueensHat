using System.Collections.ObjectModel;
using System.Windows;
using KingsAndQueensHat.Model;

namespace KingsAndQueensHat.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Tournament = new Tournament();
            DataContext = Tournament;
        }

        public Tournament Tournament { get; set; }

        public ObservableCollection<Player> Players
        {
            get { return Tournament.Players; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int teamCount;
            if (int.TryParse(TeamCountBox.Text, out teamCount))
            {
                Tournament.CreateNewRound(SpeedSlider.Value, teamCount);
            }
        }
    }
}
