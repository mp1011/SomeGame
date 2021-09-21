using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Extensions;
using SomeGame.Main.Input;
using SomeGame.Main.Services;
using System.IO;
using System.Linq;

namespace SomeGame.Main.Models
{
    class GameSystem
    {
        private Grid<byte> _tileSetData;
        private Palette[] _palettes;
        private Layer[] _layers;
        private TileSet[] _tileSets;

        public readonly Rectangle Screen = new Rectangle(0, 0, 320, 240);
        public int TileSize = 8;
        public int LayerPixelWidth = 640;
        public int LayerPixelHeight = 480;
        public int LayerTileWidth = 80;
        public int LayerTileHeight = 60;

        public IUserInputSource Input { get; }

        public GameSystem()
        {
            _layers = new Layer[]
            {
                new Layer(new TileMap(LayerTileWidth,LayerTileHeight), PaletteIndex.P1, 
                    new RotatingInt(0, LayerPixelWidth), new RotatingInt(0, LayerPixelHeight), TileSize),
                new Layer(new TileMap(LayerTileWidth,LayerTileHeight), PaletteIndex.P1,
                    new RotatingInt(0, LayerPixelWidth), new RotatingInt(0, LayerPixelHeight), TileSize),
                new Layer(new TileMap(LayerTileWidth,LayerTileHeight), PaletteIndex.P1,
                    new RotatingInt(0, LayerPixelWidth), new RotatingInt(0, LayerPixelHeight), TileSize)
            };

            Input = new MouseAndKeyboardInputSource();
        }

        public TileSet GetTileSet(PaletteIndex paletteIndex) => _tileSets[(int)paletteIndex];

        public Layer GetLayer(LayerIndex layerIndex) =>_layers[(int)layerIndex];

        public Palette GetPalette(PaletteIndex paletteIndex) => _palettes[(int)paletteIndex];

        public void SetPalettes(Palette palette1, Palette palette2, Palette palette3, Palette palette4)
        {
            _palettes = new Palette[] {
                palette1,
                palette2,
                palette3,
                palette4
            };
        }

        public void SetVram(GraphicsDevice graphicsDevice, IndexedImage[] data)
        {
            _tileSetData = CreateVramImage(data);
           
            _tileSets = _palettes
                .Select(pal =>
                {
                    var texture = new IndexedImage(_tileSetData, pal)
                                           .ToTexture2D(graphicsDevice);

                    return new TileSet(texture, TileSize);
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

        private Grid<byte> CreateVramImage(IndexedImage[] data)
        {
            if (data.Length == 1)
                return data[0].Image;

            var splitImages = data
                .Select(d => d.Image.Split(TileSize))
                .ToArray();

            var tiles = splitImages
                .SelectMany(s => s.ToArray())
                .Where(p => !p.All((x, y, t) => t == 0))
                .ToArray();

            var combined = tiles.Combine(32);
            return combined;
        }
    }
}
