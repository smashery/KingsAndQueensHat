using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingsAndQueensHat.Model;

namespace KingsAndQueensHat.TeamGeneration
{
    /// <summary>
    /// Penalise team allocations that leave some teams with a particular dominant gender
    /// , while others do not
    /// </summary>
    public class UnevenGenderSkillPenalty : IPenalty
    {
        public double ScorePenalty(List<Team> teams)
        {
            var scores = teams.Select(team => team.GenderSkills).ToList();
            int result = 0;
            foreach (var gender in new[] { Gender.Male, Gender.Female })
            {
                var total = scores.Sum(s => s[gender]);
                var teamCount = teams.Count;
                var expectedTeamGenderSkill = total / teamCount;

                // Sum the deviations from the expected team skill
                result += scores.Sum(s => Math.Abs(s[gender] - expectedTeamGenderSkill));
            }
            return result;
        }

        public double Weighting { get { return 1.0; } }
    }
}
