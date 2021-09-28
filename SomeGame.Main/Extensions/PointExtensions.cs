using Microsoft.Xna.Framework;

namespace SomeGame.Main.Extensions
{
    public static class PointExtensions
    {
        public static Point Offset(this Point p, int offX, int offY) => new Point(p.X + offX, p.Y + offY);
    }
}
