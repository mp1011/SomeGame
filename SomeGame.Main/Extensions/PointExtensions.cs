using Microsoft.Xna.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Extensions
{
    public static class PointExtensions
    {
        public static Point Offset(this Point p, int offX, int offY) => new Point(p.X + offX, p.Y + offY);
        public static Point Offset(this Point p, Point p2) => new Point(p.X + p2.X, p.Y + p2.Y);

        public static Point Scale(this Point p, int scale) => new Point(p.X * scale, p.Y * scale);

        public static Point GetRelativePosition(this Point p, int relativeX, int relativeY, Flip flip)
        {
            if ((flip & Flip.H) > 0)
                relativeX *= -1;
            if ((flip & Flip.V) > 0)
                relativeY *= -1;

            return p.Offset(relativeX, relativeY);

        }
    }
}
