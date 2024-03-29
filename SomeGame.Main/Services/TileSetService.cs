﻿using Microsoft.Xna.Framework;
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
            throw new System.NotImplementedException();
            //var imageTiles = image.Image.Split(8);

            //var tileSetIndexedImage = tileSet.Texture
            //                                 .ToIndexedImage(image.Palette)
            //                                 .Image
            //                                 .Split(8);

            //return imageTiles.Map((x, y, imageTile) =>
            //{
            //    for (int i = 0; i < tileSetIndexedImage.Size; i++)
            //    {
            //        var tilesetTile = tileSetIndexedImage[i];
            //        if (tilesetTile.Equals(imageTile))
            //            return new Tile(i, TileFlags.None);
            //        else if (tilesetTile.Equals(imageTile.CreateMirror(Flip.H)))
            //            return new Tile(i, TileFlags.FlipH);
            //        else if (tilesetTile.Equals(imageTile.CreateMirror(Flip.V)))
            //            return new Tile(i, TileFlags.FlipV);
            //        else if (tilesetTile.Equals(imageTile.CreateMirror(Flip.Both)))
            //            return new Tile(i, TileFlags.FlipH | TileFlags.FlipV);
            //    }

            //    throw new Exception("Unable to match image tile with a tileSet tile");
            //});
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
                .OrderByDescending(RankBlock)
                .SelectMany(b => GetPossibleTilesForLocation(b, neighbors))
                .OrderByDescending(p=>p.Item2)
                .Select(p=>p.Item1)
                .Distinct()
                .ToArray();     
        }

        private int RankBlock(EditorBlock editorBlock)
        {
            bool canRepeatX = true;
            bool canRepeatY = true;

            editorBlock.Grid.ForEach(0, editorBlock.Grid.Width / 2, 0, editorBlock.Grid.Height / 2,
                (x, y, t) =>
                {
                    int x2 = x + editorBlock.Grid.Width / 2;
                    if (editorBlock.Grid.Height == 1 || x2 >= editorBlock.Grid.Width || editorBlock.Grid[x2, y] != t)
                        canRepeatX = false;

                    int y2 = y + editorBlock.Grid.Width / 2;
                    if (editorBlock.Grid.Width == 1 || y2 >= editorBlock.Grid.Height || editorBlock.Grid[x, y2] != t)
                        canRepeatY = false;
                });

            int rank = 0;
            if (canRepeatX)
                rank++;
            if (canRepeatY)
                rank++;

            return rank;
        }

        private IEnumerable<(Tile,int)> GetPossibleTilesForLocation(EditorBlock tileRelations, Dictionary<Direction, Tile> neighbors)
        {
            List<(Tile,int)> possibleTiles = new List<(Tile, int)>();

            tileRelations.Grid.ForEach((x, y, tile) =>
            {
                bool? leftMatch=null, topMatch = null, bottomMatch = null, rightMatch = null;

                if(x > 0)
                {
                    var left = tileRelations.Grid[x - 1, y];
                    if (left == neighbors[Direction.Left])
                        leftMatch = true;
                    else if (neighbors[Direction.Left].Index >= 0)
                        leftMatch = false;
                }

                if (y > 0)
                {
                    var above = tileRelations.Grid[x, y-1];
                    if (above == neighbors[Direction.Up])
                        topMatch = true;
                    else if (neighbors[Direction.Up].Index >= 0)
                        topMatch = false;

                }

                if (x < tileRelations.Grid.Width-1)
                {
                    var right = tileRelations.Grid[x+1, y];
                    if (right == neighbors[Direction.Right])
                        rightMatch = true;
                    else if (neighbors[Direction.Right].Index >= 0)
                        rightMatch = false;
                }

                if (y < tileRelations.Grid.Height - 1)
                {
                    var below = tileRelations.Grid[x, y+1];
                    if (below == neighbors[Direction.Down])
                        bottomMatch = true;
                    else if (neighbors[Direction.Down].Index >= 0)
                        bottomMatch = false;
                }

                if (leftMatch.GetValueOrDefault() && rightMatch.GetValueOrDefault()
                    && topMatch.GetValueOrDefault() && bottomMatch.GetValueOrDefault())
                {
                    possibleTiles.Add(new(tile, 1));
                }
                else if (
                    (leftMatch.GetValueOrDefault() || rightMatch.GetValueOrDefault() || topMatch.GetValueOrDefault() || bottomMatch.GetValueOrDefault())
                    && (leftMatch == null || leftMatch.Value)
                    && (rightMatch == null || rightMatch.Value)
                    && (topMatch == null || topMatch.Value)
                    && (bottomMatch == null || bottomMatch.Value))
                {
                    possibleTiles.Add(new(tile, 0));
                }
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
