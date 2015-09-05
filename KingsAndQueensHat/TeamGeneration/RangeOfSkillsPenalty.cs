using KingsAndQueensHat.Model;
using KingsAndQueensHat.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat.TeamGeneration
{
    /// <summary>
    /// We want teams to have a range of skill levels; beginners, experts and intermediates.
    /// Penalise potential team sets that don't have this range
    /// </summary>
    /// <remarks>
    /// I use standard deviation
    /// </remarks>
    public class RangeOfSkillsPenalty : IPenalty
    {
        /// <summary>
        /// All teams having similar standard deviation of player skills per gender makes for a good result
        /// </summary>
        public double ScorePenalty(List<Team> teams)
        {
            double result = 0;
            foreach (var gender in new[] { Gender.Male, Gender.Female })
            {
                var standardDeviations = teams.Select(t => HatMath.StdDeviation(t.Players.Where(p => p.Gender == gender).Select(p => (double)p.SkillValue).ToList())).ToList();
                // Yo dawg
                result += HatMath.StdDeviation(standardDeviations);
            }
            return result;
        }

        public double Weighting { get { return 1.0; } }
    }
}
