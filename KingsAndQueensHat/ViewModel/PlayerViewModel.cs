using KingsAndQueensHat.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat.ViewModel
{
    public class PlayerViewModel
    {
        public PlayerViewModel(Tournament tournament)
        {
            Tournament = tournament;
        }

        public Tournament Tournament { get; private set; }

        public ObservableCollection<Player> ActivePlayers
        {
            get { return Tournament.ActivePlayers; }
        }

        public ObservableCollection<Player> AllPlayers
        {
            get { return Tournament.AllPlayers; }
        }
    }
}
