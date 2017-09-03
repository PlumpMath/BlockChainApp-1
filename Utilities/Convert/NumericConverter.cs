using System;
using System.Globalization;

namespace Utilities.Convert
{
    public static class NumericConverter
    {
        public static int ParseAsInt(this string numeric)
        {
            ValidateArgument(numeric);
            return int.TryParse(numeric, out int result) 
                ? result
                : throw new InvalidCastException();
        }

        public static double ParseAsDouble(this string numeric)
        {
            ValidateArgument(numeric);
            return double.TryParse(numeric, 
                NumberStyles.Float | NumberStyles.AllowDecimalPoint, 
                CultureInfo.InvariantCulture, 
                out double result)
                ? result
                : throw new InvalidCastException();
        }

        private static void ValidateArgument(string numeric)
        {
            if (string.IsNullOrWhiteSpace(numeric))
            {
                throw new ArgumentNullException(nameof(numeric));
            }
        }
    }
}