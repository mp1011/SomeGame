using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeGame.Main.Models
{
    class GameRectangle
    {
        public int X { get; set; }
        public int Y { get; set; }
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
}
