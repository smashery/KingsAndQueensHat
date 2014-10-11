using KingsAndQueensHat.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat.ViewModel
{
    public class TournamentSelectionViewModel
    {
        public ObservableCollection<PersistedTournament> Tournaments { get; private set; }

        public string BaseDir;

        public TournamentSelectionViewModel()
        {
            BaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KingAndQueenHat", "Tournaments");
            Directory.CreateDirectory(BaseDir);
            var directories = Directory.EnumerateDirectories(BaseDir);
            Tournaments = new ObservableCollection<PersistedTournament>(directories.Select(d => new PersistedTournament(Path.GetFileName(d))));
            foreach (var tourney in Tournaments)
            {
                tourney.Delete += DeleteTourney;
                tourney.Open += OpenTourney;
            }
            TournamentName = Enumerable.Range(1, int.MaxValue).Select(i => string.Format("New Tournament {0}", i)).FirstOrDefault(name => !Tournaments.Any(t => string.Equals(name, t.Name, StringComparison.CurrentCultureIgnoreCase)));
        }

        public event EventHandler Open;

        void OpenTourney(object sender, EventArgs e)
        {
            Open(sender, e);
        }

        void DeleteTourney(object sender, EventArgs e)
        {
            var tourney = sender as PersistedTournament;
            Tournaments.Remove(tourney);
            Directory.Delete(Path.Combine(BaseDir, tourney.Name));
        }

        public string TournamentName { get; set; }

        internal bool CanCreateTournament()
        {
            return !Tournaments.Any(t => t.Name.Equals(TournamentName, StringComparison.CurrentCultureIgnoreCase));
        }

        public StorageLocator GetStorageLocator(string name)
        {
            return new StorageLocator(Path.Combine(BaseDir, name));
        }
    }
}
