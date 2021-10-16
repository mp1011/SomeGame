using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Behaviors;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

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
            _playerState = new PlayerState { Health = new BoundedInt(12, 12), Lives = 3, Score = 0 };
            var actorFactory = new ActorFactory(ActorManager, GameSystem, _dataSerializer);

            var playerProjectile = actorFactory.CreateActor(
                 actorId: ActorId.PlayerBullet,
                 actorType: ActorType.Player | ActorType.Bullet,
                 tileset: TilesetContentKey.Bullet,
                 paletteIndex: PaletteIndex.P4,
                 behavior: new ProjectileBehavior(Direction.Right, new PixelValue(1, 0)),
                 destroyedBehavior: new EmptyDestroyedBehavior(),
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
                                playerProjectile,
                                _playerState),
                destroyedBehavior: new EmptyDestroyedBehavior(),
                collisionDetector: new BgCollisionDetector(GameSystem),
                hitBox: new Rectangle(4,0,8,14),
                position: new PixelPoint(50,100));

            var enemyProjectile = actorFactory.CreateActor(
                actorId: ActorId.SkeletonBone,
                actorType: ActorType.Enemy | ActorType.Bullet,
                tileset: TilesetContentKey.Skeleton,
                paletteIndex: PaletteIndex.P3,
                behavior: new ProjectileBehavior(Direction.Left, new PixelValue(1,0)),
                destroyedBehavior: new EmptyDestroyedBehavior(),
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
                destroyedBehavior: new EnemyDestroyedBehavior(score:100, _playerState),
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

        protected override void Update()
        {
            _hudManager.UpdateHUD(_playerState);
        }
    }
}
