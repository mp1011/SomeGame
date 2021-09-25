using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Models.AnimationModels;
using SomeGame.Main.Services;

namespace SomeGame.Main.Modules
{
    class LevelModule : GameModuleBase
    {
        private readonly DataSerializer _dataSerializer;
        public LevelModule()
        {
            _dataSerializer = new DataSerializer();
        }

        protected override Palette CreatePalette(IndexedTilesetImage[] tilesetImages, PaletteIndex index)
        {
            switch(index)
            {
                case PaletteIndex.P1: return tilesetImages[0].Palette;
                default: return tilesetImages[1].Palette;
            }            
        }

        protected override void InitializeLayer(GameSystem system, LayerIndex index, Layer layer)
        {
            if(index == LayerIndex.FG)
            {
                var loaded = _dataSerializer.Load(LevelContentKey.TestLevel);
                layer.TileMap.SetEach((x, y) => loaded.GetTile(x, y));
            }
        }

        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader, GameSystem system)
        {
            return new IndexedTilesetImage[]
            {
                resourceLoader.LoadTexture(TilesetContentKey.Tiles).ToIndexedTilesetImage(),
                resourceLoader.LoadTexture(TilesetContentKey.Hero).ToIndexedTilesetImage()
            };
        }

        protected override void InitializeSprites(GameSystem system, SpriteAnimator spriteAnimator)
        {
            var sprite = system.GetSprite(SpriteIndex.Sprite1);
            sprite.TileOffset = system.GetTileOffset(TilesetContentKey.Hero);
            sprite.Priority = SpritePriority.Front;
            sprite.Palette = PaletteIndex.P2;
            sprite.Enabled = true;
            sprite.ScrollX = sprite.ScrollX.Set(100);
            sprite.ScrollY = sprite.ScrollY.Set(100);

            spriteAnimator.SetSpriteAnimation(SpriteIndex.Sprite1, 0);
        }

        protected override SpriteAnimator InitializeAnimations()
        {
            return new SpriteAnimator(
                new SpriteFrame[]
                {
                    new SpriteFrame(TopLeft: new Tile(0, TileFlags.Solid),
                                    TopRight: new Tile(1, TileFlags.Solid),
                                    BottomLeft: new Tile(9, TileFlags.Solid),
                                    BottomRight: new Tile(10, TileFlags.Solid)),
                    new SpriteFrame(TopLeft: new Tile(2, TileFlags.Solid),
                                    TopRight: new Tile(3, TileFlags.Solid),
                                    BottomLeft: new Tile(11, TileFlags.Solid),
                                    BottomRight: new Tile(12, TileFlags.Solid)),
                    new SpriteFrame(TopLeft: new Tile(4, TileFlags.Solid),
                                    TopRight: new Tile(5, TileFlags.Solid),
                                    BottomLeft: new Tile(9, TileFlags.Solid),
                                    BottomRight: new Tile(10, TileFlags.Solid)),
                },
                new AnimationFrame[]
                {
                    new AnimationFrame(SpriteFrameIndex:0,Duration:50),
                    new AnimationFrame(SpriteFrameIndex:1,Duration:50),
                    new AnimationFrame(SpriteFrameIndex:2,Duration:50),
                },
                new Animation[]
                {
                    new Animation(0,1,2,1)
                });
        }

        protected override void Update(GameTime gameTime, GameSystem gameSystem)
        {
            
        }
    }
}
