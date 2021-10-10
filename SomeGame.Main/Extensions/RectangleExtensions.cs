using Microsoft.Xna.Framework;

namespace SomeGame.Main.Extensions
{
    public static class RectangleExtensions
    {
        public static Point TopLeft(this Rectangle r) => new Point(r.Left, r.Top);
    }
}
