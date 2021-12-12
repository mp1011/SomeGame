using Microsoft.Xna.Framework;
using SomeGame.Main.Behaviors;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using System;
using System.Collections.Generic;

namespace SomeGame.Main.Services
{
    class ActorFactory
    {
        private readonly ActorManager _actorManager;
        private readonly GameSystem _gameSystem;
        private readonly DataSerializer _dataSerializer;
        private readonly InputManager _inputManager;
        private readonly SceneManager _sceneManager;
        private readonly Scroller _scroller;
        private readonly PlayerStateManager _playerStateManager;
        private readonly AudioService _audioService;
        private readonly CollectiblesService _collectiblesService;
        private readonly PlayerFinder _playerFinder;

        public ActorFactory(ActorManager actorManager, GameSystem gameSystem, DataSerializer dataSerializer, 
            InputManager inputManager, SceneManager sceneManager, Scroller scroller, PlayerStateManager playerStateManager,
            AudioService audioService, CollectiblesService collectiblesService)
        {
            _actorManager = actorManager;
            _gameSystem = gameSystem;
            _dataSerializer = dataSerializer;
            _inputManager = inputManager;
            _sceneManager = sceneManager;
            _playerStateManager = playerStateManager;
            _audioService = audioService;
            _collectiblesService = collectiblesService;
            _scroller = scroller;
            _playerFinder = new PlayerFinder(actorManager);
        }

        public Actor CreateActor(
            ActorId actorId,
            ActorType actorType,
            Behavior behavior,
            PixelPoint position,
            Rectangle hitBox,
            ICollisionDetector collisionDetector,
            IDestroyedBehavior destroyedBehavior=null)
        {
            var tileset = GetTileset(actorId);
            var actor = new Actor(_gameSystem, actorType, tileset, behavior, destroyedBehavior, collisionDetector, hitBox, CreateAnimator(actorId, tileset));
            actor.Palette = _gameSystem.GetTilesetPalette(actor.Tileset);
            actor.WorldPosition = _gameSystem.RAM.DeclareGameRectangleWithSubpixels(position.X,position.Y, (byte)hitBox.Width, (byte)hitBox.Height);
            _actorManager.AddActor(actor);                
            return actor;
        }

        public Actor CreateActor(ActorId id, PixelPoint position, TransitionInfo transitionInfo)
        {
            switch(id)
            {
                case ActorId.Player: return CreatePlayer(position, transitionInfo);
                case ActorId.PlayerBullet: return CreatePlayerBullet();
                case ActorId.Skeleton: return CreateSkeleton(position);
                case ActorId.SkeletonBone: return CreateSkeletonBone();
                case ActorId.GhostBullet: return CreateGhostBullet();
                case ActorId.Coin: return CreateCoin();
                case ActorId.Apple: return CreateApple();
                case ActorId.Gem: return CreateGem();
                case ActorId.Meat: return CreateMeat();
                case ActorId.Key: return CreateKey();
                case ActorId.Skull: return CreateSkull();
                case ActorId.DeadSkeletonBone: return CreateDeadSkeletonBone();
                case ActorId.MovingPlatform: return CreateMovingPlatform(position);
                case ActorId.Bat: return CreateBat(position);
                case ActorId.Ghost: return CreateGhost(position);

                default: throw new Exception($"Unknown ActorId {id}");
            }
        }

        public ActorPool CreatePool(ActorId id, int count)
        {
            List<Actor> actors = new List<Actor>();
            while (count-- > 0)
                actors.Add(CreateActor(id, new PixelPoint(0, 0), new TransitionInfo()));

            return new ActorPool(actors);
        }

        public Actor CreateCoin() => CreateCollectible(ActorId.Coin, size: 8, behavior: new CoinBehavior(_audioService, _playerStateManager));
        public Actor CreateGem() => CreateCollectible(ActorId.Gem, size: 16, behavior: new GemBehavior(_audioService, _playerStateManager));
        public Actor CreateApple() => CreateCollectible(ActorId.Apple, size: 16, behavior: new AppleBehavior(_audioService, _playerStateManager));
        public Actor CreateMeat() => CreateCollectible(ActorId.Meat, size: 16, behavior: new MeatBehavior(_audioService, _playerStateManager));
        public Actor CreateKey() => CreateCollectible(ActorId.Key, size: 16, behavior: new KeyBehavior(_audioService, _playerStateManager));

