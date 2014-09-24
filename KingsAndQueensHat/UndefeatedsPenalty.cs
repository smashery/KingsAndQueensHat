using System;
using System.Linq;

namespace KingsAndQueensHat
{
    public class UndefeatedsPenalty : IPenalty
    {
        public int ScorePenalty(TeamSet teamSet)
        {
            var undefeatedsPerTeam = teamSet.Teams.Select(t => t.TotalUndefeateds).ToList();
            var totalUndefeateds = undefeatedsPerTeam.Sum();
            var expectedUndefeatedsPerTeam = totalUndefeateds / teamSet.TeamCount;

            // Sum the deviations from the expected team skill
            var result = undefeatedsPerTeam.Sum(s => Math.Abs(s - expectedUndefeatedsPerTeam));
            return result;
        }

        public double Weighting { get { return 1.0; } }
    }
}