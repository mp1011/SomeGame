using Microsoft.Xna.Framework;
using SomeGame.Main.Behaviors;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
            PaletteIndex paletteIndex,
            Behavior behavior,
            PixelPoint position,
            Rectangle hitBox,
            ICollisionDetector collisionDetector,
            IDestroyedBehavior destroyedBehavior=null,
            bool enabledAtStart=true)
        {
            var actor = new Actor(actorType, tileset, paletteIndex, behavior, destroyedBehavior, collisionDetector, hitBox, CreateAnimator(actorId, tileset));
            actor.WorldPosition = new GameRectangleWithSubpixels(position.X,position.Y, hitBox.Width,hitBox.Height);

            if (enabledAtStart)            
                actor.Create();
            
            _actorManager.TryAddActor(_gameSystem, actor);                
            return actor;
        }

        public Actor CreateActor(ActorId id, PixelPoint position)
        {
            switch(id)
            {
                case ActorId.Player: return CreatePlayer(position);
                case ActorId.PlayerBullet: return CreatePlayerBullet();
                case ActorId.Skeleton: return CreateSkeleton(position);
                case ActorId.SkeletonBone: return CreateSkeletonBone();
                case ActorId.Coin: return CreateCoin();
                default: throw new Exception($"Unknown ActorId {id}");
            }
        }

        public ActorPool CreatePool(ActorId id, int count)
        {
            List<Actor> actors = new List<Actor>();
            while (count-- > 0)
                actors.Add(CreateActor(id, new PixelPoint(0, 0)));

            return new ActorPool(actors);
        }

        public Actor CreateCoin()
        {
            var coin = CreateActor(
               actorId: ActorId.Coin,
               actorType: ActorType.Item,
               tileset: TilesetContentKey.Items,
               paletteIndex: PaletteIndex.P1,
               behavior: new CollectibleBehavior(_audioService, _playerStateManager),
               collisionDetector: new EmptyCollisionDetector(),
               hitBox: new Rectangle(0, 0, 8, 8),
               position: new PixelPoint(0,0),
               enabledAtStart:false);

            return coin;
        }

        public Actor CreatePlayer(PixelPoint position)
        {
            var playerProjectiles = CreatePool(ActorId.PlayerBullet, 2);

            return CreateActor(
                actorId: ActorId.Player,
                actorType: ActorType.Player | ActorType.Character,
                tileset: TilesetContentKey.Hero,
                paletteIndex: PaletteIndex.P2,
                behavior: new PlayerBehavior(
                                new PlatformerPlayerMotionBehavior(_inputManager),
                                new PlayerHurtBehavior(_playerStateManager),
                                new CameraBehavior(_scroller, _gameSystem),
                                new Gravity(),
                                _inputManager,
                                playerProjectiles,
                                _playerStateManager,
                                _audioService),
                destroyedBehavior: new PlayerDeathBehavior(_sceneManager),
                collisionDetector: new BgCollisionDetector(_gameSystem, _collectiblesService),
                hitBox: new Rectangle(4, 0, 8, 14),
                position: position);
        }

        public Actor CreatePlayerBullet()
        {
            var bullet = CreateActor(
               actorId: ActorId.PlayerBullet,
               actorType: ActorType.Player | ActorType.Bullet,
               tileset: TilesetContentKey.Bullet,
               paletteIndex: PaletteIndex.P2,
               behavior: new ProjectileBehavior(new PixelValue(2, 0), duration:20),
               destroyedBehavior: new EmptyDestroyedBehavior(),
               collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Enemy | ActorType.Character),
               hitBox: new Rectangle(0, 0, 8, 8),
               position: new PixelPoint(0,0)
            ); 

            bullet.CurrentAnimation = AnimationKey.Moving;
            bullet.Enabled = false;
            return bullet;
        }

        public Actor CreateSkeleton(PixelPoint position)
        {
            var bone = CreateSkeletonBone();

            return CreateActor(
                 actorId: ActorId.Skeleton,
                 actorType: ActorType.Enemy | ActorType.Character,
                 tileset: TilesetContentKey.Skeleton,
                 paletteIndex: PaletteIndex.P2,
                 behavior: new SkeletonBehavior(new Gravity(), new EnemyBaseBehavior(), bone),
                 destroyedBehavior: new EnemyDestroyedBehavior(score: 100, _playerStateManager),
                 collisionDetector: new BgCollisionDetector(_gameSystem),
                 hitBox: new Rectangle(4, 0, 8, 15),
                 position: position);
        }

        public Actor CreateSkeletonBone()
        {
            var enemyProjectile = CreateActor(
               actorId: ActorId.SkeletonBone,
               actorType: ActorType.Enemy | ActorType.Bullet,
               tileset: TilesetContentKey.Skeleton,
               paletteIndex: PaletteIndex.P2,
               behavior: new ProjectileBehavior(new PixelValue(1, 0), duration:100),
               destroyedBehavior: new EmptyDestroyedBehavior(),
               collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Player | ActorType.Character),
               hitBox: new Rectangle(0, 0, 8, 8),
               position: new PixelPoint(0, 0));

            enemyProjectile.CurrentAnimation = AnimationKey.Moving;
            enemyProjectile.Enabled = false;
            return enemyProjectile;
        }

        private SpriteAnimator CreateAnimator(ActorId actorId, TilesetContentKey tileset)
        {
            return new SpriteAnimator(_gameSystem,
                _dataSerializer.LoadSpriteFrames(tileset),
                _dataSerializer.LoadAnimations(actorId));
        }
    }
}
