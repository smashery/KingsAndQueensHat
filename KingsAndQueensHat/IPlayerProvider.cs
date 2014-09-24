using System.Collections.Generic;
using System.Linq;

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
            return playerProvider.Players.Where(p => p.Gender == gender && p.Wins == playerProvider.MaxWins(gender));
        }

        public static int MaxWins(this IPlayerProvider playerProvider, Gender gender)
        {
            return playerProvider.Players.Where(p => p.Gender == gender).Max(p => p.Wins);
        }
    }
}