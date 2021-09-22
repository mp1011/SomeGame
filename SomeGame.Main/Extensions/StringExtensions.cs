using System;

namespace SomeGame.Main.Extensions
{
    public static class StringExtensions
    {
        public static T ParseEnum<T>(this string str)
            where T:struct
        {
            T result;
            if (Enum.TryParse<T>(str, out result))
                return result;
            else
                return default;
        }
    }
}
