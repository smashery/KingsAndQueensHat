using System;
using System.Linq;
using System.Windows.Documents;
using KingsAndQueensHat.Model;
using KingsAndQueensHat.Utils;
using System.Collections.Generic;

namespace KingsAndQueensHat.TeamGeneration
{
    /// <summary>
    /// Penalise team sets that have too many winners of the same gender
    /// on the same team as each other
    /// </summary>
    public class TooManyWinnersPenalty : IPenalty
    {
        private readonly IPlayerProvider _players;

        public TooManyWinnersPenalty(IPlayerProvider players)
        {
            _players = players;
        }

        public double ScorePenalty(List<Team> teams)
        {
            return ScorePenaltyForGender(teams, Gender.Male)
                 + ScorePenaltyForGender(teams, Gender.Female);
        }

        public double ScorePenaltyForGender(List<Team> teams, Gender gender)
        {
            var maxScore = _players.MaxGameScore(gender);

            var winningPerTeam = teams.Select(t => t.Players.Count(p => p.GameScore == maxScore)).ToList();

            var totalWinning = winningPerTeam.Sum();
            var expectedWinnersPerTeam = totalWinning / (double)teams.Count;

            // Sum the square root of the deviations from the expected number of winners
            // Square root shrinks the dynamic range, so as to make the better options
            // more standout after normalisation
            var result = winningPerTeam.Sum(s => Math.Sqrt(Math.Abs(s - expectedWinnersPerTeam)));
            return result;
        }

        public double Weighting { get { return 1.0; } }
    }
}