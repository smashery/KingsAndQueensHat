using KingsAndQueensHat.Annotations;
using KingsAndQueensHat.Model;
using KingsAndQueensHat.Persistence;
using KingsAndQueensHat.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KingsAndQueensHat.ViewModel
{
    /// <summary>
    /// ViewModel for the tournament class
    /// </summary>
    public class TournamentViewModel : INotifyPropertyChanged
    {
        public TournamentViewModel()
        {
            var playerProvider = new PlayerFileReader(@"players.csv");

            Tournament = new Tournament(playerProvider);
            Tournament.LoadExistingData();
            TeamsThisRound = new ObservableCollection<Team>();
        }

        public Tournament Tournament { get; private set; }

        public ObservableCollection<Player> Players
        {
            get { return Tournament.Players; }
        }

        public int NumRounds { get { return Tournament.Rounds.Count; } }

        public int CurrentRound { get; private set; }

        public ObservableCollection<Team> TeamsThisRound { get; private set; }

        internal void CreateNewTeam(int teamCount, double speed, Func<Task, CancellationTokenSource, ICancelDialog> cancelDialogFactory)
        {
            var source = new CancellationTokenSource();
            var task = Tournament.CreateNewRound(speed, teamCount, source.Token);
            var cancelDialog = cancelDialogFactory(task, source);
            cancelDialog.ShowUntilCompleteOrCancelled();

            OnPropertyChanged("NumRounds");
            OnPropertyChanged("CanDeleteRound");

            // Update UI
            SetCurrentRound(NumRounds);
        }

        internal void DeleteLastRound()
        {
            Tournament.DeleteLastRound();
            SetCurrentRound(NumRounds);
            OnPropertyChanged("NumRounds");
            OnPropertyChanged("CanDeleteRound");
        }

        internal void DeleteAllData()
        {
            Tournament.DeleteAllData();
            SetCurrentRound(0);
            OnPropertyChanged("NumRounds");
            OnPropertyChanged("CanDeleteRound");
        }

        public bool CanDeleteRound
        {
            get { return NumRounds > 0; }
        }

        /// <summary>
        /// Set the current round to the 1-based round number within Tournament.Rounds.
        /// Set to 0 for "No current round"
        /// </summary>
        private void SetCurrentRound(int roundNumber)
        {
            CurrentRound = roundNumber;

            OnPropertyChanged("CurrentRound");

            // roundNumber is 1-based
            var roundIndex = roundNumber - 1;
            TeamSet round;
            if (roundIndex == -1)
            {
                round = null;
            }
            else
            {
                round = Tournament.Rounds[roundIndex];
            }

            TeamsThisRound.Clear();
            if (round != null)
            {
                foreach (Team team in round.Teams)
                {
                    TeamsThisRound.Add(team);
                }
            }
        }

        internal bool AllTeamsHaveResults()
        {
            return TeamsThisRound == null ||
                   TeamsThisRound.All(t => t.GameResult != GameResult.NoneYet);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
