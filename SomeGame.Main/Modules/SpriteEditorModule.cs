using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System.Collections.Generic;

namespace SomeGame.Main.Modules
{
    class SpriteEditorModule : TileEditorBaseModule
    {
        private readonly DataSerializer _dataSerializer;
        private Font _font;
        private UIButton _save;

        public SpriteEditorModule()
        {
            _dataSerializer = new DataSerializer();
        }

        protected override void InitializeLayer(LayerIndex index, Layer layer)
        {
            if(index == LayerIndex.Interface)
            {
                var spriteTiles = GameSystem.GetTileSet(PaletteIndex.P1);

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

        protected override void AfterInitialize(ResourceLoader resourceLoader, Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice)
        {
            _font = new Font(GameSystem.GetTileOffset(TilesetContentKey.Font), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-X!©");
            _save = new UIButton("SAVE", new Point(30, 0), GameSystem.GetLayer(LayerIndex.Interface), _font);

            var spriteFrames = _dataSerializer.LoadSpriteFrames(TilesetContentKey.Hero);
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
        }

        protected override IndexedTilesetImage[] LoadVramImages(ResourceLoader resourceLoader)
        {
            using var image = resourceLoader.LoadTexture(TilesetContentKey.Hero);
            var tileset = image.ToIndexedTilesetImage();

            using var fontImage = resourceLoader.LoadTexture(TilesetContentKey.Font);
            return new IndexedTilesetImage[] { image.ToIndexedTilesetImage(), fontImage.ToIndexedTilesetImage() };
        }

        protected override void Update()
        {
            var layer = GameSystem.GetLayer(LayerIndex.Interface);
            if (_save.Update(layer, Input) == UIButtonState.Pressed)
                Save();
            else
            {
                HandleStandardInput();
                HandleSelectTile();
            }
             
        }

        private void HandleSelectTile()
        {
            if (!Input.A.IsPressed())
                return;

            var mouseTile = GetCurrentMouseTile();
            if (mouseTile.Y >= 4)
                return;

            var interfaceLayer = GameSystem.GetLayer(LayerIndex.Interface);
            SelectedTile = interfaceLayer.TileMap.GetTile(mouseTile);
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
            _dataSerializer.Save(TilesetContentKey.Hero, frames);
        }
    }
}
