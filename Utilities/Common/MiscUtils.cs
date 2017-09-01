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

        public static TEnumerableEntity GetRandomEntity<TEnumerableEntity>(
            this IEnumerable<TEnumerableEntity> entities, int seed = 0)
        {
            int count = entities.Count();
            return entities.ElementAt(GetRandomNumber(count - 1, seed: seed));
        }

        public static int GetRandomNumber(int max, int min = 0, int seed = 0)
        {
            var rnd = new Random(DateTime.Now.Millisecond + seed);
            return rnd.Next(min, max);
        }

        public static double GetRandomNumber(double max, double min = 0, int seed = 0)
        {
            var rnd = new Random(DateTime.Now.Millisecond + seed);
            return rnd.NextDouble() * (max - min) + min;
        }

        public static string FormatDouble(double number, string format = "0.##")
        {
            return number.ToString(format);
        }

        public static string GetRandomName(int seed = 0)
        {
            return _names.GetRandomEntity(seed);
        }

        private static readonly IEnumerable<string> _names = new string[]
        {
            "Noah",
            "Liam",
            "Mason",
            "Jacob",
            "William",
            "Ethan",
            "James",
            "Alexander",
            "Michael",
            "Benjamin",
            "Elijah",
            "Daniel",
            "Aiden",
            "Logan",
            "Matthew",
            "Lucas",
            "Jackson",
            "David",
            "Oliver",
            "Jayden",
            "Joseph",
            "Gabriel",
            "Samuel",
            "Carter",
            "Anthony",
            "John",
            "Dylan",
            "Luke",
            "Henry",
            "Andrew",
            "Isaac",
            "Christopher",
            "Joshua",
            "Wyatt",
            "Sebastian",
            "Owen",
            "Caleb",
            "Nathan",
            "Ryan",
            "Jack",
            "Hunter",
            "Levi",
            "Christian",
            "Jaxon",
            "Julian",
            "Landon",
            "Grayson",
            "Jonathan",
            "Isaiah",
            "Charles",
            "Thomas",
            "Aaron",
            "Eli",
            "Connor",
            "Jeremiah",
            "Cameron",
            "Josiah",
            "Adrian",
            "Colton",
            "Jordan",
            "Brayden",
            "Nicholas",
            "Robert",
            "Angel",
            "Hudson",
            "Lincoln",
            "Evan",
            "Dominic",
            "Austin",
            "Gavin",
            "Nolan",
            "Parker",
            "Adam",
            "Chase",
            "Jace",
            "Ian",
            "Cooper",
            "Easton",
            "Kevin",
            "Jose",
            "Tyler",
            "Brandon",
            "Asher",
            "Jaxson",
            "Mateo",
            "Jason",
            "Ayden",
            "Zachary",
            "Carson",
            "Xavier",
            "Leo",
            "Ezra",
            "Bentley",
            "Sawyer",
            "Kayden",
            "Blake",
            "Nathaniel"
        };
    }
}