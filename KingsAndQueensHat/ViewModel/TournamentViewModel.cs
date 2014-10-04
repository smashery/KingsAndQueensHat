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

            Tournament.GameDone += (sender, args) => WonLossChanged();

            TeamsThisRound = new ObservableCollection<Team>();
            SetCurrentRound(NumRounds);
        }

        public Tournament Tournament { get; private set; }

        public ObservableCollection<Player> Players
        {
            get { return Tournament.Players; }
        }

        public int NumRounds { get { return Tournament.Rounds.Count; } }

        public int CurrentRoundNumber { get; private set; }

        private TeamSet CurrentRound { get { return CurrentRoundNumber == 0 ? null : Tournament.Rounds[CurrentRoundNumber - 1]; } }

        public ObservableCollection<Team> TeamsThisRound { get; private set; }

        internal void CreateNewRound(int teamCount, double speed, Func<Task, CancellationTokenSource, ICancelDialog> cancelDialogFactory)
        {
            if (teamCount % 2 != 0)
            {
                throw new ArgumentException("teamCount must be even");
            }
            var source = new CancellationTokenSource();
            var task = Tournament.CreateNewRound(speed, teamCount, source.Token);
            var cancelDialog = cancelDialogFactory(task, source);
            cancelDialog.ShowUntilCompleteOrCancelled();

            OnPropertyChanged("NumRounds");
            OnPropertyChanged("CanDeleteRound");

            // Update UI with teams for this round
            SetCurrentRound(NumRounds);
        }

        internal void DeleteThisRound()
        {
            Tournament.DeleteRound(CurrentRoundNumber - 1);

            // Keep the same current round number (force UI update)
            SetCurrentRound(Math.Min(CurrentRoundNumber, NumRounds));
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
        public void SetCurrentRound(int roundNumber)
        {
            CurrentRoundNumber = roundNumber;

            OnPropertyChanged("CurrentRoundNumber");
            OnPropertyChanged("CanNavigateBackwards");
            OnPropertyChanged("CanNavigateForwards");
            OnPropertyChanged("ProblematicResults");
            OnPropertyChanged("ProblematicText");

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

        internal void PreviousRound()
        {
            if (CanNavigateBackwards)
            {
                SetCurrentRound(Math.Max(1, CurrentRoundNumber - 1));
            }
        }

        internal void NextRound()
        {
            if (CanNavigateForwards)
            {
                SetCurrentRound(Math.Min(NumRounds, CurrentRoundNumber + 1));
            }
        }

        public bool CanNavigateBackwards
        {
            get { return CurrentRoundNumber > 1; }
        }

        public bool CanNavigateForwards
        {
            get { return CurrentRoundNumber < NumRounds; }
        }

        internal void WonLossChanged()
        {
            OnPropertyChanged("ProblematicResults");
            OnPropertyChanged("ProblematicText");
            foreach (var player in Players)
            {
                player.ForceUpdate();
            }
        }

        public bool ProblematicResults { get { return CurrentRound == null ? false : CurrentRound.ProblematicResults; } }

        public string ProblematicText
        {
            get
            {
                var teamsWon = TeamsThisRound.Count(t => t.GameResult == GameResult.Won);
                var teamsLost = TeamsThisRound.Count(t => t.GameResult == GameResult.Lost);
                var teamsDrawn = TeamsThisRound.Count(t => t.GameResult == GameResult.Draw);

                var singular = "team has";
                var plural = "teams have";

                var wonText = teamsWon == 1 ? singular : plural;
                var lostText = teamsLost == 1 ? singular : plural;
                var drawText = teamsDrawn == 1 ? singular : plural;
                return string.Format("These results are invalid. {0} {1} won. {2} {3} lost. {4} {5} drawn", teamsWon, wonText, teamsLost, lostText, teamsDrawn, drawText);
            }
        }
    }
}
