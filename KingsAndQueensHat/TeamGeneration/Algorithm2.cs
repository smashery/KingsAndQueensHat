using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KingsAndQueensHat.Model;

namespace KingsAndQueensHat.TeamGeneration {
    class Algorithm2 {

        private List<Player> _presentPlayers;
        private List<Team> _teams;
        public bool LoggingOn { get; set; }
        public string LoggingPath { get; set; }

        public List<Team> Generate(IPlayerProvider playerProvider, int numTeams, List<HatRound> rounds) {

            PopulateRoundResults(rounds, playerProvider);

            // create teams and distribute players
            _teams = Enumerable.Range(0, numTeams).Select(i => new Team(string.Format("Team {0}", i + 1))).ToList();
            return DistributePlayers();
        }

        private void PopulateRoundResults(IEnumerable<HatRound> rounds, IPlayerProvider playerProvider) {

            Log(String.Format("\nLog time: {0}", DateTime.Now.ToString("dd MMM yyyy HH:mm:ss")));

            // reset the values
            foreach (var player in playerProvider.AllPlayers) {
                player.GamesPlayed = 0;
                player.NumberOfWins = 0;
            }

            // calculate win percentages
            foreach (var hatRound in rounds) {
                foreach (var team in hatRound.Teams) {
                    foreach (var player in team.Players) {
                        var p = playerProvider.AllPlayers.First(x => x == player);
                        if (p == null) continue;
                        p.GamesPlayed++;
                        if (team.GameResult == GameResult.Won) p.NumberOfWins++;
                    }
                }
            }

            // calculate adjusted scores

            var averageScore = Convert.ToDecimal(playerProvider.AllPlayers.Where(x => x.GamesPlayed > 0).Average(x => x.GameScore));
            foreach (var player in playerProvider.AllPlayers) {
                // adjusted score is so that players who have missed games, but are good (high win%) don't get treated like bad players
                // start with the average of the whole field
                // based on win percent factor adjust the score up or down
                // score moves by factor times number of games played

                var winPercentFactor = (player.WinPercent - 50M) / 100; //  this a negative factor for win % < 50, positive for > 50
                var adjustedScore = averageScore + winPercentFactor * player.GamesPlayed;
                player.AdjustedScore = Math.Max(player.GameScore, adjustedScore);
            }

            _presentPlayers = Sort(playerProvider.PresentPlayers().ToList());
            Log("Games played so far: " + rounds.Count() + "\n");
            foreach (var player in _presentPlayers) {
                Log(String.Format("{0}\t{4}\tPlayed:{6}\tPoints/Adj:{5}/{1}\tWin:{2}%\tXP:{3}"
                    , player.Name.PadRight(15)
                    , player.AdjustedScore.ToString("0.00").PadLeft(5)
                    , player.WinPercent.ToString("0").PadLeft(3)
                    , player.SkillLevel.Value.ToString().PadLeft(3)
                    , player.Gender
                    , player.GameScore.ToString().PadLeft(2)
                    , player.GamesPlayed.ToString().PadLeft(2)
                    ));
            }
        }

        private void Log(string lineToWrite) {
            if (LoggingOn) File.AppendAllText(LoggingPath, lineToWrite + Environment.NewLine);
        }

        private List<Team> DistributePlayers() {
            do {
                // take top x men and distribute
                AssignTeam(Sort(_presentPlayers.Where(x => x.Gender == Gender.Male).ToList()));
                // take top x women and distribute
                AssignTeam(Sort(_presentPlayers.Where(x => x.Gender == Gender.Female).ToList()), true); // true=isTopWomen on this line
                // take bottom x men and distribute
                AssignTeam(Sort(_presentPlayers.Where(x => x.Gender == Gender.Male).ToList(), false));
                // take bottom x women and distribute
                AssignTeam(Sort(_presentPlayers.Where(x => x.Gender == Gender.Female).ToList(), false));

            } while (_presentPlayers.Any());

            return _teams;
        }

        private static List<Player> Sort(List<Player> availablePlayers, bool topFirst = true) {
            var r = new Random();
            if (topFirst) {
                availablePlayers = availablePlayers
                    .OrderByDescending(x => x.AdjustedScore)
                    .ThenByDescending(x => x.WinPercent)
                    .ThenByDescending(x => x.SkillLevel.Value)
                    .ThenByDescending(x => r.Next()) // not easily testable, but this is so it isn't just alphabetic with all other things being equal
                    .ToList();
            } else {
                availablePlayers = availablePlayers
                    .OrderBy(x => x.AdjustedScore)
                    .ThenBy(x => x.WinPercent)
                    .ThenBy(x => x.SkillLevel.Value)
                    .ThenBy(x => r.Next()) // not easily testable, but this is so it isn't just alphabetic with all other things being equal
                    .ToList();
            }

            return availablePlayers;
        }

        private void AssignTeam(List<Player> players, bool isTopWomen = false) {
            for (var i = 0; i < _teams.Count; i++) { // do once for each team
                if (!players.Any()) return;
                var thisPlayer = players.First();
                GetNextTeam(isTopWomen).AddPlayer(thisPlayer);
                players.Remove(thisPlayer); // remove from the temp list we are working with now
                _presentPlayers.Remove(thisPlayer); // remove from the master player list
            }
        }

        private Team GetNextTeam(bool isTopWomen) {
            // the main idea is to get top ranked players always facing off against each other
            // TODO: put more players in first teams so that people can fill in if no-shows for later games

            // make a copy of teams and whittle down to best suited team
            var whittledTeams = _teams.ToList();
            if (isTopWomen) {
                // TODO: if teams.Count isn't even skip the last team? Need the two top women on even teams
                whittledTeams.Reverse(); // flip the sort order - so top women aren't on same team as top men
            }

            // get team with lowest number of players, lowest points totals, (first round this is teams 1->X), then by lowest ranked captain (first player)

            // knock out teams with more players
            whittledTeams.RemoveAll(x => x.PlayerCount > whittledTeams.Min(y => y.PlayerCount));
            if (whittledTeams.Count == 1) return whittledTeams.First();

            // knock out teams with more adjusted points
            whittledTeams.RemoveAll(x => x.TotalAdjustedScore > whittledTeams.Min(y => y.TotalAdjustedScore));
            if (whittledTeams.Count == 1) return whittledTeams.First();

            // knock out higher ranked FirstPlayers - only works if at least one person has been assigned
            if (whittledTeams.Any(x => x.PlayerCount > 0)) {
                whittledTeams.RemoveAll(x => x.FirstPlayer != null && x.FirstPlayer.AdjustedScore > whittledTeams.Min(y => y.FirstPlayer.AdjustedScore));
            }

            return whittledTeams.First();
        }
    }
}
