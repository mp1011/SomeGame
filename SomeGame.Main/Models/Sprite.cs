using SomeGame.Main.Content;

namespace SomeGame.Main.Models
{
    class Sprite : TiledObject
    {
        private RamEnum<SpriteFlags> _flags;

        public bool Priority
        {
            get => (_flags & SpriteFlags.Priority) > 0;
            set => _flags.SetFlag(SpriteFlags.Priority, value);
        }

        public Flip Flip { get; set; }
        public bool Enabled { get; set; }

        public Sprite(GameSystem gameSystem, int layerPixelWidth, int layerPixelHeight, int tileSize) 
            : base(CreateBlankTilemap(), PaletteIndex.P1, 
                  new RotatingInt(0,layerPixelWidth), 
                  new RotatingInt(0,layerPixelHeight),
                  tileSize)
        {
            _flags = gameSystem.RAM.DeclareEnum(SpriteFlags.None);
        }

        private static TileMap CreateBlankTilemap()
        {
            return new TileMap(LevelContentKey.None, 2, 2);
        }

        public void SetTiles(Tile topLeft, Tile topRight, Tile bottomLeft, Tile bottomRight)
        {
            TileMap.SetTile(0, 0, topLeft);
            TileMap.SetTile(1, 0, topRight);
            TileMap.SetTile(0, 1, bottomLeft);
            TileMap.SetTile(1, 1, bottomRight);
        }
    }
}
