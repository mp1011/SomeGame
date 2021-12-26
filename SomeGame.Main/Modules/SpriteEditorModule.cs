using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Editor;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System.Collections.Generic;

namespace SomeGame.Main.Modules
{
    class SpriteEditorModule : TileEditorBaseModule
    {
        private readonly TilesetContentKey _spriteKey;
        private Font _font;
        private UIButton _save;        
        private Point _previewSpriteTile = new Point(0, 4);

        public SpriteEditorModule(TilesetContentKey spriteKey, GameStartup gameStartup)
            : base(gameStartup)
        {
            _spriteKey = spriteKey;

            SetVram(new TilesetContentKey[] { spriteKey },
                new TilesetContentKey[] { TilesetContentKey.Font },
                new TilesetContentKey[] { },
                new TilesetContentKey[] { });
        }

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {
            if(index == LayerIndex.Interface)
            {
                var spriteTiles = GameSystem.GetTileSet();

                int i = 0;
                layer.TileMap.SetEach(0, 30, 0, 4, (x, y) =>
                {
                    Tile tile;
                    if (i < spriteTiles.TotalTiles)
                        tile = new Tile(i, TileFlags.None);
                    else
                        tile = new Tile(-1, TileFlags.None);

                    i++;
                    return tile;
                });
            }
        }

        protected override void AfterInitialize()
        {
            _font = new Font(GameSystem.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");
            _save = new UIButton("SAVE", new Point(30, 0), GameSystem.GetLayer(LayerIndex.Interface), _font);

            var spriteFrames = DataSerializer.LoadSpriteFrames(_spriteKey);
            var bg = GameSystem.GetLayer(LayerIndex.BG);
            Point p = new Point(0, 4);
            foreach(var frame in spriteFrames)
            {
                bg.TileMap.SetTile(p.X, p.Y, frame.TopLeft);
                bg.TileMap.SetTile(p.X+1, p.Y, frame.TopRight);
                bg.TileMap.SetTile(p.X, p.Y+1, frame.BottomLeft);
                bg.TileMap.SetTile(p.X + 1, p.Y + 1, frame.BottomRight);

                p = p.Offset(2, 0);
                if(p.X >= 30)
                    p = new Point(0, p.Y + 2);
            }

            PreviewSprite();
        }

        //protected override IndexedTilesetImage[] LoadVramImages()
        //{
        //    using var image = resourceLoader.LoadTexture(_spriteKey);
        //    var tileset = image.ToIndexedTilesetImage();

        //    using var fontImage = resourceLoader.LoadTexture(TilesetContentKey.Font);
        //    return new IndexedTilesetImage[] { image.ToIndexedTilesetImage(), fontImage.ToIndexedTilesetImage() };
        //}

        protected override bool Update()
        {
            var layer = GameSystem.GetLayer(LayerIndex.Interface);
            if (_save.Update(layer, Input) == UIButtonState.Pressed)
                Save();
            else
            {
                HandleFlip();
                HandleStandardInput();
                HandleSelectTile();
                HandleMovePreview();

                PreviewSprite();
            }

            return true;
        }

        private void HandleFlip()
        {
            if (!Input.A.IsPressed())
                return;

            var background = GameSystem.GetLayer(LayerIndex.BG);
            var mouseTile = GetCurrentMouseTile(LayerIndex.BG);
            var currentTile = background.TileMap.GetTile(mouseTile);

            if(currentTile == SelectedTile)
            {
                switch(SelectedTile.Flags)
                {
                    case TileFlags.None:
                        SelectedTile = new Tile(SelectedTile.Index, TileFlags.FlipH);
                        break;
                    case TileFlags.FlipH:
                        SelectedTile = new Tile(SelectedTile.Index, TileFlags.FlipV);
                        break;
                    case TileFlags.FlipV:
                        SelectedTile = new Tile(SelectedTile.Index, TileFlags.FlipHV);
                        break;
                    case TileFlags.FlipHV:
                        SelectedTile = new Tile(SelectedTile.Index, TileFlags.None);
                        break;
                }
            }
        }

        private void HandleSelectTile()
        {
            if (!Input.A.IsPressed())
                return;

            var mouseTile = GetCurrentMouseTile(LayerIndex.BG);
            if (mouseTile.Y >= 4)
                return;

            var interfaceLayer = GameSystem.GetLayer(LayerIndex.Interface);
            SelectedTile = interfaceLayer.TileMap.GetTile(mouseTile);
        }

        private void HandleMovePreview()
        {
            var offset = Input.PressedDirectionVector.Scale(2);
            if (offset.X == 0 && offset.Y == 0)
                return;

            _previewSpriteTile = _previewSpriteTile.Offset(offset);
            if (_previewSpriteTile.X < 0)
                _previewSpriteTile = new Point(0, _previewSpriteTile.Y);
            if (_previewSpriteTile.Y < 4)
                _previewSpriteTile = new Point(_previewSpriteTile.X, 4);
        }

        private SpriteFrame[] CreateSpriteFrames()
        {
            var bg = GameSystem.GetLayer(LayerIndex.BG);
            List<SpriteFrame> frames = new List<SpriteFrame>();

            bg.TileMap.ForEach(new Point(0, 4), new Point(40, 40), (x, y,t) =>
              {
                  if ((x % 2) == 1 || (y % 2) == 1)
                      return;

                  var spriteFrame = new SpriteFrame(bg.TileMap[x, y], bg.TileMap[x + 1, y], bg.TileMap[x, y + 1], bg.TileMap[x + 1, y + 1]);
                  if(spriteFrame.TopLeft.Index != -1 
                    || spriteFrame.TopRight.Index != -1
                    || spriteFrame.BottomLeft.Index != -1
                    || spriteFrame.BottomRight.Index != -1)
                      frames.Add(spriteFrame);
              });

            return frames.ToArray();
        }

        private void Save()
        {
            var frames = CreateSpriteFrames();
            DataSerializer.Save(_spriteKey, frames);
        }

        private void PreviewSprite()
        {
            var bg = GameSystem.GetLayer(LayerIndex.BG);
            var fg = GameSystem.GetLayer(LayerIndex.FG);

            bg.TileMap.ForEach(_previewSpriteTile, _previewSpriteTile.Offset(2, 2), (x, y, t) =>
             {
                 fg.TileMap.SetTile(20 + (x - _previewSpriteTile.X),
                                    20 + (y - _previewSpriteTile.Y),
                                    t);
             });
        }
    }
}
