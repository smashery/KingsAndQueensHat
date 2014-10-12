using System.ComponentModel;
using System.Runtime.CompilerServices;
using KingsAndQueensHat.Annotations;
using System;
using System.Xml.Serialization;
using KingsAndQueensHat.Utils;
namespace KingsAndQueensHat.Model
{
    public class Player : INotifyPropertyChanged
    {
        private Func<Player, bool> _isWinning;
        private TournamentSettings _settings;
        public Player(string name, string skill, Gender gender, bool currentlyPresent, TournamentSettings settings, Func<Player, bool> isWinning)
        {
            Name = name;
            Skill = skill;
            Gender = gender;
            CurrentlyPresent = currentlyPresent;
            GameScore = 0;
            _settings = settings;
            _isWinning = isWinning;
        }

        /// <summary>
        /// For serialization
        /// </summary>
        protected Player()
        {

        }

        /// <summary>
        /// For deserialization
        /// </summary>
        internal void RewireWinningFunction(Func<Player, bool> isWinning)
        {
            _isWinning = isWinning;
        }

        public event EventHandler OnCurrentlyPresentChanged;
        public event EventHandler<PlayerEventArgs> OnDelete;
        
        public string Name { get; set; }

        public Gender Gender { get; set; }

        public string Skill { get; set; }

        [XmlIgnore]
        public int SkillValue
        {
            get
            {
                return _settings.SkillValueOf(Skill);
            }
        }

        private bool _currentlyPresent;
        public bool CurrentlyPresent
        {
            get { return _currentlyPresent; }
            set
            {
                if (value != _currentlyPresent)
                {
                    _currentlyPresent = value;
                    var @event = OnCurrentlyPresentChanged;
                    if (@event != null)
                    {
                        @event(this, new EventArgs());
                    }
                }
            }
        }

        [XmlIgnore]
        public int GameScore { get; private set; }

        [XmlIgnore]
        public bool PotentialMonarch
        {
            get
            {
                return _isWinning(this);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} (Score {2})", Name, Skill, GameScore);
        }

        public void GameDone(GameResult gameResult, GameResult oldGameResult)
        {
            // Undo the old game result
            GameScore -= ScoreFor(oldGameResult);

            // Apply the new game result
            GameScore += ScoreFor(gameResult);

            OnPropertyChanged("GameScore");
        }

        private int ScoreFor(GameResult result)
        {
            switch (result)
            {
                case GameResult.Won:
                    return 3;
                case GameResult.Draw:
                    return 2;
                case GameResult.Lost:
                    return 1;
                default:
                    return 0;
            }
        }

        private CommandHandler _deleteCommand;
        public CommandHandler Delete
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new CommandHandler(FireDelete, () => true));
            }
        }

        private void FireDelete()
        {
            var @event = OnDelete;
            if (@event != null)
            {
                @event(this, new PlayerEventArgs(this));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Hacky solution to the problem of updating our potential monarchitude
        /// </summary>
        internal void ForceUpdate()
        {
            OnPropertyChanged("PotentialMonarch");
        }
    }
}
