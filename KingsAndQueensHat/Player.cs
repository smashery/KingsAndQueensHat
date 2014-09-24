using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KingsAndQueensHat
{
    public class Player
    {
        public Player(string name, int skill)
        {
            Name = name;
            Skill = skill;
        }
        public string Name { get; set; }
        public int Skill { get; set; }

        public void LostGame()
        {
            Losses++;
        }

        public int Losses { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1} (Lost {2})", Name, Skill, Losses);
        }
    }
}
