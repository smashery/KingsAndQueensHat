using System;
using System.Linq;
using System.Windows.Documents;
using KingsAndQueensHat.Model;
using System.Collections.Generic;

namespace KingsAndQueensHat.TeamGeneration
{
    public class TooManyWinnersPenalty : IPenalty
    {
        private readonly IPlayerProvider _players;

        public TooManyWinnersPenalty(IPlayerProvider players)
        {
            _players = players;
        }

        public double ScorePenalty(List<Team> teamSet)
        {
            return ScorePenaltyForGender(teamSet, Gender.Male)
                 + ScorePenaltyForGender(teamSet, Gender.Female);
        }

        public double ScorePenaltyForGender(List<Team> teamSet, Gender gender)
        {
            var maxScore = _players.MaxGameScore(gender);

            var winningPerTeam = teamSet.Select(t => t.Players.Count(p => p.GameScore == maxScore)).ToList();

            var totalWinning = winningPerTeam.Sum();
            var expectedWinnersPerTeam = totalWinning / (double)teamSet.Count;

            // Sum the deviations from the expected team skill
            var result = winningPerTeam.Sum(s => Math.Abs(s - expectedWinnersPerTeam));
            return result;
        }

        /// <summary>
        /// Having too many winners is less important than other factors
        /// </summary>
        public double Weighting { get { return 0.5; } }
    }
}