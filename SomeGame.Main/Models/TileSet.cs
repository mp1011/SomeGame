using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using System.Collections.Generic;

namespace SomeGame.Main.Models
{
    class TileSet
    {
        private readonly Grid<Rectangle> _cells;
        private Dictionary<TilesetContentKey, int> _offsets = new Dictionary<TilesetContentKey, int>();

        public int TotalTiles => _cells.Size;

        public TileSet(Dictionary<TilesetContentKey,int> offsets, int tileSize, int width, int height)
        {
            _offsets = offsets;
            _cells = new Grid<Rectangle>(width / tileSize, height / tileSize,
                (x, y) => new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize));
        }

        public Rectangle GetSrcRec(TilesetContentKey key, Tile tile)
        {
            return _cells[_offsets[key] + tile.Index];
        }

        public Rectangle GetSrcRec(int index)
        {
            return _cells[index];
        }
    }
}
