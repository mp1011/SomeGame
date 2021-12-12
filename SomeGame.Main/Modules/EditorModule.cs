using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Modules
{
    abstract class EditorModule : GameModuleBase
    {
        protected EditorModule(GameStartup startup)
            : base(startup) { }

        protected virtual PaletteKeys PaletteKeys => new PaletteKeys(ImageContentKey.Palette1, ImageContentKey.Palette2, ImageContentKey.Palette1Inverse, ImageContentKey.Palette1Inverse);

        protected abstract void InitializeLayer(LayerIndex index, Layer layer);

        protected abstract IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader);

        protected virtual void AfterInitialize()
        {
        }

        public override void Initialize()
        {
            GameSystem.Input.Initialize(GameSystem.Screen);

            GameSystem.SetPalettes(
               ResourceLoader.LoadTexture(PaletteKeys.P1).ToIndexedTilesetImage().Palette,
               ResourceLoader.LoadTexture(PaletteKeys.P2).ToIndexedTilesetImage().Palette,
               ResourceLoader.LoadTexture(PaletteKeys.P3).ToIndexedTilesetImage().Palette,
               ResourceLoader.LoadTexture(PaletteKeys.P4).ToIndexedTilesetImage().Palette);

            var vramImages = LoadVramImages(ResourceLoader);

            GameSystem.SetVram(GraphicsDevice, vramImages);

            InitializeLayer(LayerIndex.BG, GameSystem.GetLayer(LayerIndex.BG));
            InitializeLayer(LayerIndex.FG, GameSystem.GetLayer(LayerIndex.FG));
            InitializeLayer(LayerIndex.Interface, GameSystem.GetLayer(LayerIndex.Interface));

            AfterInitialize();
        }
    }
}
