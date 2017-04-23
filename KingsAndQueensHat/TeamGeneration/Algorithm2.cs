using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingsAndQueensHat.Model;

namespace KingsAndQueensHat.TeamGeneration {
	class Algorithm2 {

		private List<Player> _players;
		private List<Team> _teams;
		// bit of a magic value, used for AdjustedScores
		// new players get half of the top score, plus (this value multiplied by the top score multiplied by their Win Percentage)
		private const decimal WinPercentBonusFactor = 0.25M; 
 
		public List<Team> Generate(IPlayerProvider playerProvider, int numTeams, List<HatRound> rounds) {

			_players = playerProvider.PresentPlayers().ToList();
			PopulateRoundResults(rounds);
			
			// create teams and distribute players
			_teams = Enumerable.Range(0, numTeams).Select(i => new Team(string.Format("Team {0}", i + 1))).ToList();
			return DistributePlayers();
		}

		private void PopulateRoundResults(IEnumerable<HatRound> rounds) {

			// reset the values
			foreach (var player in _players) {
				player.GamesPlayed = 0;
				player.NumberOfWins = 0;
			}
			
			// calculate win percentages
			foreach (var hatRound in rounds) {
				foreach (var team in hatRound.Teams) {
					foreach (var player in team.Players) {
						var p = _players.Find(x => x == player);
						if (p == null) continue; // player might not be in available list for this round
						p.GamesPlayed++;
						if (team.GameResult == GameResult.Won) p.NumberOfWins++;
					}
				}
			}
			// now calculate everyone's adjusted score
			var topScore = _players.Max(x => x.GameScore);
			foreach (var player in _players) {
				var adjustedScore = Convert.ToInt32(Convert.ToInt32(topScore/2) + topScore*WinPercentBonusFactor*player.WinPercent/100);
				player.AdjustedScore = Math.Max(player.GameScore, adjustedScore);
			}
		}

		private List<Team> DistributePlayers() {
			// the main idea is to get top ranked players always facing off against each other
			// TODO: put more players in first teams so that people can fill in if no-shows for later games

			do {
				// take top x men and distribute
				AssignTeam(Sort(_players.Where(x => x.Gender == Gender.Male).ToList()));
				// take top x women and distribute
				var isTopWomen = true;
				AssignTeam(Sort(_players.Where(x => x.Gender == Gender.Female).ToList()), isTopWomen);
				// take bottom x men and distribute
				AssignTeam(Sort(_players.Where(x => x.Gender == Gender.Male).ToList(), false));
				// take bottom x women and distribute
				AssignTeam(Sort(_players.Where(x => x.Gender == Gender.Female).ToList(), false));

			} while (_players.Any());

			return _teams;
		}
		
		private static List<Player> Sort(List<Player> availablePlayers, bool topFirst = true) {
			// adjusted points allows for sorting people who have missed games - they come in at halfway plus a little bit
			// - they get half the current leader's points + (magic number winPercentBonus * their winPercent)
			// - so if they've won most of the games they have played they are higher than someone who has lost most

			if (topFirst) {
				availablePlayers = availablePlayers.OrderByDescending(x => x.AdjustedScore)
				    .ThenByDescending(x => x.WinPercent)
				    .ThenByDescending(x => x.SkillLevel.Value)
				    .ToList();
			}
			else {
				availablePlayers = availablePlayers.OrderBy(x => x.AdjustedScore)
					.ThenBy(x => x.WinPercent)
					.ThenBy(x => x.SkillLevel.Value)
					.ToList();
			}

			return availablePlayers;
		}

		private void AssignTeam(List<Player> players, bool isTopWomen = false) {
			foreach (Team t in _teams) { // do once for each team
				if (!players.Any()) return;
				var thisPlayer = players.First();
				GetNextTeam(isTopWomen).AddPlayer(thisPlayer);
				players.Remove(thisPlayer); // remove from the temp list we are working with now
				_players.Remove(thisPlayer); // remove from the master player list
			}
		}

		private Team GetNextTeam(bool isTopWomen) {
			// make a copy of teams and whittle down to best suited team
			var whittledTeams = _teams.ToList();
			
			// get team with lowest number of players, lowest points totals, (first round this is teams 1->X), then by lowest ranked captain (first player)
			
			// knock out teams with more players
			whittledTeams.RemoveAll(x => x.PlayerCount > whittledTeams.Min(y => y.PlayerCount));
			if (whittledTeams.Count == 1) return whittledTeams.First();

			// knock out teams with more adjusted points
			whittledTeams.RemoveAll(x => x.TotalAdjustedScore > whittledTeams.Min(y => y.TotalAdjustedScore));
			if (whittledTeams.Count == 1) return whittledTeams.First();

			// knock out higher ranked captains - only works if at least one person has been assigned
			if (whittledTeams.Any(x => x.PlayerCount > 0)) {
				whittledTeams.RemoveAll(x => x.Captain != null && x.Captain.AdjustedScore > whittledTeams.Min(y => y.Captain.AdjustedScore));
			}

			if (isTopWomen) {
				whittledTeams.Reverse(); // flip the sort order - so top women aren't on same team as top men
			}

			return whittledTeams.First();
		}
	}
}
