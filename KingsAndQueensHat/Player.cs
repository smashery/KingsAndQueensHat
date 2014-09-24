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
        public Player(string name, int skill, Gender gender)
        {
            Name = name;
            Skill = skill;
            Gender = gender;
        }
        public string Name { get; set; }
        public int Skill { get; set; }
        public Gender Gender { get; set; }

        public void WonGame()
        {
            Wins++;
        }

        public int Wins { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}: {1} (Won {2})", Name, Skill, Wins);
        }
    }
}
