using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Editor;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Scenes;
using SomeGame.Main.Services;
using System;
using System.Linq;

namespace SomeGame.Main.Modules
{

    class LevelEditorModule : TileEditorBaseModule
    {
        private readonly Scroller _scroller;
        private readonly LevelContentKey _levelKey;
        private readonly TileSetService _tileSetService;
        private readonly UIBlockSelect _blockSelect;
        private UIMultiSelect<string> _themeSelector;
        private EnumMultiSelect<LevelEditorMode> _modeSelector;
        private Font _font;
        private EditorTileSet _editorTileset;
        private bool _nextClickPlacesTile;

        public LevelEditorModule(LevelContentKey level, ContentManager contentManager, GraphicsDevice graphicsDevice) 
            : base(contentManager, graphicsDevice)
        {
            _levelKey = level;
            _tileSetService = new TileSetService();
            _blockSelect = new UIBlockSelect(HandleBlockAction, HandleBlockMouseMove);
            _scroller = new Scroller(GameSystem);
        }

        protected override void AfterInitialize()
        {
            _scroller.SetCameraBounds(new Rectangle(0, 0, GameSystem.LayerPixelWidth, GameSystem.LayerPixelHeight));
            var layer = GameSystem.GetLayer(LayerIndex.Interface);
            _editorTileset = DataSerializer.LoadEditorTileset(TilesetContentKey.Tiles);
            _font = new Font(GameSystem.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");

            _themeSelector = new UIMultiSelect<string>(layer, _font, _editorTileset.Themes, new Point(0, 0));
            _modeSelector = new EnumMultiSelect<LevelEditorMode>(layer, _font, new Point(0, 1));

            ShowTilesInTheme();
        }

        protected override void Update()
        {
            _scroller.Update();
            var foreground = GameSystem.GetLayer(LayerIndex.FG);
            var background = GameSystem.GetLayer(LayerIndex.BG);
            var ui = GameSystem.GetLayer(LayerIndex.Interface);

            var camera = _scroller.Camera;
            if (Input.Right.IsDown())
                camera.X += 2;            
            if (Input.Left.IsDown())
                camera.X -= 2;
            if (Input.Down.IsDown())
                camera.Y += 2;
            if (Input.Up.IsDown())
                camera.Y -= 2;

            if (_modeSelector.Update(ui, Input))
            {
                _blockSelect.ClearSelection(foreground);
                return;
            }

            if (_themeSelector.Update(ui, Input))
            {
                ShowTilesInTheme();
                return;
            }

            switch(_modeSelector.SelectedItem)
            {
                case LevelEditorMode.Free:

                    if (HandleSelectTile())
                        break;
                       
                    HandleStandardInput();
                    break;
                case LevelEditorMode.Auto:

                    if (HandleSelectTile())
                        _nextClickPlacesTile = true;
                    else if (_nextClickPlacesTile)
                    {
                        HandleStandardInput();
                        if (Input.A.IsPressed() || Input.B.IsPressed())
                        {
                            _nextClickPlacesTile = false;
                            foreground.TileMap.SetEach((x, y) => new Tile(-1, TileFlags.None));
                        }
                    }
                    else
                    {
                        if (Input.B.IsDown())
                        {
                            var mouseTile = GetCurrentMouseTile(LayerIndex.BG);
                            background.TileMap.SetTile(mouseTile.X, mouseTile.Y, new Tile(-1, TileFlags.None));
                            AfterTilePlaced(mouseTile);
                        }

                        HandleAutoPlaceTile();
                    }

                    break;
                case LevelEditorMode.Relate:
                case LevelEditorMode.Move:
                case LevelEditorMode.Copy:
                    _blockSelect.Update(GetCurrentMouseTile(LayerIndex.BG), Input, foreground, background);
                    break;
                case LevelEditorMode.SetSolid:
                    HandleSetSolid(background, foreground);
                    break;
            }

            if (Input.Start.IsPressed())
                SaveMap(background.TileMap);
        }

        private void HandleSetSolid(Layer background, Layer foreground)
        {
            foreground.TileMap.SetEach((x, y) =>
            {
                var bgTile = background.TileMap.GetTile(x, y);
                if (bgTile.IsSolid)
                    return bgTile;
                else
                    return new Tile(-1, TileFlags.None);
            });

            var mouseTile = GetCurrentMouseTile(LayerIndex.BG);
            if(Input.A.IsDown())
            {
                var bgTile = background.TileMap.GetTile(mouseTile.X, mouseTile.Y);
                background.TileMap.SetTile(mouseTile.X, mouseTile.Y, new Tile(bgTile.Index, bgTile.Flags | TileFlags.Solid));
            }

            if (Input.B.IsDown())
            {
                var bgTile = background.TileMap.GetTile(mouseTile.X, mouseTile.Y);
                background.TileMap.SetTile(mouseTile.X, mouseTile.Y, new Tile(bgTile.Index, bgTile.Flags & ~TileFlags.Solid));
            }
        }

        private void SaveMap(TileMap t)
        {
            //todo, level shouldn't be same as background map
            DataSerializer.Save(new TileMap(_levelKey, t.GetGrid()));
        }

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {           
            if(index == LayerIndex.BG)
            {
                var loaded = DataSerializer.LoadTileMap(_levelKey) ;
                if (loaded.GetGrid().Width == 0)
                    loaded = CreateNew(_levelKey);

                layer.TileMap.SetEach((x, y) => loaded.GetTile(x, y));
            }
            else if(index == LayerIndex.FG)
                layer.Palette = PaletteIndex.P3;
        }

        private TileMap CreateNew(LevelContentKey levelContentKey)
        {
            switch(levelContentKey)
            {
                case LevelContentKey.TestLevelBG:
                    return new TileMap(levelContentKey, GameSystem.LayerTileWidth, GameSystem.LayerTileHeight / 2);
                default:
                    throw new Exception($"No default set for level {levelContentKey}");
            }
        }

        private void HandleBlockAction(Point start, Point end)
        {
            switch(_modeSelector.SelectedItem)
            {
                case LevelEditorMode.Relate:
                    RelateBlockRange(start, end);
                    return;
                case LevelEditorMode.Move:
                    MoveBlockRange(start, end);
                    return;
            }
        }

        private void HandleBlockMouseMove(Point start, Point end)
        {
            switch (_modeSelector.SelectedItem)
            {
                case LevelEditorMode.Relate: return;
                case LevelEditorMode.Move: 
                    PreviewBlockMove(start, end);
                    break;
            }
        }

        private void RelateBlockRange(Point start, Point end)
        {
            var bg = GameSystem.GetLayer(LayerIndex.BG);
            var block = new EditorBlock(_themeSelector.SelectedItem,
                bg.TileMap.GetGrid().Extract(start, end));

            _tileSetService.AddBlock(_editorTileset, block);
            DataSerializer.Save(_editorTileset);
        }

        private void MoveBlockRange(Point start, Point end)
        {
            var fg = GameSystem.GetLayer(LayerIndex.FG);
            var bg = GameSystem.GetLayer(LayerIndex.BG);
            var mouseTile = GetCurrentMouseTile(LayerIndex.FG);

            var selection = bg.TileMap
                                .GetGrid()
                                .Extract(start, end);

            bg.TileMap.SetEach(start, end, (x, y) => new Tile(-1, TileFlags.None));

            int selectionWidth = (end.X - start.X) + 1;
            int selectionHeight = (end.Y - start.Y) + 1;

            bg.TileMap.SetEach(mouseTile, new Point(mouseTile.X + selectionWidth, mouseTile.Y + selectionHeight),
                (x, y) => selection[x - mouseTile.X, y - mouseTile.Y]);
        }

        private void PreviewBlockMove(Point start, Point end)
        {
            var fg = GameSystem.GetLayer(LayerIndex.FG);
            var bg = GameSystem.GetLayer(LayerIndex.BG);

            var mouseTile = GetCurrentMouseTile(LayerIndex.FG);

            fg.TileMap.SetEach((x, y) => 
            {
                int selectionX = start.X + (x - mouseTile.X);
                int selectionY = start.Y + (y - mouseTile.Y);
                if (selectionX >= start.X
                    && selectionX <= end.X
                    && selectionY >= start.Y
                    && selectionY <= end.Y)
                {
                    return bg.TileMap.GetTile(selectionX, selectionY);
                }
                else
                    return new Tile(-1, TileFlags.None);                
            });
        }

        private void ShowTilesInTheme()
        {
            var layer = GameSystem.GetLayer(LayerIndex.Interface);
            var themeTiles = _editorTileset.Tiles
                                           .Where(p => p.ContainsTheme(_themeSelector.SelectedItem))
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

        private bool HandleSelectTile()
        {
            if (!Input.A.IsPressed())
                return false;

            var mouseTile = GetCurrentMouseTile(LayerIndex.Interface);
            if (mouseTile.Y >= 2 || mouseTile.X < 20)
                return false;

            var interfaceLayer = GameSystem.GetLayer(LayerIndex.Interface);
            SelectedTile = interfaceLayer.TileMap.GetTile(mouseTile);
            return true;
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
            var mouseTile = GetCurrentMouseTile(LayerIndex.BG);
            var tileChoices = _tileSetService
                .GetMatchingTiles(_editorTileset, _themeSelector.SelectedItem, bg.TileMap, mouseTile, TileChoiceMode.SemiStrict)
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

            return new IndexedTilesetImage[] { 
                image.ToIndexedTilesetImage(), 
                fontImage.ToIndexedTilesetImage()
            };
        }
    }
}
