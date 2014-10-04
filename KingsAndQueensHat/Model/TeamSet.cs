using System;
using System.Collections.Generic;
using KingsAndQueensHat.TeamGeneration;
using System.IO;
using System.Xml.Serialization;

namespace KingsAndQueensHat.Model
{
    public class TeamSet
    {
        /// <summary>
        /// For serialisation
        /// </summary>
        protected TeamSet()
        {

        }

        public List<Team> Teams { get; private set; }

        private readonly string _filename;

        public int TeamCount
        {
            get { return Teams.Count; }
        }

        public TeamSet(List<Team> teams, string filename)
        {
            Teams = teams;
            _filename = filename;
            foreach (var team in teams)
            {
                team.OnGameDone += OnGameDone;
            }
        }

        /// <summary>
        /// When any team score is recorded
        /// </summary>
        private void OnGameDone(object sender, EventArgs eventArgs)
        {
            SaveToFile();
        }

        public void SaveToFile()
        {
            using (var stream = new FileStream(_filename, FileMode.Create))
            {
                var serialiser = new XmlSerializer(typeof (TeamSet));
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

        internal void DeleteFile()
        {
            File.Delete(_filename);
        }
    }
}
