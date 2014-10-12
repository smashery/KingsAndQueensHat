using KingsAndQueensHat.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KingsAndQueensHat.Model
{
    public class TournamentSettings
    {
        private TournamentPersistence _storage;
        public TournamentSettings(TournamentPersistence storage)
        {
            _storage = storage;
            NumberOfGenerations = 1000000;
            Initialise();
            Save();
        }

        protected TournamentSettings()
        {

        }

        private void Initialise()
        {
            SkillLevels = new ObservableCollection<SkillLookup>();
            SkillLevels.Add(new SkillLookup { Name = "Novice", Skill = 10 });
            SkillLevels.Add(new SkillLookup { Name = "Beginner", Skill = 30 });
            SkillLevels.Add(new SkillLookup { Name = "Intermediate", Skill = 50 });
            SkillLevels.Add(new SkillLookup { Name = "Experienced", Skill = 80 });
            SkillLevels.Add(new SkillLookup { Name = "Guru", Skill = 100 });

            foreach (var sl in SkillLevels)
            {
                sl.PropertyChanged += (sender, args) => Save();
            }
        }

        private int _numberOfGenerations;
        public int NumberOfGenerations
        {
            get
            {
                return _numberOfGenerations;
            }
            set
            {
                if (value != _numberOfGenerations)
                {
                    _numberOfGenerations = value;
                    Save();
                }
            }
        }

        public ObservableCollection<SkillLookup> SkillLevels
        {
            get; set;
        }

        public bool SkillLevelExists(string skillLevel)
        {
            return SkillLevels.Any(sl => sl.Name == skillLevel);
        }

        public int SkillValueOf(string skillLevel)
        {
            return SkillLevel(skillLevel).Skill;
        }

        internal SkillLookup SkillLevel(string skill)
        {
            return SkillLevels.Single(sl => sl.Name == skill);
        }

        private void Save()
        {
            if (_storage != null)
            {
                var filename = _storage.SettingsPath;
                using (var stream = new FileStream(filename, FileMode.Create))
                {
                    var serializer = new XmlSerializer(typeof(TournamentSettings));
                    serializer.Serialize(stream, this);
                }
            }
        }

        internal static TournamentSettings Load(TournamentPersistence storage)
        {
            var filename = storage.SettingsPath;
            if (!File.Exists(filename))
            {
                return new TournamentSettings(storage);
            }

            using (var stream = new FileStream(filename, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TournamentSettings));
                var result = serializer.Deserialize(stream) as TournamentSettings;
                result._storage = storage;
                return result;
            }
        }
    }
}
