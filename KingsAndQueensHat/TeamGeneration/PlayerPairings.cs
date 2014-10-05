using System;
using System.Collections.Generic;
using System.Linq;
using KingsAndQueensHat.Model;

namespace KingsAndQueensHat.TeamGeneration
{
    /// <summary>
    /// Records numbers of games that a pair of players have played together previously
    /// </summary>
    public class PlayerPairings : IPenalty
    {
        private readonly Dictionary<PlayerPairing, int> _roundsPlayedTogether = new Dictionary<PlayerPairing, int>();

        public double ScorePenalty(List<Team> teamSet)
        {
            var penalty = 0;
            var sum = 0;
            foreach (var team in teamSet)
            {
                foreach (var playerPairing in team.PlayerPairings())
                {
                    int gamesTogether;
                    if (_roundsPlayedTogether.TryGetValue(playerPairing, out gamesTogether))
                    {
                        penalty += ScorePenaltyFor(gamesTogether);
                        sum += gamesTogether;
                    }
                }
            }
            return penalty;
        }

        private int ScorePenaltyFor(int gamesTogether)
        {
            if (gamesTogether == 0)
            {
                return 0;
            }

            // Playing with the same people 3 or 4 times is really bad in a short hat tournament
            // So make the penalty for each successive game together 3 times higher than the last
            return (int)Math.Pow(3, (gamesTogether));
        }

        public void PlayedTogether(PlayerPairing playerPairing)
        {
            if (!_roundsPlayedTogether.ContainsKey(playerPairing))
            {
                _roundsPlayedTogether[playerPairing] = 0;
            }
            _roundsPlayedTogether[playerPairing] += 1;
        }

        public void UndoPlayerPairing(PlayerPairing playerPairing)
        {
            _roundsPlayedTogether[playerPairing] -= 1;
        }

        public double Weighting { get { return 1.0; } }

        /// <summary>
        /// The sum of (1 for each game that a pair play in where they have already played together once before)
        /// </summary>
        public int NumberOfRepairings { get { return _roundsPlayedTogether.Sum(r => Math.Max(0, r.Value - 1)); } }

        public int NumberOfPairings { get { return _roundsPlayedTogether.Sum(r => r.Value); } }

        public void Clear()
        {
            _roundsPlayedTogether.Clear();
        }
    }
}