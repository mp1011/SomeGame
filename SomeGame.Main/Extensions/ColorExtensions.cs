using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Extensions
{
    static class ColorExtensions
    {
        public static Color Invert(this Color c) => new Color(c.PackedValue ^ 0xFFFFFF);

        public static double DistanceTo(this Color c1, Color c2)
        {
            var r = (double)c1.R - c2.R;
            var g = (double)c1.G - c2.G;
            var b = (double)c1.B - c2.B;

            r = r * r;
            g = g * g;
            b = b * b;

            return Math.Sqrt(r + g + b);
        }
    }
}
