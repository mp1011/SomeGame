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
        private readonly ResourceLoader _resourceLoader;
        private readonly GraphicsDevice _graphicsDevice;
        public RAM RAM { get; }
 
        private VramData _vram;
        private RamPalette[] _palettes;
        private Layer[] _layers;
        private Sprite[] _sprites;
        private Palette _systemPalette;

        public readonly Rectangle Screen = new Rectangle(0, 0, 320, 240);
        public readonly int TileSize = 8;
        public readonly int LayerPixelWidth = 640;
        public readonly int LayerPixelHeight = 480;
        public readonly int LayerTileWidth = 80;
        public readonly int LayerTileHeight = 60;

        public readonly int ColorsPerPalette = 32;

        public RamByte BackgroundColorIndex { get; }

        //todo, replace with a bit
        private RamByte _isPaused;
        public bool Paused
        {
            get => _isPaused == 1;
            set
            {
                _isPaused.Set(value ? 1 : 0);
            }
        }

        public Color BackgroundColor => _palettes[0][BackgroundColorIndex];

        public IUserInputSource Input { get; }

        public GameSystem(GameStartup startup)
        {
            _graphicsDevice = startup.GraphicsDevice;
            _resourceLoader = new ResourceLoader(startup.ContentManager);
            _systemPalette = _resourceLoader
                                    .LoadTexture(ImageContentKey.SystemPalette)
                                    .ToIndexedTilesetImage()
                                    .Palette;

            DebugService.GameSystem = this;
            RAM = new RAM(this, startup.RamViewer ?? new EmptyRamViewer());

            BackgroundColorIndex = RAM.DeclareByte();
            _isPaused = RAM.DeclareByte();

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

            RAM.AddLabel("Begin Palette");
            _palettes = new RamPalette[]
            {
                RAM.DeclarePalette(_systemPalette),
                RAM.DeclarePalette(_systemPalette),
                RAM.DeclarePalette(_systemPalette),
                RAM.DeclarePalette(_systemPalette),
            };

            RAM.AddLabel("Begin Sprites");
            for (int i = 0; i < _sprites.Length; i++)
                _sprites[i] = new Sprite(this, LayerPixelWidth, LayerPixelHeight, TileSize);
            RAM.AddLabel("End Sprites");
        }

        public TileSet GetTileSet() => _vram.TileSet;

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

        public IEnumerable<Sprite> GetBackSprites() => _sprites.Where(p => p.Enabled && !p.Priority);
        public IEnumerable<Sprite> GetFrontSprites() => _sprites.Where(p => p.Enabled && p.Priority);

        public RamPalette GetPalette(PaletteIndex paletteIndex) => _palettes[(int)paletteIndex];

        public int GetTileOffset(TilesetContentKey tilesetContentKey)
        {
            return _vram.Offsets[tilesetContentKey];
        }

        public byte GetVramData(Point p) => _vram.ImageData[p.X, p.Y];

        public void SetVram(TilesetContentKey[] vramImagesP1, TilesetContentKey[] vramImagesP2, TilesetContentKey[] vramImagesP3, TilesetContentKey[] vramImagesP4)                       
        {
            List<IndexedTilesetImage> images = new List<IndexedTilesetImage>();

            images.AddRange(SetVram(vramImagesP1, PaletteIndex.P1));
            images.AddRange(SetVram(vramImagesP2, PaletteIndex.P2));
            images.AddRange(SetVram(vramImagesP3, PaletteIndex.P3));
            images.AddRange(SetVram(vramImagesP4, PaletteIndex.P4));

            if (images.Count == 0)
                return;

             _vram = CreateVramImage(images);

            SaveVramSnapshot(PaletteIndex.P1);
            SaveVramSnapshot(PaletteIndex.P2);
            SaveVramSnapshot(PaletteIndex.P3);
            SaveVramSnapshot(PaletteIndex.P4);
        }

        private IndexedTilesetImage[] SetVram(TilesetContentKey[] vramImages, PaletteIndex paletteIndex)
        {
            var textures = vramImages
                .Where(v=>v != TilesetContentKey.None)
                .Select(v => _resourceLoader.LoadTexture(v).ToIndexedTilesetImage(_systemPalette))
                .ToArray();

            List<byte> colors = new List<byte>();
            colors.Add(0);

            foreach (var texture in textures)
            {
                var imageColors = texture.Image
                    .ToArray()
                    .Distinct()
                    .ToArray();

                var newColors = imageColors
                    .Except(colors)
                    .ToArray();

                if (colors.Count + newColors.Length > ColorsPerPalette)
                    throw new Exception("Too many colors");

                colors.AddRange(newColors);                
            }

            GetPalette(paletteIndex).Set(colors);

            return textures.Select(t => new IndexedTilesetImage(
                Key: t.Key,
                Image: t.Image.Map(b => GetPalette(paletteIndex).GetIndex(_systemPalette[b])),
                Palette: null))
            .ToArray();

        }

        private void SaveVramSnapshot(PaletteIndex p)
        {
            var fileName = $"VRAM_{p}.png";
            if (File.Exists(fileName))
                File.Delete(fileName);

            using var fs = File.OpenWrite(fileName);
            var texture = _vram.ToTexture2D(GetPalette(p), _graphicsDevice);
            texture.SaveAsPng(fs, texture.Width, texture.Height);
        }

        private VramData CreateVramImage(IEnumerable<IndexedTilesetImage> data)
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

            return new VramData(ImageData: combined, Offsets: offsets, TileSet:new TileSet(offsets, TileSize, combined.Width,combined.Height));
        }
    }
}
