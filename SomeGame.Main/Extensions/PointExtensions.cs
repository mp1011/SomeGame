using Microsoft.Xna.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Extensions
{
    public static class PointExtensions
    {
        public static Point GetNeighbor(this Point p, Direction d)
        {
            switch(d)
            {
                case Direction.Up: return p.Offset(0, -1);
                case Direction.UpLeft: return p.Offset(-1, -1);
                case Direction.Left: return p.Offset(-1, 0);
                case Direction.DownLeft: return p.Offset(-1, 1);
                case Direction.Down: return p.Offset(0, 1);
                case Direction.DownRight: return p.Offset(1, 1);
                case Direction.Right: return p.Offset(1, 0);
                case Direction.UpRight: return p.Offset(-1, 1);
                default: return p;
            }
        }
        public static Point Offset(this Point p, int offX, int offY) => new Point(p.X + offX, p.Y + offY);
        public static Point Offset(this Point p, Point p2) => new Point(p.X + p2.X, p.Y + p2.Y);

        public static Point Scale(this Point p, int scale) => new Point(p.X * scale, p.Y * scale);
        public static Point Divide(this Point p, int div) => new Point(p.X / div, p.Y / div);

        public static Point GetRelativePosition(this Point p, int relativeX, int relativeY, Flip flip)
        {
            if ((flip & Flip.H) > 0)
                relativeX *= -1;
            if ((flip & Flip.V) > 0)
                relativeY *= -1;

            return p.Offset(relativeX, relativeY);
        }

        public static Direction ToDirection(this Point p)
        {
            if (p.X < 0 && p.Y < 0)
                return Direction.UpLeft;
            else if (p.X < 0 && p.Y == 0)
                return Direction.Left;
            else if (p.X < 0 && p.Y > 0)
                return Direction.DownLeft;
            else if (p.X == 0 && p.Y > 0)
                return Direction.Down;
            else if (p.X > 0 && p.Y > 0)
                return Direction.DownRight;
            else if (p.X > 0 && p.Y == 0)
                return Direction.Right;
            else if (p.X > 0 && p.Y < 0)
                return Direction.UpRight;
            else
                return Direction.None;
        }

        public static Direction ToDirection(this PixelPoint p)
        {
            if (p.X < 0 && p.Y < 0)
                return Direction.UpLeft;
            else if (p.X < 0 && p.Y == 0)
                return Direction.Left;
            else if (p.X < 0 && p.Y > 0)
                return Direction.DownLeft;
            else if (p.X == 0 && p.Y > 0)
                return Direction.Down;
            else if (p.X > 0 && p.Y > 0)
                return Direction.DownRight;
            else if (p.X > 0 && p.Y == 0)
                return Direction.Right;
            else if (p.X > 0 && p.Y < 0)
                return Direction.UpRight;
            else
                return Direction.None;
        }
    }
}
