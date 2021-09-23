using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System.IO;

namespace SomeGame.Main.Modules
{

    class LevelEditorModule : GameModuleBase
    {
        private readonly DataSerializer _dataSerializer;

        public LevelEditorModule()
        {
            _dataSerializer = new DataSerializer();
        }

        protected override Palette CreatePalette(Palette basePalette, PaletteIndex index)
        {
            return basePalette;
        }

        protected override void Update(GameTime gameTime, GameSystem gameSystem)
        {
            var foreground = gameSystem.GetLayer(LayerIndex.FG);
            var background = gameSystem.GetLayer(LayerIndex.BG);

            var mouseTile = foreground.TilePointFromPixelPoint(Input.MouseX, Input.MouseY);

            foreground.TileMap.SetEach((x, y) => {
                if (x == mouseTile.X && y == mouseTile.Y)
                    return new Tile(16, TileFlags.Solid);
                else
                    return new Tile(-1, TileFlags.None);
                });

            if (Input.A.IsDown())
                background.TileMap.SetTile(mouseTile.X, mouseTile.Y, new Tile(16, TileFlags.Solid));
            if (Input.B.IsDown())
                background.TileMap.SetTile(mouseTile.X, mouseTile.Y, new Tile(-1, TileFlags.None));

            if (Input.Right.IsDown())
            {
                background.ScrollX -= 2;
                foreground.ScrollX -= 2;
            }
            if (Input.Left.IsDown())
            {
                background.ScrollX += 2;
                foreground.ScrollX += 2;
            }
            if (Input.Down.IsDown())
            {
                background.ScrollY -= 2;
                foreground.ScrollY -= 2;
            }
            if (Input.Up.IsDown())
            {
                background.ScrollY += 2;
                foreground.ScrollY -= 2;
            }

            if (Input.Start.IsPressed())
                SaveMap(background.TileMap);
        }

        private void SaveMap(TileMap t)
        {
            _dataSerializer.Save(t);
        }

        protected override void InitializeLayer(GameSystem system, LayerIndex index, Layer layer)
        {      
            if(index == LayerIndex.BG)
            {
                var loaded = _dataSerializer.Load(LevelContentKey.TestLevel);
                layer.TileMap.SetEach((x, y) => loaded.GetTile(x, y));
            }
        }

        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader, GameSystem system)
        {
            using var image = resourceLoader.LoadTexture(TilesetContentKey.Tiles);
            return new IndexedTilesetImage[] { image.ToIndexedTilesetImage()};
        }
    }
}
