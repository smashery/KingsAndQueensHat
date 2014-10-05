using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingsAndQueensHat.Utils
{
    public class HatMath
    {
        public static double Square(double x)
        {
            return x * x;
        }

        public static double StdDeviation(IList<double> numbers)
        {
            var meanSkill = numbers.Sum() / (double)numbers.Count;
            var sumOfSquares = numbers.Sum(x => Square(x - meanSkill));
            var meanSumOfSquares = sumOfSquares / numbers.Count;
            var standardDeviation = Math.Sqrt(meanSumOfSquares);
            return standardDeviation;
        }
    }
}
