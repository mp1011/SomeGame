using SomeGame.Main.Content;
using System.Collections.Generic;

namespace SomeGame.Main.Models
{
    public record IndexedTilesetImage(TilesetContentKey Key, MemoryGrid<byte> Image, Palette Palette) : IndexedImage(Image,Palette);
    public record IndexedImage(MemoryGrid<byte> Image, Palette Palette);

    public class RamTile
    {
        private readonly RamByte _tileIndex;
        private readonly RamEnum<TileFlags> _flags;

        public byte Index => _tileIndex;
        public TileFlags Flags => _flags;

        public bool IsSolid => (Flags & TileFlags.Solid) != 0;
        public bool IsCollectible => (Flags & TileFlags.Collectible) != 0;

        public RamTile(RamByte tileIndex, RamEnum<TileFlags> flags)
        {
            _tileIndex = tileIndex;
            _flags = flags;
        }

        public void Set(Tile tile)
        {
            if (tile != null)
            {
                _tileIndex.Set(tile.Index);
                _flags.Set(tile.Flags);
            }
            else
            {
                _tileIndex.Set(255);
                _flags.Set(TileFlags.None);
            }
        }

        public static implicit operator Tile(RamTile rt) => new Tile(rt._tileIndex, rt._flags);
    }

    public record Tile(byte Index, TileFlags Flags)
    {
        public Tile(int Index, TileFlags flags) : this((byte)(Index < 0 ? 255 : Index), flags) { }

        public Tile() : this(0, TileFlags.None) { }

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
}
