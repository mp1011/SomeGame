using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SomeGame.Main.Modules
{
    class PaletteCreatorModule : GameModuleBase
    {
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
                .ToDictionary(k=>k.Key,v=>v);

           // FitToPalette(images, TilesetContentKey.Tiles, TilesetContentKey.Items, graphicsDevice);
      
            //SaveReducedColorImage(images.Single(p => p.Key == TilesetContentKey.Items), 16, graphicsDevice);

         
            CreatePaletteImage(images, ImageContentKey.Palette1, graphicsDevice, TilesetContentKey.Tiles);
            CreatePaletteImage(images, ImageContentKey.Palette2, graphicsDevice, TilesetContentKey.Hero, TilesetContentKey.Bullet,
                TilesetContentKey.Skeleton, TilesetContentKey.Hero, TilesetContentKey.Font);

           // CreatePaletteImage(palettes[0].CreateTransformed(c=> new Color(255-c.R,255-c.G,255-c.B)), ImageContentKey.Palette1Inverse, graphicsDevice);
           // CreatePaletteImage(palettes[2], ImageContentKey.Palette3, graphicsDevice);

        }

        private void FitToPalette(IndexedTilesetImage[] images, TilesetContentKey fromKey, TilesetContentKey toKey, GraphicsDevice graphicsDevice)
        {
            var fromImage = images.Single(p => p.Key == fromKey);
            var toImage = images.Single(p => p.Key == toKey);

            Dictionary<byte, byte> map = new Dictionary<byte, byte>();
            foreach(var color in toImage.Palette.ToArray())
            {
                var closestColor = fromImage.Palette
                    .OrderBy(p => color.DistanceTo(p))
                    .First();

                map[toImage.Palette.GetIndex(color)] = fromImage.Palette.GetIndex(closestColor);
            }

            var adjustedImage = toImage.Image.Map(b => map.GetValueOrDefault(b, b));

            var texture = new IndexedImage(adjustedImage, fromImage.Palette)
                .ToTexture2D(graphicsDevice);

            var fileName = $"{toKey}_adjustedTo_{fromKey}.png";
            if (File.Exists(fileName))
                File.Delete(fileName);

            using (var fs = File.OpenWrite(fileName))
                texture.SaveAsPng(fs, texture.Width, texture.Height);   
        }

        private void CreatePaletteImage(Dictionary<TilesetContentKey, IndexedTilesetImage> sourceImages, ImageContentKey key, GraphicsDevice graphicsDevice, 
            params TilesetContentKey[] colorSources)
        {
            var colors = colorSources
                .SelectMany(p => sourceImages[p].Palette)
                .Distinct()
                .ToArray();

            if (colors.Length > GameSystem.ColorsPerPalette)
                throw new Exception("Too many colors");
            
            byte b = 0;
            var grid = new Grid<byte>(8, (int)Math.Ceiling(GameSystem.ColorsPerPalette / 8.0),
                (x, y) => b++);

            var indexedImage = new IndexedTilesetImage(TilesetContentKey.None, grid, new Palette(colors));
            _dataSerializer.SaveImage(key, indexedImage.ToTexture2D(graphicsDevice));
        }

        private void CreateExtendedPaletteImage(Palette p, ImageContentKey key, GraphicsDevice graphicsDevice)
        {
            byte b = 0;
            var grid = new Grid<byte>(8, p.Count() / 8,
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

        private void SaveReducedColorImage(IndexedTilesetImage image, int colors, GraphicsDevice graphicsDevice)
        {
            var fileName = $"{image.Key}_reduced_{colors}.png";
            if (File.Exists(fileName))
                File.Delete(fileName);

            using (var fs = File.OpenWrite(fileName))
            {
                var img = CreateReducedColorImage(image, colors, graphicsDevice);
                img.SaveAsPng(fs, img.Width, img.Height);
            }
        }

        private Texture2D CreateReducedColorImage(IndexedTilesetImage image, int maxColors, GraphicsDevice graphicsDevice)
        {
            var paletteColors = image.Palette.ToArray();

            var colors = paletteColors.Select(c =>
            {
                double minDistance = double.MaxValue;
                Color closestColor = c;
                
                foreach (var otherColor in paletteColors)
                {
                    if (c.A == 0)
                        break;
                    else if (otherColor.A == 0)
                        continue;
                    else if (otherColor == c)
                        continue;

                    var distance = c.DistanceTo(otherColor);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestColor = otherColor;
                    }
                }

                return new { Color = c, ClosestColor = closestColor, Distance = minDistance };
            })
            .OrderBy(p => p.Distance)
            .ToArray();

            Dictionary<Color, Color> map = new Dictionary<Color, Color>();

            while(colors.Any() && paletteColors.Length > maxColors)
            {
                var closestColor = colors.First();
                colors = colors.Where(c => c.Color != closestColor.Color && c.Color != closestColor.ClosestColor).ToArray();

                map.Add(closestColor.Color, closestColor.ClosestColor);
                paletteColors = paletteColors.Where(p => p != closestColor.Color).ToArray();
            }

            var reducedPalette = new Palette(paletteColors);
            var reducedColorImage = image.Image.Map(b =>
            {
                var newColor = map.GetValueOrDefault(image.Palette[b], image.Palette[b]);
                return reducedPalette.GetIndex(newColor);
            });

            if (reducedPalette.Count() > maxColors)
                return CreateReducedColorImage(new IndexedTilesetImage(image.Key, reducedColorImage, reducedPalette), maxColors, graphicsDevice);
            else 
                return new IndexedImage(reducedColorImage, reducedPalette)
                    .ToTexture2D(graphicsDevice);
        }

    }
}
