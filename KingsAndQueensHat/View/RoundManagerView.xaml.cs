using KingsAndQueensHat.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KingsAndQueensHat.View
{
    /// <summary>
    /// Interaction logic for RoundManagerView.xaml
    /// </summary>
    public partial class RoundManagerView : UserControl
    {
        public RoundManagerView()
        {
            InitializeComponent();
        }

        private RoundManagerViewModel ViewModel { get { return DataContext as RoundManagerViewModel; } }

        /// <summary>
        /// Generate the teams for the next round
        /// </summary>
        private void GenerateTeamsButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentRoundViewModel.ProblematicResults)
            {
                if (MessageBox.Show("Results recorded are invalid.\r\nAre you sure you want to generate new rounds?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.No)
                {
                    return;
                }
            }
            // Warn the user if they're looking at the last round and haven't recorded results
            // (they will already have been warned for previous rounds)
            else if (ViewModel.CurrentRoundNumber == ViewModel.NumRounds && !ViewModel.CurrentRoundViewModel.AllTeamsHaveResults())
            {
                if (MessageBox.Show("Not all teams have results recorded.\r\nAre you sure you want to generate new rounds?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
                {
                    return;
                }
            }

            try
            {
                ViewModel.CreateNewRound((t, s) =>
                {
                    var dialog = new CancelDialog(t, s);
                    dialog.Owner = Window.GetWindow(this);
                    return dialog;
                });
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to permanently delete\r\nthese teams and their results?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                ViewModel.DeleteThisRound();
            }
        }

        private void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to permanently delete all data?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                ViewModel.DeleteAllData();
            }
        }

        private void PreviousRoundButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PreviousRound();
        }

        private void NextRoundButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NextRound();
        }
    }
}
