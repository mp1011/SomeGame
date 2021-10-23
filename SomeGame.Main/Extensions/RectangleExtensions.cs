using Microsoft.Xna.Framework;

namespace SomeGame.Main.Extensions
{
    public static class RectangleExtensions
    {
        public static Point TopLeft(this Rectangle r) => new Point(r.Left, r.Top);

        public static Rectangle LeftSection(this Rectangle r, int width) =>
            new Rectangle(r.X, r.Y, width, r.Height);

        public static Rectangle RightSection(this Rectangle r, int width) =>
            new Rectangle(r.X + r.Width - width, r.Y, width, r.Height);

        public static Rectangle TopSection(this Rectangle r, int height) =>
            new Rectangle(r.X, r.Y, r.Width, height);
        public static Rectangle BottomSection(this Rectangle r, int height) =>
           new Rectangle(r.X, r.Y + r.Height - height, r.Width, height);

    }
}
