using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat
{
    public class Team
    {
        public Team()
        {
            Players = new List<Player>();
        }

        public List<Player> Players { get; private set; }

        public void AddPlayer(Player player)
        {
            Players.Add(player);
        }

        public int TotalSkill
        {
            get { return Players.Sum(p => p.Skill); }
        }
        
        public int PlayerCount
        {
            get { return Players.Count; }
        }

        public int Men
        {
            get { return Players.Count(p => p.Gender == Gender.Male); }
        }

        public int Women
        {
            get { return Players.Count(p => p.Gender == Gender.Female); }
        }


        public IEnumerable<PlayerPairing> PlayerPairings()
        {
            for (int i = 0; i < PlayerCount; ++i)
            {
                for (int j = i + 1; j < PlayerCount; ++j)
                {
                    yield return new PlayerPairing(Players[i], Players[j]);
                }
            }
        }

        /// <summary>
        /// For each pair of players, record that they played together
        /// </summary>
        public void AddToPairingsCount(PlayerPairings pairings)
        {
            foreach (var playerPairing in PlayerPairings())
            {
                pairings.PlayedTogether(playerPairing);
            }
        }

        public void Won()
        {
            foreach (var player in Players)
            {
                player.WonGame();
            }
        }

        public override string ToString()
        {
            return string.Format("{0} Men, {1} Women. Skill: {2}", Men, Women, TotalSkill);
        }
    }
}
