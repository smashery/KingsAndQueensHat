using KingsAndQueensHat.Annotations;
using KingsAndQueensHat.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat.ViewModel
{
    public class RoundViewModel : INotifyPropertyChanged
    {
        private Tournament Tournament;

        public RoundViewModel(Tournament tournament)
        {
            Tournament = tournament;
            Tournament.GameDone += (sender, args) => WonLossChanged();

            TeamsThisRound = new ObservableCollection<Team>();
        }

        public int CurrentNumberOfTeams
        {
            get
            {
                if (CurrentRound == null)
                {
                    return 0;
                }
                return CurrentRound.TeamCount;
            }
        }

        private HatRound CurrentRound { get; set; }

        public ObservableCollection<Team> TeamsThisRound { get; private set; }

        public ObservableCollection<Player> AllPlayers
        {
            get { return Tournament.AllPlayers; }
        }

        public void SetCurrentRound(HatRound round)
        {
            CurrentRound = round;
            OnPropertyChanged("ProblematicResults");
            OnPropertyChanged("ProblematicText");

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

        internal void WonLossChanged()
        {
            OnPropertyChanged("ProblematicResults");
            OnPropertyChanged("ProblematicText");
            foreach (var player in AllPlayers)
            {
                player.ForceUpdate();
            }
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
