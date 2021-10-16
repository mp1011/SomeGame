﻿using Microsoft.Xna.Framework;
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
                _font.WriteToLayer(value.ToString().ToUpper().PadRight(20), layer, new Point(1, 2));
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
                var layer = GameSystem.GetLayer(LayerIndex.Interface);
                _font.WriteToLayer(SelectedTheme.ToUpper().PadRight(15), layer, new Point(0, 1));

                UpdateTileChoices(GetCurrentMouseTile());
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

            _editorTileset = StartNew(resourceLoader);
            //_editorTileset = _dataSerializer.Load(TilesetContentKey.Tiles);

            SelectedThemeIndex = new RotatingInt(0, _editorTileset.Themes.Length);
            CurrentTileChoiceMode = TileChoiceMode.Free;

            _saveButton = new UIButton("SAVE", new Point(30, 0), GameSystem.GetLayer(LayerIndex.Interface), _font);            
        }

        private EditorTileSet StartNew(ResourceLoader resourceLoader)
        {
            var tileSet = new EditorTileSet();
            _tileSetService.AddBlock(tileSet, new EditorBlock("ROCK", new Grid<Tile>()));
            _tileSetService.AddBlock(tileSet, new EditorBlock("BUSH", new Grid<Tile>()));
            _tileSetService.AddBlock(tileSet, new EditorBlock("DECOR", new Grid<Tile>()));
            _tileSetService.AddBlock(tileSet, new EditorBlock("FENCE", new Grid<Tile>()));
            return tileSet;
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
