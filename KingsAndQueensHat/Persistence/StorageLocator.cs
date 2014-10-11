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
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public IEnumerable<string> GetHatRoundPaths()
        {
            return Directory.EnumerateFiles(_path, string.Format("*{0}", Constants.DataFileExtension));

        }

        internal string PlayerListFilename
        {
            get { return Path.Combine(_path, "players.xml"); }
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
