using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Models
{
    class TileMap
    {
        private RamGrid<RamTile> _tiles;

        public LevelContentKey LevelKey { get; }

        public TileMap(GameSystem gameSystem, LevelContentKey levelContentKey, int tilesX, int tilesY)
        {
            LevelKey = levelContentKey;
            _tiles = new TileGrid(gameSystem, tilesX, tilesY, ()=> gameSystem.RAM.DeclareTile(255, TileFlags.None));
        }

        public TileMap(LevelContentKey levelContentKey, RamGrid<RamTile> tiles)
        {
            LevelKey = levelContentKey;
            _tiles = tiles;
        }

        public Tile this[int x, int y] => _tiles[x,y];

        public int TilesX => _tiles.Width;
        public int TilesY => _tiles.Height;
        public RamGrid<RamTile> GetGrid() => _tiles;

        public RamTile GetTile(int x, int y) => _tiles[new RotatingInt(x,TilesX) , new RotatingInt(y,TilesY)];
        public RamTile GetTile(Point p) => _tiles[p.X, p.Y];

        public void SetTile(int x, int y, Tile tile)
        {
            _tiles[new BoundedInt(x,TilesX), new BoundedInt(y,TilesY)].Set(tile);
        }

        public void Set(RamGrid<RamTile> tiles)
        {
            _tiles = tiles;
        }

        public void ForEach(Action<int, int, RamTile> action) => _tiles.ForEach(action);

        public void ForEach(Point upperLeftTile, Point bottomRightTile, Action<int, int, RamTile> action)
        {
            _tiles.ForEach(upperLeftTile.X, bottomRightTile.X, upperLeftTile.Y, bottomRightTile.Y, action);
        }

        public void SetEach(Func<int, int, Tile> createTile)
        {
            _tiles.ForEach((x, y, t) => t.Set(createTile(x, y)));
        }
        public void SetEach(int xStart, int xEnd, int yStart, int yEnd, Func<int, int, Tile> createTile)
        {
            _tiles.ForEach(xStart,xEnd,yStart,yEnd, (x, y, t) => t.Set(createTile(x, y)));
        }

        public void SetEach(Point upperLeft, Point bottomRight, Func<int, int, Tile> createTile)
        {
            _tiles.ForEach(upperLeft.X, bottomRight.X, upperLeft.Y, bottomRight.Y, (x, y, t) => t.Set(createTile(x, y)));
        }
    }
}
