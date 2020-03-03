using System;
using System.Linq;
using System.Text;

namespace LogWire.Controller.Utils
{
    public static class StringUtils
    {

        private static readonly Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static byte[] ToUTF8(this string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        public static string ToBase64(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

    }
}
