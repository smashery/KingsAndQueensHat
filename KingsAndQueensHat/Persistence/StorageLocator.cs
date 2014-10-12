using KingsAndQueensHat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat.Persistence
{
    public class TournamentPersistence
    {
        public TournamentPersistence(string name)
        {
            Name = name;
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
        }

        public string Name { get; private set; }

        private string Path
        {
            get
            {
                return System.IO.Path.Combine(Constants.StorageDirectory(), Name);
            }
        }

        public IEnumerable<string> GetHatRoundPaths()
        {
            return Directory.EnumerateFiles(Path, string.Format("*{0}", Constants.DataFileExtension));

        }

        internal string PlayerListFilename
        {
            get { return System.IO.Path.Combine(Path, "players.xml"); }
        }

        internal string SettingsPath
        {
            get { return System.IO.Path.Combine(Path, "settings.xml"); }
        }

        internal string GetNextHatRoundPath()
        {
            string filename;
            int i = 1;
            do
            {
                filename = System.IO.Path.Combine(Path, string.Format("{0}{1}", i, Constants.DataFileExtension));
                i++;
            } while (File.Exists(filename));
            return filename;
        }
    }
}
