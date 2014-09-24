using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat
{
    public class PlayerProvider
    {
        public List<Player> Players { get; private set; }

        public PlayerProvider()
        {
            Players = new List<Player>();

            for (int i = 0; i < 4; ++i)
            {
                for (int j = 100; j > 0; j -= 10)
                {
                    Players.Add(new Player(string.Format("{0}-{1}", j, i), j));
                }
            }
        }

        public int TotalSkill { get { return Players.Sum(p => p.Skill); } }

    }
}
