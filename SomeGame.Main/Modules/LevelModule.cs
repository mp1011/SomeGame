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
                case PaletteIndex.P2: return tilesetImages[1].Palette;
                case PaletteIndex.P3: return tilesetImages[2].Palette;
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
                resourceLoader.LoadTexture(TilesetContentKey.Hero).ToIndexedTilesetImage(),
                resourceLoader.LoadTexture(TilesetContentKey.Skeleton).ToIndexedTilesetImage(),
            };
        }

        protected override void InitializeActors()
        {
            var actorFactory = new ActorFactory(ActorManager, GameSystem);

            actorFactory.CreateActor(
                tileset: TilesetContentKey.Hero,
                paletteIndex: PaletteIndex.P2,
                animations: new Dictionary<AnimationKey, byte> 
                {
                   [AnimationKey.Idle] = 0,
                   [AnimationKey.Jumping] = 1,
                   [AnimationKey.Moving] = 2,            
                   [AnimationKey.Attacking] = 3
                },
                behavior: new PlayerBehavior(
                                new PlatformerPlayerMotionBehavior(InputManager),
                                new CameraBehavior(SceneManager, GameSystem),
                                new Gravity()),
                collisionDetector: new BgCollisionDetector(GameSystem),
                position: new GameRectangleWithSubpixels(50,100,8,14));

           var projectile = actorFactory.CreateActor(
                tileset: TilesetContentKey.Skeleton,
                paletteIndex: PaletteIndex.P3,
                animations: new Dictionary<AnimationKey, byte>
                {
                    [AnimationKey.Moving] = 7,
                },
                behavior: new ProjectileBehavior(Direction.Left, new PixelValue(1,0)),
                collisionDetector: new EmptyCollisionDetector(),
                position: new GameRectangleWithSubpixels(0,0,8,8));

            projectile.CurrentAnimation = AnimationKey.Moving;


           actorFactory.CreateActor(
                tileset: TilesetContentKey.Skeleton,
                paletteIndex: PaletteIndex.P3,
                animations: new Dictionary<AnimationKey, byte>
                {
                    [AnimationKey.Idle] = 4,
                    [AnimationKey.Moving] = 5,
                    [AnimationKey.Attacking] = 6
                },
                behavior: new SkeletonBehavior(new Gravity(), new EnemyBaseBehavior(), projectile),
                collisionDetector: new BgCollisionDetector(GameSystem),
                position: new GameRectangleWithSubpixels(150, 40,8,15));

            projectile.Enabled = false;
        }

        protected override SpriteAnimator InitializeAnimations()
        {
            List<SpriteFrame> spriteFrames = new List<SpriteFrame>();
            spriteFrames.AddRange(_dataSerializer.LoadSpriteFrames(TilesetContentKey.Hero));
            spriteFrames.AddRange(_dataSerializer.LoadSpriteFrames(TilesetContentKey.Skeleton));

            return new SpriteAnimator(
                GameSystem,
                spriteFrames,
                new AnimationFrame[]
                {
                    //player idle
                    new AnimationFrame(SpriteFrameIndex:0,Duration:50),
                    new AnimationFrame(SpriteFrameIndex:1,Duration:50),
                    new AnimationFrame(SpriteFrameIndex:2,Duration:50),

                    //player jump 
                    new AnimationFrame(SpriteFrameIndex:4,Duration:50),
                    new AnimationFrame(SpriteFrameIndex:3,Duration:50),

                    //player attack
                    new AnimationFrame(SpriteFrameIndex:5,Duration:50),
                    new AnimationFrame(SpriteFrameIndex:6,Duration:50),

                    //player walk
                    new AnimationFrame(SpriteFrameIndex:7,Duration:10),
                    new AnimationFrame(SpriteFrameIndex:8,Duration:10),
                    new AnimationFrame(SpriteFrameIndex:9,Duration:10),
                    new AnimationFrame(SpriteFrameIndex:10,Duration:10),
                    new AnimationFrame(SpriteFrameIndex:11,Duration:10),

                    //skeleton idle
                    new AnimationFrame(SpriteFrameIndex:12, Duration:10),

                    //skeleton walk
                    new AnimationFrame(SpriteFrameIndex:13, Duration:20),
                    new AnimationFrame(SpriteFrameIndex:14, Duration:20),

                    //skeleton attack
                    new AnimationFrame(SpriteFrameIndex:15, Duration:50),

                    //skeleton bone
                    new AnimationFrame(SpriteFrameIndex:16, Duration:10),
                    new AnimationFrame(SpriteFrameIndex:17, Duration:10),
                    new AnimationFrame(SpriteFrameIndex:18, Duration:18),
                    new AnimationFrame(SpriteFrameIndex:19, Duration:19),
                },
                new Animation[]
                {
                    new Animation(0,1,2,1), //idle
                    new Animation(3,4), //jump
                    new Animation(7,8,9,10,11), //walk 
                    new Animation(5,6), // attack
                    new Animation(12), // skeleton idle
                    new Animation(13,14), // skeleton walk
                    new Animation(15), // skeleton attack
                    new Animation(16,17,18,19), //skeleton bone
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
