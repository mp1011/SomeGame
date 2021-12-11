using Microsoft.Xna.Framework;
using System;

namespace SomeGame.Main.Models
{
    interface IGameRectangle
    {
        int X { get; set; }
        int Y { get; set; }
        int Width { get; }
        int Height { get; }

        Point Center { get; set; }
        //Point TopLeft
        //{
        //    get => new Point(X, Y);
        //    set
        //    {
        //        X = value.X;
        //        Y = value.Y;
        //    }
        //}
        //public Point BottomRight => new Point(X+Width-1, Y+Height-1);

        //public Point Center
        //{
        //    get => new Point(X + Width / 2, Y + Width / 2);
        //    set
        //    {
        //        X = value.X - Width / 2;
        //        Y = value.Y - Width / 2;
        //    }
        //}

        //public int Bottom => Y + Height;
        //public int Right => X + Width;

        //public int Top => Y;

        //public int Left => X;

        //public Direction GetHorizontalDirectionTo(IGameRectangle other)
        //{
        //    if (other.Center.X < Center.X)
        //        return Direction.Left;
        //    else
        //        return Direction.Right;
        //}

        //public int GetAbsoluteXDistance(IGameRectangle other)
        //{
        //    return Math.Abs(other.Center.X - Center.X);
        //}
    }

    static class IGameRectangleExtensions
    {
        public static Point TopLeft(this IGameRectangle rec) => new Point(rec.X, rec.Y);
        public static Point BottomRight(this IGameRectangle rec) => new Point(rec.X+rec.Width, rec.Y+rec.Height);
        public static int Right(this IGameRectangle rec) => rec.X + rec.Width;
        public static int Left(this IGameRectangle rec) => rec.X;
        public static int Top(this IGameRectangle rec) => rec.Y ;
        public static int Bottom(this IGameRectangle rec) => rec.Y + rec.Height;

        public static Rectangle ToXNARec(this IGameRectangle r) => new Rectangle(r.X,r.Y,r.Width,r.Height);

        public static bool IntersectsWith(this IGameRectangle rec, Rectangle other)
        {
            return rec.ToXNARec().Intersects(other);
        }

        public static bool IntersectsWithRec(this IGameRectangle rec, IGameRectangle other)
        {
            return rec.ToXNARec().Intersects(other.ToXNARec());
        }

        public static Direction GetHorizontalDirectionTo(this IGameRectangle rec, IGameRectangle other)
        {
            if (other.Center.X < rec.Center.X)
                return Direction.Left;
            else
                return Direction.Right;
        }

        public static int GetAbsoluteXDistance(this IGameRectangle rec, IGameRectangle other)
        {
            return Math.Abs(other.Center.X - rec.Center.X);
        }
    }


    class GameRectangleWithSubpixels : IGameRectangle
    {
        public int Width { get; }
        public int Height { get; }

        public GameRectangleWithSubpixels(int x, int y, int width, int height) 
        {
            XPixel = new PixelValue(x, 0);
            YPixel = new PixelValue(y, 0);
            Width = width;
            Height = height;
        }

        public GameRectangleWithSubpixels(PixelValue x, PixelValue y, int width, int height) 
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


        public PixelValue LeftPixel => XPixel;
        public PixelValue TopPixel => YPixel;
        public PixelValue RightPixel => LeftPixel + Width;
        public PixelValue BottomPixel => TopPixel + Height;


        public int X
        {
            get => XPixel;
            set => XPixel = value;
        }

        public int Y
        {
            get => YPixel;
            set => YPixel = value;
        }

        public override bool Equals(object obj)
        {
            if(obj is GameRectangleWithSubpixels other)
            {
                return Width == other.Width
                    && Height == other.Height
                    && XPixel == other.XPixel
                    && YPixel == other.YPixel;
            }

            return false;
        }

        public Point Center
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public PixelPoint GetDirectionTo(GameRectangleWithSubpixels other)
        {
            return new PixelPoint(other.XPixel - XPixel, other.YPixel - YPixel)
                                 .Normalize();
        }

        public override string ToString() => $"X:{XPixel} Y:{YPixel} Width:{Width} Height:{Height}";
  
        public static implicit operator Rectangle(GameRectangleWithSubpixels rec)
        {
            return new Rectangle(rec.X, rec.Y, rec.Width, rec.Height);
        }
    
    }

    class BoundedGameRectangle : IGameRectangle
    {
        private BoundedRamInt _boundedX, _boundedY;
        private RamInt _width, _height;
        public int X { get => _boundedX; set => _boundedX = _boundedX.Set(value); }
        public int Y { get => _boundedY; set => _boundedY = _boundedY.Set(value); }

        public int MaxX { get => _boundedX.Max; set => _boundedX.Max = value; }
        public int MaxY { get => _boundedY.Max; set => _boundedY.Max = value; }

        public int Width => _width;
        public int Height => _height;

        public Point Center
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public BoundedGameRectangle(BoundedRamInt x, BoundedRamInt y, RamInt width, RamInt height) 
        {
            _width = width;
            _height = height;
            _boundedX = x;
            _boundedY = y;
        }
    }

    record RamGameRectangle(RamPixelValue X, RamPixelValue Y, RamByte Width, RamByte Height)  : IGameRectangle
    {
        int IGameRectangle.X { get => X.Pixel; set => X.Set(value); }
        int IGameRectangle.Y { get => Y.Pixel; set => Y.Set(value); }

        int IGameRectangle.Width => Width;

        int IGameRectangle.Height => Height;


        public PixelValue LeftPixel => X;
        public PixelValue TopPixel => Y;
        public PixelValue RightPixel => LeftPixel + Width;
        public PixelValue BottomPixel => TopPixel + Height;

        public static implicit operator Rectangle(RamGameRectangle r)
        {
            return new Rectangle(r.X.Pixel, r.Y.Pixel, r.Width, r.Height);
        }

        public PixelPoint GetDirectionTo(RamGameRectangle other)
        {
            var thisX = new PixelValue(X.Pixel, X.SubPixel);
            var thisY = new PixelValue(Y.Pixel, Y.SubPixel);
            var otherX = new PixelValue(other.X.Pixel, other.X.SubPixel);
            var otherY = new PixelValue(other.Y.Pixel, other.Y.SubPixel);

            return new PixelPoint(otherX - thisX, otherY - thisY)
                                 .Normalize();
        }

        public Point Center
        {
            get => new Point(X.Pixel + (Width / 2),
                             Y.Pixel + (Height / 2));

            set
            {
                X.Set(value.X - (Width / 2));
                Y.Set(value.Y - (Height / 2));
            }
        }
    }
}
