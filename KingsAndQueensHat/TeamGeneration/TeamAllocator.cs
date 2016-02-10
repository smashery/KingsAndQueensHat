using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using KingsAndQueensHat.Model;
using KingsAndQueensHat.Utils;
using System;

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

            // We restart the snaking per gender so that players should have good matchups. However, this has
            // the possible side effect of creating one team with 2 more players than their opponent (one more 
            // male, one more female). We detect this possibility here and prevent it by iterating through the teams backwards
            var needToReverseSecond = NeedToReverseSecond(playerProvider);

            // Randomly spread out all the guru male players
            // Then the guru female players
            // Then the experienced males
            // Then the experienced females
            // etc.
            var groupedByGender = playerProvider.PresentPlayers().GroupBy(p => p.Gender);
            var thenGroupedByExperience = groupedByGender.Select(g => g.GroupBy(p => p.SkillLevel).OrderByDescending(g2 => g2.Key.Value));
            foreach (var group in thenGroupedByExperience)
            {
                List<Player> players = new List<Player>();
                foreach (var subGroup in group)
                {
                    players.AddRange(subGroup.ToList().Shuffle());
                }

                var teamLoop = teams.Snake().GetEnumerator();
                foreach (var player in players)
                {
                    teamLoop.MoveNext();
                    teamLoop.Current.AddPlayer(player);
                }
                if (needToReverseSecond)
                {
                    teams.Reverse();
                }
            }
            
            return teams;
        }

        private bool NeedToReverseSecond(IPlayerProvider playerProvider)
        {
            // Since the snake resets every two passes through the teams, we modulo by that to see which direction the snake is going
            // on its last leg
            var numMalePlayers = playerProvider.PresentPlayers().Count(p => p.Gender == Gender.Male) % (NumTeams * 2);
            var maleGoingRight = numMalePlayers < NumTeams;
            var numFemalePlayers = playerProvider.PresentPlayers().Count(p => p.Gender == Gender.Female) % (NumTeams * 2) - NumTeams;
            var femaleGoingRight = numFemalePlayers < NumTeams;

            // If both going the same direction at the end, we need to reverse the allocation of the second gender to ensure we don't
            // have too many people on a team
            return maleGoingRight == femaleGoingRight;
        }

        public int NumTeams { get; set; }
    }
}
