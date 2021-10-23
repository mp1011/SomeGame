using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Models;
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
        private readonly HUDManager _hudManager;
        private readonly CollectiblesService _collectiblesService;
        private readonly Scroller _scroller;

        public SceneLoader(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice, 
            DataSerializer dataSerializer, ActorFactory actorFactory, AudioService audioService, 
            HUDManager hudManager, CollectiblesService collectiblesService, Scroller scroller, GameSystem gameSystem)
        {
            _gameSystem = gameSystem;
            _scroller = scroller;
            _actorFactory = actorFactory;
            _resourceLoader = resourceLoader;
            _graphicsDevice = graphicsDevice;
            _dataSerializer = dataSerializer;
            _audioService = audioService;
            _collectiblesService = collectiblesService;
            _hudManager = hudManager;
        }

        public void InitializeScene(SceneInfo sceneInfo)
        {
            _gameSystem.SetPalettes(
               _resourceLoader.LoadTexture(sceneInfo.PaletteKeys.P1).ToIndexedTilesetImage().Palette,
               _resourceLoader.LoadTexture(sceneInfo.PaletteKeys.P2).ToIndexedTilesetImage().Palette,
               _resourceLoader.LoadTexture(sceneInfo.PaletteKeys.P3).ToIndexedTilesetImage().Palette,
               _resourceLoader.LoadTexture(sceneInfo.PaletteKeys.P4).ToIndexedTilesetImage().Palette);

            var vramImages = sceneInfo.VramImages
                .Select(p => _resourceLoader.LoadTexture(p.TileSet)
                                           .ToIndexedTilesetImage(_gameSystem.GetPalette(p.Palette)))
                .ToArray();


            _gameSystem.SetVram(_graphicsDevice, vramImages);

            InitializeLayer(sceneInfo.BgMap, LayerIndex.BG);
            InitializeLayer(sceneInfo.FgMap, LayerIndex.FG);
            InitializeInterfaceLayer(sceneInfo.InterfaceType);
            PlaceCollectibles(sceneInfo);
            InitializeActors(sceneInfo);
            InitializeSounds(sceneInfo);

            _scroller.SetCameraBounds(sceneInfo.Bounds);
        }

        private void InitializeInterfaceLayer(InterfaceType interfaceType)
        {
            switch(interfaceType)
            {
                case InterfaceType.PlayerStatus:
                    _hudManager.Initialize();
                    break;
            }
        }

        private void InitializeLayer(LayerInfo layerInfo, LayerIndex layerIndex)
        {
            var tileMap = _dataSerializer.LoadTileMap(layerInfo.Key);
            var layer = _gameSystem.GetLayer(layerIndex);
            layer.TileMap.SetEach((x, y) => tileMap.GetTile(x, y));
            layer.ScrollFactor = layerInfo.ScrollFactor;            
        }

        private void InitializeActors(SceneInfo sceneInfo)
        {
            foreach (var actorStart in sceneInfo.Actors)
                _actorFactory.CreateActor(actorStart.ActorId, actorStart.Position);

            _collectiblesService.CreateCollectedItemActors(_actorFactory);
        }

        private void PlaceCollectibles(SceneInfo sceneInfo)
        {
            foreach(var collectiblePlacement in sceneInfo.CollectiblePlacements)
            {
                _collectiblesService.AddCollectible(
                    collectiblePlacement.Id,
                    collectiblePlacement.Position,
                    collectiblePlacement.Position2);
            }
        }

        private void InitializeSounds(SceneInfo sceneInfo)
        {
            foreach (var sfx in sceneInfo.Sounds)
                _audioService.LoadSound(sfx.Key, sfx.MaxOccurences);
        }
    }
}
