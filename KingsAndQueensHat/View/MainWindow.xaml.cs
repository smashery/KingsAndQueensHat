using System.Collections.ObjectModel;
using System.Windows;
using System.Threading;
using KingsAndQueensHat.Model;
using KingsAndQueensHat.Persistence;
using KingsAndQueensHat.ViewModel;

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

            try
            {
                ViewModel = new TournamentViewModel();
            }
            catch (InvalidPlayerListException e)
            {
                MessageBox.Show(string.Format("{0}\r\nSee README.txt to help diagnose the error", e.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }
            catch (InvalidRoundException e)
            {
                MessageBox.Show(string.Format("{0}\r\nDelete the file or correct it", e.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            // Set an initial meaningful value for player count; at least 10 players per team, with even number of teams
            var numberOfPlayers = ViewModel.ActivePlayers.Count;
            TeamCountBox.Text = ((numberOfPlayers / 20) * 2).ToString();

            DataContext = ViewModel;
        }

        public TournamentViewModel ViewModel { get; private set; }

        /// <summary>
        /// Generate the teams for the next round
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ProblematicResults)
            {
                if (MessageBox.Show("Results recorded are invalid.\r\nAre you sure you want to generate new rounds?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.No)
                {
                    return;
                }
            }
            // Warn the user if they're looking at the last round and haven't recorded results
            // (they will already have been warned for previous rounds)
            else if (ViewModel.CurrentRoundNumber == ViewModel.NumRounds && !ViewModel.AllTeamsHaveResults())
            {
                if (MessageBox.Show("Not all teams have results recorded.\r\nAre you sure you want to generate new rounds?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
                {
                    return;
                }
            }

            int teamCount;
            if (int.TryParse(TeamCountBox.Text, out teamCount))
            {
                if (teamCount % 2 != 0)
                {
                    MessageBox.Show("Please enter an even number of teams", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    ViewModel.CreateNewRound(teamCount, SpeedSlider.Value, (t, s) => 
                        {
                            var dialog = new CancelDialog(t, s);
                            dialog.Owner = Window.GetWindow(this);
                            return dialog;
                        });
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid number of teams", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void ResultsButton_Click(object sender, RoutedEventArgs e)
        {
            var resultsWindow = new ResultsWindow(ViewModel);
            resultsWindow.ShowDialog();
        }
    }
}
