using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using KingsAndQueensHat.Model;
using KingsAndQueensHat.Utils;

namespace KingsAndQueensHat.TeamGeneration
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

        public List<Team> CreateTeams(IPlayerProvider playerProvider)
        {
            var teams = Enumerable.Range(0, NumTeams).Select(i => new Team(string.Format("Team {0}", i + 1))).ToList();

            var males = playerProvider.ActivePlayers.Where(p => p.Gender == Gender.Male).ToList().Shuffle();
            var females = playerProvider.ActivePlayers.Where(p => p.Gender == Gender.Female).ToList().Shuffle();

            // Allocate all males, then all females
            var players = males.Concat(females);

            var teamLoop = teams.Loop().GetEnumerator();
            foreach (var player in players)
            {
                teamLoop.MoveNext();
                teamLoop.Current.AddPlayer(player);
            }
            return teams;
        }

        public int NumTeams { get; set; }
    }
}
