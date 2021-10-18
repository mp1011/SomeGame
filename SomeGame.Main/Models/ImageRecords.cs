using SomeGame.Main.Content;
using System.Collections.Generic;

namespace SomeGame.Main.Models
{
    public record PaletteKeys(ImageContentKey P1, ImageContentKey P2, ImageContentKey P3, ImageContentKey P4);
    public record IndexedTilesetImage(TilesetContentKey Key, Grid<byte> Image, Palette Palette) : IndexedImage(Image,Palette);
    public record IndexedImage(Grid<byte> Image, Palette Palette);
    public record Tile(int Index, TileFlags Flags)
    {
        public bool IsSolid => (Flags & TileFlags.Solid) != 0;
        public bool IsCollectible => (Flags & TileFlags.Collectible) != 0;

        public Tile NextFlip() => new Tile(Index, GetNextFlipFlags(Flags));

        private static TileFlags GetNextFlipFlags(TileFlags flags)
        {
            var flipBase = flags & ~TileFlags.FlipHV;
            var currentFlip = flags & TileFlags.FlipHV;

            switch (currentFlip)
            {
                case TileFlags.None: return flipBase | TileFlags.FlipH;
                case TileFlags.FlipH: return flipBase | TileFlags.FlipV;
                case TileFlags.FlipV: return flipBase | TileFlags.FlipHV;
                default: return flipBase;
            }
        }
    }

    public record VramData(Grid<byte> ImageData, Dictionary<TilesetContentKey,int> Offsets);

}
