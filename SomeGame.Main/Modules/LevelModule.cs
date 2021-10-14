using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private HUDManager _hudManager;
        private PlayerState _playerState;

        public LevelModule()
        {
            _dataSerializer = new DataSerializer();
        }

        protected override void AfterInitialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
        {
            _hudManager = new HUDManager(GameSystem);
            _hudManager.DrawTiles();

            _playerState = new PlayerState { Health = new BoundedInt(12, 12), Lives = 3, Score = 0 };
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
                resourceLoader.LoadTexture(TilesetContentKey.Bullet).ToIndexedTilesetImage(),
                resourceLoader.LoadTexture(TilesetContentKey.Hud).ToIndexedTilesetImage(),
                resourceLoader.LoadTexture(TilesetContentKey.Font).ToIndexedTilesetImage(),
            };
        }

        protected override void InitializeActors()
        {
            var actorFactory = new ActorFactory(ActorManager, GameSystem, _dataSerializer);

            var playerProjectile = actorFactory.CreateActor(
                 actorId: ActorId.PlayerBullet,
                 actorType: ActorType.Player | ActorType.Bullet,
                 tileset: TilesetContentKey.Bullet,
                 paletteIndex: PaletteIndex.P4,
                 behavior: new ProjectileBehavior(Direction.Right, new PixelValue(1, 0)),
                 collisionDetector: new ActorCollisionDetector(ActorManager, ActorType.Enemy | ActorType.Character),
                 hitBox: new Rectangle(0,0,8,8),
                 position: new PixelPoint(0,0)
            );;

            playerProjectile.CurrentAnimation = AnimationKey.Moving;
          
            actorFactory.CreateActor(
                actorId: ActorId.Player,
                actorType: ActorType.Player | ActorType.Character,
                tileset: TilesetContentKey.Hero,
                paletteIndex: PaletteIndex.P2,
                behavior: new PlayerBehavior(
                                new PlatformerPlayerMotionBehavior(InputManager),
                                new PlayerHurtBehavior(),
                                new CameraBehavior(SceneManager, GameSystem),
                                new Gravity(),
                                InputManager, 
                                playerProjectile),
                collisionDetector: new BgCollisionDetector(GameSystem),
                hitBox: new Rectangle(4,0,8,14),
                position: new PixelPoint(50,100));

            var enemyProjectile = actorFactory.CreateActor(
                actorId: ActorId.SkeletonBone,
                actorType: ActorType.Enemy | ActorType.Bullet,
                tileset: TilesetContentKey.Skeleton,
                paletteIndex: PaletteIndex.P3,
                behavior: new ProjectileBehavior(Direction.Left, new PixelValue(1,0)),
                collisionDetector: new ActorCollisionDetector(ActorManager, ActorType.Player | ActorType.Character),
                hitBox: new Rectangle(0,0,8,8),
                position: new PixelPoint(0,0));

            enemyProjectile.CurrentAnimation = AnimationKey.Moving;


           actorFactory.CreateActor(
                actorId: ActorId.Skeleton,
                actorType: ActorType.Enemy | ActorType.Character,
                tileset: TilesetContentKey.Skeleton,
                paletteIndex: PaletteIndex.P3,
                behavior: new SkeletonBehavior(new Gravity(), new EnemyBaseBehavior(), enemyProjectile),
                collisionDetector: new BgCollisionDetector(GameSystem),
                hitBox: new Rectangle(4,0,8,15),
                position: new PixelPoint(150, 40));

            enemyProjectile.Enabled = false;
            playerProjectile.Enabled = false;

        }

        protected override Scene InitializeScene()
        {
            return new Scene(new Rectangle(0, 0, GameSystem.LayerPixelWidth, GameSystem.LayerPixelHeight), GameSystem);
        }

        int dummy = 0;

        protected override void Update()
        {
            dummy++;
            if (dummy == 100)
            {
                dummy = 0;
                _playerState.Health -= 1;
            }
            _hudManager.UpdateHUD(_playerState);
        }
    }
}
