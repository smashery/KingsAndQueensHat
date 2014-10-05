using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KingsAndQueensHat.Model;

namespace KingsAndQueensHat.TeamGeneration
{
    /// <summary>
    /// Allow Team sets to be penalised for having uneven skill levels
    /// </summary>
    /// <remarks>
    /// This algorithm sums the skills of each team, returning a large 
    /// penalty if there is a big difference between them
    /// Because some teams will have an extra player, they will have a 
    /// slightly lower average skill level; but the benefit of having an
    /// extra sub balances this; so to keep things simple, I just use
    /// the sum of skills for each team.
    /// </remarks>
    public class UnevenSkillPenalty : IPenalty
    {
        public double ScorePenalty(List<Team> teams)
        {
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