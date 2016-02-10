using System;
using System.Collections.Generic;
using System.Linq;

namespace KingsAndQueensHat.Utils
{
    public static class ListExtensions
    {
        private static readonly Random rng = new Random();
        /// <summary>
        /// Randomly shuffle the elements in this list, returning a copy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public static IList<T> Shuffle<T>(this IList<T> inputList)
        {
            IList<T> list = inputList.ToList();
            
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        /// <summary>
        /// Provide a never-ending loop through this list
        /// </summary>
        public static IEnumerable<T> Loop<T>(this IList<T> input)
        {
            while (true)
            {
                foreach (var item in input)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Provide a never-ending loop going forwards then backwards through this list
        /// </summary>
        public static IEnumerable<T> Snake<T>(this IList<T> input)
        {
            while (true)
            {
                for (int i = 0; i < input.Count; ++i)
                {
                    yield return input[i];
                }
                for (int i = input.Count - 1; i >= 0; --i)
                {
                    yield return input[i];
                }
            }
        }
    }
}
