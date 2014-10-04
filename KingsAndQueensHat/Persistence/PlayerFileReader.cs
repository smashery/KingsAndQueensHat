using System;
using System.Collections.Generic;
using System.IO;
using KingsAndQueensHat.Model;

namespace KingsAndQueensHat.Persistence
{
    public class PlayerFileReader : IPlayerProvider
    {
        public List<Player> Players { get; private set; }

        public PlayerFileReader(string filename)
        {
            Players = new List<Player>();
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

                        Players.Add(new Player(name, skill, gender, this.IsWinning));
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new InvalidPlayerListException("Cannot access players.csv file");
            }
        }

    }
}
