using SomeGame.Main.Models;
using System;

namespace SomeGame.Main.Extensions
{
    public static class NumberExtensions
    {
        public static int RoundToZero(this double d)
        {
            if (d > 0)
                return (int)Math.Floor(d);
            else if (d < 0)
                return (int)Math.Ceiling(d);
            else
                return 0;
        }

        public static int RoundToZero(this float f)
        {
            if (f > 0)
                return (int)Math.Floor(f);
            else if (f < 0)
                return (int)Math.Ceiling(f);
            else
                return 0;
        }

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
