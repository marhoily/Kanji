using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp1
{
    public static class Helpers
    {
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