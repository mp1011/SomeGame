using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.GameInterface;
using SomeGame.Main.Models;
using SomeGame.Main.SceneControllers;
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
        private IGameInterface _gameInterface;
        private ISceneController _sceneController;

        public SceneModule(SceneContentKey initialScene, GameStartup startup) 
            : base(startup)
        {
            GameSystem.RAM.AddLabel("Begin Scene");
            _initialScene = initialScene;
            _playerStateManager = new PlayerStateManager(GameSystem);
            _audioService = new AudioService(ResourceLoader);
            _scroller = new Scroller(GameSystem);
            _collectiblesService = new CollectiblesService(GameSystem,_scroller);         
            _sceneManager = new SceneManager();            
            _actorManager = new ActorManager(GameSystem, _scroller);
            _actorFactory = new ActorFactory(_actorManager, GameSystem, DataSerializer, InputManager, 
                _sceneManager, _scroller, _playerStateManager, _audioService, _collectiblesService);
            _sceneLoader = new SceneLoader(ResourceLoader, GraphicsDevice, DataSerializer, _actorFactory, 
                _audioService, _collectiblesService, _scroller, GameSystem, _playerStateManager, _actorManager, 
                InputManager, _sceneManager, RenderService as RasterBasedRenderService);
            GameSystem.RAM.AddLabel("End Scene");
        }

        protected override void OnInitialize()
        {
            GameSystem.Input.Initialize(GameSystem.Screen);
            _sceneManager.QueueNextScene(_initialScene);
        }

        protected override bool Update()
        {
            var sceneUpdate = _sceneManager.Update(_sceneLoader);
            if(sceneUpdate.NewScene)
            {
                _gameInterface = sceneUpdate.GameInterface;
                _sceneController = sceneUpdate.Controller;
            }

            _scroller.Update();        
            _actorManager.Update(_sceneManager.CurrentScene);
            _gameInterface.Update();
            _audioService.UpdateMusic();
            _sceneController.Update();

            return true;
        }
    }
}
