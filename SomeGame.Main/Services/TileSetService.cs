using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SomeGame.Main.Services
{
    class TileSetService
    {
        public IndexedImage CreateTilesetFromImage(IndexedImage indexedImage)
        {
            var tiles = indexedImage.Image
                                    .Split(8)
                                    .ToArray();

            var distinctTiles = tiles
                                   .Distinct(new MirrorAgnosticGridComparer<byte>())
                                   .ToArray()
                                   .Combine(32);

            return new IndexedImage(distinctTiles, indexedImage.Palette);
        }

        public Grid<Tile> CreateTileMapFromImageAndTileset(IndexedImage image, TileSet tileSet)
        {
            var imageTiles = image.Image.Split(8);

            var tileSetIndexedImage = tileSet.Texture
                                             .ToIndexedImage(image.Palette)
                                             .Image
                                             .Split(8);

            return imageTiles.Map((x, y, imageTile) =>
            {
                for (int i = 0; i < tileSetIndexedImage.Size; i++)
                {
                    var tilesetTile = tileSetIndexedImage[i];
                    if (tilesetTile.Equals(imageTile))
                        return new Tile(i, TileFlags.None);
                    else if (tilesetTile.Equals(imageTile.CreateMirror(Flip.H)))
                        return new Tile(i, TileFlags.FlipH);
                    else if (tilesetTile.Equals(imageTile.CreateMirror(Flip.V)))
                        return new Tile(i, TileFlags.FlipV);
                    else if (tilesetTile.Equals(imageTile.CreateMirror(Flip.Both)))
                        return new Tile(i, TileFlags.FlipH | TileFlags.FlipV);
                }

                throw new Exception("Unable to match image tile with a tileSet tile");
            });
        }

        public EditorTileSet BuildEditorTileset(EditorBlock[] blocks)
        {
            var tileset = new EditorTileSet();

            foreach (var block in blocks)
                FillEditorTiles(tileset, block.Grid, block.Theme);

            return tileset;
        }

        public EditorTile[] GetMatchingTiles(EditorTileSet editorTileSet, string theme, TileMap tileMap, 
                                             Point tileLocation, TileChoiceMode tileChoiceMode)
        {
            return editorTileSet.Tiles
                .Where(p => p.ContainsTheme(theme)
                            && CanTileBePlacedAtLocation(p, editorTileSet, tileMap, tileLocation, tileChoiceMode))
                .ToArray();
        }

        public void AddTileRelationsFromTileMap(EditorTileSet editorTileSet, TileMap tileMap)
        {
            FillEditorTiles(editorTileSet, tileMap.GetGrid(), null);
        }

        private bool CanTileBePlacedAtLocation(EditorTile proposedTile, EditorTileSet editorTileSet, TileMap tileMap, Point tileLocation, TileChoiceMode tileChoiceMode)
        {
            if (tileChoiceMode == TileChoiceMode.Free)
                return true;

            foreach (Direction direction in Enum.GetValues<Direction>())
            {
                if (direction == Direction.None)
                    continue;

                var neighbor = tileMap.GetGrid().GetNeighborOrDefault(tileLocation.X, tileLocation.Y, direction);
                if (neighbor == null)
                    continue;

                var neighborEditorTile = editorTileSet.GetOrAddTile(neighbor);
                if (neighborEditorTile.Tile.Index < 0 && tileChoiceMode == TileChoiceMode.SemiStrict)
                    continue;

                if (!neighborEditorTile.Matches[direction.Opposite()].Contains(proposedTile))
                    return false;
            }

            return true;
        }

        private void FillEditorTiles(EditorTileSet tileSet, Grid<Tile> grid, string theme)
        {
            grid.ForEach((x, y, t) =>
            {
                if (t.Index < 0)
                    return;

                var editorTile = tileSet.GetOrAddTile(t);
                if(theme != null)
                    editorTile.AddTheme(theme);

                foreach (Direction direction in Enum.GetValues<Direction>())
                {
                    if (direction == Direction.None)
                        continue;

                    var neighbor = grid.GetNeighborOrDefault(x, y, direction);
                    if (neighbor == null || neighbor.Index < 0)
                        continue;

                    editorTile.AddMatch(direction, tileSet.GetOrAddTile(neighbor));
                }
            });
        }
    }

    class MirrorAgnosticGridComparer<T> : IEqualityComparer<Grid<T>>
    {
        public bool Equals(Grid<T> x, Grid<T> y)
        {
            if (x.Equals(y))
                return true;

            if (x.Equals(y.CreateMirror(Flip.H)))
                return true;

            if (x.Equals(y.CreateMirror(Flip.V)))
                return true;

            if (x.Equals(y.CreateMirror(Flip.Both)))
                return true;

            return false;
        }

        public int GetHashCode([DisallowNull] Grid<T> obj)
        {
            return obj.GetHashCode();
        }
    }
}
