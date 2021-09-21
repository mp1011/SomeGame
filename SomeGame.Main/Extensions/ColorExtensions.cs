using Microsoft.Xna.Framework;

namespace SomeGame.Main.Extensions
{
    static class ColorExtensions
    {
        public static Color Invert(this Color c) => new Color(c.PackedValue ^ 0xFFFFFF);
    }
}
