using System;
using System.Collections.Generic;
using System.Linq;

namespace BullsnCows
{
    public static class RandomExtension
    {
        private static Random random = new Random();

        public static T RandomValue<T>(this IEnumerable<T> source)
        {
            return RandomValue(source, 1).First();
        }

        public static IEnumerable<T> RandomValue<T>(this IEnumerable<T> source, int count)
        {
            List<T> list = source.ToList();

            for (int i = 0; i < count; i++)
            {
                int ix = random.Next(list.Count);
                yield return list[ix];
                list.RemoveAt(ix);
            }
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return RandomValue(source, source.Count());
        }
    }
}
