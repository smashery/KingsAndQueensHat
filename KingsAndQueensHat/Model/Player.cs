using System.ComponentModel;
using System.Runtime.CompilerServices;
using KingsAndQueensHat.Annotations;
using System;
using System.Xml.Serialization;
namespace KingsAndQueensHat.Model
{
    public class Player : INotifyPropertyChanged
    {
        private Func<Player, bool> _isWinning;
        public Player(string name, int skill, Gender gender, Func<Player, bool> isWinning)
        {
            Name = name;
            Skill = skill;
            Gender = gender;
            GameScore = 0;
            _isWinning = isWinning;
        }

        protected Player()
        {

        }
        
        public string Name { get; set; }
        [XmlIgnore]
        public Gender Gender { get; set; }

        private int Skill { get; set; }

        public int GetSkill()
        {
            return Skill;
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
                    return 2;
                case GameResult.Draw:
                    return 1;
                default:
                    return 0;
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
