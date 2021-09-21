using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System.Linq;

namespace SomeGame.Main.Modules
{
    class GameSystemTestModule : GameModuleBase
    {
        protected override Palette CreatePalette(Palette basePalette, PaletteIndex index)
        {
            if (index == PaletteIndex.P1)
                return basePalette;

            return new Palette(Enumerable.Range(0, 32)
                                         .Select(i => new Color(i * 4, i, i * 3)));
        }

        protected override void Update(GameTime gameTime, GameSystem gameSystem)
        {
            var layer = gameSystem.GetLayer(LayerIndex.FG);

            if (Input.Left.IsDown())
                layer.ScrollX -= 2;
            if (Input.Right.IsDown())
                layer.ScrollX += 2;
            if (Input.Up.IsDown())
                layer.ScrollY -= 2;
            if (Input.Down.IsDown())
                layer.ScrollY += 2;
        }

        protected override void InitializeLayer(GameSystem system, LayerIndex index, Layer layer)
        {
            if(index == LayerIndex.FG)
            {
                layer.TileMap.SetTile(0, 0, new Tile(16, TileFlags.None));

                layer.TileMap.SetTile(40, 0, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(40, 1, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(41, 0, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(41, 1, new Tile(17, TileFlags.None));

                layer.TileMap.SetTile(0, 20, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(1, 20, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(2, 20, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(0, 21, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(1, 21, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(2, 21, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(0, 22, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(1, 22, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(2, 22, new Tile(17, TileFlags.None));


                layer.TileMap.SetTile(40, 20, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(41, 20, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(42, 20, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(43, 20, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(40, 21, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(41, 21, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(42, 21, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(43, 21, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(40, 22, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(41, 22, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(42, 22, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(43, 22, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(40, 23, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(41, 23, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(42, 23, new Tile(17, TileFlags.None));
                layer.TileMap.SetTile(43, 23, new Tile(17, TileFlags.None));


            }
        }

        protected override IndexedImage[] LoadVramImages(ResourceLoader resourceLoader, GameSystem system)
        {
            using var image = resourceLoader.LoadTexture(TilesetContentKey.Tiles);
            using var font = resourceLoader.LoadTexture(TilesetContentKey.Font);

            return new IndexedImage[] { image.ToIndexedImage(), font.ToIndexedImage() };
        }
    }
}
