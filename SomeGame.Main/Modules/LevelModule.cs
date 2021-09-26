using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System.Collections.Generic;

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

        protected override void InitializeActors(GameSystem system, SpriteAnimator spriteAnimator, ActorManager actorManager)
        {          
            var animationSet = new Dictionary<AnimationKey, byte>();
            animationSet[AnimationKey.Idle] = 0;

            var player = new Actor(TilesetContentKey.Hero, PaletteIndex.P2, animationSet);
            player.WorldPosition.X = 100;
            player.WorldPosition.Y = 100;

            actorManager.TryAddActor(system, player);                
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

        protected override Scene InitializeScene(GameSystem gameSystem)
        {
            return new Scene(new Rectangle(0, 0, 1000, gameSystem.LayerPixelHeight), gameSystem);
        }

        bool dummyValue;
        protected override void Update(GameSystem gameSystem, Scene currentScene)
        {
            if (dummyValue)
            {
                currentScene.Camera.X -= 1;
                if (currentScene.Camera.X <= 0)
                    dummyValue = false;
            }
            else
            {
                currentScene.Camera.X += 1;
                if (currentScene.Camera.X >= 200)
                    dummyValue = true;
            }
        }
    }
}
