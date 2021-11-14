using SomeGame.Main.Content;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Models
{
    class EditorTileSet
    {
        public List<EditorBlock> Blocks { get; } = new List<EditorBlock>();

        public TilesetContentKey Key { get; }

        public string[] Themes => Blocks.Select(p => p.Theme)
                                     .Distinct()
                                     .ToArray();

        public EditorBlock[] GetBlocks(string theme)
        {
            return Blocks.Where(b => b.Theme == theme)
                         .ToArray();
        }

        public Tile[] GetTilesInTheme(string theme)
        {
            return Blocks.Where(p => p.Theme == theme)
                         .SelectMany(b => b.Grid.ToArray())
                         .Distinct()
                         .OrderBy(p => p.Index)
                         .ToArray();
        }

        public EditorTileSet(TilesetContentKey key)
        {
            Key = key;
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

        public EditorBlock(string theme)
        {
            Theme = theme;
            Grid = new Grid<Tile>(1, 1, (x, y) => new Tile(-1, TileFlags.None));
        }
    }
}
