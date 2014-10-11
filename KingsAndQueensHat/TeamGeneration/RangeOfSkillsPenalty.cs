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
        /// All teams having similar standard deviation of player skills makes for a good result
        /// </summary>
        public double ScorePenalty(List<Team> teams)
        {
            var standardDeviations = teams.Select(t => HatMath.StdDeviation(t.Players.Select(p => (double)p.Skill).ToList())).ToList();
            var x = teams.Select(t => t.Players.Select(p => p.Skill)).ToList();
            // Yo dawg
            var result = HatMath.StdDeviation(standardDeviations);
            return result;
        }

        public double Weighting { get { return 1.0; } }
    }
}
