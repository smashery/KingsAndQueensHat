using System;
using System.Linq;
using System.Collections.Generic;
using KingsAndQueensHat.TeamGeneration;
using System.IO;
using System.Xml.Serialization;

namespace KingsAndQueensHat.Model
{
    public class HatRound
    {
        /// <summary>
        /// For serialisation
        /// </summary>
        protected HatRound()
        {

        }

        public List<Team> Teams { get; private set; }

        private readonly string _filename;

        public int TeamCount
        {
            get { return Teams.Count; }
        }

        public HatRound(List<Team> teams, string filename)
        {
            Teams = teams;
            _filename = filename;
            foreach (var team in teams)
            {
                team.OnGameDone += OnGameDone;
            }
        }

        public event EventHandler GameDone;

        /// <summary>
        /// When any team score is recorded
        /// </summary>
        private void OnGameDone(object sender, EventArgs eventArgs)
        {
            SaveToFile();

            // Guard against race conditions
            var gameDone = GameDone;
            if (gameDone != null)
            {
                gameDone(sender, eventArgs);
            }
        }

        public void SaveToFile()
        {
            using (var stream = new FileStream(_filename, FileMode.Create))
            {
                var serialiser = new XmlSerializer(typeof (HatRound));
                serialiser.Serialize(stream, this);
            }
        }

        public void AddRoundToPairingCount(PlayerPairings pairings)
        {
            foreach (var team in Teams)
            {
                team.AddToPairingsCount(pairings);
            }
        }

        internal void Delete(PlayerPairings pairings)
        {
            File.Delete(_filename);
            foreach (var team in Teams)
            {
                team.OnDelete(pairings);
            }
        }

        public bool ProblematicResults
        {
            get
            {
                var numTeams = Teams.Count;
                var numWins = Teams.Count(t => t.GameResult == GameResult.Won);
                var numDraws = Teams.Count(t => t.GameResult == GameResult.Draw);
                var numLosses = Teams.Count(t => t.GameResult == GameResult.Lost);

                // More than half the teams can't win or lose
                if (numWins > numTeams / 2)
                {
                    return true;
                }
                if (numLosses > numTeams / 2)
                {
                    return true;
                }

                // Furthermore, if all games have been completed
                if (Teams.All(t => t.GameResult != GameResult.NoneYet))
                {
                    // Number of wins must equal number of losses
                    if (numWins != numLosses)
                    {
                        return true;
                    }
                    // And must be an even number of draws
                    if (numDraws % 2 != 0)
                    {
                        return true;
                    }
                }

                // Everything shiny, captain
                return false;
            }
        }
    }
}
