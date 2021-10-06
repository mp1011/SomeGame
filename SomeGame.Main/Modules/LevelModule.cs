using Microsoft.Xna.Framework;
using SomeGame.Main.Behaviors;
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

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {
            if(index == LayerIndex.FG)
            {
                var loaded = _dataSerializer.Load(LevelContentKey.TestLevel);

                //temporary
                loaded.ForEach((x, y, t) =>
                {
                    if (t.Index >= 0)
                        loaded.SetTile(x, y, new Tile(t.Index, TileFlags.Solid));
                });

                layer.TileMap.SetEach((x, y) => loaded.GetTile(x, y));
            }
        }

        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader)
        {
            return new IndexedTilesetImage[]
            {
                resourceLoader.LoadTexture(TilesetContentKey.Tiles).ToIndexedTilesetImage(),
                resourceLoader.LoadTexture(TilesetContentKey.Hero).ToIndexedTilesetImage()
            };
        }

        protected override void InitializeActors()
        {          
            var animationSet = new Dictionary<AnimationKey, byte>();
            animationSet[AnimationKey.Idle] = 0;
            animationSet[AnimationKey.Jumping] = 1;
            animationSet[AnimationKey.Moving] = 2;            
            animationSet[AnimationKey.Attacking] = 3;

            var playerBehavior = new PlayerBehavior(
                new PlatformerPlayerMotionBehavior(InputManager),
                new CameraBehavior(SceneManager, GameSystem), 
                new BgCollisionBehavior(GameSystem),
                new Gravity());

            var player = new Actor(TilesetContentKey.Hero, PaletteIndex.P2, playerBehavior, animationSet);
            player.WorldPosition.X = 50;
            player.WorldPosition.Y = 100;

            ActorManager.TryAddActor(GameSystem, player);                
        }

        protected override SpriteAnimator InitializeAnimations()
        {
            var spriteFrames = _dataSerializer.LoadSpriteFrames(TilesetContentKey.Hero);

            return new SpriteAnimator(
                GameSystem,
                spriteFrames,
                new AnimationFrame[]
                {
                    //idle
                    new AnimationFrame(SpriteFrameIndex:0,Duration:50),
                    new AnimationFrame(SpriteFrameIndex:1,Duration:50),
                    new AnimationFrame(SpriteFrameIndex:2,Duration:50),

                    //jump 
                    new AnimationFrame(SpriteFrameIndex:4,Duration:50),
                    new AnimationFrame(SpriteFrameIndex:3,Duration:50),

                    //walk
                    new AnimationFrame(SpriteFrameIndex:5,Duration:10),
                    new AnimationFrame(SpriteFrameIndex:6,Duration:10),
                    new AnimationFrame(SpriteFrameIndex:7,Duration:10),
                    new AnimationFrame(SpriteFrameIndex:8,Duration:10),

                    //attack
                    new AnimationFrame(SpriteFrameIndex:9,Duration:50),
                    new AnimationFrame(SpriteFrameIndex:10,Duration:50),

                },
                new Animation[]
                {
                    new Animation(0,1,2,1), //idle
                    new Animation(3,4), //jump
                    new Animation(5,6,7,8), //walk 
                    new Animation(9,10) // attack
                });
        }

        protected override Scene InitializeScene()
        {
            return new Scene(new Rectangle(0, 0, GameSystem.LayerPixelWidth, GameSystem.LayerPixelHeight), GameSystem);
        }

        protected override void Update()
        {
        }
    }
}
