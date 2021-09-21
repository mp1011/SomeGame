using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;

namespace SomeGame.Main.Models
{
    class Layer
    {
        private readonly int _tileSize;

        public TileMap TileMap { get; }        
        public PaletteIndex Palette { get; set; }
        public RotatingInt ScrollX { get; set; }
        public RotatingInt ScrollY { get; set; }

        public Layer(TileMap tileMap, PaletteIndex palette, RotatingInt scrollX, RotatingInt scrollY, int tileSize)
        {
            _tileSize = tileSize;

            TileMap = tileMap;
            Palette = palette;
            ScrollX = scrollX;
            ScrollY = scrollY;
        }

        public Point TilePointFromPixelPoint(int x, int y)
        {
            if (ScrollX != 0 || ScrollY != 0)
                throw new System.NotImplementedException();

            var tileX = (x / _tileSize).Clamp(0, TileMap.TilesX - 1);
            var tileY = (y / _tileSize).Clamp(0, TileMap.TilesY - 1);
            return new Point(tileX, tileY);
        }
    }
}
