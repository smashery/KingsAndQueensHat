using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat
{
    /// <summary>
    /// Allocates a set of players to a set of teams
    /// </summary>
    public class TeamAllocator
    {
        public TeamAllocator(int numTeams)
        {
            NumTeams = numTeams;
        }

        public TeamSet CreateTeams(PlayerProvider playerProvider)
        {
            var teams = Enumerable.Range(0, NumTeams).Select(i => new Team()).ToList();

            var totalSkill = playerProvider.TotalSkill;

            var players = playerProvider.Players.Shuffle();
            var teamLoop = teams.Loop().GetEnumerator();
            foreach (var player in players)
            {
                teamLoop.MoveNext();
                teamLoop.Current.AddPlayer(player);
            }
            return new TeamSet(teams);
        }

        public int NumTeams { get; set; }
    }
}
