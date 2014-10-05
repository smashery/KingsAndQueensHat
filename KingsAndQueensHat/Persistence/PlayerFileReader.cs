using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KingsAndQueensHat.Model;

namespace KingsAndQueensHat.Persistence
{
    public class PlayerFileReader : IPlayerProvider
    {
        public List<Player> AllPlayers { get; private set; }

        public List<Player> PastPlayers { get; private set; }

        public List<Player> ActivePlayers { get; private set; }

        public PlayerFileReader(string filename)
        {
            AllPlayers = new List<Player>();
            PastPlayers = new List<Player>();
            ActivePlayers = new List<Player>();

            if (!File.Exists(filename))
            {
                throw new InvalidPlayerListException("players.csv file not found");
            }
            try
            {
                using (var reader = File.OpenText(filename))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        // Skip empty lines
                        if (line.Trim() == string.Empty)
                        {
                            continue;
                        }

                        var parts = line.Split(new[] { ',' });
                        if (parts.Length != 3)
                        {
                            throw new InvalidPlayerListException(string.Format("Every line must have three entries: Name, Gender, Skill ({0})", line));
                        }

                        var name = parts[0];

                        Gender gender;
                        if (!Enum.TryParse(parts[1], true, out gender))
                        {
                            throw new InvalidPlayerListException(string.Format("Second entry on each line must be either \"Male\" or \"Female\" ({0})", parts[1]));
                        }


                        int skill;
                        if (!int.TryParse(parts[2], out skill))
                        {
                            throw new InvalidPlayerListException(string.Format("Third entry on each line must be an integer ({0})", parts[2]));
                        }

                        var p = new Player(name, skill, gender, this.IsWinning);
                        ActivePlayers.Add(p);
                        AllPlayers.Add(p);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new InvalidPlayerListException("Cannot access players.csv file");
            }
        }

        public Player GetPastPlayer(string name, Gender gender)
        {
            var result = PastPlayers.SingleOrDefault(p => p.Name == name && p.Gender == gender);
            if (result == null)
            {
                result = new Player(name, 0, gender, this.IsWinning);
                PastPlayers.Add(result);
                AllPlayers.Add(result);
            }
            return result;
        }

    }
}
