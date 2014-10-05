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

            // Sum the deviations from the expected team skill
            var result = winningPerTeam.Sum(s => Math.Abs(s - expectedWinnersPerTeam));
            return result;
        }

        public double Weighting { get { return 1.0; } }
    }
}