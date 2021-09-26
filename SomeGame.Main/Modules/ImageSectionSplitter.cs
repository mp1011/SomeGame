using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;
using System.IO;

namespace SomeGame.Main.Modules
{
    class ImageSectionSplitter : GameModuleBase
    {
        private readonly TileSetService _tileSetService;
        private GraphicsDevice _graphicsDevice;

        private ImageContentKey _imageKey;
        private TilesetContentKey _tileSetKey;
        private IndexedImage _image;

        public ImageSectionSplitter(ImageContentKey imageKey, TilesetContentKey tilesetContentKey)
        {
            _imageKey = imageKey;
            _tileSetKey = tilesetContentKey;
            _tileSetService = new TileSetService();
        }

        protected override void AfterInitialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }
        protected override Palette CreatePalette(IndexedTilesetImage[] tilesetImages, PaletteIndex index)
        {
            if (index == PaletteIndex.P1)
                return tilesetImages[0].Palette;
            else 
                return tilesetImages[0].Palette.CreateTransformed(c => new Color(c.R, 0, 0));
        }

        protected override void InitializeLayer(GameSystem system, LayerIndex index, Layer layer)
        {
            if(index == LayerIndex.BG)
            {
                var grid = _tileSetService.CreateTileMapFromImageAndTileset(_image, system.GetTileSet(PaletteIndex.P1));
                layer.TileMap.SetEach(0,grid.Width,0, grid.Height, (x, y) => grid[x, y]);
            }
            else if(index == LayerIndex.FG)
            {
                layer.Palette = PaletteIndex.P2;
            }
        }

        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader, GameSystem system)
        {
            _image = resourceLoader.LoadTexture(_imageKey)
                .ToIndexedImage();

            return new IndexedTilesetImage[] { resourceLoader.LoadTexture(_tileSetKey)
                                                             .ToIndexedTilesetImage() };
        }

        private Point? _dragStart;
        private Point _mouseTile;
        private Point _lastMouseTile;

        protected override void Update(GameSystem gameSystem, Scene currentScene)
        {
            var background = gameSystem.GetLayer(LayerIndex.BG);
            var foreground = gameSystem.GetLayer(LayerIndex.FG);

            _mouseTile = background.TilePointFromPixelPoint(Input.MouseX, Input.MouseY);
            
            foreground.TileMap.SetTile(_mouseTile.X, _mouseTile.Y, background.TileMap.GetTile(_mouseTile));

            if (!_mouseTile.Equals(_lastMouseTile))
                foreground.TileMap.SetTile(_lastMouseTile.X, _lastMouseTile.Y, new Tile(0, TileFlags.None));

            if (Input.A.IsPressed())
            {
                _dragStart = _mouseTile;
            }
            else if (Input.A.IsReleased() && _dragStart.HasValue)
            {
                foreground.TileMap.SetEach((x, y) =>
                {
                    return new Tile(0, TileFlags.None);
                });

                ExportDragArea(_dragStart.Value, _mouseTile, gameSystem.GetPalette(PaletteIndex.P1), gameSystem.TileSize);
                _dragStart = null;
            }

            if (Input.A.IsDown() && _dragStart.HasValue)
                ShowDragArea(_dragStart.Value, _mouseTile, foreground, background);

            _lastMouseTile = _mouseTile;
        }

        private void ExportDragArea(Point start, Point end, Palette palette, int tileSize)
        {
            var mult = new Point(tileSize, tileSize);

            var extracted = new IndexedImage(_image.Image.Extract(start*mult, (end*mult+ mult) -new Point(1,1)), palette);
            var texture = extracted.ToTexture2D(_graphicsDevice);

            using var fs = File.OpenWrite($"Capture_{DateTime.Now.Ticks}.png");
            texture.SaveAsPng(fs, texture.Width, texture.Height);
        }

        private void ShowDragArea(Point start, Point end, Layer foreground, Layer background)
        {
            foreground.TileMap.SetEach((x, y) =>
            {
                if(x >= start.X 
                    && x <= end.X
                    && y >= start.Y
                    && y <= end.Y)
                {
                    return background.TileMap.GetTile(x, y);
                }
                else
                {
                    return new Tile(0, TileFlags.None);
                }
            });
        }

    }
}
