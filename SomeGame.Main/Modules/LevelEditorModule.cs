using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Editor;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Modules
{

    class LevelEditorModule : TileEditorBaseModule
    {
        private readonly Scroller _scroller;
        private readonly SceneContentKey _scene;
        private readonly LevelContentKey _levelKey;
        private readonly TilesetWithPalette[] _tilesets;
        private readonly TileSetService _tileSetService;
        private readonly UIBlockSelect _blockSelect;
        private readonly PaletteIndex _tilePalette;

        private TileMap _tileMap;
        private UIMultiSelect<string> _themeSelector;
        private UIMultiSelect<ActorId> _objectSelector;
        private UIMultiSelect<CollectibleId> _collectiblesSelector;

        private EnumMultiSelect<LevelEditorMode> _modeSelector;
        private Font _font;
        private EditorTileSet _editorTileset;
        private bool _nextClickPlacesTile;
       
        private SpriteFrame _selectedSprite = new SpriteFrame(new Tile(), new Tile(), new Tile(), new Tile());
        private List<ActorStart> _actorStarts = new List<ActorStart>();
        private List<CollectiblePlacement> _collectiblePlacements = new List<CollectiblePlacement>();


        protected override PaletteKeys PaletteKeys { get; }

        public LevelEditorModule(SceneContentKey scene, LayerIndex editLayer, ContentManager contentManager, GraphicsDevice graphicsDevice) 
            : base(contentManager, graphicsDevice)
        {
            _tileSetService = new TileSetService();
            _blockSelect = new UIBlockSelect(HandleBlockAction, HandleBlockMouseMove);
            _scroller = new Scroller(GameSystem);
            _scene = scene;
            var sceneInfo = DataSerializer.Load(scene);
            PaletteKeys = sceneInfo.PaletteKeys;
            GameSystem.BackgroundColorIndex = sceneInfo.BackgroundColor;
            _tilesets = sceneInfo.VramImages;
            

            if (editLayer == LayerIndex.BG)
            {
                _levelKey = sceneInfo.BgMap.Key;
                _tilePalette = sceneInfo.BgMap.Palette;
                _editorTileset = DataSerializer.LoadEditorTileset(_tilesets[0].TileSet);
            }
            else if (editLayer == LayerIndex.FG)
            {
                _levelKey = sceneInfo.FgMap.Key;
                _tilePalette = sceneInfo.FgMap.Palette;
                _editorTileset = DataSerializer.LoadEditorTileset(_tilesets[1].TileSet);
                _actorStarts = sceneInfo.Actors.ToList();
                _collectiblePlacements = sceneInfo.CollectiblePlacements.ToList();
            }
            else
                throw new ArgumentException("Invalid layer index");
        }

        protected override void AfterInitialize()
        {
            var layer = GameSystem.GetLayer(LayerIndex.Interface);
            _font = new Font(GameSystem.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");

            _objectSelector = new EnumMultiSelect<ActorId>(layer, _font, new Point(0, 0));
            _collectiblesSelector = new EnumMultiSelect<CollectibleId>(layer, _font, new Point(0, 0));
            _themeSelector = new UIMultiSelect<string>(layer, _font, _editorTileset.Themes, new Point(0, 0));
            _modeSelector = new EnumMultiSelect<LevelEditorMode>(layer, _font, new Point(0, 1));

            OnThemeChanged();
        }

        private void UpdateSelectedActor()
        {
            try
            {
                var tileset = ActorFactory.GetTileset(_objectSelector.SelectedItem);
                var offset = GameSystem.GetTileOffset(tileset);

                var spriteFrames = DataSerializer.LoadSpriteFrames(tileset);
                var animations = DataSerializer.LoadAnimations(_objectSelector.SelectedItem);
                _selectedSprite = spriteFrames[animations.Values.First().Frames[0].SpriteFrameIndex];

                GameSystem.GetLayer(LayerIndex.FG).TileOffset = offset;
            }
            catch
            {
                _selectedSprite = new SpriteFrame(new Tile(), new Tile(), new Tile(), new Tile());
            }
        }

        private void UpdateSelectedCollectible()
        {
            try
            {                
                var spriteFrames = DataSerializer.LoadSpriteFrames(TilesetContentKey.Items);
                var animations = DataSerializer.LoadAnimations((ActorId)_collectiblesSelector.SelectedItem);
                _selectedSprite = spriteFrames[animations.Values.First().Frames[0].SpriteFrameIndex];
                GameSystem.GetLayer(LayerIndex.FG).TileOffset = GameSystem.GetTileOffset(TilesetContentKey.Items);
            }
            catch
            {
                _selectedSprite = new SpriteFrame(new Tile(), new Tile(), new Tile(), new Tile());
            }
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
                if (_modeSelector.SelectedItem == LevelEditorMode.Objects)
                {
                    _objectSelector.Refresh(ui);
                    UpdateSelectedActor();
                }
                else if (_modeSelector.SelectedItem == LevelEditorMode.Collectibles)
                {
                    _collectiblesSelector.Refresh(ui);
                    UpdateSelectedCollectible();
                }
                else
                {
                    _themeSelector.Refresh(ui);
                    foreground.TileOffset = GameSystem.GetTileOffset(_editorTileset.Key);
                }

                _blockSelect.ClearSelection(foreground);
                return;
            }

            if (_modeSelector.SelectedItem == LevelEditorMode.Objects)
            {
                if (_objectSelector.Update(ui, Input))
                    UpdateSelectedActor();
            }
            else if (_modeSelector.SelectedItem == LevelEditorMode.Collectibles)
            {
                if (_collectiblesSelector.Update(ui, Input))
                    UpdateSelectedCollectible();
            }
            else
            {
                if (_themeSelector.Update(ui, Input))
                {
                    OnThemeChanged();
                    return;
                }
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
                            _lastAutoplacedTile = GetCurrentMouseTile(LayerIndex.BG);
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
                case LevelEditorMode.Objects:
                    HandlePlaceObjects(background, foreground);
                    break;
                case LevelEditorMode.Collectibles:
                    HandlePlaceCollectible(background, foreground);
                    break;

            }

            if (Input.Start.IsPressed())
                SaveMap();
        }

        protected override void AfterTilePlaced(Point location)
        {
            var tlt = _scroller.GetTopLeftTile(LayerIndex.BG);
            System.Diagnostics.Debug.WriteLine($"Placed tile at {location.X + tlt.X}, {location.Y + tlt.Y}");


            var bgTile = GameSystem.GetLayer(LayerIndex.BG).TileMap.GetTile(location);
            var topLeftTile = _scroller.GetTopLeftTile(LayerIndex.BG);
            _tileMap.SetTile(location.X + topLeftTile.X, location.Y+topLeftTile.Y, bgTile);
        }

        private void HandlePlaceObjects(Layer background, Layer foreground)
        {
            var mouseTile = GetCurrentMouseTile(LayerIndex.BG);
            if (mouseTile.Y < 2)
                return;

            var worldTile = mouseTile.Offset(_scroller.GetTopLeftTile(LayerIndex.FG));

            foreground.TileMap.SetEach((x, y) =>
            {
                if (x == mouseTile.X && y == mouseTile.Y)
                    return _selectedSprite.TopLeft;
                if (x == mouseTile.X+1 && y == mouseTile.Y)
                    return _selectedSprite.TopRight;
                if (x == mouseTile.X && y == mouseTile.Y+1)
                    return _selectedSprite.BottomLeft;
                if (x == mouseTile.X + 1 && y == mouseTile.Y + 1)
                    return _selectedSprite.BottomRight;
                else
                    return new Tile();
            });

            var topLeftScrolledTile = _scroller.GetTopLeftTile(LayerIndex.FG);

            foreach(var actorStart in _actorStarts.Where(p=>p.ActorId == _objectSelector.SelectedItem))
            {
                int worldTileX = actorStart.Position.X / GameSystem.TileSize;
                int worldTileY = actorStart.Position.Y / GameSystem.TileSize;

                if (worldTileX < topLeftScrolledTile.X 
                    || worldTileY < topLeftScrolledTile.Y
                    || worldTileX >= topLeftScrolledTile.X + GameSystem.LayerTileWidth
                    || worldTileY >= topLeftScrolledTile.Y + GameSystem.LayerTileHeight)
                    continue;

                var layerTileX = worldTileX - topLeftScrolledTile.X;
                var layerTileY = worldTileY - topLeftScrolledTile.Y;

                foreground.TileMap.SetEach(layerTileX, layerTileX + 2, layerTileY, layerTileY + 2, (x, y) =>
                {
                    if (x == layerTileX && y == layerTileY)
                        return _selectedSprite.TopLeft;
                    if (x == layerTileX + 1 && y == layerTileY)
                        return _selectedSprite.TopRight;
                    if (x == layerTileX && y == layerTileY + 1)
                        return _selectedSprite.BottomLeft;
                    if (x == layerTileX + 1 && y == layerTileY + 1)
                        return _selectedSprite.BottomRight;
                    else
                        return new Tile();
                });
            }

            if(Input.A.IsPressed())
            {
                _actorStarts.Add(new ActorStart(_objectSelector.SelectedItem, new PixelPoint(worldTile.X * GameSystem.TileSize, worldTile.Y * GameSystem.TileSize)));
            }
            else if(Input.B.IsPressed())
            {
                _actorStarts.RemoveAll(a =>
                {
                    if (a.ActorId != _objectSelector.SelectedItem)
                        return false;

                    int tileX = a.Position.X / GameSystem.TileSize;
                    int tileY = a.Position.Y / GameSystem.TileSize;
                    return worldTile.X >= tileX && worldTile.X <= tileX + 1
                           && worldTile.Y >= tileY && worldTile.Y <= tileY + 1;
                });
            }
        }

        private void HandlePlaceCollectible(Layer background, Layer foreground)
        {
            var mouseTile = GetCurrentMouseTile(LayerIndex.BG);
            if (mouseTile.Y < 2)
                return;

            var worldTile = mouseTile.Offset(_scroller.GetTopLeftTile(LayerIndex.FG));

            foreground.TileMap.SetEach((x, y) =>
            {
                if (x == mouseTile.X && y == mouseTile.Y)
                    return _selectedSprite.TopLeft;
                if (x == mouseTile.X + 1 && y == mouseTile.Y)
                    return _selectedSprite.TopRight;
                if (x == mouseTile.X && y == mouseTile.Y + 1)
                    return _selectedSprite.BottomLeft;
                if (x == mouseTile.X + 1 && y == mouseTile.Y + 1)
                    return _selectedSprite.BottomRight;
                else
                    return new Tile();
            });

            var topLeftScrolledTile = _scroller.GetTopLeftTile(LayerIndex.FG);

            foreach (var collectiblePlacement in _collectiblePlacements.Where(p => p.Id == _collectiblesSelector.SelectedItem))
            {
                int worldTileX = collectiblePlacement.Position.X;
                int worldTileY = collectiblePlacement.Position.Y;

                if (worldTileX < topLeftScrolledTile.X
                    || worldTileY < topLeftScrolledTile.Y
                    || worldTileX >= topLeftScrolledTile.X + GameSystem.LayerTileWidth
                    || worldTileY >= topLeftScrolledTile.Y + GameSystem.LayerTileHeight)
                    continue;

                var layerTileX = worldTileX - topLeftScrolledTile.X;
                var layerTileY = worldTileY - topLeftScrolledTile.Y;

                foreground.TileMap.SetEach(layerTileX, layerTileX + 2, layerTileY, layerTileY + 2, (x, y) =>
                {
                    if (x == layerTileX && y == layerTileY)
                        return _selectedSprite.TopLeft;
                    if (x == layerTileX + 1 && y == layerTileY)
                        return _selectedSprite.TopRight;
                    if (x == layerTileX && y == layerTileY + 1)
                        return _selectedSprite.BottomLeft;
                    if (x == layerTileX + 1 && y == layerTileY + 1)
                        return _selectedSprite.BottomRight;
                    else
                        return new Tile();
                });
            }

            if (Input.A.IsPressed())
            {
                _collectiblePlacements.Add(new CollectiblePlacement(_collectiblesSelector.SelectedItem, new Point(worldTile.X, worldTile.Y)));
            }
            else if (Input.B.IsPressed())
            {
                _collectiblePlacements.RemoveAll(a =>
                {
                    if (a.Id != _collectiblesSelector.SelectedItem)
                        return false;

                    int tileX = a.Position.X;
                    int tileY = a.Position.Y;
                    return worldTile.X >= tileX && worldTile.X <= tileX + 1
                           && worldTile.Y >= tileY && worldTile.Y <= tileY + 1;
                });
            }
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

            var sceneInfo = DataSerializer
                                .Load(_scene)
                                .SetActorsAndCollectibles(_actorStarts, _collectiblePlacements);

            DataSerializer.Save(_scene, sceneInfo);
        }

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {
            if (index == LayerIndex.BG)
            {
                layer.Palette = _tilePalette;
                _tileMap = DataSerializer.LoadTileMap(_levelKey);
                if (_tileMap.GetGrid().Width == 0)
                    _tileMap = CreateNew(_levelKey);

                _scroller.SetTileMaps(_tileMap, new TileMap(LevelContentKey.None, _tileMap.TilesX, _tileMap.TilesY));
                _scroller.Initialize();
            }
            else if (index == LayerIndex.FG)
                layer.Palette = _tilePalette;
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
                case LevelContentKey.TestLevel3:
                    return new TileMap(levelContentKey, GameSystem.LayerTileWidth * 2, GameSystem.LayerTileHeight / 2);
                case LevelContentKey.TestLevel3BG:
                    return new TileMap(levelContentKey, GameSystem.LayerTileWidth / 2, GameSystem.LayerTileHeight / 2);
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

            _editorTileset.Blocks.Add(block);
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

        private void OnThemeChanged()
        {
            var interfaceLayer = GameSystem.GetLayer(LayerIndex.Interface);
            var background = GameSystem.GetLayer(LayerIndex.BG);
            var foreground = GameSystem.GetLayer(LayerIndex.FG);

            background.TileOffset = GameSystem.GetTileOffset(_editorTileset.Key);

            interfaceLayer.TileOffset = background.TileOffset;
            interfaceLayer.Palette = background.Palette;

            foreground.TileOffset = background.TileOffset;
            foreground.Palette = background.Palette == PaletteIndex.P4 ? PaletteIndex.P1 : background.Palette + 1;

            _themeSelector.Refresh(interfaceLayer);
            _modeSelector.Refresh(interfaceLayer);

            var themeTiles = _editorTileset.GetTilesInTheme(_themeSelector.SelectedItem);

            int i = 0;
            interfaceLayer.TileMap.SetEach(20, 38, 0, 2, (x, y) =>
            {
                Tile tile;
                if (i < themeTiles.Length)
                    tile = themeTiles[i];
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

        private Point _lastAutoplacedTile;
        private void HandleAutoPlaceTile()
        {
            if (GetCurrentMouseTile(LayerIndex.Interface).Y < 2)
                return;

            var mouseTile = GetCurrentMouseTile(LayerIndex.BG);
            bool mouseTileChanged = false;

            if(_lastAutoplacedTile != mouseTile)
            {
                _lastAutoplacedTile = mouseTile;
                mouseTileChanged = true;
            }

            if (Input.A.IsPressed() || (Input.A.IsDown() && mouseTileChanged))
                AutoPlaceTile();
        }

        private void AutoPlaceTile()
        {
            var bg = GameSystem.GetLayer(LayerIndex.BG);
            var mouseTile = GetCurrentMouseTile(LayerIndex.BG);


            var tileChoices = _tileSetService
                .GetMatchingTiles(_editorTileset, _themeSelector.SelectedItem, bg.TileMap, mouseTile, TileChoiceMode.SemiStrict)
                .ToArray();

            if (tileChoices.Length == 0)
                return;

            var currentTile = bg.TileMap.GetTile(mouseTile);
            var index = tileChoices.GetIndexAfter(currentTile);          
            bg.TileMap.SetTile(mouseTile.X, mouseTile.Y, tileChoices[index]);
            AfterTilePlaced(mouseTile);
        }


        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader)
        {
            return _tilesets
                        .Select(t =>resourceLoader.LoadTexture(t.TileSet).ToIndexedTilesetImage(GameSystem.GetPalette(t.Palette)))
                        .ToArray();
        }
    }
}
