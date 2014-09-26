using System;
using System.Xml.Serialization;
namespace KingsAndQueensHat.Model
{
    public class Player
    {
        public Player(string name, int skill, Gender gender)
        {
            Name = name;
            Skill = skill;
            Gender = gender;
            GameScore = 0;
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
