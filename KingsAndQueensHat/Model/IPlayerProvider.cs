using System.Collections.Generic;
using System.Linq;
using KingsAndQueensHat.Model;
using System;

namespace KingsAndQueensHat
{
    public interface IPlayerProvider
    {
        List<Player> Players { get; }
    }

    public static class PlayerProviderExtensions
    {
        public static int TotalSkill(this IPlayerProvider playerProvider)
        {
            return playerProvider.Players.Sum(p => p.Skill);
        }

        public static IEnumerable<Player> WinningPlayers(this IPlayerProvider playerProvider, Gender gender)
        {
            return playerProvider.Players.Where(p => p.Gender == gender && p.GameScore == playerProvider.MaxGameScore(gender));
        }

        public static int MaxGameScore(this IPlayerProvider playerProvider, Gender gender)
        {
            return playerProvider.Players.Where(p => p.Gender == gender).Max(p => p.GameScore);
        }

        public static bool IsWinning(this IPlayerProvider playerProvider, Player player)
        {
            return player.GameScore > 0 && playerProvider.WinningPlayers(player.Gender).Contains(player);
        }

        public static Player PlayerWithName(this IPlayerProvider playerProvider, string name)
        {
            return playerProvider.Players.SingleOrDefault(p => p.Name == name);
        }
    }
}