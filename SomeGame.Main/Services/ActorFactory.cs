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
        private readonly PlayerState _playerState;
        private readonly AudioService _audioService;
        public ActorFactory(ActorManager actorManager, GameSystem gameSystem, DataSerializer dataSerializer, 
            InputManager inputManager, SceneManager sceneManager, PlayerState playerState,
            AudioService audioService)
        {
            _actorManager = actorManager;
            _gameSystem = gameSystem;
            _dataSerializer = dataSerializer;
            _inputManager = inputManager;
            _sceneManager = sceneManager;
            _playerState = playerState;
            _audioService = audioService;
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
            IDestroyedBehavior destroyedBehavior=null)
        {
            var actor = new Actor(actorType, tileset, paletteIndex, behavior, destroyedBehavior, collisionDetector, hitBox, CreateAnimator(actorId, tileset));
            actor.WorldPosition = new GameRectangleWithSubpixels(position.X,position.Y, hitBox.Width,hitBox.Height);

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
                                new PlayerHurtBehavior(),
                                new CameraBehavior(_sceneManager, _gameSystem),
                                new Gravity(),
                                _inputManager,
                                playerProjectiles,
                                _playerState,
                                _audioService),
                collisionDetector: new BgCollisionDetector(_gameSystem),
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
               behavior: new ProjectileBehavior(Direction.Right, new PixelValue(2, 0), duration:20),
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
                 destroyedBehavior: new EnemyDestroyedBehavior(score: 100, _playerState),
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
               behavior: new ProjectileBehavior(Direction.Left, new PixelValue(1, 0), duration:100),
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
