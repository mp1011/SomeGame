using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Scenes;
using SomeGame.Main.Services;

namespace SomeGame.Main.Modules
{
    class SceneDefinitionModule : IGameModule
    {
        private readonly DataSerializer _dataSerializer;
        private readonly GameSystem _gameSystem;

        public Rectangle Screen => new Rectangle(0, 0, 20, 20);

        public SceneDefinitionModule()
        {
            _dataSerializer = new DataSerializer();
            _gameSystem = new GameSystem();
        }


        public Color BackgroundColor => Color.Black;

        private SceneInfo CreateTest3()
        {
            return new SceneInfo(
               BgMap: new LayerInfo(LevelContentKey.TestLevel3BG, PaletteIndex.P1, ScrollFactor: 70),
               FgMap: new LayerInfo(LevelContentKey.TestLevel3, PaletteIndex.P2, ScrollFactor: 100),
               InterfaceType.PlayerStatus,
               Bounds: new Rectangle(0, 0, _gameSystem.LayerPixelWidth * 2, _gameSystem.Screen.Height),
               PaletteKeys: new PaletteKeys(ImageContentKey.Palette5, ImageContentKey.Palette6, ImageContentKey.Palette2, ImageContentKey.Palette3),
               BackgroundColor:14,
               VramImages: new TilesetWithPalette[]
               {
                    new TilesetWithPalette(TilesetContentKey.Mountains, PaletteIndex.P1),
                    new TilesetWithPalette(TilesetContentKey.Tiles1, PaletteIndex.P2),
                    new TilesetWithPalette(TilesetContentKey.Items, PaletteIndex.P2),
                    new TilesetWithPalette(TilesetContentKey.Hero, PaletteIndex.P3),
                    new TilesetWithPalette(TilesetContentKey.Skeleton, PaletteIndex.P3),
                    new TilesetWithPalette(TilesetContentKey.Bat, PaletteIndex.P3),
                    new TilesetWithPalette(TilesetContentKey.Bullet, PaletteIndex.P3),
                    new TilesetWithPalette(TilesetContentKey.Hud, PaletteIndex.P3),
                    new TilesetWithPalette(TilesetContentKey.Font, PaletteIndex.P3),
                    new TilesetWithPalette(TilesetContentKey.Gizmos, PaletteIndex.P3)
               },
               Sounds: new SoundInfo[]
               {
                    new SoundInfo(SoundContentKey.GetCoin,3),
                    new SoundInfo(SoundContentKey.Swish, 2)
               },
               Actors: new ActorStart[]
               {
                    new ActorStart(ActorId.Player, new PixelPoint(50,100))
               },
               CollectiblePlacements: new CollectiblePlacement[]
               {                   
               },
               Transitions: new SceneTransitions(Right: SceneContentKey.Test2));
        }

