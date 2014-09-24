using System;
using System.Linq;

namespace KingsAndQueensHat
{
    public class UnevenSkillPenalty : IPenalty
    {
        public double ScorePenalty(TeamSet teamSet)
        {
            var teams = teamSet.Teams;
            var scores = teams.Select(team => team.TotalSkill).ToList();
            var total = scores.Sum();
            var teamCount = teams.Count;
            var expectedTeamSkill = total / teamCount;

            // Sum the deviations from the expected team skill
            var result = scores.Sum(s => Math.Abs(s - expectedTeamSkill));
            return result;
        }

        public double Weighting { get { return 1.0; } }

    }
}