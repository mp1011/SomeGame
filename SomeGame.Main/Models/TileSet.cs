using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using System.Collections.Generic;

namespace SomeGame.Main.Models
{
    class TileSet
    {
        public Texture2D Texture { get; }
        private readonly Grid<Rectangle> _cells;
        private Dictionary<TilesetContentKey, int> _offsets = new Dictionary<TilesetContentKey, int>();

        public int TotalTiles => _cells.Size;

        public TileSet(Texture2D texture, Dictionary<TilesetContentKey,int> offsets, int tileSize)
        {
            _offsets = offsets;
            Texture = texture;
            _cells = new Grid<Rectangle>(texture.Width / tileSize, texture.Height / tileSize,
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
