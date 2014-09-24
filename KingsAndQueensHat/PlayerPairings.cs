using System;
using System.Collections.Generic;
using System.Linq;

namespace KingsAndQueensHat
{
    /// <summary>
    /// Records numbers of games that a pair of players have played together previously
    /// </summary>
    public class PlayerPairings : IPenalty
    {
        private readonly Dictionary<PlayerPairing, int> _roundsPlayedTogether = new Dictionary<PlayerPairing, int>();

        private const int FudgeFactor = 10;

        public int ScorePenalty(TeamSet teamSet)
        {
            var penalty = 0;
            var sum = 0;
            foreach (var team in teamSet.Teams)
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
            if (gamesTogether < 1)
            {
                return 0;
            }

            // This formula is set up such that one pair playing together three times is worse than 
            // two playing together twice.
            return FudgeFactor * (int)Math.Pow(2, (gamesTogether)) - FudgeFactor / 2;
        }

        public void PlayedTogether(PlayerPairing playerPairing)
        {
            if (!_roundsPlayedTogether.ContainsKey(playerPairing))
            {
                _roundsPlayedTogether[playerPairing] = 0;
            }
            _roundsPlayedTogether[playerPairing] += 1;
        }

        public double Weighting { get { return 1.0; } }
        public int NumberOfRepairings { get { return _roundsPlayedTogether.Sum(r => r.Value - 1); } }
    }
}