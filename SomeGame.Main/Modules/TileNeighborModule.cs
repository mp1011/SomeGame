using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Modules
{
    class TileNeighborModule : TileEditorBaseModule
    {
        private UIButton _saveButton;
        private TileChoiceMode _tileChoiceMode = TileChoiceMode.Free;

        private TileChoiceMode CurrentTileChoiceMode
        {
            get => _tileChoiceMode;
            set
            {
                _tileChoiceMode = value;
                var layer = GameSystem.GetLayer(LayerIndex.Interface);

                var fontTiles = _font.FromString(value.ToString().ToUpper().PadRight(20));
                layer.TileMap.SetEach(0, fontTiles.Length, 1, 2, (x, y) => fontTiles[x]);
            }
        }

        private readonly TileSetService _tileSetService;
        private EditorBlock[] _blocks;
        private EditorTile[] _tileChoices;
        private EditorTileSet _editorTileset;
        private Font _font;



        private string SelectedTheme => _editorTileset.Themes[_selectedThemeIndex];

        private RotatingInt _selectedThemeIndex;
        private RotatingInt SelectedThemeIndex
        {
            get => _selectedThemeIndex;
            set
            {
                _selectedThemeIndex = value;
                var fontTiles = _font.FromString(SelectedTheme.ToUpper().PadRight(15));

                var layer = GameSystem.GetLayer(LayerIndex.Interface);
                layer.TileMap.SetEach(0, fontTiles.Length, 0, 1, (x, y) => fontTiles[x]);

                UpdateTileChoices(_lastMouseTile);
            }
        }


        private RotatingInt _selectedTileIndex;
        private RotatingInt SelectedTileIndex
        {
            get => _selectedTileIndex;
            set
            {
                if (_tileChoices.Length == 0)
                    return;

                _selectedTileIndex = value;
                SelectedTile = _tileChoices[_selectedTileIndex].Tile;
            }
        }

        public TileNeighborModule(TileSetService tileSetService)
        {
            _tileSetService = tileSetService;
        }
        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {
           
        }

        protected override void AfterInitialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
        {
            _font = new Font(GameSystem.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");
            
            _editorTileset = _dataSerializer.Load(TilesetContentKey.Tiles);

            SelectedThemeIndex = new RotatingInt(0, _editorTileset.Themes.Length);
            CurrentTileChoiceMode = TileChoiceMode.Free;

            _saveButton = new UIButton("SAVE", new Point(30, 0), GameSystem.GetLayer(LayerIndex.Interface), _font);            
        }

        private EditorTileSet StartNew(ResourceLoader resourceLoader)
        {
            List<Texture2D> blockTextures = new List<Texture2D>();
            blockTextures.AddRange(resourceLoader.LoadBlocks("bluerock"));
            blockTextures.AddRange(resourceLoader.LoadBlocks("bush"));
            //redo this blockTextures.AddRange(resourceLoader.LoadBlocks("crate"));
            blockTextures.AddRange(resourceLoader.LoadBlocks("chest"));
            blockTextures.AddRange(resourceLoader.LoadBlocks("fence"));
            blockTextures.AddRange(resourceLoader.LoadBlocks("flower"));
            blockTextures.AddRange(resourceLoader.LoadBlocks("grass"));
            blockTextures.AddRange(resourceLoader.LoadBlocks("ladder"));
            blockTextures.AddRange(resourceLoader.LoadBlocks("rock"));
            blockTextures.AddRange(resourceLoader.LoadBlocks("sign"));
            blockTextures.AddRange(resourceLoader.LoadBlocks("smallrock"));
            blockTextures.AddRange(resourceLoader.LoadBlocks("smallcrate"));
            //redo this blockTextures.AddRange(resourceLoader.LoadBlocks("spikes"));
            blockTextures.AddRange(resourceLoader.LoadBlocks("spring"));
            blockTextures.AddRange(resourceLoader.LoadBlocks("tree"));
            blockTextures.AddRange(resourceLoader.LoadBlocks("vines"));
            blockTextures.AddRange(resourceLoader.LoadBlocks("water"));

            var tileset = GameSystem.GetTileSet(PaletteIndex.P1);
            _blocks = blockTextures
                                .Select(b =>
                                {
                                    var theme = b.Name.Split('\\')[1].Split('_')[0];
                                    var indexedImage = b.ToIndexedImage(GameSystem.GetPalette(PaletteIndex.P1));
                                    var grid = _tileSetService.CreateTileMapFromImageAndTileset(indexedImage, tileset);
                                    return new EditorBlock(theme, grid);
                                })
                                .ToArray();

            return _tileSetService.BuildEditorTileset(_blocks);
        }

        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader)
        {
            using var image = resourceLoader.LoadTexture(TilesetContentKey.Tiles);
            var tileset = image.ToIndexedTilesetImage();

            using var fontImage = resourceLoader.LoadTexture(TilesetContentKey.Font);
            return new IndexedTilesetImage[] { image.ToIndexedTilesetImage(), fontImage.ToIndexedTilesetImage() };
        }

        protected override void Update()
        {
            if(Input.Up.IsPressed())
                SelectedThemeIndex--;
            else if(Input.Down.IsPressed())
                SelectedThemeIndex++;

            if (Input.Right.IsPressed())
                SelectedTileIndex++;
            else if (Input.Left.IsPressed())
                SelectedTileIndex--;


            if(Input.Start.IsPressed())
            {
                var newChoiceMode = CurrentTileChoiceMode + 1;
                if (newChoiceMode > TileChoiceMode.Strict)
                    newChoiceMode = TileChoiceMode.Free;

                CurrentTileChoiceMode = newChoiceMode;
            }

            var saveBtnState = _saveButton.Update(GameSystem.GetLayer(LayerIndex.Interface), Input);
            if (saveBtnState == UIButtonState.None)
                HandleStandardInput();
            else if (saveBtnState == UIButtonState.Pressed)
                Save();
        }

        protected override void MouseTileChanged(Point mouseTile)
        {
            UpdateTileChoices(mouseTile);
        }

        private void UpdateTileChoices(Point mouseTile)
        {
            var bg = GameSystem.GetLayer(LayerIndex.BG);
            _tileChoices = _tileSetService.GetMatchingTiles(_editorTileset, SelectedTheme, bg.TileMap, mouseTile, CurrentTileChoiceMode);

            int currentIndex = SelectedTileIndex;
            SelectedTileIndex = new RotatingInt(currentIndex, _tileChoices.Length);

            System.Diagnostics.Debug.WriteLine($"{_tileChoices.Length} choices available at {mouseTile}");
        }

        private void Save()
        {
            var bg = GameSystem.GetLayer(LayerIndex.BG);
            _tileSetService.AddTileRelationsFromTileMap(_editorTileset, bg.TileMap);
            _dataSerializer.Save(_editorTileset);
        }

    }
}
