using System.Collections.Generic;
using System.Linq;
using KingsAndQueensHat.Model;
using System;
using System.Collections.ObjectModel;

namespace KingsAndQueensHat
{
    public interface IPlayerProvider
    {
        /// <summary>
        /// Any player who has played a game in the tournament
        /// </summary>
        ObservableCollection<Player> AllPlayers { get; }

        void ImportFromCsv(string filename);

        Player NewPlayer(string NewPlayerName, Gender gender, string skill);

        event EventHandler<PlayerEventArgs> RemovePlayerFromRound;
    }

    public static class PlayerProviderExtensions
    {
        public static IEnumerable<Player> WinningPlayers(this IPlayerProvider playerProvider, Gender gender)
        {
            return playerProvider.AllPlayers.Where(p => p.Gender == gender && p.GameScore == playerProvider.MaxGameScore(gender));
        }

        public static int MaxGameScore(this IPlayerProvider playerProvider, Gender gender)
        {
            var genderPlayers = playerProvider.AllPlayers.Where(p => p.Gender == gender).ToList();
            if (genderPlayers.Any())
            {
                return genderPlayers.Max(p => p.GameScore);
            }
            return 0;
        }

        public static bool IsWinning(this IPlayerProvider playerProvider, Player player)
        {
            return player.GameScore > 0 && playerProvider.WinningPlayers(player.Gender).Contains(player);
        }

        public static IEnumerable<Player> PresentPlayers(this IPlayerProvider playerProvider)
        {
            return playerProvider.AllPlayers.Where(p => p.CurrentlyPresent);
        }

        public static Player GetPlayer(this IPlayerProvider playerProvider, string name)
        {
            return playerProvider.AllPlayers.SingleOrDefault(p => p.Name == name);
        }

        public static bool PlayerExists(this IPlayerProvider playerProvider, string name)
        {
            return playerProvider.GetPlayer(name) != null;
        }
    }
}