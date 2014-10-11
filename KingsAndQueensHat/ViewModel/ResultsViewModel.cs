using KingsAndQueensHat.Annotations;
using KingsAndQueensHat.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat.ViewModel
{
    public class ResultsViewModel : INotifyPropertyChanged
    {
        private Tournament _tournament;
        public ResultsViewModel(Tournament tournament)
        {
            _tournament = tournament;
            _tournament.GameDone += (sender, args) => Refresh();
            _tournament.PlayerDataChanged += (sender, args) => Refresh();
            Results = new ObservableCollection<GenderRanking>();
            Refresh();
        }

        private void Refresh()
        {
            Results.Clear();
            Results.Add(new GenderRanking(Gender.Female, _tournament.AllPlayers.Where(p => p.Gender == Gender.Female)));
            Results.Add(new GenderRanking(Gender.Male, _tournament.AllPlayers.Where(p => p.Gender == Gender.Male)));
            OnPropertyChanged("ProblematicResults");
        }

        public bool ProblematicResults
        {
            get
            {
                return _tournament.Rounds.Any(r => r.ProblematicResults);
            }
        }

        public ObservableCollection<GenderRanking> Results { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
