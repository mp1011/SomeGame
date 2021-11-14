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

        public Tile[] GetMatchingTiles(EditorTileSet editorTileSet, string theme, TileMap tileMap, 
                                             Point tileLocation, TileChoiceMode tileChoiceMode)
        {
            var blocks = editorTileSet.GetBlocks(theme);
            var neighbors = new Dictionary<Direction, Tile>();
            neighbors[Direction.Up] = tileMap.GetTile(tileLocation.GetNeighbor(Direction.Up));
            neighbors[Direction.Down] = tileMap.GetTile(tileLocation.GetNeighbor(Direction.Down));
            neighbors[Direction.Left] = tileMap.GetTile(tileLocation.GetNeighbor(Direction.Left));
            neighbors[Direction.Right] = tileMap.GetTile(tileLocation.GetNeighbor(Direction.Right));

            return blocks
                .SelectMany(b => GetPossibleTilesForLocation(b, neighbors))
                .Distinct()
                .ToArray();     
        }

        private IEnumerable<Tile> GetPossibleTilesForLocation(EditorBlock tileRelations, Dictionary<Direction, Tile> neighbors)
        {
            List<Tile> possibleTiles = new List<Tile>();

            tileRelations.Grid.ForEach((x, y, tile) =>
            {
                bool comparedAny = false;
                if(x > 0 && neighbors[Direction.Left].Index >= 0)
                {
                    comparedAny = true;
                    var left = tileRelations.Grid[x - 1, y];
                    if (left != neighbors[Direction.Left])
                        return;
                }

                if (y > 0 && neighbors[Direction.Up].Index >= 0)
                {
                    comparedAny = true;
                    var above = tileRelations.Grid[x, y-1];
                    if (above != neighbors[Direction.Up])
                        return;
                }

                if (x < tileRelations.Grid.Width-1 && neighbors[Direction.Right].Index >= 0)
                {
                    comparedAny = true;
                    var right = tileRelations.Grid[x+1, y];
                    if (right != neighbors[Direction.Right])
                        return;
                }

                if (y < tileRelations.Grid.Height - 1 && neighbors[Direction.Down].Index >= 0)
                {
                    comparedAny = true;
                    var below = tileRelations.Grid[x, y+1];
                    if (below != neighbors[Direction.Down])
                        return;
                }

                if(comparedAny)
                    possibleTiles.Add(tile);
            });

            return possibleTiles;
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
