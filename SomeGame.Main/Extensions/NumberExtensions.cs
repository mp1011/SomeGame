using SomeGame.Main.Models;

namespace SomeGame.Main.Extensions
{
    public static class NumberExtensions
    {

        internal static RotatingInt AsRotatingInt(this int value, int max) => new RotatingInt(value, max);
        public static int Clamp(this int value, int max) => value.Clamp(0, max);

        public static int Clamp(this int value, int min, int max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        public static int Clamp(this double value, int min, int max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return (int)value;
        }

        public static byte ClampToByte(this int value)
        {
            return (byte)value.Clamp(0, 255);
        }
        public static byte ClampToByte(this double value)
        {
            return ((int)value).ClampToByte();
        }

    }
}
