using Microsoft.Xna.Framework;
using System.Linq;

namespace SomeGame.Main.Models
{
    class Font
    {
        private readonly string _charset;
        private readonly int _tileOffset = 0;

        public Font(int tileOffset, string charset)
        {
            _tileOffset = tileOffset;
            _charset = charset;
        }

        private Tile[] GetTilesForLayer(string str, int layerTileOffset)
        {
            return str.Select(chr => GetTile(chr, layerTileOffset))
                      .ToArray();
        }

        public void WriteToLayer(string str, Layer layer, Point location)
        {
            var fontTiles = GetTilesForLayer(str, layer.TileOffset);
            layer.TileMap.SetEach(location.X, location.X + fontTiles.Length, location.Y, location.Y + 1, (x, y) => fontTiles[x - location.X]);
        }

        private Tile GetTile(char c, int layerTileOffset)
        {
            var index = _charset.IndexOf(c);
            if (index < 0)
                return new Tile(-1, TileFlags.None);
            else
                return new Tile((_tileOffset-layerTileOffset) + index, TileFlags.None);
        }
    }
}
