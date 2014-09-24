using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat
{
    public class RoundCreator
    {
        public TeamSet CreateApproximatelyOptimalTeams(IList<IPenalty> penaltyScorers, IPlayerProvider playerProvider)
        {
            var allocator = new TeamAllocator(NumTeams);
            var allPossibleTeamSets = Enumerable.Range(0, NumberToTest).Select(x => allocator.CreateTeams(playerProvider)).ToList();

            var totalProportionateScores = allPossibleTeamSets.ToDictionary(teamSet => teamSet, teamSet => 0.0);

            foreach (var penaltyScorer in penaltyScorers)
            {
                var scores = allPossibleTeamSets.Select(teamSet => new {teamSet, score= penaltyScorer.ScorePenalty(teamSet)}).ToList();
                var min = scores.Min(x => x.score);
                var range = scores.Max(x => x.score) - min;
                if (range == 0)
                {
                    continue;
                }

                // Represent the score as a value from 0 to 1, based on how far across the range of values it was.
                var scoresWithinRange = scores.Select(x => new {x.teamSet, proportion=(x.score - min) / (double)range});
                foreach (var teamSetAndProportion in scoresWithinRange)
                {
                    totalProportionateScores[teamSetAndProportion.teamSet] += teamSetAndProportion.proportion * penaltyScorer.Weighting;
                }
            }

            // Return the smallest one
            var smallestValue = totalProportionateScores.Min(tps => tps.Value);
            return totalProportionateScores.First(x => x.Value == smallestValue).Key;
        }

        public const int NumberToTest = 30000;
        const int NumTeams = 4;
    }
}
