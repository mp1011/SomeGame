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
        private readonly RasterBasedRenderService _renderService;

        public ActorFactory(ActorManager actorManager, GameSystem gameSystem, DataSerializer dataSerializer, 
            InputManager inputManager, SceneManager sceneManager, Scroller scroller, PlayerStateManager playerStateManager,
            AudioService audioService, CollectiblesService collectiblesService, RasterBasedRenderService renderService)
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
            _renderService = renderService;
        }

        public Actor CreateActor(
            ActorId actorId,
            ActorType actorType,
            Behavior behavior,
            PixelPoint position,
            Rectangle hitBox,
            PaletteIndex palette,
            ICollisionDetector collisionDetector,
            IDestroyedBehavior destroyedBehavior=null)
        {
            var tileset = GetTileset(actorId);
            var actor = new Actor(_gameSystem, actorType, tileset, behavior, destroyedBehavior, collisionDetector, hitBox, CreateAnimator(actorId, tileset));
            actor.Palette = palette;
            actor.WorldPosition = _gameSystem.RAM.DeclareGameRectangleWithSubpixels(position.X,position.Y + hitBox.Top, (byte)hitBox.Width, (byte)hitBox.Height);
            _actorManager.AddActor(actor);                
            return actor;
        }

        public Actor CreateActor(ActorId id, PixelPoint position, PaletteIndex palette, TransitionInfo transitionInfo)
        {
            switch(id)
            {
                case ActorId.Player: return CreatePlayer(position, transitionInfo, palette);
                case ActorId.PlayerBullet: return CreatePlayerBullet(palette);
                case ActorId.Skeleton: return CreateSkeleton(position, palette);
                case ActorId.SkeletonBone: return CreateSkeletonBone(palette);
                case ActorId.GhostBullet: return CreateGhostBullet(palette);
                case ActorId.Coin: return CreateCoin();
                case ActorId.Apple: return CreateApple();
                case ActorId.Gem: return CreateGem();
                case ActorId.Meat: return CreateMeat();
                case ActorId.Key: return CreateKey();
                case ActorId.Skull: return CreateSkull(palette);
                case ActorId.DeadSkeletonBone: return CreateDeadSkeletonBone(palette);
                case ActorId.MovingPlatform: return CreateMovingPlatform(position, palette);
                case ActorId.Spring: return CreateSpring(position, palette);
                case ActorId.TouchVanishingBlock: return CreateTouchVanishingBlock(position, palette);
                case ActorId.TimedVanishingBlock: return CreateTimedVanishingBlock(position, palette);
                case ActorId.SpikeBlock: return CreateSpikeBlock(position, palette);
                case ActorId.Bat: return CreateBat(position, palette);
                case ActorId.Ghost: return CreateGhost(position, palette);

                default: throw new Exception($"Unknown ActorId {id}");
            }
        }

        public ActorPool CreatePool(ActorId id, int count, PaletteIndex palette)
        {
            List<Actor> actors = new List<Actor>();
            while (count-- > 0)
                actors.Add(CreateActor(id, new PixelPoint(0, 0), palette, new TransitionInfo()));

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
               palette: PaletteIndex.P2, 
               behavior: behavior,
               collisionDetector: new EmptyCollisionDetector(),
               hitBox: new Rectangle(0, 0, size, size),
               position: new PixelPoint(-100, -100));
        }


        public Actor CreatePlayer(PixelPoint position, TransitionInfo transitionInfo, PaletteIndex palette)
        {
            var playerProjectiles = CreatePool(ActorId.PlayerBullet, 2, palette);

            var inputQueue = _gameSystem.RAM.DeclareEnum(InputQueue.None);

            return CreateActor(
                actorId: ActorId.Player,
                actorType: ActorType.Player | ActorType.Character,        
                palette: palette,
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
                destroyedBehavior: new PlayerDeathBehavior(_gameSystem, _sceneManager, _playerStateManager,_audioService,_renderService),
                collisionDetector: new PlayerCollisionDetector(
                                            new BgCollisionDetector(_gameSystem, _scroller.GetTilemap(LayerIndex.FG), _actorManager),
                                            new CollectiblesCollectionDetector(_collectiblesService)),
                hitBox: new Rectangle(4, 0, 8, 14),
                position: position);
        }

        public Actor CreatePlayerBullet(PaletteIndex palette)
        {
            var bullet = CreateActor(
               actorId: ActorId.PlayerBullet,
               palette: palette,
               actorType: ActorType.Player | ActorType.Bullet,
               behavior: new ProjectileBehavior(_gameSystem, new PixelValue(2, 80), duration:20),               
               destroyedBehavior: new EmptyDestroyedBehavior(),
               collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Enemy | ActorType.Character),
               hitBox: new Rectangle(0, 0, 8, 8),
               position: new PixelPoint(-100, -100)
            ); 

            bullet.CurrentAnimation = AnimationKey.Moving;
            bullet.State = ActorState.Destroyed; 
            return bullet;
        }

        public Actor CreateSkeleton(PixelPoint position, PaletteIndex palette)
        {
            var bone = CreateSkeletonBone(palette);

            var skull = CreateSkull(palette);
            var bones = CreatePool(ActorId.DeadSkeletonBone, 3, palette);

            return CreateActor(
                 actorId: ActorId.Skeleton,
                 palette: palette,
                 actorType: ActorType.Enemy | ActorType.Character,
                 behavior: new SkeletonBehavior(_gameSystem, new Gravity(), new EnemyBaseBehavior(_gameSystem), bone),
                 destroyedBehavior: new SkeletonDestroyedBehavior(score: 100, _playerStateManager, skull,bones, _audioService),
                 collisionDetector: new BgCollisionDetector(_gameSystem, _scroller.GetTilemap(LayerIndex.FG), _actorManager),
                 hitBox: new Rectangle(4, 0, 8, 15),
                 position: position);
        }

        public Actor CreateSkeletonBone(PaletteIndex palette)
        {
            var enemyProjectile = CreateActor(
               actorId: ActorId.SkeletonBone,
               actorType: ActorType.Enemy | ActorType.Bullet,
               palette:palette,
               behavior: new ProjectileBehavior(_gameSystem, new PixelValue(1, 0), duration:100),
               destroyedBehavior: new EmptyDestroyedBehavior(),
               collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Player | ActorType.Character),
               hitBox: new Rectangle(0, 0, 8, 8),
               position: new PixelPoint(-100, -100));

            enemyProjectile.CurrentAnimation = AnimationKey.Moving;
            enemyProjectile.State = ActorState.Destroyed;
            return enemyProjectile;
        }

        public Actor CreateGhostBullet(PaletteIndex palette)
        {
            var enemyProjectile = CreateActor(
               actorId: ActorId.GhostBullet,
               actorType: ActorType.Enemy | ActorType.Bullet,
               behavior: new GhostBulletBehavior(),
               palette: palette,
               destroyedBehavior: new EmptyDestroyedBehavior(),
               collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Player | ActorType.Character),
               hitBox: new Rectangle(0, 0, 8, 8),
               position: new PixelPoint(-100, -100));

            enemyProjectile.CurrentAnimation = AnimationKey.Moving;
            enemyProjectile.State = ActorState.Destroyed;
            return enemyProjectile;
        }

        public Actor CreateBat(PixelPoint position, PaletteIndex palette)
        {
            return CreateActor(
                 actorId: ActorId.Bat,
                 actorType: ActorType.Enemy | ActorType.Character,
                 palette: palette,
                 behavior: new BatBehavior(_gameSystem, new EnemyBaseBehavior(_gameSystem), _playerFinder),
                 destroyedBehavior: new EnemyDestroyedBehavior(score:25, _playerStateManager, _audioService),
                 collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Player | ActorType.Character),
                 hitBox: new Rectangle(4, 4, 8, 8),
                 position: position);
        }
        public Actor CreateGhost(PixelPoint position, PaletteIndex palette)
        {
            var bullet = CreateGhostBullet(palette);

            var ghost = CreateActor(
                 actorId: ActorId.Ghost,
                 actorType: ActorType.Enemy | ActorType.Character,
                 palette: palette,
                 behavior: new GhostBehavior(_gameSystem, new EnemyBaseBehavior(_gameSystem), _playerFinder, bullet),
                 destroyedBehavior: new GhostDestroyedBehavior(gameSystem: _gameSystem, score:200, bullet, _playerStateManager, _audioService),
                 collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Player | ActorType.Character),
                 hitBox: new Rectangle(4, 4, 8, 16),
                 position: position);

            ghost.CurrentAnimation = AnimationKey.Moving;

            return ghost;
        }

        public Actor CreateSkull(PaletteIndex palette) => 
            CreateDebris(ActorId.Skull, TilesetContentKey.Skeleton,palette);

        public Actor CreateDeadSkeletonBone(PaletteIndex palette) =>
            CreateDebris(ActorId.DeadSkeletonBone, TilesetContentKey.Skeleton,palette);

        private Actor CreateDebris(ActorId id, TilesetContentKey tileSet, PaletteIndex palette)
        {
            var debris = CreateActor(
               actorId: id,
               actorType: ActorType.Decoration,
               palette: palette,
               behavior: new DebrisBehavior(new Gravity(), _scroller),
               destroyedBehavior: new EmptyDestroyedBehavior(),
               collisionDetector: new EmptyCollisionDetector(),
               hitBox: new Rectangle(0, 0, 8, 8),
               position: new PixelPoint(-100, -100));

            debris.CurrentAnimation = AnimationKey.Moving;
            debris.State = ActorState.Destroyed;
            return debris;
        }

        public Actor CreateMovingPlatform(PixelPoint position, PaletteIndex palette)
        {
            var platform = CreateActor(
               actorId: ActorId.MovingPlatform,
               actorType: ActorType.Gizmo,
               behavior: new MovingPlatformBehavior(),
               destroyedBehavior: new EmptyDestroyedBehavior(),
               collisionDetector: new EmptyCollisionDetector(),
               hitBox: new Rectangle(0, 0, 16, 16),
               palette: palette,
               position: position
            );

            platform.CurrentAnimation = AnimationKey.Moving;
            return platform;
        }

        public Actor CreateSpring(PixelPoint position, PaletteIndex palette)
        {
            var spring = CreateActor(
               actorId: ActorId.Spring,
               actorType: ActorType.Gizmo,
               behavior: new SpringBehavior(_gameSystem,_audioService,_scroller),
               destroyedBehavior: new EmptyDestroyedBehavior(),
               collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Character),
               hitBox: new Rectangle(0, 8, 16, 8),
               palette: palette,
               position: position
            );

            return spring;
        }

        public Actor CreateTouchVanishingBlock(PixelPoint position, PaletteIndex palette)
        {
            var block = CreateActor(
              actorId: ActorId.TouchVanishingBlock,
              actorType: ActorType.Gizmo,
              behavior: new TouchVanishingBlockBehavior(_gameSystem, _scroller, _audioService),
              destroyedBehavior: new EmptyDestroyedBehavior(),
              collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Character),
              hitBox: new Rectangle(0, 0, 16, 16),
              palette: palette,
              position: position
           );

            return block;
        }

        public Actor CreateTimedVanishingBlock(PixelPoint position, PaletteIndex palette)
        {
            var block = CreateActor(
              actorId: ActorId.TimedVanishingBlock,
              actorType: ActorType.Gizmo,
              behavior: new TimedVanishingBlockBehavior(_gameSystem, _scroller, _audioService),
              destroyedBehavior: new EmptyDestroyedBehavior(),
              collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Character),
              hitBox: new Rectangle(0, 0, 16, 16),
              palette: palette,
              position: position
           );

            return block;
        }

        public Actor CreateSpikeBlock(PixelPoint position, PaletteIndex palette)
        {
            var spikes = new Actor[]
            {
                CreateVSpike(position,palette),
                CreateHSpike(position,palette),
                CreateVSpike(position,palette),
                CreateHSpike(position,palette),
            };

            var block = CreateActor(
              actorId: ActorId.SpikeBlock,
              actorType: ActorType.Gizmo,
              behavior: new SpikeBlockBehavior(_gameSystem, _scroller, _audioService, spikes),
              destroyedBehavior: new EmptyDestroyedBehavior(),
              collisionDetector: new ActorCollisionDetector(_actorManager, ActorType.Character),
              hitBox: new Rectangle(0, 0, 16, 16),
              palette: palette,
              position: position
           );

            return block;
        }

        public Actor CreateVSpike(PixelPoint position, PaletteIndex palette)
        {
            return CreateActor(
              actorId: ActorId.SpikeV,
              actorType: ActorType.Gizmo,
              behavior: new EmptyBehavior(),
              destroyedBehavior: new EmptyDestroyedBehavior(),
              collisionDetector: new EmptyCollisionDetector(),
              hitBox: new Rectangle(0, 8, 16, 8),
              palette: palette,
              position: position
           );
        }

        public Actor CreateHSpike(PixelPoint position, PaletteIndex palette)
        {
            return CreateActor(
              actorId: ActorId.SpikeH,
              actorType: ActorType.Gizmo,
              behavior: new EmptyBehavior(),
              destroyedBehavior: new EmptyDestroyedBehavior(),
              collisionDetector: new EmptyCollisionDetector(),
              hitBox: new Rectangle(0, 8, 16, 8),
              palette: palette,
              position: position
           );
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
                case ActorId.Spring: return TilesetContentKey.Gizmos;
                case ActorId.TouchVanishingBlock: return TilesetContentKey.Gizmos;
                case ActorId.TimedVanishingBlock: return TilesetContentKey.Gizmos;
                case ActorId.SpikeBlock: return TilesetContentKey.Gizmos;
                case ActorId.SpikeV: return TilesetContentKey.Gizmos;
                case ActorId.SpikeH: return TilesetContentKey.Gizmos;
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
