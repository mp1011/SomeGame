﻿using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.GameInterface;
using SomeGame.Main.Models;
using SomeGame.Main.SceneControllers;
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
        private readonly CollectiblesService _collectiblesService;
        private readonly Scroller _scroller;
        private readonly PlayerStateManager _playerStateManager;
        private readonly ActorManager _actorManager;
        private readonly InputManager _inputManager;
        private readonly SceneManager _sceneManager;

        public SceneLoader(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice, 
            DataSerializer dataSerializer, ActorFactory actorFactory, AudioService audioService, 
            CollectiblesService collectiblesService, Scroller scroller, 
            GameSystem gameSystem, PlayerStateManager playerStateManager, ActorManager actorManager, InputManager inputManager,
            SceneManager sceneManager)
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
        }

        public Scene LoadScene(TransitionInfo sceneTransition)
        {
            UnloadPreviousScene();

            //redo this
            switch(sceneTransition.NextScene)
            {
                case SceneContentKey.Level1TitleCard:
                    var sceneInfo = _dataSerializer.Load(SceneContentKey.LevelTitleCard);
                    sceneInfo = new SceneInfo(sceneInfo.BgMap, sceneInfo.FgMap, sceneInfo.InterfaceType, sceneInfo.Song,
                        sceneInfo.Bounds, sceneInfo.PaletteKeys, sceneInfo.BackgroundColor, sceneInfo.VramImages,
                        sceneInfo.Sounds, sceneInfo.Actors, sceneInfo.CollectiblePlacements, new SceneTransitions(Right: SceneContentKey.Test3));
                    return new Scene(sceneTransition.NextScene, sceneInfo, _gameSystem);
                default:
                    return new Scene(sceneTransition.NextScene, _dataSerializer.Load(sceneTransition.NextScene), _gameSystem);
            }
            
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

            _gameSystem.SetVram(_graphicsDevice, vramImages);
            _gameSystem.SetTilesetPalettes(sceneInfo.VramImages);

            var bg = InitializeLayer(sceneInfo.BgMap, LayerIndex.BG, vramImages[0].Key);
            var fg = InitializeLayer(sceneInfo.FgMap, LayerIndex.FG, vramImages[1].Key);
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
        }

        public IGameInterface CreateInterfaceLayer(SceneInfo sceneInfo)
        { 
            switch(sceneInfo.InterfaceType)
            {
                case InterfaceType.PlayerStatus:
                    return new PlayerStatusInterface(_playerStateManager, _gameSystem);
                case InterfaceType.TitleCard:
                    return new TitleCardInterface(_gameSystem, sceneInfo.Transitions.Right);
                default:
                    return new EmptyGameInterface();
            }
        }

        public ISceneController CreateSceneController(SceneInfo sceneInfo)
        {
            if (sceneInfo.InterfaceType == InterfaceType.TitleCard)
                return new TitleCardSceneController(sceneInfo.Transitions.Right, _inputManager, _sceneManager);
            else
                return new EmptySceneController();
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
            foreach (var actorStart in sceneInfo.Actors)            
                _actorFactory.CreateActor(actorStart.ActorId, actorStart.Position, transitionInfo);
            
            if(sceneInfo.CollectiblePlacements.Any())
                _collectiblesService.CreateCollectedItemActors(_actorFactory);
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
            _playerStateManager.ResetPlayerState();
            _audioService.UnloadSounds();
            _actorManager.UnloadAll();
            _collectiblesService.Reset();
        }
    }
}
