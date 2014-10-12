using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using KingsAndQueensHat.Annotations;
using KingsAndQueensHat.TeamGeneration;
using KingsAndQueensHat.Utils;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace KingsAndQueensHat.Model
{
    public class Team : INotifyPropertyChanged
    {
        public string Name { get; set; }
        private CommandHandler _wonCommand;
        private CommandHandler _lostCommand;
        private CommandHandler _drewCommand;

        public Team(string name)
        {
            Name = name;
            Players = new ObservableCollection<Player>();
            GameResult = GameResult.NoneYet;
        }

        /// <summary>
        /// For serialisation
        /// </summary>
        protected Team()
        {
        }

        public event EventHandler OnGameDone;

        public ObservableCollection<Player> Players { get; private set; }

        public GameResult GameResult { get; set; }

        /// <summary>
        /// The game result as a string
        /// </summary>
        public string GameResultStr
        {
            get 
            {
                if (GameResult == GameResult.NoneYet)
                {
                    return string.Empty;
                }
                return GameResult.ToString();
            }
        }

        /// <summary>
        /// Add a player to this team
        /// </summary>
        /// <param name="player"></param>
        public void AddPlayer(Player player)
        {
            Players.Add(player);
        }

        /// <summary>
        /// Add a player to the team after pairings have already been generated
        /// </summary>
        public void AddPlayerLate(Player player, PlayerPairings pairings)
        {
            AddPlayer(player);

            foreach (var pairing in PlayerPairings(player))
            {
                pairings.PlayedTogether(pairing);
            }
            player.GameDone(GameResult, Model.GameResult.NoneYet);
        }

        [XmlIgnore]
        public int TotalSkill
        {
            get { return Players.Sum(p => p.SkillValue); }
        }

        [XmlIgnore]
        public int PlayerCount
        {
            get { return Players.Count; }
        }

        [XmlIgnore]
        public int Men
        {
            get { return Players.Count(p => p.Gender == Gender.Male); }
        }

        [XmlIgnore]
        public int Women
        {
            get { return Players.Count(p => p.Gender == Gender.Female); }
        }

        /// <summary>
        /// The full set of pairings between all players
        /// </summary>
        public IEnumerable<PlayerPairing> PlayerPairings()
        {
            for (int i = 0; i < PlayerCount; ++i)
            {
                for (int j = i + 1; j < PlayerCount; ++j)
                {
                    yield return new PlayerPairing(Players[i], Players[j]);
                }
            }
        }

        /// <summary>
        /// The player pairings for a given player
        /// </summary>
        public IEnumerable<PlayerPairing> PlayerPairings(Player player)
        {
            foreach (var player2 in Players.Where(p => p != player))
            {
                yield return new PlayerPairing(player, player2);
            }
        }

        /// <summary>
        /// For each pair of players, record that they played together
        /// </summary>
        public void AddToPairingsCount(PlayerPairings pairings)
        {
            foreach (var playerPairing in PlayerPairings())
            {
                pairings.PlayedTogether(playerPairing);
            }
        }

        public void OnDelete(PlayerPairings pairings)
        {
            // Undo the players' scores
            UpdateGameScore(Model.GameResult.NoneYet);

            // Undo the players' pairings
            foreach (var playerPairing in PlayerPairings())
            {
                pairings.UndoPlayerPairing(playerPairing);
            }
        }

        public void GameDone(GameResult gameResult)
        {
            UpdateGameScore(gameResult);

            OnPropertyChanged("GameResultStr");
            Won.RaiseCanExecuteChanged();
            Drew.RaiseCanExecuteChanged();
            Lost.RaiseCanExecuteChanged();
            var @event = OnGameDone;
            if (@event != null)
            {
                @event(this, new EventArgs());
            }
        }

        private void UpdateGameScore(GameResult gameResult)
        {
            var oldGameResult = GameResult;
            GameResult = gameResult;
            foreach (var player in Players)
            {
                player.GameDone(gameResult, oldGameResult);
            }
        }

        public CommandHandler Won
        {
            get
            {
                return _wonCommand ?? (_wonCommand = new CommandHandler(() => GameDone(GameResult.Won), () => GameResult != GameResult.Won));
            }
        }

        public CommandHandler Drew
        {
            get
            {
                return _drewCommand ?? (_drewCommand = new CommandHandler(() => GameDone(GameResult.Draw), () => GameResult != GameResult.Draw));
            }
        }

        public CommandHandler Lost
        {
            get
            {
                return _lostCommand ?? (_lostCommand = new CommandHandler(() => GameDone(GameResult.Lost), () => GameResult != GameResult.Lost));
            }
        }

        public override string ToString()
        {
            return string.Format("{0} Men, {1} Women. Skill: {2}", Men, Women, TotalSkill);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Permanently delete a player from the team
        /// </summary>
        internal void RemovePlayer(Player player, PlayerPairings pairings)
        {
            Players.Remove(player);

            // Undo the players' pairings
            foreach (var playerPairing in PlayerPairings(player))
            {
                pairings.UndoPlayerPairing(playerPairing);
            }
        }
    }
}
