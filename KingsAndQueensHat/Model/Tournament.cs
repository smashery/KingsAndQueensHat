﻿using System.Collections.Generic;
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

namespace KingsAndQueensHat.Model
{
    /// <summary>
    /// Root of the model
    /// </summary>
    public class Tournament : INotifyPropertyChanged
    {
        public Tournament()
        {
            PlayerProvider = new PlayerFileReader(@"TestData.csv");
            Players = new ObservableCollection<Player>(PlayerProvider.Players.OrderByDescending(p => p.GetSkill()));
            Rounds = new ObservableCollection<TeamSet>();
            Teams = new ObservableCollection<Team>();
        }

        private IPlayerProvider PlayerProvider { get; set; }

        public ObservableCollection<Player> Players { get; private set; }

        public ObservableCollection<TeamSet> Rounds { get; private set; }
        public ObservableCollection<Team> Teams { get; private set; }

        public int NumRounds { get { return Rounds.Count; }}

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
                    var teams = doc.SelectNodes("/TeamSet/Teams/Team");
                    var teamList = new List<Team>();
                    foreach (XmlNode team in teams)
                    {
                        var teamName = team.SelectSingleNode("Name").InnerText;
                        Team t = new Team(teamName);
                        var players = team.SelectNodes("Players/Player");
                        foreach (XmlNode player in players)
                        {
                            var name = player.SelectSingleNode("Name").InnerText;
                            Player p = PlayerProvider.PlayerWithName(name);
                            if (p != null)
                            {
                                t.AddPlayer(p);
                            }
                        }
                        GameResult gameResult;
                        if (Enum.TryParse(team.SelectSingleNode("GameResult").InnerText, out gameResult))
                        {
                            if (gameResult != GameResult.NoneYet)
                            {
                                t.GameDone(gameResult);
                            }
                        }

                        teamList.Add(t);
                    }
                    var round = new TeamSet(teamList, file);
                    AddRound(round);
                }
                catch (Exception)
                {
                    // Let's just be overly lenient here
                }
            }
        }

        private void AddRound(TeamSet round)
        {
            SetCurrentRound(round);
            Rounds.Add(round);
            round.AddRoundToPairingCount(_playerPairings);
            OnPropertyChanged("NumRounds");
            OnPropertyChanged("CanDeleteRound");
        }

        /// <summary>
        /// Set the UI to the current set of teams
        /// </summary>
        private void SetCurrentRound(TeamSet round)
        {
            Teams.Clear();
            if (round != null)
            {
                foreach (Team team in round.Teams)
                {
                    Teams.Add(team);
                }
            }
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
            var penalties = new IPenalty[] { penalty1, _playerPairings, penalty3 };

            var teams = await teamCreator.CreateApproximatelyOptimalTeams(penalties, PlayerProvider, numTeamGens, teamCount, cancel);
            
            string filename;
            int i = 1;
            do
            {
                filename = string.Format("{0}.hatround", i);
                i++;
            } while (File.Exists(filename));

            var round = new TeamSet(teams, filename);
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

        public bool CanDeleteRound
        {
            get { return NumRounds > 0; }
        }

        internal void DeleteLastRound()
        {
            var lastRound = Rounds.LastOrDefault();
            if (lastRound == null)
            {
                return;
            }

            lastRound.DeleteFile();
            Rounds.Remove(lastRound);
            var newTeams = Rounds.LastOrDefault();
            SetCurrentRound(newTeams);
            OnPropertyChanged("NumRounds");
            OnPropertyChanged("CanDeleteRound");
        }

        internal void DeleteAllData()
        {
            while (Rounds.Count > 0)
            {
                var lastRound = Rounds.Last();
                lastRound.DeleteFile();
                Rounds.Remove(lastRound);
            }
            SetCurrentRound(null);
            OnPropertyChanged("NumRounds");
            OnPropertyChanged("CanDeleteRound");
        }

        internal bool AllTeamsHaveResults()
        {
            return Teams == null ||
                   Teams.All(t => t.GameResult != GameResult.NoneYet);
        }
    }
}