        private SceneInfo CreateTest1()
        {
            return new SceneInfo(
                BgMap: new LayerInfo(LevelContentKey.TestLevelBG, PaletteIndex.P1, ScrollFactor: 70),
                FgMap: new LayerInfo(LevelContentKey.TestLevel, PaletteIndex.P1, ScrollFactor: 100),
                InterfaceType.PlayerStatus,
                Bounds: new Rectangle(0, 0, _gameSystem.LayerPixelWidth*2, _gameSystem.Screen.Height),
                PaletteKeys: new PaletteKeys(ImageContentKey.Palette1, ImageContentKey.Palette2, ImageContentKey.Palette3, ImageContentKey.Palette3),
                BackgroundColor: 0,
                VramImages: new TilesetWithPalette[]
                {
                    new TilesetWithPalette(TilesetContentKey.Tiles, PaletteIndex.P1),
                    new TilesetWithPalette(TilesetContentKey.Hero, PaletteIndex.P2),
                    new TilesetWithPalette(TilesetContentKey.Skeleton, PaletteIndex.P2),
                    new TilesetWithPalette(TilesetContentKey.Bullet, PaletteIndex.P2),
                    new TilesetWithPalette(TilesetContentKey.Hud, PaletteIndex.P2),
                    new TilesetWithPalette(TilesetContentKey.Font, PaletteIndex.P2),
                    new TilesetWithPalette(TilesetContentKey.Items, PaletteIndex.P1),
                    new TilesetWithPalette(TilesetContentKey.Gizmos, PaletteIndex.P2)
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
                    new ActorStart(ActorId.Skeleton, new PixelPoint(500,100)),
                    new ActorStart(ActorId.MovingPlatform, new PixelPoint(200,100))
                },
                CollectiblePlacements: new CollectiblePlacement[]
                {
                    new CollectiblePlacement(CollectibleId.Coin, new Point(8,15), new Point(12,15)),
                    new CollectiblePlacement(CollectibleId.Coin, new Point(25, 9), new Point(30, 10)),
                    new CollectiblePlacement(CollectibleId.Gem, new Point(35, 8)),
                    new CollectiblePlacement(CollectibleId.Apple, new Point(40, 8)),
                    new CollectiblePlacement(CollectibleId.Meat, new Point(45, 8)),
                    new CollectiblePlacement(CollectibleId.Key, new Point(50, 8))
                },
                Transitions: new SceneTransitions(Right: SceneContentKey.Test2));
        }
        private SceneInfo CreateTest2()
        {
            return new SceneInfo(
                BgMap: new LayerInfo(LevelContentKey.TestLevelBG, PaletteIndex.P1, ScrollFactor: 70),
                FgMap: new LayerInfo(LevelContentKey.TestLevel2, PaletteIndex.P1, ScrollFactor: 100),
                InterfaceType.PlayerStatus,
                Bounds: new Rectangle(0, 0, _gameSystem.Screen.Width, _gameSystem.Screen.Height),
                PaletteKeys: new PaletteKeys(ImageContentKey.Palette1, ImageContentKey.Palette2, ImageContentKey.Palette3, ImageContentKey.Palette3),
                BackgroundColor: 0,
                VramImages: new TilesetWithPalette[]
                {
                    new TilesetWithPalette(TilesetContentKey.Tiles, PaletteIndex.P1),
                    new TilesetWithPalette(TilesetContentKey.Hero, PaletteIndex.P2),
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
                },
                CollectiblePlacements: new CollectiblePlacement[]
                {
                },
                Transitions: new SceneTransitions(Left: SceneContentKey.Test1));
        }
        private SceneInfo CreateLongMapTest()
        {
            return new SceneInfo(
                BgMap: new LayerInfo(LevelContentKey.TestLevelBG, PaletteIndex.P1, ScrollFactor: 70),
                FgMap: new LayerInfo(LevelContentKey.LongMapTest, PaletteIndex.P1, ScrollFactor: 100),
                InterfaceType.PlayerStatus,
                Bounds: new Rectangle(0, 0, _gameSystem.Screen.Width, _gameSystem.Screen.Height),
                PaletteKeys: new PaletteKeys(ImageContentKey.Palette1, ImageContentKey.Palette2, ImageContentKey.Palette3, ImageContentKey.Palette3),
                BackgroundColor: 0,
                VramImages: new TilesetWithPalette[]
                {
                    new TilesetWithPalette(TilesetContentKey.Tiles, PaletteIndex.P1),
                    new TilesetWithPalette(TilesetContentKey.Hero, PaletteIndex.P2),
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
                },
                CollectiblePlacements: new CollectiblePlacement[]
                {
                },
                Transitions: new SceneTransitions());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        }

        public void Initialize()
        {
            _dataSerializer.Save(SceneContentKey.Test1, CreateTest1());
            _dataSerializer.Save(SceneContentKey.Test2, CreateTest2());
            _dataSerializer.Save(SceneContentKey.Test3, CreateTest3());
            _dataSerializer.Save(SceneContentKey.LongMapTest, CreateLongMapTest());
        }

        public void OnWindowSizeChanged(Viewport viewport)
        {
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
