using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using KingsAndQueensHat.Persistence;
using KingsAndQueensHat.TeamGeneration;

namespace KingsAndQueensHat.Model
{
    /// <summary>
    /// Root of the model
    /// </summary>
    public class Tournament
    {
        public Tournament()
        {
            PlayerProvider = new PlayerFileReader(@"TestData.csv");
            Players = new ObservableCollection<Player>(PlayerProvider.Players.OrderByDescending(p => p.Skill));
            Rounds = new ObservableCollection<TeamSet>();
            Teams = new ObservableCollection<Team>();
        }

        private IPlayerProvider PlayerProvider { get; set; }

        public ObservableCollection<Player> Players { get; private set; }

        public ObservableCollection<TeamSet> Rounds { get; private set; }

        public ObservableCollection<Team> Teams { get; private set; }



        public void CreateNewRound()
        {
            var teamCreator = new RoundCreator();
            var penalty1 = new UnevenSkillPenalty();
            var penalty2 = new PlayerPairings();
            var penalty3 = new TooManyWinnersPenalty(PlayerProvider);
            var penalties = new IPenalty[] { penalty1, penalty2, penalty3 };

            var teams = teamCreator.CreateApproximatelyOptimalTeams(penalties, PlayerProvider);
            teams.AddRoundToPairingCount(penalty2);
            Rounds.Add(teams);

            Teams.Clear();
            foreach (Team team in teams.Teams)
            {
                Teams.Add(team);
            }
        }
    }
}