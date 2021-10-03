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
            animationSet[AnimationKey.Moving] = 1;

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
            return new SpriteAnimator(
                GameSystem,
                new SpriteFrame[]
                {
                    //idle
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
                    //walk 
                    new SpriteFrame(TopLeft: new Tile(18, TileFlags.Solid),
                                    TopRight: new Tile(19, TileFlags.Solid),
                                    BottomLeft: new Tile(24, TileFlags.Solid),
                                    BottomRight: new Tile(25, TileFlags.Solid)),
                    new SpriteFrame(TopLeft: new Tile(20, TileFlags.Solid),
                                    TopRight: new Tile(21, TileFlags.Solid),
                                    BottomLeft: new Tile(26, TileFlags.Solid),
                                    BottomRight: new Tile(27, TileFlags.Solid)),
                    new SpriteFrame(TopLeft: new Tile(22, TileFlags.Solid),
                                    TopRight: new Tile(23, TileFlags.Solid),
                                    BottomLeft: new Tile(28, TileFlags.Solid),
                                    BottomRight: new Tile(29, TileFlags.Solid)),
                },
                new AnimationFrame[]
                {
                    //idle
                    new AnimationFrame(SpriteFrameIndex:0,Duration:50),
                    new AnimationFrame(SpriteFrameIndex:1,Duration:50),
                    new AnimationFrame(SpriteFrameIndex:2,Duration:50),

                    //walk
                    new AnimationFrame(SpriteFrameIndex:3,Duration:10),
                    new AnimationFrame(SpriteFrameIndex:4,Duration:10),
                    new AnimationFrame(SpriteFrameIndex:5,Duration:10),

                },
                new Animation[]
                {
                    new Animation(0,1,2,1),
                    new Animation(3,4,5)
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
