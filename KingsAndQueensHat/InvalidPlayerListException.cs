using System;

namespace KingsAndQueensHat
{
    public class InvalidPlayerListException : Exception
    {
        public InvalidPlayerListException(string message) : base(message)
        {
            
        }
    }
}