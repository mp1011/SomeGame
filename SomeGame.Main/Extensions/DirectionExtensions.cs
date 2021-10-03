using Microsoft.Xna.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Extensions
{
    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction d)
        {
            switch (d)
            {
                case Direction.Up: return Direction.Down;
                case Direction.UpLeft: return Direction.DownRight;
                case Direction.Left: return Direction.Right;
                case Direction.DownLeft: return Direction.UpRight;
                case Direction.Down: return Direction.Up;
                case Direction.DownRight: return Direction.UpLeft;
                case Direction.Right: return Direction.Left;
                case Direction.UpRight: return Direction.DownLeft;
                default:return d;
            }
        }

        public static Point ToPoint(this Direction d)
        {
            switch (d)
            {
                case Direction.Up: return new Point(0, -1);
                case Direction.UpLeft: return new Point(-1, -1);
                case Direction.Left: return new Point(-1, 0);
                case Direction.DownLeft: return new Point(1, -1);
                case Direction.Down: return new Point(0, 1);
                case Direction.DownRight: return new Point(1, 1);
                case Direction.Right: return new Point(1, 0);
                case Direction.UpRight: return new Point(-1, 1);
                default: return Point.Zero;
            }
        }
    }
}
