using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KingsAndQueensHat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var teamCreator = new RoundCreator();
            var playerProvider = new PlayerFileReader(@"TestData.csv");

            var penalty1 = new UnevenSkillPenalty();
            var penalty2 = new PlayerPairings();
            var penalty3 = new TooManyWinnersPenalty(playerProvider);
            var penalties = new IPenalty[] { penalty1, penalty2, penalty3 };

            for (int i = 0; i < 10; ++i)
            {
                var teams = teamCreator.CreateApproximatelyOptimalTeams(penalties, playerProvider);
                teams.AddRoundToPairingCount(penalty2);
                var repairings = penalty2.NumberOfRepairings;
                
                // Two arbitrary teams lose
                teams.Teams[0].Won();
                teams.Teams[1].Won();
            }
        }
    }
}
