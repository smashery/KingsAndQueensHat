using KingsAndQueensHat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat.ViewModel
{
    public class SettingsViewModel
    {
        private TournamentSettings _settings;

        public SettingsViewModel(TournamentSettings settings)
        {
            _settings = settings;
        }

        public int NumberOfGenerations
        {
            get
            {
                return _settings.NumberOfGenerations;
            }
            set
            {
                _settings.NumberOfGenerations = value;
            }
        }

        public IEnumerable<SkillLevel> SkillLevels
        {
            get
            {
                return _settings.SkillLevels;
            }
        }
    }
}
