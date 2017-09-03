using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Utilities.Common
{
    public static class MiscUtils
    {
        public static string HashEncode(string text)
        {
            byte[] data = Encoding.Default.GetBytes(text);
            byte[] computed = new SHA256Managed().ComputeHash(data);
            string result = BitConverter.ToString(computed);
            return result.Replace("-", "").ToLower();
        }

        public static TEnumerableEntity GetRandomEntity<TEnumerableEntity>(this IEnumerable<TEnumerableEntity> entities)
        {
            int count = entities.Count();
            int index = GetRandomNumber(count);
            return entities.ElementAt(index);
        }

        private static int _randomSeed = -1;

        public static int GetRandomNumber(int max, int min = 0)
        {
            return GetRandom().Next(min, max);
        }

        public static double GetRandomNumber(double max, double min = 0)
        {
            return GetRandom().NextDouble() * (max - min) + min;
        }

        private static Random GetRandom()
        {
            _randomSeed++;
            return new Random(_randomSeed);
        }

        public static string FormatDouble(double number, string format = "0.##")
        {
            return number.ToString(format);
        }

        public static bool ContinueByRandom()
        {
            return GetRandomNumber(1) == 1;
        } 

        
    }
}