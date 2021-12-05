using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;

namespace SomeGame.Main.Models
{
    abstract class TiledObject
    {
        private readonly int _tileSize;
        public int TileOffset { get; set; }

        public TileMap TileMap { get; }
        public PaletteIndex Palette { get; set; }
        public RotatingInt ScrollX { get; set; }
        public RotatingInt ScrollY { get; set; }

        private readonly int _tileXMax;
        private readonly int _tileYMax;


        public TiledObject(TileMap tileMap, PaletteIndex palette, RotatingInt scrollX, RotatingInt scrollY, int tileSize)
        {
            _tileSize = tileSize;
            TileMap = tileMap;
            Palette = palette;
            ScrollX = scrollX;
            ScrollY = scrollY;

            _tileXMax = tileMap.TilesX;
            _tileYMax = tileMap.TilesY;
        }

        public Point TilePointFromScreenPixelPoint(int x, int y)
        {
            x = x - ScrollX.Value;
            y = y - ScrollY.Value;

            while (x < 0)
                x += ScrollX.Max;
            while (x >= ScrollX.Max)
                x -= ScrollX.Max;

            while (y < 0)
                y += ScrollY.Max;
            while (y >= ScrollY.Max)
                y -= ScrollY.Max;

            x = x / _tileSize;
            y = y / _tileSize;

            if (x < 0)
                x = 0;
            if (x > _tileXMax - 1)
                x = _tileXMax - 1;
            if (y < 0)
                y = 0;
            if (y > _tileYMax - 1)
                y = _tileYMax - 1;

            return new Point(x, y);
        }

        public GameRectangleWithSubpixels GetTileLayerPosition(int tileX, int tileY)
        {
            return new GameRectangleWithSubpixels(tileX * _tileSize, tileY * _tileSize, _tileSize, _tileSize);
        }
    }
}
