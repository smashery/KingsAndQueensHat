using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KingsAndQueensHat.Model
{
    public class PlayerEventArgs : EventArgs
    {
        public PlayerEventArgs(Player player)
        {
            Player = player;
        }

        public Player Player { get; private set; }
    }
}
