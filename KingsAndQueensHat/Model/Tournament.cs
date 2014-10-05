using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using KingsAndQueensHat.Annotations;
using KingsAndQueensHat.Persistence;
using KingsAndQueensHat.TeamGeneration;
using System.IO;
using System.Xml;
using System;
using System.Threading;
using System.Threading.Tasks;
using KingsAndQueensHat.Utils;
using System.Diagnostics;

namespace KingsAndQueensHat.Model
{
    /// <summary>
    /// Root of the model
    /// </summary>
    public class Tournament : INotifyPropertyChanged
    {
        public Tournament(IPlayerProvider playerProvider)
        {
            PlayerProvider = playerProvider;
            ActivePlayers = new ObservableCollection<Player>(PlayerProvider.ActivePlayers);
            AllPlayers = new ObservableCollection<Player>(PlayerProvider.AllPlayers);
            Rounds = new ObservableCollection<HatRound>();
        }

        private IPlayerProvider PlayerProvider { get; set; }

        public ObservableCollection<Player> ActivePlayers { get; private set; }
        public ObservableCollection<Player> AllPlayers { get; private set; }

        public ObservableCollection<HatRound> Rounds { get; private set; }

        public event EventHandler GameDone;

        public void LoadExistingData()
        {
            Rounds.Clear();
            _playerPairings.Clear();

            var files = Directory.EnumerateFiles(".", "*.hatround");
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
                            var genderStr = player.SelectSingleNode("Gender").InnerText;
                            Gender gender = (Gender)Enum.Parse(typeof(Gender), genderStr);
                            Player p = PlayerProvider.GetActivePlayer(name, gender);

                            // Player list may have changed throughout the day, so accept this difference
                            if (p == null)
                            {
                                p = PlayerProvider.GetPastPlayer(name, gender);
                            }
                            t.AddPlayer(p);
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

            // Ensure that if any new players were discovered, we're aware of them
            AllPlayers.Clear();
            foreach (var p in PlayerProvider.AllPlayers)
            {
                AllPlayers.Add(p);
            }
        }

        private void AddRound(HatRound round)
        {
            Rounds.Add(round);
            round.AddRoundToPairingCount(_playerPairings);

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
        private PlayerPairings _playerPairings = new PlayerPairings();
		
        /// <summary>
        /// Create a new set of teams
        /// </summary>
        /// <param name="speed">Optimisation hint (0..100)</param>
        /// <param name="teamCount">The number of teams to generate</param>
        public async Task CreateNewRound(double speed, int teamCount, CancellationToken cancel)
        {
            // Run between 5000 and ~1000000 attempts

            var numTeamGens = (int)(speed * 10000 + 5000);

            var teamCreator = new RoundCreator();
            var penalty1 = new UnevenSkillPenalty();
            var penalty3 = new TooManyWinnersPenalty(PlayerProvider);
            var penalty4 = new RangeOfSkillsPenalty();
            var penalties = new IPenalty[] { penalty1, _playerPairings, penalty3, penalty4 };

            var teams = await teamCreator.CreateApproximatelyOptimalTeams(penalties, PlayerProvider, numTeamGens, teamCount, cancel);
            
            string filename;
            int i = 1;
            do
            {
                filename = string.Format("{0}.hatround", i);
                i++;
            } while (File.Exists(filename));

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
                lastRound.Delete(_playerPairings);
                Rounds.Remove(lastRound);
            }
            var pairings = _playerPairings.NumberOfPairings;
            Trace.Assert(pairings == 0);
        }

        internal void DeleteRound(int roundNum)
        {
            var round = Rounds[roundNum];
            round.Delete(_playerPairings);
            Rounds.RemoveAt(roundNum);

        }
    }
}