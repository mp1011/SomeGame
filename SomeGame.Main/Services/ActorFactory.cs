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
        }

        public Actor CreateActor(
            ActorId actorId,
            ActorType actorType,
            TilesetContentKey tileset,
            Behavior behavior,
            PixelPoint position,
            Rectangle hitBox,
            ICollisionDetector collisionDetector,
            IDestroyedBehavior destroyedBehavior=null)
        {
            var actor = new Actor(actorType, tileset, behavior, destroyedBehavior, collisionDetector, hitBox, CreateAnimator(actorId, tileset));
            actor.WorldPosition = new GameRectangleWithSubpixels(position.X,position.Y, hitBox.Width,hitBox.Height);
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
                case ActorId.Coin: return CreateCoin();
                case ActorId.Apple: return CreateApple();
                case ActorId.Gem: return CreateGem();
                case ActorId.Meat: return CreateMeat();
                case ActorId.Key: return CreateKey();
                case ActorId.Skull: return CreateSkull();
                case ActorId.DeadSkeletonBone: return CreateDeadSkeletonBone();
                case ActorId.MovingPlatform: return CreateMovingPlatform(position);
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
               tileset: TilesetContentKey.Items,
               behavior: behavior,
               collisionDetector: new EmptyCollisionDetector(),
               hitBox: new Rectangle(0, 0, size, size),
               position: new PixelPoint(-100, -100));
        }


        public Actor CreatePlayer(PixelPoint position, TransitionInfo transitionInfo)
        {
            var playerProjectiles = CreatePool(ActorId.PlayerBullet, 2);

            return CreateActor(
                actorId: ActorId.Player,
                actorType: ActorType.Player | ActorType.Character,
                tileset: TilesetContentKey.Hero,
                behavior: new PlayerBehavior(
                                new PlatformerPlayerMotionBehavior(_inputManager),
                                new PlayerHurtBehavior(_playerStateManager),
                                new CameraBehavior(_scroller, _gameSystem),
                                new Gravity(),
                                _inputManager,
                                playerProjectiles,
                                new DestroyOnFall(_sceneManager),
                                _sceneManager,
                                _audioService,
                                transitionInfo),
                destroyedBehavior: new PlayerDeathBehavior(_sceneManager),
                collisionDetector: new BgCollisionDetector(_gameSystem, _scroller, _actorManager, _collectiblesService),
                hitBox: new Rectangle(4, 0, 8, 14),
                position: position);
        }

        public Actor CreatePlayerBullet()
        {
            var bullet = CreateActor(
               actorId: ActorId.PlayerBullet,
               actorType: ActorType.Player | ActorType.Bullet,
               tileset: TilesetContentKey.Bullet,
               behavior: new ProjectileBehavior(new PixelValue(2, 150), duration:20),               
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
                 tileset: TilesetContentKey.Skeleton,
                 behavior: new SkeletonBehavior(new Gravity(), new EnemyBaseBehavior(), bone),
                 destroyedBehavior: new SkeletonDestroyedBehavior(score: 100, _playerStateManager, skull,bones),
                 collisionDetector: new BgCollisionDetector(_gameSystem, _scroller, _actorManager),
                 hitBox: new Rectangle(4, 0, 8, 15),
                 position: position);
        }

        public Actor CreateSkeletonBone()
        {
            var enemyProjectile = CreateActor(
               actorId: ActorId.SkeletonBone,
               actorType: ActorType.Enemy | ActorType.Bullet,
               tileset: TilesetContentKey.Skeleton,
               behavior: new ProjectileBehavior(new PixelValue(1, 0), duration:100),
               destroyedBehavior: new EmptyDestroyedBehavior(),
               collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Player | ActorType.Character),
               hitBox: new Rectangle(0, 0, 8, 8),
               position: new PixelPoint(-100, -100));

            enemyProjectile.CurrentAnimation = AnimationKey.Moving;
            return enemyProjectile;
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
               tileset: tileSet,
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
               tileset: TilesetContentKey.Gizmos,
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
    }
}
