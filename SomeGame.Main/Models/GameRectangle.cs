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

        public GameRectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
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
