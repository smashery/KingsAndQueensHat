using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KingsAndQueensHat.Model;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace KingsAndQueensHat.Persistence
{
    public class PlayerListFile : IPlayerProvider
    {
        public ObservableCollection<Player> AllPlayers { get; private set; }

        private TournamentPersistence _storage;

        private TournamentSettings _settings;

        public PlayerListFile(TournamentPersistence storage, TournamentSettings settings)
        {
            _storage = storage;
            _settings = settings;
            AllPlayers = new ObservableCollection<Player>();
            Load();
        }

        public event EventHandler<PlayerEventArgs> PlayerDeleted;

        public void Load()
        {
            var filename = _storage.PlayerListFilename;
            if (!File.Exists(filename))
            {
                return;
            }

            using (var stream = new FileStream(filename, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Player>));
                var players = serializer.Deserialize(stream) as List<Player>;
                foreach (var p in players.OrderBy(p => p.Name))
                {
                    AllPlayers.Add(p);
                }
            }

            foreach (var player in AllPlayers)
            {
                player.RewireWinningFunction(this.IsWinning);
                WireEvents(player);
            }
        }

        private void WireEvents(Player player)
        {
            player.OnCurrentlyPresentChanged += playerPresenceChanged;
            player.OnDelete += playerDelete;
        }

        void playerDelete(object sender, PlayerEventArgs e)
        {
            AllPlayers.Remove(e.Player);
            SaveToFile();
            var @event = PlayerDeleted;
            if (@event != null)
            {
                @event(sender, e);
            }
        }

        void playerPresenceChanged(object sender, EventArgs e)
        {
            SaveToFile();
        }

        public void SaveToFile()
        {
            var filename = _storage.PlayerListFilename;
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Player>));
                serializer.Serialize(stream, AllPlayers.ToList());
            }
        }

        public Player NewPlayer(string name, Gender gender, string skill)
        {
            var player = new Player(name, skill, gender, true, _settings, this.IsWinning);

            AddPlayerAtCorrectPosition(player);
            WireEvents(player);
            SaveToFile();
            return player;
        }

        private void AddPlayerAtCorrectPosition(Player player)
        {
            // Find the place to insert them
            var index = AllPlayers.Concat(new[] { player }).OrderBy(p => p.Name).ToList().IndexOf(player);
            AllPlayers.Insert(index, player);
        }

        public void ImportFromCsv(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new InvalidPlayerListException(string.Format("{0}: File not found", filename));
            }
            var newPlayers = new List<Player>();
            try
            {
                using (var reader = File.OpenText(filename))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        // Skip empty lines
                        if (line.Trim() == string.Empty)
                        {
                            continue;
                        }

                        var parts = line.Split(new[] { ',' });
                        if (parts.Length != 3)
                        {
                            throw new InvalidPlayerListException(string.Format("Every line must have three entries: Name, Gender, Skill ({0})", line));
                        }

                        var name = parts[0];

                        Gender gender;
                        if (!Enum.TryParse(parts[1], true, out gender))
                        {
                            throw new InvalidPlayerListException(string.Format("Second entry on each line must be either \"Male\" or \"Female\" ({0})", parts[1]));
                        }


                        string skill = parts[2];
                        if (!_settings.SkillLevelExists(skill))
                        {
                            throw new InvalidPlayerListException(string.Format("Unknown skill: {0}", skill));
                        }

                        newPlayers.Add(new Player(name, skill, gender, true, _settings, this.IsWinning));
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new InvalidPlayerListException(string.Format("Cannot access file: {0}", filename));
            }

            foreach (var player in newPlayers)
            {
                // Skip any duplicates
                if (!AllPlayers.Any(p => p.Name.Equals(player.Name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    WireEvents(player);
                    AddPlayerAtCorrectPosition(player);
                }
            }

            SaveToFile();
        }
    }
}
