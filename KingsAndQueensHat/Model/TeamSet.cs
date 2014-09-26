using System.Collections.Generic;
using KingsAndQueensHat.TeamGeneration;

namespace KingsAndQueensHat.Model
{
    public class TeamSet
    {
        public IList<Team> Teams { get; set; }

        public int TeamCount
        {
            get { return Teams.Count; }
        }

        public TeamSet(IList<Team> teams)
        {
            Teams = teams;
        }

        public void AddRoundToPairingCount(PlayerPairings pairings)
        {
            foreach (var team in Teams)
            {
                team.AddToPairingsCount(pairings);
            }
        }
    }
}
