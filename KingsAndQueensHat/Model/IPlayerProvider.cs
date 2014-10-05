using System.Collections.Generic;
using System.Linq;
using KingsAndQueensHat.Model;
using System;

namespace KingsAndQueensHat
{
    public interface IPlayerProvider
    {
        /// <summary>
        /// Players currently available for team selection
        /// </summary>
        List<Player> ActivePlayers { get; }

        /// <summary>
        /// Any player who has played a game in the tournament
        /// </summary>
        List<Player> AllPlayers { get; }

        /// <summary>
        /// Players that were in previous rounds but are no longer
        /// </summary>
        List<Player> PastPlayers { get; }

        /// <summary>
        /// Get or create a player who played before but is not in the current roster of players
        /// </summary>
        Player GetPastPlayer(string name, Gender gender);
    }

    public static class PlayerProviderExtensions
    {
        public static IEnumerable<Player> WinningPlayers(this IPlayerProvider playerProvider, Gender gender)
        {
            return playerProvider.AllPlayers.Where(p => p.Gender == gender && p.GameScore == playerProvider.MaxGameScore(gender));
        }

        public static int MaxGameScore(this IPlayerProvider playerProvider, Gender gender)
        {
            return playerProvider.AllPlayers.Where(p => p.Gender == gender).Max(p => p.GameScore);
        }

        public static bool IsWinning(this IPlayerProvider playerProvider, Player player)
        {
            return player.GameScore > 0 && playerProvider.WinningPlayers(player.Gender).Contains(player);
        }

        public static Player GetActivePlayer(this IPlayerProvider playerProvider, string name, Gender gender)
        {
            return playerProvider.ActivePlayers.SingleOrDefault(p => p.Name == name && p.Gender == gender);
        }
    }
}