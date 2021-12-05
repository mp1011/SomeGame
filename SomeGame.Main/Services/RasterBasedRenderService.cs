using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Models;
using System.Diagnostics;
using System.Linq;

namespace SomeGame.Main.Services
{
    class RasterBasedRenderService : IRenderService
    {
        private readonly GameSystem _gameSystem;
        private readonly int _spriteSize;
        private Stopwatch _drawTimer = new Stopwatch();
        private int _timingTrials;
        private long _totalDrawTime;

        public RasterBasedRenderService(GameSystem gameSystem)
        {
            _gameSystem = gameSystem;
            _spriteSize = _gameSystem.TileSize * 2;
        }



        public void DrawFrame(SpriteBatch spriteBatch)
        {
            _drawTimer.Restart();

            var bgLayer = _gameSystem.GetLayer(LayerIndex.BG);
            var fgLayer = _gameSystem.GetLayer(LayerIndex.FG);
            var interfaceLayer = _gameSystem.GetLayer(LayerIndex.Interface);

            var backSprites = _gameSystem.GetBackSprites().ToArray();
            var frontSprites = _gameSystem.GetFrontSprites().ToArray();


            for (int y = 0; y < _gameSystem.Screen.Height; y++)
            {
                for (int x = 0; x < _gameSystem.Screen.Width; x++)
                {
                    if (DrawLayerPixel(spriteBatch, x, y, interfaceLayer))
                        continue;
                    if (DrawSpritePixel(spriteBatch, x, y, frontSprites))
                        continue;
                    if (DrawLayerPixel(spriteBatch, x, y, fgLayer))
                        continue;
                    if (DrawLayerPixel(spriteBatch, x, y, bgLayer))
                        continue;
                    if (DrawSpritePixel(spriteBatch, x, y, backSprites))
                        continue;
                }
            }

            _drawTimer.Stop();
            _timingTrials++;
            _totalDrawTime += _drawTimer.ElapsedMilliseconds;
            if(_timingTrials == 100)
            {
                Debug.WriteLine($"Avg Draw Time={_totalDrawTime / _timingTrials} ms");
                _timingTrials = 0;
                _totalDrawTime = 0;
            }
        }

        private bool DrawSpritePixel(SpriteBatch spriteBatch, int screenX, int screenY, Sprite[] sprites)
        {
            foreach(var sprite in sprites)
            {
                if (DrawSpritePixel(spriteBatch, screenX, screenY, sprite))
                    return true;
            }

            return false;
        }

        private bool DrawSpritePixel(SpriteBatch spriteBatch, int screenX, int screenY, Sprite sprite)
        {
            if (screenX < sprite.ScrollX
                || screenX >= sprite.ScrollX + _spriteSize
                || screenY < sprite.ScrollY
                || screenY >= sprite.ScrollY + _spriteSize)
                return false;

            var tileLocation = sprite.TilePointFromScreenPixelPoint(screenX, screenY);
            var tile = sprite.TileMap.GetTile(tileLocation);

            if (tile.Index == -1)
                return false;

            var tileSet = _gameSystem.GetTileSet(sprite.Palette);

            var tileSrcRec = tileSet.GetSrcRec(sprite.TileOffset + tile.Index);

            var tileScreenLocation = new Point(sprite.ScrollX + tileLocation.X * _gameSystem.TileSize,
                                               sprite.ScrollY + tileLocation.Y * _gameSystem.TileSize);
            var tileSrcPt = new Point(tileSrcRec.X + (screenX- tileScreenLocation.X),
                                      tileSrcRec.Y + (screenY- tileScreenLocation.Y));

            var colorByte = _gameSystem.GetVramData(tileSrcPt);
            if (colorByte == 0)
                return false;

            var paletteImage = _gameSystem.GetPaletteTexture(sprite.Palette);

            spriteBatch.Draw(texture: paletteImage,
                 destinationRectangle: new Rectangle(screenX, screenY, 1, 1),
                 sourceRectangle: new Rectangle(colorByte, 0, 1, 1), Color.White);

            return true;

        }


        private bool DrawLayerPixel(SpriteBatch spriteBatch, int screenX, int screenY, Layer layer)
        {            
            var tileLocation = layer.TilePointFromScreenPixelPoint(screenX, screenY);
            var tile = layer.TileMap.GetTile(tileLocation);

            if (tile.Index == -1)
                return false;

            var tileSet = _gameSystem.GetTileSet(layer.Palette);

            var tileSrcRec = tileSet.GetSrcRec(layer.TileOffset + tile.Index);

            var tileScreenLocation = new Point(layer.ScrollX + tileLocation.X * _gameSystem.TileSize,
                                               layer.ScrollY + tileLocation.Y * _gameSystem.TileSize);

            var tileSrcPt = new Point(tileSrcRec.X + (screenX - tileScreenLocation.X), 
                                      tileSrcRec.Y + (screenY - tileScreenLocation.Y));

            var colorByte = _gameSystem.GetVramData(tileSrcPt);
            if (colorByte == 0)
                return false;

            var paletteImage = _gameSystem.GetPaletteTexture(layer.Palette);

            spriteBatch.Draw(texture: paletteImage,
                 destinationRectangle: new Rectangle(screenX, screenY, 1, 1),
                 sourceRectangle: new Rectangle(colorByte, 0, 1, 1), Color.White);

            return true;
        }
    }
}
