using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.GameInterface;
using SomeGame.Main.Models;
using SomeGame.Main.SceneControllers;
using SomeGame.Main.Scenes;
using System.Linq;

namespace SomeGame.Main.Services
{
    class SceneLoader
    {
        private readonly ResourceLoader _resourceLoader;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly DataSerializer _dataSerializer;
        private readonly ActorFactory _actorFactory;
        private readonly GameSystem _gameSystem;
        private readonly AudioService _audioService;
        private readonly CollectiblesService _collectiblesService;
        private readonly Scroller _scroller;
        private readonly PlayerStateManager _playerStateManager;
        private readonly ActorManager _actorManager;
        private readonly InputManager _inputManager;
        private readonly SceneManager _sceneManager;
        private readonly RasterBasedRenderService _renderService;

        public SceneLoader(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice, 
            DataSerializer dataSerializer, ActorFactory actorFactory, AudioService audioService, 
            CollectiblesService collectiblesService, Scroller scroller, 
            GameSystem gameSystem, PlayerStateManager playerStateManager, ActorManager actorManager, InputManager inputManager,
            SceneManager sceneManager, RasterBasedRenderService renderService)
        {
            _actorManager = actorManager;
            _playerStateManager = playerStateManager;
            _gameSystem = gameSystem;
            _scroller = scroller;
            _actorFactory = actorFactory;
            _resourceLoader = resourceLoader;
            _graphicsDevice = graphicsDevice;
            _dataSerializer = dataSerializer;
            _audioService = audioService;
            _collectiblesService = collectiblesService;
            _inputManager = inputManager;
            _sceneManager = sceneManager;
            _renderService = renderService;
        }

        public Scene LoadScene(TransitionInfo sceneTransition)
        {
            UnloadPreviousScene();

            var titleCardScene = sceneTransition.NextScene.GetSceneAfterTitleCard();
            if(titleCardScene != SceneContentKey.None)
            {
                var sceneInfo = _dataSerializer.Load(SceneContentKey.LevelTitleCard);
                sceneInfo = new SceneInfo(sceneInfo.BgMap, sceneInfo.FgMap, sceneInfo.InterfaceType, sceneInfo.Song,
                    sceneInfo.Bounds, sceneInfo.BackgroundColor, 
                    sceneInfo.VramImagesP1, sceneInfo.VramImagesP2, sceneInfo.VramImagesP3, sceneInfo.VramImagesP4,
                    sceneInfo.Sounds, sceneInfo.Actors, sceneInfo.CollectiblePlacements, new SceneTransitions(Right: titleCardScene));
                return new Scene(sceneTransition.NextScene, sceneInfo, _gameSystem);
            }

            return new Scene(sceneTransition.NextScene, _dataSerializer.Load(sceneTransition.NextScene), _gameSystem);            
        }

        public void InitializeScene(SceneInfo sceneInfo, TransitionInfo sceneTransition)
        {
            _gameSystem.RAM.MarkSceneDataAddress();
            _gameSystem.RAM.AddLabel("Begin Current Level");
          
            _gameSystem.BackgroundColorIndex.Set(sceneInfo.BackgroundColor);
            _gameSystem.SetVram(sceneInfo.VramImagesP1, sceneInfo.VramImagesP2, sceneInfo.VramImagesP3, sceneInfo.VramImagesP4);
          
            var bg = InitializeLayer(sceneInfo.BgMap, LayerIndex.BG, sceneInfo.VramImagesP1[0]);
            var fg = InitializeLayer(sceneInfo.FgMap, LayerIndex.FG, sceneInfo.VramImagesP2[0]);
            _scroller.SetTileMaps(bg, fg);

            InitializeActors(sceneInfo, sceneTransition);
            PlaceCollectibles(sceneInfo, fg);
            InitializeSounds(sceneInfo);

            _scroller.Initialize();

            if (sceneInfo.Song == MusicContentKey.None)
                _audioService.StopMusic();
            else
            {
                var song = _dataSerializer.LoadSong(sceneInfo.Song);
                _audioService.LoadSong(song);
                _audioService.StartMusic();
            }

            _gameSystem.RAM.AddLabel("End Current Level");
        }

        public IGameInterface CreateInterfaceLayer(Scene scene)
        { 
            switch(scene.SceneInfo.InterfaceType)
            {
                case InterfaceType.PlayerStatus:
                    return new PlayerStatusInterface(_playerStateManager, _gameSystem, _renderService, _scroller);
                case InterfaceType.TitleCard:
                    return new TitleCardInterface(_gameSystem, scene.Key);
                default:
                    return new EmptyGameInterface();
            }
        }

        public ISceneController CreateSceneController(SceneInfo sceneInfo)
        {
            switch(sceneInfo.InterfaceType)
            {
                case InterfaceType.TitleCard:
                    return new TitleCardSceneController(sceneInfo.Transitions.Right, _inputManager, _sceneManager);
                case InterfaceType.PlayerStatus:
                    return new PlayableSceneController(_gameSystem,_inputManager,_audioService);
                default:
                    return new EmptySceneController();
            }
        }

        private TileMap InitializeLayer(LayerInfo layerInfo, LayerIndex layerIndex, TilesetContentKey tilesetKey)
        {
            var tileMap = _dataSerializer.LoadTileMap(layerInfo.Key);
            var layer = _gameSystem.GetLayer(layerIndex);
            layer.Palette = layerInfo.Palette;
            layer.TileOffset = _gameSystem.GetTileOffset(tilesetKey);
            layer.TileMap.SetEach((x, y) => tileMap.GetTile(x, y));
            layer.ScrollFactor = layerInfo.ScrollFactor;

            return tileMap;
        }

        private void InitializeActors(SceneInfo sceneInfo, TransitionInfo transitionInfo)
        {
            _gameSystem.RAM.AddLabel("Begin Actors");
            foreach (var actorStart in sceneInfo.Actors)
                _actorFactory.CreateActor(actorStart.ActorId, actorStart.Position, actorStart.Palette, transitionInfo);

            if (sceneInfo.CollectiblePlacements.Any())
                _collectiblesService.CreateCollectedItemActors(_actorFactory);
            _gameSystem.RAM.AddLabel("End Actors");

        }

        private void PlaceCollectibles(SceneInfo sceneInfo, TileMap tileMap)
        {
            foreach(var collectiblePlacement in sceneInfo.CollectiblePlacements)
            {
                _collectiblesService.AddCollectible(
                    collectiblePlacement.Id,
                    collectiblePlacement.Position,
                    tileMap);
            }
        }

        private void InitializeSounds(SceneInfo sceneInfo)
        {
            foreach (var sfx in sceneInfo.Sounds)
                _audioService.LoadSound(sfx.Key, sfx.MaxOccurences);
        }

        private void UnloadPreviousScene()
        {
            _playerStateManager.ResetPlayerHealth();
            _audioService.UnloadSounds();
            _actorManager.UnloadAll();
            _collectiblesService.Reset();
            _gameSystem.RAM.ResetSceneData();
            _renderService.ClearInterrupts();
            _renderService.SetEffect(null);
        }
    }
}
