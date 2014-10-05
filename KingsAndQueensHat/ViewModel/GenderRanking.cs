using KingsAndQueensHat.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat.ViewModel
{
    public class GenderRanking
    {
        public GenderRanking(Gender gender, IEnumerable<Player> players)
        {
            Gender = gender;
            Players = new ObservableCollection<Player>(players.OrderByDescending(p => p.GameScore));
            Trace.Assert(Players.All(p => p.Gender == Gender));
        }

        public ObservableCollection<Player> Players { get; private set; }

        private Gender Gender { get; set; }

        public string GenderStr { get { return Gender.ToString(); } }
    }
}
