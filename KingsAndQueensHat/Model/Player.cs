using System;
using System.Xml.Serialization;
namespace KingsAndQueensHat.Model
{
    public class Player
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

        [XmlIgnore]
        public int Skill { get; set; }

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

        public void GameDone(GameResult gameResult)
        {
            switch (gameResult)
            {
                case GameResult.Won:
                    GameScore += 2;
                    break;
                case GameResult.Draw:
                    GameScore += 1;
                    break;
            }
        }
    }
}
