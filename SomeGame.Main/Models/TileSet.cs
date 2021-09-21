using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SomeGame.Main.Models
{
    class TileSet
    {
        public Texture2D Texture { get; }
        private readonly Grid<Rectangle> _cells;

        public int TotalTiles => _cells.Size;

        public TileSet(Texture2D texture, int tileSize)
        {
            Texture = texture;
            _cells = new Grid<Rectangle>(texture.Width / tileSize, texture.Height / tileSize,
                (x, y) => new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize));
        }

        public Rectangle GetSrcRec(Tile tile)
        {
            return _cells[tile.Index];
        }
    }
}
