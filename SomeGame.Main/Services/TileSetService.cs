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
                for(int i = 0; i < tileSetIndexedImage.Size; i++)
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
