using System;
using System.Collections;
using System.Diagnostics;

namespace KingsAndQueensHat
{
    public class PlayerPairing : IEquatable<PlayerPairing>
    {
        private readonly Player _player1;
        private readonly Player _player2;

        public PlayerPairing(Player player1, Player player2)
        {
            Trace.Assert(player1 != player2);
            _player1 = player1;
            _player2 = player2;
        }

        public override bool Equals(object obj)
        {
            var playerPairing = obj as PlayerPairing;
            if (playerPairing != null)
            {
                return Equals(playerPairing);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _player1.GetHashCode() ^ _player2.GetHashCode();
        }

        public bool Equals(PlayerPairing other)
        {
            return other._player1 == _player1 &&
                   other._player2 == _player2
                   ||
                   other._player1 == _player2 &&
                   other._player2 == _player1;
        }
    }
}