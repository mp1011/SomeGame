using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Scenes;
using SomeGame.Main.Services;

namespace SomeGame.Main.Modules
{
    class SceneModule : GameModuleBase
    {
        private readonly CollectiblesService _collectiblesService;
        private readonly ActorFactory _actorFactory;
        private readonly ActorManager _actorManager;
        private readonly Scroller _scroller;
        private readonly SceneLoader _sceneLoader;
        private readonly SceneManager _sceneManager;
        private readonly PlayerStateManager _playerStateManager;
        private readonly AudioService _audioService;
        private readonly HUDManager _hudManager;

        public SceneModule(ContentManager contentManager, GraphicsDevice graphicsDevice) 
            : base(contentManager, graphicsDevice)
        {
            _playerStateManager = new PlayerStateManager();
            _hudManager = new HUDManager(_playerStateManager, GameSystem);
            _audioService = new AudioService(ResourceLoader);
            _collectiblesService = new CollectiblesService(GameSystem, GameSystem.GetLayer(LayerIndex.FG));         
            _sceneManager = new SceneManager(GameSystem);
            _scroller = new Scroller(GameSystem);
            _actorManager = new ActorManager(GameSystem, _scroller);
            _actorFactory = new ActorFactory(_actorManager, GameSystem, DataSerializer, InputManager, 
                _sceneManager, _scroller, _playerStateManager, _audioService, _collectiblesService);
            _sceneLoader = new SceneLoader(ResourceLoader, GraphicsDevice, DataSerializer, _actorFactory, 
                _audioService, _hudManager, _collectiblesService, _scroller, GameSystem);
        }

        public override void Initialize()
        {
            GameSystem.Input.Initialize(GameSystem.Screen);

            var scene = GetInitialScene();
            _sceneManager.QueueNextScene(scene);
        }

        private Scene GetInitialScene()
        {
            return new Scene(new SceneInfo(
                BgMap: new LayerInfo(LevelContentKey.TestLevelBG, ScrollFactor: 70),
                FgMap: new LayerInfo(LevelContentKey.TestLevel, ScrollFactor:100),
                InterfaceType.PlayerStatus,
                Bounds: new Rectangle(0, 0, GameSystem.LayerPixelWidth, GameSystem.Screen.Height),
                PaletteKeys: new PaletteKeys(ImageContentKey.Palette1, ImageContentKey.Palette2, ImageContentKey.Palette3, ImageContentKey.Palette3),
                VramImages: new TilesetWithPalette[]
                {
                    new TilesetWithPalette(TilesetContentKey.Tiles, PaletteIndex.P1),
                    new TilesetWithPalette(TilesetContentKey.Hero, PaletteIndex.P2),
                    new TilesetWithPalette(TilesetContentKey.Skeleton, PaletteIndex.P2),
                    new TilesetWithPalette(TilesetContentKey.Bullet, PaletteIndex.P2),
                    new TilesetWithPalette(TilesetContentKey.Hud, PaletteIndex.P2),
                    new TilesetWithPalette(TilesetContentKey.Font, PaletteIndex.P2),
                    new TilesetWithPalette(TilesetContentKey.Items, PaletteIndex.P1)
                },
                Sounds: new SoundInfo[]
                {
                     new SoundInfo(SoundContentKey.GetCoin,3),
                     new SoundInfo(SoundContentKey.Swish, 2)
                },
                Actors: new ActorStart[]
                {
                    new ActorStart(ActorId.Player, new PixelPoint(50,100)),
                    new ActorStart(ActorId.Skeleton, new PixelPoint(150,120)),
                    new ActorStart(ActorId.Skeleton, new PixelPoint(300,100))
                }, 
                CollectiblePlacements: new CollectiblePlacement[]
                {
                    new CollectiblePlacement(CollectibleId.Coin, new Point(8,15), new Point(12,15)),
                    new CollectiblePlacement(CollectibleId.Coin, new Point(25, 12), new Point(30, 15))
                }),                
                GameSystem);
        }

        protected override void Update()
        {
            _scroller.Update();
            var sceneResult = _sceneManager.Update();
            if (sceneResult.LoadScene != null)
            {
                UnloadPreviousScene();
                _sceneLoader.InitializeScene(sceneResult.LoadScene.SceneInfo);
            }

            _actorManager.Update(_sceneManager.CurrentScene);
            _hudManager.Update();
        }

        private void UnloadPreviousScene()
        {
            _playerStateManager.ResetPlayerState();
            _audioService.UnloadSounds();
            _actorManager.UnloadAll();
        }
    }
}
