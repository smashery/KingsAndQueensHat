using System.Collections.ObjectModel;
using System.Windows;
using KingsAndQueensHat.Model;
using System.Threading;

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
            Tournament.LoadExistingData();
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
                var source = new CancellationTokenSource();
                var task = Tournament.CreateNewRound(SpeedSlider.Value, teamCount, source.Token);
                var cancelDialog = new CancelDialog(task, source);
                cancelDialog.ShowDialog();
            }
        }
    }
}
