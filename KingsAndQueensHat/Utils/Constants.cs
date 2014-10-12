using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat.Utils
{
    public static class Constants
    {
        public const string DataFileExtension = ".hat";

        public const string PlayerFile = "players.csv";
        public const string TitleBarText = "Hat Tournament Generator";

        public static string VersionText()
        {
            var assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

        public static string StorageDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KingAndQueenHat", "Tournaments");
        }
    }
}
