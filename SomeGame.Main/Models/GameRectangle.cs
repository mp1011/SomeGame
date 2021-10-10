using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeGame.Main.Models
{
    class GameRectangle
    {
        public virtual int X { get; set; }
        public virtual int Y { get; set; }
        public int Width { get; }
        public int Height { get;}

        public Point TopLeft => new Point(X, Y);
        public Point BottomRight => new Point(X+Width-1, Y+Height-1);

        public int Bottom => Y + Height;
        public int Right => X + Width;

        public int Top => Y;

        public int Left => X;

        public GameRectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool IntersectsWith(Rectangle other)
        {
            return ((Rectangle)this).Intersects(other);
        }

        public static implicit operator Rectangle(GameRectangle r) => new Rectangle(r.X,r.Y,r.Width,r.Height);
    }

    class GameRectangleWithSubpixels : GameRectangle
    {
        public GameRectangleWithSubpixels(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        public GameRectangleWithSubpixels(PixelValue x, PixelValue y, int width, int height) : base(0, 0, width, height)
        {
            XPixel = x;
            YPixel = y;
        }


        public GameRectangleWithSubpixels Copy() => new GameRectangleWithSubpixels(0, 0, Width, Height)
        {
            XPixel = XPixel,
            YPixel = YPixel
        };

        public PixelValue XPixel { get; set; } = new PixelValue(0, 0);
        public PixelValue YPixel { get; set; } = new PixelValue(0, 0);


        public override int X
        {
            get => XPixel;
            set => XPixel = value;
        }

        public override int Y
        {
            get => YPixel;
            set => YPixel = value;
        }


    }

    class BoundedGameRectangle : GameRectangle
    {
        private BoundedInt _boundedX, _boundedY;

        public override int X { get => _boundedX.Value; set => _boundedX = _boundedX.Set(value); }
        public override int Y { get => _boundedY.Value; set => _boundedY = _boundedY.Set(value); }

        public BoundedGameRectangle(int x, int y, int width, int height, int maxX, int maxY) 
            : base(x, y, width, height)
        {
            _boundedX = new BoundedInt(x, maxX);
            _boundedY = new BoundedInt(y, maxY);
        }
    }
}
