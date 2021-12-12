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
        private EnumMultiSelect<ImageContentKey> _paletteSelector;
        private EnumMultiSelect<TilesetContentKey> _imageSelector;
        private UIButton _addColorsButton;
        private UIButton _saveButton;
        private Palette _constructedPalette;

        private Font _font;
        public PaletteCreatorModule(GameStartup gameStartup) 
            : base(gameStartup)
        {
        }

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {
        }

        protected override void AfterInitialize()
        {

            _font = new Font(GameSystem.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");
            _paletteSelector = new EnumMultiSelect<ImageContentKey>(GameSystem.GetLayer(LayerIndex.Interface), _font, new Point(4, 0));
            _imageSelector = new EnumMultiSelect<TilesetContentKey>(GameSystem.GetLayer(LayerIndex.Interface), _font, new Point(4, 1));
            
            _addColorsButton = new UIButton("ADD COLORS", new Point(16, 2), GameSystem.GetLayer(LayerIndex.Interface), _font);
            _saveButton = new UIButton("SAVE", new Point(35,2), GameSystem.GetLayer(LayerIndex.Interface), _font);

            _font.WriteToLayer("PAL ", GameSystem.GetLayer(LayerIndex.Interface), new Point(0, 0));
            _font.WriteToLayer("IMG ", GameSystem.GetLayer(LayerIndex.Interface), new Point(0, 1));

            //var images = Enum.GetValues<TilesetContentKey>()
            //    .Where(t => t != TilesetContentKey.None)
            //    .Select(t => ResourceLoader.LoadTexture(t).ToIndexedTilesetImage())
            //    .ToDictionary(k=>k.Key,v=>v);

            //FitToPalette(images, TilesetContentKey.Tiles, TilesetContentKey.Items, graphicsDevice);

            //SaveReducedColorImage(images.Single(p => p.Key == TilesetContentKey.Items), 16, graphicsDevice);


            //  CreatePaletteImage(images, ImageContentKey.Palette1, GraphicsDevice, TilesetContentKey.Tiles);
            //  CreatePaletteImage(images, ImageContentKey.Palette2, GraphicsDevice, TilesetContentKey.Hero, TilesetContentKey.Bullet,
            //   TilesetContentKey.Skeleton, TilesetContentKey.Hero, TilesetContentKey.Font, TilesetContentKey.Hud);

            // CreatePaletteImage(palettes[0].CreateTransformed(c=> new Color(255-c.R,255-c.G,255-c.B)), ImageContentKey.Palette1Inverse, graphicsDevice);
            // CreatePaletteImage(palettes[2], ImageContentKey.Palette3, graphicsDevice);

        }

        private IndexedTilesetImage FitToPalette(IndexedTilesetImage image, Palette palette)
        {
            Dictionary<byte, byte> map = new Dictionary<byte, byte>();
            foreach(var color in palette.ToArray())
            {
                if (color.A != 255)
                    continue;

                var closestColor = image.Palette
                    .OrderBy(p => color.DistanceTo(p))
                    .First();

                map[image.Palette.GetIndex(color)] = palette.GetIndex(closestColor);
            }

            var adjustedImage = image.Image.Map(b => map.GetValueOrDefault(b, b));
            return new IndexedTilesetImage(image.Key, adjustedImage, palette);
        }

        private void CreateExtendedPaletteImage(Palette p, ImageContentKey key, GraphicsDevice graphicsDevice)
        {
            byte b = 0;
            var grid = new Grid<byte>(8, p.Count() / 8,
                (x, y) => b++);

            var indexedImage = new IndexedTilesetImage(TilesetContentKey.None, grid, p);
            DataSerializer.SaveImage(key, indexedImage.ToTexture2D(graphicsDevice));
        }

        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader)
        {
             return new IndexedTilesetImage[]
             {
                ResourceLoader.LoadTexture(TilesetContentKey.Font).ToIndexedTilesetImage()
             };
        }

        protected override void Update()
        {
            var layer = GameSystem.GetLayer(LayerIndex.Interface);

            if(_addColorsButton.Update(layer, Input) == UIButtonState.Pressed)
                AddToPalette();

            if (_saveButton.Update(layer, Input) == UIButtonState.Pressed)
                SavePalette();

                if (Input.Left.IsPressed())
            {
                _imageSelector.Increment(layer);
                UpdateImage();
            }
            if (Input.Right.IsPressed())
            {
                _imageSelector.Decrement(layer);
                UpdateImage();
            }

            if (Input.Up.IsPressed())
            {
                _paletteSelector.Increment(layer);
                UpdateImage();
            }

            if (Input.Down.IsPressed())
            {
                _paletteSelector.Decrement(layer);
                UpdateImage();
            }
        }

        private void AddToPalette()
        {
            var currentColors = GameSystem
                .GetPalette(PaletteIndex.P1)
                .ToArray();

            if (_constructedPalette == null)
            {
                _constructedPalette = new Palette(currentColors);
                return;
            }


            _constructedPalette = new Palette(_constructedPalette
                                                .Union(currentColors)
                                                .Distinct());

            UpdateImage(_constructedPalette);
        }

        private void SavePalette()
        {
            if (_constructedPalette.Count() > GameSystem.ColorsPerPalette)
                throw new Exception("Too many colors");

            byte b = 0;
            var grid = new Grid<byte>(8, (int)Math.Ceiling(GameSystem.ColorsPerPalette / 8.0),
                (x, y) => b++);

            var indexedImage = new IndexedTilesetImage(TilesetContentKey.None, grid, _constructedPalette);
            DataSerializer.SaveImage(_paletteSelector.SelectedItem, indexedImage.ToTexture2D(GraphicsDevice));
        }

        private void UpdateImage()
        {
            if (_paletteSelector.SelectedItem == ImageContentKey.None)
                return;

            var palette = ResourceLoader
                .LoadTexture(_paletteSelector.SelectedItem)
                .ToIndexedImage()
                .Palette;

            UpdateImage(palette);
        }

        private void UpdateImage(Palette palette)
        {
            if (_imageSelector.SelectedItem == TilesetContentKey.None)
                return;

            var image = ResourceLoader
                .LoadTexture(_imageSelector.SelectedItem)
                .ToIndexedTilesetImage();

            image = FitToPalette(image, palette);

            GameSystem.SetPalettes(palette, palette, palette, palette);
            GameSystem.SetVram(GraphicsDevice, new IndexedTilesetImage[]
            {
                ResourceLoader.LoadTexture(TilesetContentKey.Font).ToIndexedTilesetImage(),
                image
            });

            var bg = GameSystem.GetLayer(LayerIndex.BG);
            int i = 0;

            bg.TileMap.SetEach((x, y) =>
            {
                if(y < 4)
                    return new Tile(-1, TileFlags.None);

                if (i < image.Image.Size/ (GameSystem.TileSize*GameSystem.TileSize))
                    return new Tile(GameSystem.GetTileOffset(_imageSelector.SelectedItem) + (i++), TileFlags.None);
                else
                    return new Tile(-1, TileFlags.None);
            });

            _font.WriteToLayer($"COLORS {palette.Count().ToString("000")}", GameSystem.GetLayer(LayerIndex.Interface), new Point(0, 2));
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
