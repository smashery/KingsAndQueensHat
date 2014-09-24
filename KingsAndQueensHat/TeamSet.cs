using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat
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
