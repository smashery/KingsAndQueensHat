using KingsAndQueensHat.Annotations;
using KingsAndQueensHat.Model;
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
    public class RoundManagerViewModel : INotifyPropertyChanged
    {
        public RoundManagerViewModel(Tournament tournament)
        {
            Tournament = tournament;
            CurrentRoundViewModel = new RoundViewModel(Tournament);

            // Start looking at the last round
            SetCurrentRound(NumRounds);

            Initialise();
        }

        private void Initialise()
        {
            // Set an initial meaningful value for player count; at least 10 players per team, with even number of teams
            int numberOfTeams;
            if (NumRounds > 0)
            {
                numberOfTeams = CurrentRoundViewModel.CurrentNumberOfTeams;
            }
            else
            {
                var numberOfPlayers = Tournament.PlayerProvider.PresentPlayers().Count();
                numberOfTeams = ((numberOfPlayers / 20) * 2);
            }
            numberOfTeams = Math.Max(2, numberOfTeams);
            TeamCountStr = numberOfTeams.ToString();

        }

        public Tournament Tournament { get; private set; }
        public RoundViewModel CurrentRoundViewModel { get; private set; }

        public ObservableCollection<Player> AllPlayers
        {
            get { return Tournament.AllPlayers; }
        }

        public int NumRounds { get { return Tournament.Rounds.Count; } }

        public int CurrentRoundNumber { get; private set; }

        public string TeamCountStr { get; set; }

        public int TeamCount
        {
            get
            {
                int result;
                if (int.TryParse(TeamCountStr, out result))
                {
                    return result;
                }
                return -1;
            }
        }

        internal void CreateNewRound(Func<Task, CancellationTokenSource, ICancelDialog> cancelDialogFactory, Action<string> errorAction)
        {
            if (TeamCount > 0 && TeamCount % 2 != 0)
            {
                throw new ArgumentException("Number of teams must be even");
            }
            var source = new CancellationTokenSource();
            var task = Tournament.CreateNewRound(TeamCount, source.Token);
            var cancelDialog = cancelDialogFactory(task, source);
            cancelDialog.ShowUntilCompleteOrCancelled();
            if (task.IsFaulted)
            {
                errorAction(task.Exception.InnerException.Message);
            }
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

        internal void ExportThisRound(string filename)
        {
            Tournament.ExportRoundToCsv(CurrentRoundNumber - 1, filename);
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
