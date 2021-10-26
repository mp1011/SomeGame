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
        private TileMap _tileMap;
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
            var layer = GameSystem.GetLayer(LayerIndex.Interface);
            _editorTileset = DataSerializer.LoadEditorTileset(TilesetContentKey.Tiles);

            _editorTileset.Tiles.RemoveAll(t => t.Tile.Index >= GameSystem.GetTileOffset(TilesetContentKey.Font));

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
                SaveMap();
        }

        protected override void AfterTilePlaced(Point location)
        {
            var bgTile = GameSystem.GetLayer(LayerIndex.BG).TileMap.GetTile(location);
            var topLeftTile = _scroller.GetTopLeftTile(LayerIndex.BG);
            _tileMap.SetTile(location.X + topLeftTile.X, location.Y+topLeftTile.Y, bgTile);
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
                AfterTilePlaced(mouseTile);
            }

            if (Input.B.IsDown())
            {
                var bgTile = background.TileMap.GetTile(mouseTile.X, mouseTile.Y);
                background.TileMap.SetTile(mouseTile.X, mouseTile.Y, new Tile(bgTile.Index, bgTile.Flags & ~TileFlags.Solid));
                AfterTilePlaced(mouseTile);
            }
        }

        private void SaveMap()
        {
            DataSerializer.Save(_tileMap);
        }

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {           
            if(index == LayerIndex.BG)
            {
                _tileMap = DataSerializer.LoadTileMap(_levelKey) ;
                if (_tileMap.GetGrid().Width == 0)
                    _tileMap = CreateNew(_levelKey);

                _scroller.SetTileMaps(_tileMap, new TileMap(LevelContentKey.None, _tileMap.TilesX, _tileMap.TilesY));
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
                case LevelContentKey.TestLevel:
                    return new TileMap(levelContentKey, GameSystem.LayerTileWidth*2, GameSystem.LayerTileHeight / 2);
                case LevelContentKey.TestLevel2:
                    return new TileMap(levelContentKey, GameSystem.LayerTileWidth/2, GameSystem.LayerTileHeight / 2);
                case LevelContentKey.LongMapTest:
                    return new TileMap(levelContentKey, GameSystem.LayerTileWidth * 4, GameSystem.LayerTileHeight / 2);
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
                    MoveOrCopyBlockRange(start, end, isCopy:false);
                    return;
                case LevelEditorMode.Copy:
                    MoveOrCopyBlockRange(start, end, isCopy:true);
                    return;
            }
        }

        private void HandleBlockMouseMove(Point start, Point end)
        {
            switch (_modeSelector.SelectedItem)
            {
                case LevelEditorMode.Relate: return;
                case LevelEditorMode.Move:
                case LevelEditorMode.Copy:
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

        private void MoveOrCopyBlockRange(Point start, Point end, bool isCopy)
        {
            var fg = GameSystem.GetLayer(LayerIndex.FG);
            var bg = GameSystem.GetLayer(LayerIndex.BG);
            var mouseTile = GetCurrentMouseTile(LayerIndex.FG);

            var selection = bg.TileMap
                                .GetGrid()
                                .Extract(start, end);

            if (!isCopy)
            {
                bg.TileMap.SetEach(start, end, (x, y) => new Tile(-1, TileFlags.None));
                AfterBlockAction(start, end);
            }

            int selectionWidth = (end.X - start.X) + 1;
            int selectionHeight = (end.Y - start.Y) + 1;

            bg.TileMap.SetEach(mouseTile, new Point(mouseTile.X + selectionWidth, mouseTile.Y + selectionHeight),
                (x, y) => selection[x - mouseTile.X, y - mouseTile.Y]);

            AfterBlockAction(mouseTile, new Point(mouseTile.X + selectionWidth, mouseTile.Y + selectionHeight));
        }

        private void AfterBlockAction(Point upperLeft, Point lowerRight)
        {
            var p = upperLeft;
            while(true)
            {
                AfterTilePlaced(p);
                p = p.Offset(1, 0);
                if(p.X > lowerRight.X)
                    p = new Point(upperLeft.X, p.Y + 1);

                if (p.Y > lowerRight.Y)
                    return;
            }
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
            if (!Input.A.IsPressed())
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
            currentIndex = currentIndex + 1;

            bg.TileMap.SetTile(mouseTile.X, mouseTile.Y, tileChoices[currentIndex]);
            System.Diagnostics.Debug.WriteLine($"Autoplaced Tile: {tileChoices[currentIndex]}");
            AfterTilePlaced(mouseTile);
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
