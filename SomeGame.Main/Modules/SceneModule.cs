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
        private readonly SceneContentKey _initialScene;
        private readonly CollectiblesService _collectiblesService;
        private readonly ActorFactory _actorFactory;
        private readonly ActorManager _actorManager;
        private readonly Scroller _scroller;
        private readonly SceneLoader _sceneLoader;
        private readonly SceneManager _sceneManager;
        private readonly PlayerStateManager _playerStateManager;
        private readonly AudioService _audioService;
        private readonly HUDManager _hudManager;

        public SceneModule(SceneContentKey initialScene, ContentManager contentManager, GraphicsDevice graphicsDevice) 
            : base(contentManager, graphicsDevice)
        {
            _initialScene = initialScene;
            _playerStateManager = new PlayerStateManager();
            _hudManager = new HUDManager(_playerStateManager, GameSystem);
            _audioService = new AudioService(ResourceLoader);
            _collectiblesService = new CollectiblesService(GameSystem, GameSystem.GetLayer(LayerIndex.FG));         
            _sceneManager = new SceneManager();
            _scroller = new Scroller(GameSystem);
            _actorManager = new ActorManager(GameSystem, _scroller);
            _actorFactory = new ActorFactory(_actorManager, GameSystem, DataSerializer, InputManager, 
                _sceneManager, _scroller, _playerStateManager, _audioService, _collectiblesService);
            _sceneLoader = new SceneLoader(ResourceLoader, GraphicsDevice, DataSerializer, _actorFactory, 
                _audioService, _hudManager, _collectiblesService, _scroller, GameSystem, _playerStateManager, _actorManager);
        }

        public override void Initialize()
        {
            GameSystem.Input.Initialize(GameSystem.Screen);
            _sceneManager.QueueNextScene(_initialScene);
        }

        protected override void Update()
        {
            _scroller.Update();
            _sceneManager.Update(_sceneLoader);
           
            _actorManager.Update(_sceneManager.CurrentScene);
            _hudManager.Update();
        }
    }
}
