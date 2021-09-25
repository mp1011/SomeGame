namespace SomeGame.Main.Models
{
    class Layer : TiledObject
    {
        public Layer(TileMap tileMap, PaletteIndex palette, RotatingInt scrollX, RotatingInt scrollY, int tileSize) : base(tileMap, palette, scrollX, scrollY, tileSize)
        {
        }
    }
}
