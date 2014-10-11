using KingsAndQueensHat.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat.ViewModel
{
    public class ResultsViewModel
    {
        private Tournament _tournament;
        public ResultsViewModel(Tournament tournament)
        {
            _tournament = tournament;
            _tournament.GameDone += (sender, args) => Refresh();
            Results = new ObservableCollection<GenderRanking>();
            Refresh();
        }

        private void Refresh()
        {
            Results.Clear();
            Results.Add(new GenderRanking(Gender.Female, _tournament.AllPlayers.Where(p => p.Gender == Gender.Female)));
            Results.Add(new GenderRanking(Gender.Male, _tournament.AllPlayers.Where(p => p.Gender == Gender.Male)));
        }


        public ObservableCollection<GenderRanking> Results { get; private set; }
    }
}
