using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Editor;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SomeGame.Main.Modules
{
    class PaletteCreatorModule : EditorModule
    {
        private EnumMultiSelect<TilesetContentKey> _paletteSelector;
        private EnumMultiSelect<TilesetContentKey> _imageSelector;
        private UIButton _saveButton;
        private Font _font;

        public PaletteCreatorModule(GameStartup gameStartup) 
            : base(gameStartup)
        {
            SetVram(new TilesetContentKey[] { TilesetContentKey.Font },
                    new TilesetContentKey[] { },
                    new TilesetContentKey[] {  },
                    new TilesetContentKey[] { });
        }

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {
        }

        protected override void AfterInitialize()
        {
            _font = new Font(GameSystem.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");
            _paletteSelector = new EnumMultiSelect<TilesetContentKey>(GameSystem.GetLayer(LayerIndex.Interface), _font, new Point(0, 0));
            _imageSelector = new EnumMultiSelect<TilesetContentKey>(GameSystem.GetLayer(LayerIndex.Interface), _font, new Point(0, 1));            
            _saveButton = new UIButton("SAVE", new Point(35,2), GameSystem.GetLayer(LayerIndex.Interface), _font);

            UpdateImages();

            GameSystem.GetLayer(LayerIndex.Interface).Palette = PaletteIndex.P1;
        }

        private void UpdateImages()
        {
            GameSystem.SetVram(
                new TilesetContentKey[] { TilesetContentKey.Font },
                new TilesetContentKey[] { _paletteSelector.SelectedItem },
                new TilesetContentKey[] { _imageSelector.SelectedItem },
                new TilesetContentKey[] { _paletteSelector.SelectedItem } );

            _imageSelector.Refresh(GameSystem.GetLayer(LayerIndex.Interface));
            _paletteSelector.Refresh(GameSystem.GetLayer(LayerIndex.Interface));

            if (_paletteSelector.SelectedItem == TilesetContentKey.None
                || _imageSelector.SelectedItem == TilesetContentKey.None)
                return;

            int i = 1;
            var layer = GameSystem.GetLayer(LayerIndex.BG);

            layer.TileMap.SetEach((x, y) =>
            {
                if (y < 4) return new Tile(-1, TileFlags.None);
                return new Tile(i++, TileFlags.None);
            });

            layer.Palette = PaletteIndex.P4;
            layer.TileOffset = GameSystem.GetTileOffset(_imageSelector.SelectedItem);

            AdjustPalette();
        }

        private void AdjustPalette()
        {
            var paletteImage = Load(_paletteSelector.SelectedItem);
            var targetImage = Load(_imageSelector.SelectedItem);

            var ramPalette = GameSystem.GetPalette(PaletteIndex.P4);

            var adjustedPalette = targetImage.Palette.CreateTransformed(
                oldColor =>
                {
                    return paletteImage.Palette.OrderBy(c => c.DistanceTo(oldColor))
                                               .First();
                });

            ramPalette.Set(adjustedPalette);
        }

        private IndexedTilesetImage Load(TilesetContentKey key)
        {
            return ResourceLoader.LoadTexture(key)
                .ToIndexedTilesetImage();
        }

        protected override bool Update()
        {
            var layer = GameSystem.GetLayer(LayerIndex.Interface);

     
            if (_saveButton.Update(layer, Input) == UIButtonState.Pressed)
                SaveAdjustedImage();

            if (_paletteSelector.Update(layer, Input))
                UpdateImages();

            if (_imageSelector.Update(layer, Input))
                UpdateImages();

            return true;
        }

        private void SaveAdjustedImage()
        {
            var ramPalette = GameSystem.GetPalette(PaletteIndex.P4);
            var adjustedImage = ResourceLoader.LoadTexture(_imageSelector.SelectedItem)
                .ToIndexedTilesetImage(ramPalette);

            DataSerializer.SaveTilesetImage($"{_imageSelector.SelectedItem}_Adjusted",
                adjustedImage.ToTexture2D(GraphicsDevice));
        }
    }
}
