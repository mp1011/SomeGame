using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Modules
{
    class PaletteCreatorModule : GameModuleBase
    {
        private IndexedTilesetImage[] _images;
        private readonly DataSerializer _dataSerializer;

        public PaletteCreatorModule()
        {
            _dataSerializer = new DataSerializer();
        }

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {
        }

        protected override void AfterInitialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
        {
            var images = Enum.GetValues<TilesetContentKey>()
                .Where(t => t != TilesetContentKey.None)
                .Select(t => resourceLoader.LoadTexture(t).ToIndexedTilesetImage())
                .ToArray();

            var palettes = CreatePalettes(images);

            CreatePaletteImage(palettes[0], ImageContentKey.Palette1, graphicsDevice);
            CreatePaletteImage(palettes[1], ImageContentKey.Palette2, graphicsDevice);
            CreatePaletteImage(palettes[0].CreateTransformed(c=> new Color(255-c.R,255-c.G,255-c.B)), ImageContentKey.Palette1Inverse, graphicsDevice);
        }

        private void CreatePaletteImage(Palette p, ImageContentKey key, GraphicsDevice graphicsDevice)
        {
            byte b = 0;
            var grid = new Grid<byte>(8, (int)Math.Ceiling(GameSystem.ColorsPerPalette / 8.0),
                (x, y) => b++);

            var indexedImage = new IndexedTilesetImage(TilesetContentKey.None, grid, p);
            _dataSerializer.SaveImage(key, indexedImage.ToTexture2D(graphicsDevice));
        }

        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader)
        {
            return Enum.GetValues<TilesetContentKey>()
                .Where(t => t != TilesetContentKey.None)
                .Select(t => resourceLoader.LoadTexture(t).ToIndexedTilesetImage())
                .ToArray();
        }

        protected override void Update()
        {
        }

        public Palette[] CreatePalettes(IndexedTilesetImage[] images)
        {
            List<Color> colors = new List<Color>();
            List<Palette> palettes = new List<Palette>();

            foreach (var image in images)
            {
                var newColors = image.Palette
                                     .Except(colors)
                                     .ToArray();

                if (newColors.Length > GameSystem.ColorsPerPalette)
                    throw new Exception("Too many colors");

                if (colors.Count + newColors.Length > GameSystem.ColorsPerPalette)
                {
                    palettes.Add(new Palette(colors));
                    colors.Clear();
                }

                colors.AddRange(newColors);
            }

            if (colors.Any())
                palettes.Add(new Palette(colors));

            if (palettes.Count > 4)
                throw new Exception("Too many colors");


            var adjustedImages = images.Select(img => AdjustToBestPalette(img, palettes))
                .ToArray();

            return palettes.ToArray();            
        }

        private IndexedTilesetImage AdjustToBestPalette(IndexedTilesetImage image, IEnumerable<Palette> palettes)
        {
            foreach (var palette in palettes)
            {
                var map = image.Palette.CreateMap(palette);
                if (map != null)
                {
                    return new IndexedTilesetImage(
                        image.Key,
                        image.Image.Map(b => map[b]),
                        palette);
                }
            }

            return image;
        }

    }
}
