using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System.Linq;

namespace SomeGame.Main.Modules
{
    class FontTestModule : GameModuleBase
    {
        protected override Palette CreatePalette(IndexedTilesetImage[] tilesetImages, PaletteIndex index) => tilesetImages[0].Palette;

        protected override void Update(GameSystem gameSystem, Scene currentScene)
        {
        }

        protected override void InitializeLayer(GameSystem system, LayerIndex index, Layer layer)
        {
            if(index == LayerIndex.FG)
            {
                var font = new Font(system.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");
                var tiles = font.FromString("HALLO WORLD");

                layer.TileMap.SetEach(0, tiles.Length, 0, 1, (x, y) => tiles[x]);
            }
        }

        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader, GameSystem system)
        {
            using var font = resourceLoader.LoadTexture(TilesetContentKey.Font);
            return new IndexedTilesetImage[] { font.ToIndexedTilesetImage() };
        }
    }
}
