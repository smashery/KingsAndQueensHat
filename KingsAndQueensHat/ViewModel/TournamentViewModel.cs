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

            CurrentRoundViewModel = new RoundViewModel(Tournament);

            // Start looking at the last round
            SetCurrentRound(NumRounds);
        }

        public RoundViewModel CurrentRoundViewModel { get; private set; }

        public Tournament Tournament { get; private set; }

        public string Test { get { return "Hi"; } }

        public ObservableCollection<Player> ActivePlayers
        {
            get { return Tournament.ActivePlayers; }
        }

        public ObservableCollection<Player> AllPlayers
        {
            get { return Tournament.AllPlayers; }
        }

        public int NumRounds { get { return Tournament.Rounds.Count; } }

        public int CurrentRoundNumber { get; private set; }

        internal void CreateNewRound(int teamCount, Func<Task, CancellationTokenSource, ICancelDialog> cancelDialogFactory)
        {
            if (teamCount % 2 != 0)
            {
                throw new ArgumentException("teamCount must be even");
            }
            var source = new CancellationTokenSource();
            var task = Tournament.CreateNewRound(teamCount, source.Token);
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

            // roundNumber is 1-based
            var roundIndex = roundNumber - 1;
            HatRound round;
            if (roundIndex == -1)
            {
                round = null;
            }
            else
            {
                round = Tournament.Rounds[roundIndex];
            }

            CurrentRoundViewModel.SetCurrentRound(round);
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
    }
}
