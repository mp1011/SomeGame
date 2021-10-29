using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Models
{
    class TileMap
    {
        private Grid<Tile> _tiles;

        public LevelContentKey LevelKey { get; }

        public TileMap(LevelContentKey levelContentKey, int tilesX, int tilesY)
        {
            LevelKey = levelContentKey;
            _tiles = new Grid<Tile>(tilesX, tilesY, (x,y)=> new Tile(-1, TileFlags.None));
        }

        public TileMap(LevelContentKey levelContentKey, Grid<Tile> tiles)
        {
            LevelKey = levelContentKey;
            _tiles = tiles;
        }

        public Tile this[int x, int y] => _tiles[x,y];

        public int TilesX => _tiles.Width;
        public int TilesY => _tiles.Height;
        public Grid<Tile> GetGrid() => _tiles;

        public Tile GetTile(int x, int y) => _tiles[new RotatingInt(x,TilesX) , new RotatingInt(y,TilesY)];
        public Tile GetTile(Point p) => _tiles[p.X, p.Y];

        public void SetTile(int x, int y, Tile tile)
        {
            _tiles[new BoundedInt(x,TilesX), new BoundedInt(y,TilesY)] = tile;
        }

        public void Set(Grid<Tile> tiles)
        {
            _tiles = tiles;
        }

        public void ForEach(Action<int, int, Tile> action) => _tiles.ForEach(action);

        public void ForEach(Point upperLeftTile, Point bottomRightTile, Action<int, int, Tile> action)
        {
            _tiles.ForEach(upperLeftTile.X, bottomRightTile.X, upperLeftTile.Y, bottomRightTile.Y, action);
        }

        public void SetEach(Func<int, int, Tile> createTile) => _tiles.SetEach(createTile);
        public void SetEach(int xStart, int xEnd, int yStart, int yEnd, Func<int, int, Tile> createTile) => 
            _tiles.SetEach(xStart, xEnd, yStart, yEnd, createTile);

        public void SetEach(Point upperLeft, Point bottomRight, Func<int, int, Tile> createTile) => 
            _tiles.SetEach(upperLeft.X, bottomRight.X, upperLeft.Y, bottomRight.Y, createTile);

    }
}
