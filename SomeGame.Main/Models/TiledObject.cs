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

        public TiledObject(TileMap tileMap, PaletteIndex palette, RotatingInt scrollX, RotatingInt scrollY, int tileSize)
        {
            _tileSize = tileSize;
            TileMap = tileMap;
            Palette = palette;
            ScrollX = scrollX;
            ScrollY = scrollY;
        }

        public Point TilePointFromLayerPixelPoint(Point pixelPoint)
        {
            var tileX = (pixelPoint.X / _tileSize).Clamp(0, TileMap.TilesX - 1);
            var tileY = (pixelPoint.Y / _tileSize).Clamp(0, TileMap.TilesY - 1);
            return new Point(tileX, tileY);
        }
        
        public Point TilePointFromScreenPixelPoint(int x, int y)
        {
            var sx = x - ScrollX;
            var sy = y - ScrollY;

            var tileX = (sx / _tileSize).Clamp(0, TileMap.TilesX - 1);
            var tileY = (sy / _tileSize).Clamp(0, TileMap.TilesY - 1);
            return new Point(tileX, tileY);
        }

        public GameRectangleWithSubpixels GetTileLayerPosition(int tileX, int tileY)
        {
            return new GameRectangleWithSubpixels(tileX * _tileSize, tileY * _tileSize, _tileSize, _tileSize);
        }
    }
}
