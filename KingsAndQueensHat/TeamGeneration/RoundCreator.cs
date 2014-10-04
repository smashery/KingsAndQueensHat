using System.Collections.Generic;
using System.Linq;
using KingsAndQueensHat.Model;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace KingsAndQueensHat.TeamGeneration
{
    public class RoundCreator
    {
        /// <summary>
        /// Temporary, "working" result for team set selection
        /// </summary>
        private class PossibleTeamSet
        {
            public PossibleTeamSet(List<Team> teams, List<double> scores)
            {
                Teams = teams;
                Scores = scores;
            }
            public List<Team> Teams;
            public List<double> Scores;

            public double NormalisedScore;
        }

        public async Task<List<Team>> CreateApproximatelyOptimalTeams(IList<IPenalty> penaltyScorers, IPlayerProvider playerProvider, int numTeamGens, int numTeams, CancellationToken cancel)
        {
            List<PossibleTeamSet> teamsAndScores = await GeneratePossibleTeamSets(penaltyScorers, playerProvider, numTeamGens, numTeams, cancel);

            SetNormalisedScores(teamsAndScores, penaltyScorers);

            // Return the smallest one
            var smallestValue = teamsAndScores.Min(tps => tps.NormalisedScore);
            var result = teamsAndScores.First(x => x.NormalisedScore == smallestValue);
            return result.Teams;
        }

        /// <summary>
        /// Determine the dynamic range for each penalty (0..1) and add that to the normalised score for each
        /// </summary>
        /// <remarks>
        /// This step is a lot faster
        /// </remarks>
        private static void SetNormalisedScores(List<PossibleTeamSet> teamsAndScores, IList<IPenalty> penaltyScorers)
        {
            for (int penaltyIndex = 0; penaltyIndex < penaltyScorers.Count; penaltyIndex++)
            {
                var weighting = penaltyScorers[penaltyIndex].Weighting;
                var min = teamsAndScores.Min(x => x.Scores[penaltyIndex]);
                double range = teamsAndScores.Max(x => x.Scores[penaltyIndex]) - min;
                if (range == 0)
                {
                    continue;
                }

                foreach (var possibleResult in teamsAndScores)
                {
                    var normalisedScore = (possibleResult.Scores[penaltyIndex] - min) / range;
                    possibleResult.NormalisedScore += normalisedScore * weighting;
                }
            }
        }

        /// <summary>
        /// Generate a lot of possible team sets.
        /// </summary>
        /// <remarks>
        /// This step can take a while
        /// </remarks>
        private static Task<List<PossibleTeamSet>> GeneratePossibleTeamSets(IList<IPenalty> penaltyScorers, IPlayerProvider playerProvider, int numTeamGens, int numTeams, CancellationToken cancel)
        {
            return Task.Run(() =>
                {
                    var allocator = new TeamAllocator(numTeams);

                    List<PossibleTeamSet> teamsAndScores = new List<PossibleTeamSet>();

                    // Do all the heavy lifting up front
                    while (!cancel.IsCancellationRequested && teamsAndScores.Count < numTeamGens)
                    {
                        var teams = allocator.CreateTeams(playerProvider);
                        var scores = penaltyScorers.Select(pS => pS.ScorePenalty(teams)).ToList();
                        teamsAndScores.Add(new PossibleTeamSet(teams, scores));

                    }
                    return teamsAndScores;
                });
        }
    }
}
