namespace SomeGame.Main.Models
{
    class Layer : TiledObject
    {
        private RamByte _scrollFactor;

        public byte ScrollFactor
        {
            get => _scrollFactor;
            set => _scrollFactor.Set(value);
        }

        public Layer(GameSystem gameSystem,  TileMap tileMap, PaletteIndex palette, RotatingInt scrollX, RotatingInt scrollY, int tileSize) : 
            base(tileMap, palette, scrollX, scrollY, tileSize)
        {
            _scrollFactor = gameSystem.RAM.DeclareByte(100);
        }
    }
}
