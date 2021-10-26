using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Models
{
    class EditorTileSet
    {
        public List<EditorTile> Tiles { get; } = new List<EditorTile>();

        public string[] Themes => Tiles.SelectMany(p => p.Themes)
                                     .Distinct()
                                     .ToArray();
        public EditorTileSet()
        {
        }

        public EditorTile GetOrAddTile(Tile t)
        {
            var tile = Tiles.FirstOrDefault(p => p.Tile == t);
            if (tile == null)
            {
                tile = new EditorTile(t);
                Tiles.Add(tile);
            }

            return tile;
        }
    }

    class EditorBlock
    {
        public string Theme { get; }
        public Grid<Tile> Grid { get; }

        public EditorBlock(string theme, Grid<Tile> grid)
        {
            Theme = theme;
            Grid = grid;
        }
    }

    class EditorTile
    {
        public List<string> Themes { get; } = new List<string>();
        public Tile Tile { get; }

        public bool ContainsTheme(string theme) => Themes.Contains(theme);
        public Dictionary<Direction, List<EditorTile>> Matches { get; } = new Dictionary<Direction, List<EditorTile>>();

        public override string ToString() 
        {
            return $"#{Tile.Index} {string.Join("+", Themes.ToArray())}";
        }

        public EditorTile(Tile tile)
        {
            Tile = tile;

            foreach (Direction direction in Enum.GetValues<Direction>())
            {
                if (direction == Direction.None)
                    continue;

                Matches[direction] = new List<EditorTile>();
            }
        }

        public void AddTheme(string str)
        {
            if (!Themes.Contains(str))
                Themes.Add(str);
        }

        public void AddMatch(Direction direction, EditorTile other)
        {
            var list = Matches[direction];
            if (!list.Contains(other))
                list.Add(other);

            var otherList = other.Matches[direction.Opposite()];
            if (!otherList.Contains(this))
                otherList.Add(this);
        }
    }
}
