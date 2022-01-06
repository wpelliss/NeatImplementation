using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2.Helpers
{
    internal class Helper
    {
        public static double gaussianRand()
        {
            double rand = 0;
            Random mathRandom = new Random();

            for (var i = 0; i < 6; i++)
            {
                rand += mathRandom.NextDouble();
            }

            return rand / 6;
        }

        public static double std0()
        {
            return (gaussianRand() - 0.5) * 2;
        }

        public static double random(double min, double max)
        {
            Random mathRandom = new Random();

            return min + mathRandom.NextDouble() * (max - min);
        }

        public static double sigmoid(double val)
        {
            return 1 / (1 + Math.Exp(-val));
        }

        public static T[] Shuffle<T>(T[] array)
        {
            Random rng = new Random();
            int n = array.Length;
            T[] finalArray = (T[])(array.Clone());
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = finalArray[n];
                finalArray[n] = finalArray[k];
                finalArray[k] = temp;
            }
            return finalArray;
        }
    }
}
