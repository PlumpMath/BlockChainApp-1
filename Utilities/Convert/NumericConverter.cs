using System;
using System.Globalization;

namespace Utilities.Convert
{
    public static class NumericConverter
    {
        public static int ParseAsInt(this string numeric)
        {
            return int.TryParse(numeric, out int result) 
                ? result
                : throw new InvalidCastException();
        }

        public static double ParseAsDouble(this string numeric)
        {
            return double.TryParse(numeric, 
                NumberStyles.Float | NumberStyles.AllowDecimalPoint, 
                CultureInfo.InvariantCulture, 
                out double result)
                ? result
                : throw new InvalidCastException();
        }
    }
}