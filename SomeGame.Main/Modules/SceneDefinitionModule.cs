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

        private SceneInfo CreateTest1()
        {
            return new SceneInfo(
                BgMap: new LayerInfo(LevelContentKey.TestLevelBG, ScrollFactor: 70),
                FgMap: new LayerInfo(LevelContentKey.TestLevel, ScrollFactor: 100),
                InterfaceType.PlayerStatus,
                Bounds: new Rectangle(0, 0, _gameSystem.LayerPixelWidth, _gameSystem.Screen.Height),
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
                },
                Transitions: new SceneTransitions(Right: SceneContentKey.Test2));
        }
        private SceneInfo CreateTest2()
        {
            return new SceneInfo(
                BgMap: new LayerInfo(LevelContentKey.TestLevelBG, ScrollFactor: 70),
                FgMap: new LayerInfo(LevelContentKey.TestLevel2, ScrollFactor: 100),
                InterfaceType.PlayerStatus,
                Bounds: new Rectangle(0, 0, _gameSystem.Screen.Width, _gameSystem.Screen.Height),
                PaletteKeys: new PaletteKeys(ImageContentKey.Palette1, ImageContentKey.Palette2, ImageContentKey.Palette3, ImageContentKey.Palette3),
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

        public void Draw(SpriteBatch spriteBatch)
        {
        }

        public void Initialize()
        {
            _dataSerializer.Save(SceneContentKey.Test1, CreateTest1());
            _dataSerializer.Save(SceneContentKey.Test2, CreateTest2());

        }

        public void OnWindowSizeChanged(Viewport viewport)
        {
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
