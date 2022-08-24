using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp1
{
    public static class Helpers
    {
        public static Card[] Shuffle2(
            this Random rnd, 
            Card[] array)
        {
            var all = array.ToList();

            var wordsByChar = all.Where(r => r.Kanji.Length > 1)
                .Select(r => r.Kanji.Random(rnd, c => (c, r)))
                .ToLookup(t => t.c, t => t.r);

            var result = all
                .Where(r => r.Kanji.Length == 1)
                .Shuffle(rnd)
                .SelectMany(r => wordsByChar[r.Kanji.Single()])
                .ToArray();
            var a = result.Select(x=>x.Kanji).OrderBy(x => x).ToList();
            var b = array.Select(x=>x.Kanji).OrderBy(x=>x).ToList();
            if (!a.SequenceEqual(b))
                1.ToString();

            return result;
        }

        public static TElem Random<TElem>(this IList<TElem> array, Random rnd)
        {
            return array[rnd.Next(array.Count)];
        }
        public static TResult Random<TElem, TResult>(this IEnumerable<TElem> array, Random rnd, Func<TElem, TResult> selector)
        {
            var buffer = array.ToList();
            return selector(buffer.Random(rnd));
        }
        public static List<TElem> Shuffle<TElem>(this IEnumerable<TElem> array, Random rnd)
        {
            var result = array.ToList();
            rnd.Shuffle(result);
            return result;
        }

        /// <summary>
        /// Knuth shuffle
        /// </summary>        
        public static void Shuffle<T>(this Random rnd, IList<T> array)
        {
            int n = array.Count;
            while (n > 1)
            {
                n--;
                int i = rnd.Next(n + 1);
                var temp = array[i];
                array[i] = array[n];
                array[n] = temp;
            }
        }
    }
}