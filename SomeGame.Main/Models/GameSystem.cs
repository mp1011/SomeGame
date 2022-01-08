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

        private readonly RamGrid<RamNibble> _patternTable;

        private Dictionary<TilesetContentKey, byte> _offsets; //todo - ram'ify this

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

        public readonly int ColorsPerPalette = 16;

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

            if (startup.ContentManager != null)
                _systemPalette = new Palette(_resourceLoader
                                        .LoadTexture(ImageContentKey.SystemPalette)
                                        .ToColorData());

            DebugService.GameSystem = this;
            RAM = new RAM(this, startup.RamViewer ?? new EmptyRamViewer());

            BackgroundColorIndex = RAM.DeclareByte();
            _isPaused = RAM.DeclareByte();

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

            RAM.AddLabel("Begin Pattern Table");
            _patternTable = RAM.DeclareNibbleGrid(128, 256);

            RAM.AddLabel("Begin Sprites");
            for (int i = 0; i < _sprites.Length; i++)
                _sprites[i] = new Sprite(this, LayerPixelWidth, LayerPixelHeight, TileSize);

            RAM.AddLabel("Begin Layer TileMaps");
            _layers = new Layer[]
            {
                new Layer(this,new TileMap(this, LevelContentKey.None, LayerTileWidth,LayerTileHeight), PaletteIndex.P1,
                    new RotatingInt(0, LayerPixelWidth), new RotatingInt(0, LayerPixelHeight), TileSize),
                new Layer(this,new TileMap(this, LevelContentKey.None,LayerTileWidth,LayerTileHeight), PaletteIndex.P1,
                    new RotatingInt(0, LayerPixelWidth), new RotatingInt(0, LayerPixelHeight), TileSize),
                new Layer(this,new TileMap(this, LevelContentKey.None,LayerTileWidth,LayerTileHeight), PaletteIndex.P1,
                    new RotatingInt(0, LayerPixelWidth), new RotatingInt(0, LayerPixelHeight), TileSize)
            };
            RAM.AddLabel("End Layer TileMaps");
        }

        public Layer GetLayer(LayerIndex layerIndex) =>_layers[(int)layerIndex];

        public Sprite GetSprite(SpriteIndex spriteIndex) => _sprites[(int)spriteIndex];

        public IEnumerable<Sprite> GetBackSprites() => _sprites.Where(p => p.Enabled && !p.Priority);
        public IEnumerable<Sprite> GetFrontSprites() => _sprites.Where(p => p.Enabled && p.Priority);

        public RamPalette GetPalette(PaletteIndex paletteIndex) => _palettes[(int)paletteIndex];

        public byte GetTileOffset(TilesetContentKey tilesetContentKey)
        {
            return _offsets[tilesetContentKey];
        }

        public byte GetPatternTableData(Point p) => _patternTable[p.X, p.Y];

        public void SetVram(TilesetContentKey[] vramImagesP1, TilesetContentKey[] vramImagesP2, TilesetContentKey[] vramImagesP3, TilesetContentKey[] vramImagesP4)                       
        {
            List<IndexedTilesetImage> images = new List<IndexedTilesetImage>();

            images.AddRange(SetVram(vramImagesP1, PaletteIndex.P1));
            images.AddRange(SetVram(vramImagesP2, PaletteIndex.P2));
            images.AddRange(SetVram(vramImagesP3, PaletteIndex.P3));
            images.AddRange(SetVram(vramImagesP4, PaletteIndex.P4));

            if (images.Count == 0)
                return;

            FillPatternTable(images);

            SavePatternTableSnapshot(PaletteIndex.P1);
            SavePatternTableSnapshot(PaletteIndex.P2);
            SavePatternTableSnapshot(PaletteIndex.P3);
            SavePatternTableSnapshot(PaletteIndex.P4);
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
                    .Select(t=>(byte)t)
                    .Distinct()
                    .ToArray();

                var newColors = imageColors
                    .Except(colors)
                    .ToArray();
   
                colors.AddRange(newColors);
                if (colors.Count > ColorsPerPalette)
                    colors = colors.Take(ColorsPerPalette).ToList(); //todo, should give error
                      
            }

            GetPalette(paletteIndex).Set(colors);

            return textures.Select(t => new IndexedTilesetImage(
                Key: t.Key,
                Image: t.Image.Map(b => GetPalette(paletteIndex).GetIndex(_systemPalette[b])),
                Palette: null))
            .ToArray();
        }

        private void SavePatternTableSnapshot(PaletteIndex p)
        {
            var fileName = $"PatternTable_{p}.png";
            if (File.Exists(fileName))
                File.Delete(fileName);

            using var fs = File.OpenWrite(fileName);
            var texture = _patternTable.ToTexture2D(GetPalette(p), _graphicsDevice);
            texture.SaveAsPng(fs, texture.Width, texture.Height);
        }

        private void FillPatternTable(IEnumerable<IndexedTilesetImage> data)
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

            _offsets = new Dictionary<TilesetContentKey, byte>();
            byte currentOffset = 0;
            foreach(var tileGroup in tileGroups)
            {
                _offsets[tileGroup.Key] = currentOffset;
                currentOffset = (byte)(currentOffset + tileGroup.Data.Length);
            }

            List<MemoryGrid<byte>> finalTiles = new List<MemoryGrid<byte>>();          
            finalTiles.AddRange(tileGroups.SelectMany(p => p.Data));

            while(finalTiles.Count < 512)
                finalTiles.Add(new MemoryGrid<byte>(8, 8, (x, y) => 0));
            
            var combined = finalTiles.Combine(_patternTable.Width/TileSize);

            if (combined.Height > _patternTable.Height)            
                throw new Exception("Too many tiles");
            
            _patternTable.ForEach((x, y, n) =>
            {
                n.Set(combined[x, y]);
            });
        }

        public Rectangle GetPatternTableBlock(byte tileIndex)
        {
            int tileWidth = _patternTable.Width / TileSize;
            int row = tileIndex / tileWidth;
            int col = tileIndex % tileWidth;
            return new Rectangle(col * TileSize, row * TileSize, TileSize, TileSize);
        }
    }
}
