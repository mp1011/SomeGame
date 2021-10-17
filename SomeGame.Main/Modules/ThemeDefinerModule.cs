using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Editor;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Modules
{
    class ThemeDefinerModule : GameModuleBase
    {
        private readonly TileSetService _tileSetService;
        private UIMultiSelect<string> _themeSelector;
        private UIButton _save;
        private UIBlockSelect _blockSelect;

        private Font _font;
        private EditorTileSet _editorTileset;
        private readonly DataSerializer _dataSerializer;

        private ImageContentKey _imageKey;
        private TilesetContentKey _tileSetKey;
        private IndexedImage _image;

        public ThemeDefinerModule(ImageContentKey imageKey, TilesetContentKey tilesetContentKey)
        {
            _imageKey = imageKey;
            _tileSetKey = tilesetContentKey;
            _tileSetService = new TileSetService();
            _dataSerializer = new DataSerializer();

            _blockSelect = new UIBlockSelect(AssignTheme,(x,y)=> { });
        }

        protected override void AfterInitialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
        {
            _font = new Font(GameSystem.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");

            _editorTileset = _dataSerializer.LoadEditorTileset(_tileSetKey) ?? CreateNewTileset();

           // var t = _editorTileset.GetOrAddTile(new Tile(-2, TileFlags.None));
           // t.Themes.Add("TREE");
          //  t.Themes.Add("VINE");

            _themeSelector = new UIMultiSelect<string>(GameSystem.GetLayer(LayerIndex.Interface), 
                _font, _editorTileset.Themes, new Point(0, 0));

            _save = new UIButton("SAVE", new Point(20, 0), GameSystem.GetLayer(LayerIndex.Interface), _font);

        }

        private EditorTileSet CreateNewTileset()
        {
            switch(_tileSetKey)
            {
                case TilesetContentKey.Tiles:

                    var ets = new EditorTileSet();
                    var tile = ets.GetOrAddTile(new Tile(-1, TileFlags.None));
                    tile.Themes.Add("ROCK");
                    tile.Themes.Add("BUSH");
                    tile.Themes.Add("FENCE");
                    tile.Themes.Add("DECOR");
                    tile.Themes.Add("TREE");
                    tile.Themes.Add("VINE");

                    return ets;
                default:
                    throw new Exception("no default set for " + _tileSetKey);
            }
        }

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {
            if(index == LayerIndex.BG)
            {
                var grid = _tileSetService.CreateTileMapFromImageAndTileset(_image, GameSystem.GetTileSet(PaletteIndex.P1));
                layer.TileMap.SetEach(0,grid.Width,1, grid.Height+1, (x, y) => grid[x, y-1]);
            }
            else if(index == LayerIndex.FG)
            {
                layer.Palette = PaletteIndex.P3;
            }
        }

        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader)
        {
            _image = resourceLoader.LoadTexture(_imageKey)
                .ToIndexedImage();

            return new IndexedTilesetImage[] 
            { 
                resourceLoader.LoadTexture(_tileSetKey).ToIndexedTilesetImage(),
                resourceLoader.LoadTexture(TilesetContentKey.Font).ToIndexedTilesetImage()
            };
        }



        private Point _mouseTile;
        private Point _lastMouseTile;

        protected override void Update()
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
        }

        private void AssignTheme(Point start, Point end)
        {
            var bg = GameSystem.GetLayer(LayerIndex.BG);
            var block = new EditorBlock(_themeSelector.SelectedItem,
                bg.TileMap.GetGrid().Extract(start, end));

            _tileSetService.AddBlock(_editorTileset, block);
        }

      
    }
}
