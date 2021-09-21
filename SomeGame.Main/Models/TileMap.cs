using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using System;

namespace SomeGame.Main.Models
{
    class TileMap
    {
        private Grid<Tile> _tiles;

        public TileMap(int tilesX, int tilesY)
        {
            _tiles = new Grid<Tile>(tilesX, tilesY, (x,y)=> new Tile(-1, TileFlags.None));
        }

        public int TilesX => _tiles.Width;
        public int TilesY => _tiles.Height;

        public Tile GetTile(int x, int y) => _tiles[x, y];
        public Tile GetTile(Point p) => _tiles[p.X, p.Y];

        public void SetTile(int x, int y, Tile tile)
        {
            _tiles[x, y] = tile;
        }

        public void Set(Grid<Tile> tiles)
        {
            _tiles = tiles;
        }

        public void ForEach(Action<int, int, Tile> action) => _tiles.ForEach(action);
        public void SetEach(Func<int, int, Tile> createTile) => _tiles.SetEach(createTile);
        public void SetEach(int xStart, int xEnd, int yStart, int yEnd, Func<int, int, Tile> createTile) => _tiles.SetEach(xStart, xEnd, yStart, yEnd, createTile);

    }
}
