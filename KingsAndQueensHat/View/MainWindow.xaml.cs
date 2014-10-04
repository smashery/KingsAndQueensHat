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

        /// <summary>
        /// Generate the teams for the next round
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Tournament.AllTeamsHaveResults())
            {
                if (MessageBox.Show("Not all teams have results recorded.\r\nAre you sure you want to generate new rounds?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
                {
                    return;
                }
            }
            int teamCount;
            if (int.TryParse(TeamCountBox.Text, out teamCount))
            {
                var source = new CancellationTokenSource();
                var task = Tournament.CreateNewRound(SpeedSlider.Value, teamCount, source.Token);
                var cancelDialog = new CancelDialog(task, source);
                cancelDialog.ShowDialog();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to permanently delete\r\nthese teams and their results?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                Tournament.DeleteLastRound();
            }
        }

        private void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to permanently delete all data?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                Tournament.DeleteAllData();
            }
        }
    }
}
