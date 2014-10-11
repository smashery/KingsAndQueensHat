

using KingsAndQueensHat.Annotations;
using KingsAndQueensHat.Model;
using KingsAndQueensHat.Persistence;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace KingsAndQueensHat.ViewModel
{
    /// <summary>
    /// ViewModel for the tournament class
    /// </summary>
    public class TournamentViewModel : INotifyPropertyChanged
    {
        public TournamentViewModel()
        {
            Tournament = new Tournament(new StorageLocator("."));
            Tournament.LoadExistingData();

            ResultsViewModel = new ResultsViewModel(Tournament);
            RoundManagerViewModel = new RoundManagerViewModel(Tournament);
            PlayerViewModel = new PlayerViewModel(Tournament);
        }

        public RoundManagerViewModel RoundManagerViewModel { get; private set; }

        public ResultsViewModel ResultsViewModel { get; private set; }

        public PlayerViewModel PlayerViewModel { get; private set; }

        public Tournament Tournament { get; private set; }
        
        public ObservableCollection<Player> ActivePlayers
        {
            get { return Tournament.ActivePlayers; }
        }

        public ObservableCollection<Player> AllPlayers
        {
            get { return Tournament.AllPlayers; }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
