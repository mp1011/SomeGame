namespace SomeGame.Main.Models
{
    public record IndexedImage(Grid<byte> Image, Palette Palette);

    public record Tile(int Index, TileFlags Flags);
}
