using System.Linq;
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

        public TeamSet CreateTeams(IPlayerProvider playerProvider)
        {
            var teams = Enumerable.Range(0, NumTeams).Select(i => new Team()).ToList();

            var totalSkill = playerProvider.TotalSkill();

            var males = playerProvider.Players.Where(p => p.Gender == Gender.Male).ToList().Shuffle();
            var females = playerProvider.Players.Where(p => p.Gender == Gender.Female).ToList().Shuffle();

            // Allocate all males, then all females
            var players = males.Concat(females);

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
