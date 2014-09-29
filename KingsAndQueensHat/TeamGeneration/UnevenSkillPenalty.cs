using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KingsAndQueensHat.Model;

namespace KingsAndQueensHat.TeamGeneration
{
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