        private Actor CreateCollectible(ActorId id, int size, CollectibleBehavior behavior)
        {
            return CreateActor(
               actorId: id,
               actorType: ActorType.Item,
               behavior: behavior,
               collisionDetector: new EmptyCollisionDetector(),
               hitBox: new Rectangle(0, 0, size, size),
               position: new PixelPoint(-100, -100));
        }


        public Actor CreatePlayer(PixelPoint position, TransitionInfo transitionInfo)
        {
            var playerProjectiles = CreatePool(ActorId.PlayerBullet, 2);

            var inputQueue = _gameSystem.RAM.DeclareEnum(InputQueue.None);

            return CreateActor(
                actorId: ActorId.Player,
                actorType: ActorType.Player | ActorType.Character,
                behavior: new PlayerBehavior(
                                _gameSystem,
                                inputQueue,
                                new PlatformerPlayerMotionBehavior(_gameSystem, _inputManager, _audioService, inputQueue),
                                new PlayerHurtBehavior(_gameSystem, _playerStateManager, _audioService),
                                new CameraBehavior(_scroller, _gameSystem),
                                new Gravity(),
                                _inputManager,
                                playerProjectiles,
                                new DestroyOnFall(_sceneManager),
                                _sceneManager,
                                _audioService,
                                transitionInfo),
                destroyedBehavior: new PlayerDeathBehavior(_gameSystem, _sceneManager),
                collisionDetector: new PlayerCollisionDetector(
                                            new BgCollisionDetector(_gameSystem, _scroller.GetTilemap(LayerIndex.FG), _actorManager),
                                            new CollectiblesCollectionDetector(_collectiblesService)),
                hitBox: new Rectangle(4, 0, 8, 14),
                position: position);
        }

        public Actor CreatePlayerBullet()
        {
            var bullet = CreateActor(
               actorId: ActorId.PlayerBullet,
               actorType: ActorType.Player | ActorType.Bullet,
               behavior: new ProjectileBehavior(_gameSystem, new PixelValue(2, 80), duration:20),               
               destroyedBehavior: new EmptyDestroyedBehavior(),
               collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Enemy | ActorType.Character),
               hitBox: new Rectangle(0, 0, 8, 8),
               position: new PixelPoint(-100, -100)
            ); 

            bullet.CurrentAnimation = AnimationKey.Moving;
            return bullet;
        }

        public Actor CreateSkeleton(PixelPoint position)
        {
            var bone = CreateSkeletonBone();

            var skull = CreateSkull();
            var bones = CreatePool(ActorId.DeadSkeletonBone, 3);

            return CreateActor(
                 actorId: ActorId.Skeleton,
                 actorType: ActorType.Enemy | ActorType.Character,
                 behavior: new SkeletonBehavior(_gameSystem, new Gravity(), new EnemyBaseBehavior(_gameSystem), bone),
                 destroyedBehavior: new SkeletonDestroyedBehavior(score: 100, _playerStateManager, skull,bones, _audioService),
                 collisionDetector: new BgCollisionDetector(_gameSystem, _scroller.GetTilemap(LayerIndex.FG), _actorManager),
                 hitBox: new Rectangle(4, 0, 8, 15),
                 position: position);
        }

        public Actor CreateSkeletonBone()
        {
            var enemyProjectile = CreateActor(
               actorId: ActorId.SkeletonBone,
               actorType: ActorType.Enemy | ActorType.Bullet,
               behavior: new ProjectileBehavior(_gameSystem, new PixelValue(1, 0), duration:100),
               destroyedBehavior: new EmptyDestroyedBehavior(),
               collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Player | ActorType.Character),
               hitBox: new Rectangle(0, 0, 8, 8),
               position: new PixelPoint(-100, -100));

