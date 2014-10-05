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
        public ResultsViewModel(TournamentViewModel tournamentViewModel)
        {
            Results = new ObservableCollection<GenderRanking>();
            Results.Add(new GenderRanking(Gender.Female, tournamentViewModel.AllPlayers.Where(p => p.Gender == Gender.Female)));
            Results.Add(new GenderRanking(Gender.Male, tournamentViewModel.AllPlayers.Where(p => p.Gender == Gender.Male)));
        }

        public ObservableCollection<GenderRanking> Results { get; private set; }
    }
}
