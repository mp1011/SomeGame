using SomeGame.Main.Content;
using System.Collections.Generic;

namespace SomeGame.Main.Models
{
    public record IndexedTilesetImage(TilesetContentKey Key, Grid<byte> Image, Palette Palette) : IndexedImage(Image,Palette);
    public record IndexedImage(Grid<byte> Image, Palette Palette);
    public record Tile(int Index, TileFlags Flags)
    {
        public bool IsSolid => (Flags & TileFlags.Solid) != 0;
    }

    public record VramData(Grid<byte> ImageData, Dictionary<TilesetContentKey,int> Offsets);

}
