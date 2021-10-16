using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;
using System.Linq;

namespace SomeGame.Main.Modules
{

    class LevelEditorModule : TileEditorBaseModule
    {
        private readonly TileSetService _tileSetService;
        private UIMultiSelect<string> _tileThemeSelector;
        private EnumMultiSelect<LevelEditorMode> _modeSelector;
        private Font _font;
        private EditorTileSet _editorTileset;
        
        public LevelEditorModule()
        {
            _tileSetService = new TileSetService();
        }

        protected override void Update()
        {
            var foreground = GameSystem.GetLayer(LayerIndex.FG);
            var background = GameSystem.GetLayer(LayerIndex.BG);
            var ui = GameSystem.GetLayer(LayerIndex.Interface);

            if (Input.Right.IsDown())
            {
                background.ScrollX -= 2;
                foreground.ScrollX -= 2;
            }
            if (Input.Left.IsDown())
            {
                background.ScrollX += 2;
                foreground.ScrollX += 2;
            }
            if (Input.Down.IsDown())
            {
                background.ScrollY -= 2;
                foreground.ScrollY -= 2;
            }
            if (Input.Up.IsDown())
            {
                background.ScrollY += 2;
                foreground.ScrollY -= 2;
            }

            _modeSelector.Update(ui, Input);
            if (_tileThemeSelector.Update(ui, Input))
                ShowTilesInTheme();

            switch(_modeSelector.SelectedItem)
            {
                case LevelEditorMode.Free:
                    HandleStandardInput();
                    break;
                case LevelEditorMode.Auto:
                    HandleAutoPlaceTile();
                    break;

            }
            HandleSelectTile();

            if (Input.Start.IsPressed())
                SaveMap(background.TileMap);
        }
        private void SaveMap(TileMap t)
        {
            _dataSerializer.Save(t);
        }

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {           
            if(index == LayerIndex.BG)
            {
                var loaded = _dataSerializer.Load(LevelContentKey.TestLevel);
                layer.TileMap.SetEach((x, y) => loaded.GetTile(x, y));
            }
        }

        protected override void AfterInitialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
        {
            var layer = GameSystem.GetLayer(LayerIndex.Interface);
            _editorTileset = _dataSerializer.LoadEditorTileset(TilesetContentKey.Tiles);
            _font = new Font(GameSystem.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");

            _tileThemeSelector = new UIMultiSelect<string>(layer, _font, _editorTileset.Themes, new Point(0, 0));
            _modeSelector = new EnumMultiSelect<LevelEditorMode>(layer, _font, new Point(0, 1));

            ShowTilesInTheme();
        }

        private void ShowTilesInTheme()
        {
            var layer = GameSystem.GetLayer(LayerIndex.Interface);
            var themeTiles = _editorTileset.Tiles
                                           .Where(p => p.ContainsTheme(_tileThemeSelector.SelectedItem))
                                           .ToArray();
            int i = 0;
            layer.TileMap.SetEach(20, 38, 0, 2, (x, y) =>
            {
                Tile tile;
                if (i < themeTiles.Length)
                    tile = themeTiles[i].Tile;
                else
                    tile = new Tile(-1, TileFlags.None);

                i++;
                return tile;
            });
        }

        private void HandleSelectTile()
        {
            if (!Input.A.IsPressed())
                return;

            var mouseTile = GetCurrentMouseTile();
            if (mouseTile.Y >= 2 || mouseTile.X < 20)
                return;

            var interfaceLayer = GameSystem.GetLayer(LayerIndex.Interface);
            SelectedTile = interfaceLayer.TileMap.GetTile(mouseTile);
        }

        private void HandleAutoPlaceTile()
        {
            int delta = 0;
            if (Input.A.IsPressed())
                delta = 1;
            else if (Input.A.IsPressed())
                delta = -1;
            else
                return;

            var bg = GameSystem.GetLayer(LayerIndex.BG);
            var mouseTile = GetCurrentMouseTile();
            var tileChoices = _tileSetService
                .GetMatchingTiles(_editorTileset, _tileThemeSelector.SelectedItem, bg.TileMap, mouseTile, TileChoiceMode.SemiStrict)
                .Select(p => p.Tile)
                .ToArray();

            if (tileChoices.Length == 0)
                return;

            var currentTile = bg.TileMap.GetTile(mouseTile);

            var currentIndex = tileChoices.GetRotatingIndex(currentTile);
            currentIndex = currentIndex + delta;

            bg.TileMap.SetTile(mouseTile.X, mouseTile.Y, tileChoices[currentIndex]);
        }


        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader)
        {
            using var image = resourceLoader.LoadTexture(TilesetContentKey.Tiles);

            using var fontImage = resourceLoader.LoadTexture(TilesetContentKey.Font);
            return new IndexedTilesetImage[] { image.ToIndexedTilesetImage(), fontImage.ToIndexedTilesetImage() };
        }
    }
}
