﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Behaviors;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Modules
{
    class LevelModule : GameModuleBase
    {
        private readonly DataSerializer _dataSerializer;
        private HUDManager _hudManager;
        private PlayerState _playerState;

        public LevelModule()
        {
            _dataSerializer = new DataSerializer();
        }

        protected override PaletteKeys PaletteKeys { get; } = 
            new PaletteKeys(ImageContentKey.Palette1, 
                            ImageContentKey.Palette2, 
                            ImageContentKey.Palette3, 
                            ImageContentKey.Palette2);

        protected override void AfterInitialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
        {
            _hudManager = new HUDManager(GameSystem);
            _hudManager.DrawTiles();

            CollectiblesService.AddCoins(new Point(8,15), new Point(12,15));

            CollectiblesService.AddCoins(new Point(25, 12), new Point(30, 15));


        }

        protected override void LoadSounds(ResourceLoader resourceLoader)
        {
            AudioService.LoadSound(resourceLoader, SoundContentKey.Swish, 2);
            AudioService.LoadSound(resourceLoader, SoundContentKey.GetCoin, 2);
        }

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {
            if (index == LayerIndex.BG)
            {
                var loaded = _dataSerializer.LoadTileMap(LevelContentKey.TestLevelBG);               
                layer.TileMap.SetEach((x, y) => loaded.GetTile(x, y));
                layer.ScrollFactor = 70;
            }
            
            if (index == LayerIndex.FG)
            {
                var loaded = _dataSerializer.LoadTileMap(LevelContentKey.TestLevel);
                layer.TileMap.SetEach((x, y) => loaded.GetTile(x, y));
            }
        }

        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader)
        {
            return new IndexedTilesetImage[]
            {
                resourceLoader.LoadTexture(TilesetContentKey.Tiles)
                              .ToIndexedTilesetImage(GameSystem.GetPalette(PaletteIndex.P1)),

                resourceLoader.LoadTexture(TilesetContentKey.Hero)
                              .ToIndexedTilesetImage(GameSystem.GetPalette(PaletteIndex.P2)),

                resourceLoader.LoadTexture(TilesetContentKey.Skeleton)
                              .ToIndexedTilesetImage(GameSystem.GetPalette(PaletteIndex.P2)),

                resourceLoader.LoadTexture(TilesetContentKey.Bullet)
                              .ToIndexedTilesetImage(GameSystem.GetPalette(PaletteIndex.P2)),

                resourceLoader.LoadTexture(TilesetContentKey.Hud)
                              .ToIndexedTilesetImage(GameSystem.GetPalette(PaletteIndex.P2)),

                resourceLoader.LoadTexture(TilesetContentKey.Font)
                              .ToIndexedTilesetImage(GameSystem.GetPalette(PaletteIndex.P2)),

                resourceLoader.LoadTexture(TilesetContentKey.Items)
                              .ToIndexedTilesetImage(GameSystem.GetPalette(PaletteIndex.P1)),
            };
        }

        protected override void InitializeActors()
        {
            _playerState = new PlayerState { Health = new BoundedInt(12, 12), Lives = 3, Score = 0 };
            var actorFactory = new ActorFactory(ActorManager, GameSystem, _dataSerializer, InputManager,
                SceneManager, _playerState,AudioService, CollectiblesService);

            actorFactory.CreatePlayer(new PixelPoint(50, 100));
            actorFactory.CreateSkeleton(new PixelPoint(150, 120));
            actorFactory.CreateSkeleton(new PixelPoint(300, 100));
            CollectiblesService.CreateCollectedItemActors(actorFactory);
        }

        protected override Scene InitializeScene()
        {
            return new Scene(new Rectangle(0, 0, GameSystem.LayerPixelWidth, GameSystem.Screen.Height), GameSystem);
        }

        protected override void Update()
        {
            _hudManager.UpdateHUD(_playerState);
        }
    }
}
