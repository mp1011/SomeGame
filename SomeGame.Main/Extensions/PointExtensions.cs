using Microsoft.Xna.Framework;

namespace SomeGame.Main.Extensions
{
    public static class PointExtensions
    {
        public static Point Offset(this Point p, int offX, int offY) => new Point(p.X + offX, p.Y + offY);
        public static Point Offset(this Point p, Point p2) => new Point(p.X + p2.X, p.Y + p2.Y);

        public static Point Scale(this Point p, int scale) => new Point(p.X * scale, p.Y * scale);

    }
}
