using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KingsAndQueensHat.Persistence
{
    public class InvalidRoundException : Exception
    {
        public InvalidRoundException(string message) : base(message)
        {

        }
    }
}
