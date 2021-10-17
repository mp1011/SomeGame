using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Input;
using SomeGame.Main.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SomeGame.Main.Models
{
    class GameSystem
    {
        private VramData _tileSetData;
        private Palette[] _palettes;
        private Layer[] _layers;
        private TileSet[] _tileSets;
        private Sprite[] _sprites;

        public readonly Rectangle Screen = new Rectangle(0, 0, 320, 240);
        public int TileSize = 8;
        public int LayerPixelWidth = 640;
        public int LayerPixelHeight = 480;
        public int LayerTileWidth = 80;
        public int LayerTileHeight = 60;
        public int ColorsPerPalette = 24;

        public IUserInputSource Input { get; }

        public GameSystem()
        {
            DebugService.GameSystem = this;

            _layers = new Layer[]
            {
                new Layer(new TileMap(LevelContentKey.None, LayerTileWidth,LayerTileHeight), PaletteIndex.P1, 
                    new RotatingInt(0, LayerPixelWidth), new RotatingInt(0, LayerPixelHeight), TileSize),
                new Layer(new TileMap(LevelContentKey.None,LayerTileWidth,LayerTileHeight), PaletteIndex.P1,
                    new RotatingInt(0, LayerPixelWidth), new RotatingInt(0, LayerPixelHeight), TileSize),
                new Layer(new TileMap(LevelContentKey.None,LayerTileWidth,LayerTileHeight), PaletteIndex.P1,
                    new RotatingInt(0, LayerPixelWidth), new RotatingInt(0, LayerPixelHeight), TileSize)
            };

            Input = new MouseAndKeyboardInputSource();
            _sprites = new Sprite[Enum.GetValues<SpriteIndex>().Length];

            for (int i = 0; i < _sprites.Length; i++)
                _sprites[i] = new Sprite(LayerPixelWidth, LayerPixelHeight, TileSize);
        }

        public TileSet GetTileSet(PaletteIndex paletteIndex) => _tileSets[(int)paletteIndex];

        public Layer GetLayer(LayerIndex layerIndex) =>_layers[(int)layerIndex];

        public Sprite GetSprite(SpriteIndex spriteIndex) => _sprites[(int)spriteIndex];

        public SpriteIndex? GetFreeSpriteIndex()
        {
            var index = Array.FindIndex(_sprites, p => !p.Enabled);
            if (index < 0)
                return null;
            else
                return (SpriteIndex)index;
        }

        public IEnumerable<Sprite> GetBackSprites() => _sprites.Where(p => p.Enabled && p.Priority == SpritePriority.Back);
        public IEnumerable<Sprite> GetFrontSprites() => _sprites.Where(p => p.Enabled && p.Priority == SpritePriority.Front);


        public Palette GetPalette(PaletteIndex paletteIndex) => _palettes[(int)paletteIndex];

        public int GetTileOffset(TilesetContentKey tilesetContentKey)
        {
            return _tileSetData.Offsets[tilesetContentKey];
        }

        public void SetPalettes(Palette palette1, Palette palette2, Palette palette3, Palette palette4)
        {
            _palettes = new Palette[] {
                palette1,
                palette2,
                palette3,
                palette4
            };
        }

        public void SetVram(GraphicsDevice graphicsDevice, IndexedTilesetImage[] data)
        {
            _tileSetData = CreateVramImage(data);
           
            _tileSets = _palettes
                .Select(pal =>
                {
                    var texture = new IndexedImage(_tileSetData.ImageData, pal)
                                           .ToTexture2D(graphicsDevice);

                    return new TileSet(texture, _tileSetData.Offsets, TileSize);
                }).ToArray();

            SaveVramSnapshot(PaletteIndex.P1);
            SaveVramSnapshot(PaletteIndex.P2);
            SaveVramSnapshot(PaletteIndex.P3);
            SaveVramSnapshot(PaletteIndex.P4);
        }

        private void SaveVramSnapshot(PaletteIndex p)
        {
            var fileName = $"VRAM_{p}.png";
            if (File.Exists(fileName))
                File.Delete(fileName);

            using var fs = File.OpenWrite(fileName);
            var texture = _tileSets[(int)p].Texture;
            texture.SaveAsPng(fs, texture.Width, texture.Height);
        }

        private VramData CreateVramImage(IndexedTilesetImage[] data)
        {
            var splitImages = data
                .Select(d => new { Key = d.Key, Data = d.Image.Split(TileSize) })
                .ToArray();

            var tileGroups = splitImages.Select(p => new
            {
                Key = p.Key,
                Data = p.Data.Where(p => !p.All((x, y, t) => t == 0))
                             .ToArray()
            }).ToArray();

            var offsets = new Dictionary<TilesetContentKey, int>();
            int currentOffset = 0;
            foreach(var tileGroup in tileGroups)
            {
                offsets[tileGroup.Key] = currentOffset;
                currentOffset += tileGroup.Data.Length;
            }

            var combined = tileGroups
                            .SelectMany(p=>p.Data)
                            .Combine(32);

            return new VramData(ImageData: combined, Offsets: offsets);
        }
    }
}
