using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Modules
{
    class SceneDefinitionModule : IGameModule
    {
        private readonly DataSerializer _dataSerializer;
        private readonly GameSystem _gameSystem;


        public void Initialize()
        {
            UpdateScene(SceneContentKey.Level1, CreateLevel1);
            UpdateScene(SceneContentKey.LevelTitleCard, CreateLevelTitleCard);

            //_dataSerializer.Save(SceneContentKey.Test1, CreateTest1());
            //_dataSerializer.Save(SceneContentKey.Test2, CreateTest2());
            //_dataSerializer.Save(SceneContentKey.Test3, CreateTest3());
            //_dataSerializer.Save(SceneContentKey.LongMapTest, CreateLongMapTest());
        }



        public Rectangle Screen => new Rectangle(0, 0, 20, 20);

        public SceneDefinitionModule(GameStartup gameStartup)
        {
            _dataSerializer = new DataSerializer();
            _gameSystem = new GameSystem(gameStartup);
        }


        public Color BackgroundColor => Color.Black;

        private void UpdateScene(SceneContentKey key, Func<SceneInfo,SceneInfo> createScene)
        {
            var current = _dataSerializer.Load(key);
            var newScene = createScene(current);
            _dataSerializer.Save(key, newScene);
        }

        private SceneInfo CreateLevelTitleCard(SceneInfo previouslySaved)
        {
            return new SceneInfo(
               BgMap: new LayerInfo(LevelContentKey.None, PaletteIndex.P1, ScrollFactor: 100),
               FgMap: new LayerInfo(LevelContentKey.None, PaletteIndex.P1, ScrollFactor: 100),
               InterfaceType.TitleCard,
               MusicContentKey.None,
               Bounds: new Rectangle(0, 0, _gameSystem.Screen.Width, _gameSystem.Screen.Height),
               BackgroundColor: 5,
               VramImagesP1: new TilesetContentKey[]
               {
                    TilesetContentKey.Hud,
                    TilesetContentKey.Font, 
               },
               VramImagesP2: new TilesetContentKey[] 
               {
                    TilesetContentKey.Bullet
               },
               VramImagesP3: new TilesetContentKey[] { },
               VramImagesP4: new TilesetContentKey[] { },
               Sounds: new SoundInfo[]
               {
                    new SoundInfo(SoundContentKey.Collect,3),
                    new SoundInfo(SoundContentKey.Shoot, 2),
                    new SoundInfo(SoundContentKey.Destroy, 2),
                    new SoundInfo(SoundContentKey.Jump, 1),
                    new SoundInfo(SoundContentKey.Hurt, 1),
               },
               Actors: previouslySaved?.Actors ?? new ActorStart[] { },
               CollectiblePlacements: previouslySaved?.CollectiblePlacements ?? new CollectiblePlacement[] { },
               Transitions: new SceneTransitions());
        }

        private SceneInfo CreateLevel1(SceneInfo previousSaved)
        {
            return new SceneInfo(
               BgMap: new LayerInfo(LevelContentKey.TestLevel3BG, PaletteIndex.P1, ScrollFactor: 70),
               FgMap: new LayerInfo(LevelContentKey.TestLevel3, PaletteIndex.P2, ScrollFactor: 100),
               InterfaceType.PlayerStatus,
               MusicContentKey.Song1,
               Bounds: new Rectangle(0, 0, _gameSystem.LayerPixelWidth * 2, _gameSystem.Screen.Height),
               BackgroundColor:2,
               VramImagesP1: new TilesetContentKey[]
               {
                    TilesetContentKey.Mountains
               },
               VramImagesP2: new TilesetContentKey[]
               { 
                    TilesetContentKey.Tiles1,
                    TilesetContentKey.Items,
               },
               VramImagesP3: new TilesetContentKey[]
               {
                    TilesetContentKey.Hero,
                    TilesetContentKey.Skeleton,
                    TilesetContentKey.Bat,
                    TilesetContentKey.Ghost,
                    TilesetContentKey.Bullet2,
                    TilesetContentKey.Bullet,
                    TilesetContentKey.Gizmos
               },
               VramImagesP4: new TilesetContentKey[]
               {
                    TilesetContentKey.Hud,
                    TilesetContentKey.Font,
               },
               Sounds: new SoundInfo[]
               {
                    new SoundInfo(SoundContentKey.Collect,3),
                    new SoundInfo(SoundContentKey.Shoot, 2),
                    new SoundInfo(SoundContentKey.Destroy, 2),
                    new SoundInfo(SoundContentKey.Jump, 1),
                    new SoundInfo(SoundContentKey.Hurt, 1),
               },
               Actors: previousSaved?.Actors ?? new ActorStart[] { },
               CollectiblePlacements: previousSaved?.CollectiblePlacements ?? new CollectiblePlacement[] { },
               Transitions: new SceneTransitions(Right: SceneContentKey.Level1));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        }

        public void OnWindowSizeChanged(Viewport viewport)
        {
        }

        public bool Update(GameTime gameTime)
        {
            return false;
        }
    }
}
