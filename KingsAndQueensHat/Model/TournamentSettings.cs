using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat.Model
{
    public class TournamentSettings
    {
        public TournamentSettings()
        {
            NumberOfGenerations = 1000000;
            Initialise();
        }

        public event EventHandler SkillsChanged;

        private void Initialise()
        {
            SkillLevels = new ObservableCollection<SkillLookup>();
            SkillLevels.Add(new SkillLookup { Name = "Novice", Skill = 10 });
            SkillLevels.Add(new SkillLookup { Name = "Beginner", Skill = 30 });
            SkillLevels.Add(new SkillLookup { Name = "Intermediate", Skill = 50 });
            SkillLevels.Add(new SkillLookup { Name = "Experienced", Skill = 80 });
            SkillLevels.Add(new SkillLookup { Name = "Guru", Skill = 100 });
        }

        public int NumberOfGenerations { get; set; }

        public ObservableCollection<SkillLookup> SkillLevels
        {
            get; private set;
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
    }
}
