using Microsoft.Xna.Framework;

namespace SomeGame.Main.Extensions
{
    public static class RectangleExtensions
    {
        public static Point TopLeft(this Rectangle r) => new Point(r.Left, r.Top);

        public static bool IntersectsOrTouches(this Rectangle r1, Rectangle r2)
        {
            return !(r1.Left > r2.Right)
                && !(r1.Right < r2.Left)
                && !(r1.Top > r2.Bottom)
                && !(r1.Bottom < r2.Top);
        }
    }
}
