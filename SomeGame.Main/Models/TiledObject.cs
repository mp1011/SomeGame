using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;

namespace SomeGame.Main.Models
{
    abstract class TiledObject
    {
        private readonly int _tileSize;

        public TileMap TileMap { get; }
        public PaletteIndex Palette { get; set; }
        public RotatingInt ScrollX { get; set; }
        public RotatingInt ScrollY { get; set; }

        public TiledObject(TileMap tileMap, PaletteIndex palette, RotatingInt scrollX, RotatingInt scrollY, int tileSize)
        {
            _tileSize = tileSize;
            TileMap = tileMap;
            Palette = palette;
            ScrollX = scrollX;
            ScrollY = scrollY;
        }

        public Point TilePointFromPixelPoint(int x, int y)
        {
            var sx = x - ScrollX;
            var sy = y - ScrollY;

            var tileX = (sx / _tileSize).Clamp(0, TileMap.TilesX - 1);
            var tileY = (sy / _tileSize).Clamp(0, TileMap.TilesY - 1);
            return new Point(tileX, tileY);
        }
    }
}
