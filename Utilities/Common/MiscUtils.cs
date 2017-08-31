using System;
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

        public static int GetRandomIndex(int max, int min = 0)
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            return rnd.Next(max, min);
        }
    }
}