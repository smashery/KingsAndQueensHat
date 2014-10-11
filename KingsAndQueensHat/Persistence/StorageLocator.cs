using KingsAndQueensHat.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat.Persistence
{
    public class StorageLocator
    {
        private string _path;

        public StorageLocator(string path)
        {
            _path = path;
        }

        public IEnumerable<string> GetHatRoundPaths()
        {
            return Directory.EnumerateFiles(_path, string.Format("*{0}", Constants.DataFileExtension));

        }

        internal IPlayerProvider GetPlayerProvider()
        {
            return new PlayerFileReader(Path.Combine(_path, "players.csv"));
        }

        internal string GetNextHatRoundPath()
        {
            string filename;
            int i = 1;
            do
            {
                filename = string.Format("{0}{1}", i, Constants.DataFileExtension);
                i++;
            } while (File.Exists(filename));
            return filename;
        }
    }
}
