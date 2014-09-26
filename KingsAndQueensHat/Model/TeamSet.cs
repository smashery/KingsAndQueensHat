using System.Collections.Generic;
using KingsAndQueensHat.TeamGeneration;
using System.IO;
using System.Xml.Serialization;

namespace KingsAndQueensHat.Model
{
    public class TeamSet
    {
        public TeamSet()
        {

        }

        public List<Team> Teams { get; set; }

        public int TeamCount
        {
            get { return Teams.Count; }
        }

        public TeamSet(List<Team> teams)
        {
            Teams = teams;
        }

        public void SaveToFile(Stream stream)
        {
            var serialiser = new XmlSerializer(typeof(TeamSet));
            serialiser.Serialize(stream, this);
        }

        public void AddRoundToPairingCount(PlayerPairings pairings)
        {
            foreach (var team in Teams)
            {
                team.AddToPairingsCount(pairings);
            }
        }
    }
}
