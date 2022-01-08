using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Editor;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Modules
{
    class ThemeDefinerModule : EditorModule
    {
        private readonly TileSetService _tileSetService;
        private UIMultiSelect<string> _themeSelector;
        private UIButton _save;
        private UIBlockSelect _blockSelect;

        private Font _font;
        private EditorTileSet _editorTileset;
        private readonly DataSerializer _dataSerializer;

        private TilesetContentKey _tileSetKey;

        public ThemeDefinerModule(TilesetContentKey tilesetContentKey, 
            GameStartup gameStartup) : base(gameStartup)
        {
            _tileSetKey = tilesetContentKey;
            _tileSetService = new TileSetService();
            _dataSerializer = new DataSerializer();

            _blockSelect = new UIBlockSelect(AssignTheme,(x,y)=> { });

            SetVram(p1: new TilesetContentKey[] { tilesetContentKey }, p2: new TilesetContentKey[] { TilesetContentKey.Font },
                p3: new TilesetContentKey[] { tilesetContentKey }, p4: new TilesetContentKey[] { });
        }

        protected override void AfterInitialize()
        {
            _font = new Font(GameSystem.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");

            _editorTileset = _dataSerializer.LoadEditorTileset(_tileSetKey) ?? CreateNewTileset();

            _themeSelector = new UIMultiSelect<string>(GameSystem.GetLayer(LayerIndex.Interface), 
                _font, _editorTileset.Themes, new Point(0, 0));

            _save = new UIButton("SAVE", new Point(20, 0), GameSystem.GetLayer(LayerIndex.Interface), _font);

            var highlightPalette = GameSystem.GetPalette(PaletteIndex.P3);
            foreach (var c in highlightPalette.Colors)
                c.Set((byte)c + 4);

        }

        private EditorTileSet CreateNewTileset()
        {
            switch(_tileSetKey)
            {
                case TilesetContentKey.Tiles:
                    var ets = new EditorTileSet(TilesetContentKey.Tiles);
                    ets.Blocks.Add(new EditorBlock("ROCK"));
                    ets.Blocks.Add(new EditorBlock("BUSH"));
                    ets.Blocks.Add(new EditorBlock("FENCE"));
                    ets.Blocks.Add(new EditorBlock("DECOR"));
                    ets.Blocks.Add(new EditorBlock("TREE"));
                    ets.Blocks.Add(new EditorBlock("VINE"));
                    return ets;
                case TilesetContentKey.Tiles1:
                    ets = new EditorTileSet(TilesetContentKey.Tiles1);
                    ets.Blocks.Add(new EditorBlock("BROWNROCK"));
                    ets.Blocks.Add(new EditorBlock("GRAYROCK"));
                    ets.Blocks.Add(new EditorBlock("DECOR"));
                    return ets;
                case TilesetContentKey.Mountains:
                    ets = new EditorTileSet(TilesetContentKey.Mountains);
                    ets.Blocks.Add(new EditorBlock("MOUNTAIN"));
                    ets.Blocks.Add(new EditorBlock("CLOUDS"));
                    return ets;               
                default:
                    throw new Exception("no default set for " + _tileSetKey);
            }
        }

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {
            if(index == LayerIndex.BG)
            {
                int i = 0;
                layer.TileMap.SetEach((x, y) =>
                {
                    if (y <= 2)
                        return new Tile(255, TileFlags.None);
                    else
                        return new Tile(i++, TileFlags.None);
                });
            }
            else if(index == LayerIndex.FG)
            {
                layer.Palette = PaletteIndex.P3;
                layer.TileMap.SetEach((x, y) => new Tile(255, TileFlags.None));                
            }
            else if (index == LayerIndex.Interface)
            {
                layer.TileMap.SetEach((x, y) => new Tile(255, TileFlags.None));
            }
        }

        private Point _mouseTile;
        private Point _lastMouseTile;

        protected override bool Update()
        {
            _themeSelector.Update(GameSystem.GetLayer(LayerIndex.Interface), Input);

            var background = GameSystem.GetLayer(LayerIndex.BG);
            var foreground = GameSystem.GetLayer(LayerIndex.FG);
        
            _mouseTile = background.TilePointFromScreenPixelPoint(Input.MouseX, Input.MouseY);

            if (!_blockSelect.HasSelection)
            {
                foreground.TileMap.SetTile(_mouseTile.X, _mouseTile.Y, background.TileMap.GetTile(_mouseTile));

                if (!_mouseTile.Equals(_lastMouseTile))
                    foreground.TileMap.SetTile(_lastMouseTile.X, _lastMouseTile.Y, new Tile(-1, TileFlags.None));
            }

            _blockSelect.Update(_mouseTile, Input, foreground,background);

            _lastMouseTile = _mouseTile;

            if(_save.Update(GameSystem.GetLayer(LayerIndex.Interface), Input) == UIButtonState.Pressed)
                _dataSerializer.Save(_editorTileset);

            return true;
        }

        private void AssignTheme(Point start, Point end)
        {
            var bg = GameSystem.GetLayer(LayerIndex.BG);
            var block = new EditorBlock(_themeSelector.SelectedItem,
                bg.TileMap.GetGrid().Extract(start, end));

            _editorTileset.Blocks.Add(block);
        }

    }
}
