using System;

namespace KingsAndQueensHat.Persistence
{
    public class InvalidPlayerListException : Exception
    {
        public InvalidPlayerListException(string message) : base(message)
        {
            
        }
    }
}