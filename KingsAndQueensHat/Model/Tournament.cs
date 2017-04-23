using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using KingsAndQueensHat.Annotations;
using KingsAndQueensHat.Persistence;
using KingsAndQueensHat.TeamGeneration;
using System.Xml;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace KingsAndQueensHat.Model
{
    /// <summary>
    /// Root of the model
    /// </summary>
    public class Tournament : INotifyPropertyChanged
    {
        public Tournament(TournamentPersistence storageLocator)
        {
            _storage = storageLocator;
            LoadSettings();

            var provider = new PlayerListFile(_storage, Settings);
            PlayerProvider = provider;
            PlayerPairings = new PlayerPairings();
            provider.PlayerDeleted += PlayerDeleted;
            Rounds = new ObservableCollection<HatRound>();
        }

        private void LoadSettings()
        {
            Settings = TournamentSettings.Load(_storage);
        }

        void PlayerDeleted(object sender, PlayerEventArgs e)
        {
            DeletePlayerFromTournament(e.Player);
        }

        public TournamentSettings Settings { get; private set; }

        private TournamentPersistence _storage;

        internal IPlayerProvider PlayerProvider { get; set; }

        public ObservableCollection<Player> AllPlayers { get { return PlayerProvider.AllPlayers; } }

        public ObservableCollection<HatRound> Rounds { get; private set; }

        public event EventHandler GameDone;

        public event EventHandler PlayerDataChanged;

        public void LoadExistingData()
        {
            Rounds.Clear();
            PlayerPairings.Clear();

            var files = _storage.GetHatRoundPaths();
            foreach (var file in files)
            {
                try
                {
                    var doc = new XmlDocument();
                    using (var stream = new FileStream(file, FileMode.Open))
                    {
                        doc.Load(stream);
                    }
                    var teams = doc.SelectNodes("/HatRound/Teams/Team");

                    // Basic validation
                    if (teams.Count == 0 || teams.Count % 2 != 0)
                    {
                        throw new Exception();
                    }

                    var teamList = new List<Team>();
                    foreach (XmlNode team in teams)
                    {
                        var teamName = team.SelectSingleNode("Name").InnerText;
                        Team t = new Team(teamName);
                        var players = team.SelectNodes("Players/Player");

                        // Basic validation
                        if (players.Count == 0)
                        {
                            throw new Exception();
                        }

                        foreach (XmlNode player in players)
                        {
                            var name = player.SelectSingleNode("Name").InnerText;
                            Player p = PlayerProvider.GetPlayer(name);

                            // Player list may have changed throughout the day, so accept this difference
                            if (p != null)
                            {
                                t.AddPlayer(p);
                            }
                        }
                        GameResult gameResult = (GameResult)Enum.Parse(typeof(GameResult), team.SelectSingleNode("GameResult").InnerText);
                        if (gameResult != GameResult.NoneYet)
                        {
                            t.GameDone(gameResult);
                        }

                        teamList.Add(t);
                    }
                    var round = new HatRound(teamList, file);
                    AddRound(round);
                }
                catch (Exception)
                {
                    throw new InvalidRoundException(string.Format("Round file {0} is an invalid file", file));
                }
            }
        }

        private void AddRound(HatRound round)
        {
            Rounds.Add(round);
            round.AddRoundToPairingCount(PlayerPairings);

            round.GameDone += (sender, args) =>
                {
                    // Guard against race conditions
                    var gameDone = GameDone;
                    if (gameDone != null)
                    {
                        gameDone(sender, args);
                    }
                };
        }

        /// <summary>
        /// A record of who has played with whom
        /// </summary>
        public PlayerPairings PlayerPairings;
		
        /// <summary>
        /// Create a new set of teams
        /// </summary>
        /// <param name="teamCount">The number of teams to generate</param>
        public async Task CreateNewRound(int teamCount, CancellationToken cancel) {
	        var teams = new List<Team>();
			
			if (Settings.Algorithm2) {
				var algo2 = new Algorithm2();
				teams = algo2.Generate(PlayerProvider, teamCount, Rounds.ToList());
			}
			else {
				var numTeamGens = Settings.NumberOfGenerations;

				var teamCreator = new RoundCreator();
				var winnersPenalty = new TooManyWinnersPenalty(PlayerProvider);
				var penalties = new IPenalty[] { PlayerPairings, winnersPenalty };

				teams = await teamCreator.CreateApproximatelyOptimalTeams(penalties, PlayerProvider, numTeamGens, teamCount, cancel);
			}

            var filename = _storage.GetNextHatRoundPath();

            var round = new HatRound(teams, filename);
            round.SaveToFile();

            AddRound(round);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void DeleteAllData()
        {
            while (Rounds.Count > 0)
            {
                var lastRound = Rounds.Last();
                lastRound.Delete(PlayerPairings);
                Rounds.Remove(lastRound);
            }
            var pairings = PlayerPairings.NumberOfPairings;
            Trace.Assert(pairings == 0);
        }

        internal void DeleteRound(int roundNum)
        {
            var round = Rounds[roundNum];
            round.Delete(PlayerPairings);
            Rounds.RemoveAt(roundNum);
        }

        internal void ExportRoundToCsv(int roundNum, string filename)
        {
            Rounds[roundNum].ExportToCsv(filename);
        }

        internal void AddPlayerToLastRound(Player player)
        {
            var round = Rounds.Last();
            round.AddPlayer(player, PlayerPairings);
            var @event = PlayerDataChanged;
            if (@event != null)
            {
                @event(this, new EventArgs());
            }
        }

        internal void DeletePlayerFromTournament(Player player)
        {
            foreach (var round in Rounds)
            {
                round.DeletePlayer(player, PlayerPairings);
            }

            var @event = PlayerDataChanged;
            if (@event != null)
            {
                @event(this, new EventArgs());
            }
        }
    }
}