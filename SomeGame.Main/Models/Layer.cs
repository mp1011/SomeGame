namespace SomeGame.Main.Models
{
    class Layer : TiledObject
    {
        public byte ScrollFactor { get; set; } = 100;

        public Layer(TileMap tileMap, PaletteIndex palette, RotatingInt scrollX, RotatingInt scrollY, int tileSize) : base(tileMap, palette, scrollX, scrollY, tileSize)
        {
        }
    }
}