            enemyProjectile.CurrentAnimation = AnimationKey.Moving;
            return enemyProjectile;
        }

        public Actor CreateGhostBullet()
        {
            var enemyProjectile = CreateActor(
               actorId: ActorId.GhostBullet,
               actorType: ActorType.Enemy | ActorType.Bullet,
               behavior: new GhostBulletBehavior(),
               destroyedBehavior: new EmptyDestroyedBehavior(),
               collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Player | ActorType.Character),
               hitBox: new Rectangle(0, 0, 8, 8),
               position: new PixelPoint(-100, -100));

            enemyProjectile.CurrentAnimation = AnimationKey.Moving;
            return enemyProjectile;
        }

        public Actor CreateBat(PixelPoint position)
        {
            return CreateActor(
                 actorId: ActorId.Bat,
                 actorType: ActorType.Enemy | ActorType.Character,
                 behavior: new BatBehavior(_gameSystem, new EnemyBaseBehavior(_gameSystem), _playerFinder),
                 destroyedBehavior: new EnemyDestroyedBehavior(score:25, _playerStateManager, _audioService),
                 collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Player | ActorType.Character),
                 hitBox: new Rectangle(4, 4, 8, 8),
                 position: position);
        }
        public Actor CreateGhost(PixelPoint position)
        {
            var bullet = CreateGhostBullet();

            return CreateActor(
                 actorId: ActorId.Ghost,
                 actorType: ActorType.Enemy | ActorType.Character,
                 behavior: new GhostBehavior(_gameSystem, new EnemyBaseBehavior(_gameSystem), _playerFinder, bullet),
                 destroyedBehavior: new GhostDestroyedBehavior(gameSystem: _gameSystem, score:200, bullet, _playerStateManager, _audioService),
                 collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Player | ActorType.Character),
                 hitBox: new Rectangle(4, 4, 8, 16),
                 position: position);
        }

        public Actor CreateSkull() => 
            CreateDebris(ActorId.Skull, TilesetContentKey.Skeleton);

        public Actor CreateDeadSkeletonBone() =>
            CreateDebris(ActorId.DeadSkeletonBone, TilesetContentKey.Skeleton);

        private Actor CreateDebris(ActorId id, TilesetContentKey tileSet)
        {
            var debris = CreateActor(
               actorId: id,
               actorType: ActorType.Decoration,
               behavior: new DebrisBehavior(new Gravity(), _scroller),
               destroyedBehavior: new EmptyDestroyedBehavior(),
               collisionDetector: new EmptyCollisionDetector(),
               hitBox: new Rectangle(0, 0, 8, 8),
               position: new PixelPoint(-100, -100));

            debris.CurrentAnimation = AnimationKey.Moving;
            return debris;
        }

        public Actor CreateMovingPlatform(PixelPoint position)
        {
            var bullet = CreateActor(
               actorId: ActorId.MovingPlatform,
               actorType: ActorType.Gizmo,
               behavior: new MovingPlatformBehavior(),
               destroyedBehavior: new EmptyDestroyedBehavior(),
               collisionDetector: new EmptyCollisionDetector(),
               hitBox: new Rectangle(0, 0, 16, 16),
               position: position
            );

            bullet.CurrentAnimation = AnimationKey.Moving;
            return bullet;
        }

        private SpriteAnimator CreateAnimator(ActorId actorId, TilesetContentKey tileset)
        {
            return new SpriteAnimator(_gameSystem,
                _dataSerializer.LoadSpriteFrames(tileset),
                _dataSerializer.LoadAnimations(actorId));
        }

        public static TilesetContentKey GetTileset(ActorId actorId)
        {
            switch (actorId)
            {
                case ActorId.Apple: return TilesetContentKey.Items;
                case ActorId.Coin: return TilesetContentKey.Items;
                case ActorId.DeadSkeletonBone: return TilesetContentKey.Skeleton;
                case ActorId.Gem: return TilesetContentKey.Items;
                case ActorId.Key: return TilesetContentKey.Items;
                case ActorId.Meat: return TilesetContentKey.Items;
                case ActorId.MovingPlatform: return TilesetContentKey.Gizmos;
                case ActorId.Player: return TilesetContentKey.Hero;
                case ActorId.PlayerBullet: return TilesetContentKey.Bullet;
                case ActorId.Skeleton: return TilesetContentKey.Skeleton;
                case ActorId.SkeletonBone: return TilesetContentKey.Skeleton;
                case ActorId.Skull: return TilesetContentKey.Skeleton;
                case ActorId.Bat: return TilesetContentKey.Bat;
                case ActorId.Ghost: return TilesetContentKey.Ghost;
                case ActorId.GhostBullet: return TilesetContentKey.Bullet2;

                default: throw new Exception("No tileset set for " + actorId);
            }
        }
    }
}
