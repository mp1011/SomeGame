﻿using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Scenes;
using System.Collections.Generic;
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
        private readonly PlayerStateManager _playerStateManager;
        private readonly ActorManager _actorManager;

        public SceneLoader(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice, 
            DataSerializer dataSerializer, ActorFactory actorFactory, AudioService audioService, 
            HUDManager hudManager, CollectiblesService collectiblesService, Scroller scroller, 
            GameSystem gameSystem, PlayerStateManager playerStateManager, ActorManager actorManager)
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
            _hudManager = hudManager;
        }

        public Scene LoadScene(TransitionInfo sceneTransition)
        {
            UnloadPreviousScene();
            return new Scene(sceneTransition.NextScene, _dataSerializer.Load(sceneTransition.NextScene), _gameSystem);
        }

        public void InitializeScene(SceneInfo sceneInfo, TransitionInfo sceneTransition)
        {
            _gameSystem.SetPalettes(
               _resourceLoader.LoadTexture(sceneInfo.PaletteKeys.P1).ToIndexedTilesetImage().Palette,
               _resourceLoader.LoadTexture(sceneInfo.PaletteKeys.P2).ToIndexedTilesetImage().Palette,
               _resourceLoader.LoadTexture(sceneInfo.PaletteKeys.P3).ToIndexedTilesetImage().Palette,
               _resourceLoader.LoadTexture(sceneInfo.PaletteKeys.P4).ToIndexedTilesetImage().Palette);

            _gameSystem.BackgroundColorIndex = sceneInfo.BackgroundColor;

            var vramImages = sceneInfo.VramImages
                .Select(p => _resourceLoader.LoadTexture(p.TileSet)
                                           .ToIndexedTilesetImage(_gameSystem.GetPalette(p.Palette)))
                .ToArray();

            var tilesetToPalette = sceneInfo.VramImages.ToDictionary(k => k.TileSet, v => v.Palette);

            _gameSystem.SetVram(_graphicsDevice, vramImages);

            var bg = InitializeLayer(sceneInfo.BgMap, LayerIndex.BG, vramImages[0].Key);
            var fg = InitializeLayer(sceneInfo.FgMap, LayerIndex.FG, vramImages[1].Key);
            _scroller.SetTileMaps(bg, fg);

            InitializeInterfaceLayer(sceneInfo.InterfaceType);
            InitializeActors(sceneInfo, sceneTransition, tilesetToPalette);
            PlaceCollectibles(sceneInfo);
            InitializeSounds(sceneInfo);
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

        private void InitializeActors(SceneInfo sceneInfo, TransitionInfo transitionInfo, 
            Dictionary<TilesetContentKey, PaletteIndex> palettes)
        {
            foreach (var actorStart in sceneInfo.Actors)
            {
                var actor = _actorFactory.CreateActor(actorStart.ActorId, actorStart.Position, transitionInfo);
                actor.Palette = palettes[actor.Tileset];
            }

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

        private void UnloadPreviousScene()
        {
            _playerStateManager.ResetPlayerState();
            _audioService.UnloadSounds();
            _actorManager.UnloadAll();
            _collectiblesService.Reset();
        }
    }
}
