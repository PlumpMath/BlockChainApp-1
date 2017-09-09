using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
            return new Random(DateTime.Now.Millisecond + _randomSeed);
        }

        public static string FormatDouble(double number, string format = "0.##")
        {
            return number.ToString(format);
        }

        public static string GetEnumValueDescription<TEnum>(this TEnum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            return attributes.Length > 0 
                ? attributes[0].Description : 
                enumValue.ToString();
        }

        public static int CorrectRiskness(this int riskness, int max = 100, int min = 0)
        {
            if (riskness > max)
            {
                riskness = max;
            }
            else if (riskness < min)
            {
                riskness = min;
            }
            return riskness;
        }

        
    }
}