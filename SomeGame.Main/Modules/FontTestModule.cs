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
        protected override Palette CreatePalette(Palette basePalette, PaletteIndex index) => basePalette;

        protected override void Update(GameTime gameTime, GameSystem gameSystem)
        {
        }

        protected override void InitializeLayer(GameSystem system, LayerIndex index, Layer layer)
        {
            if(index == LayerIndex.FG)
            {
                var font = new Font(system.GetTileSet(PaletteIndex.P1), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");
                var tiles = font.FromString("HALLO WORLD");

                layer.TileMap.SetEach(0, tiles.Length, 0, 1, (x, y) => tiles[x]);
            }
        }

        protected override IndexedImage[] LoadVramImages(ResourceLoader resourceLoader, GameSystem system)
        {
            using var font = resourceLoader.LoadTexture(TilesetContentKey.Font);
            return new IndexedImage[] { font.ToIndexedImage() };
        }
    }
}
