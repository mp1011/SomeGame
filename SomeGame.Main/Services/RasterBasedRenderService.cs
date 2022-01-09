using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Models;
using SomeGame.Main.RasterEffects;
using SomeGame.Main.RasterInterrupts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SomeGame.Main.Services
{

    class RasterBasedRenderService : IRenderService
    {
        private readonly GameSystem _gameSystem;
        private readonly int _spriteSize;
        private Texture2D _canvas;
        private Stopwatch _drawTimer = new Stopwatch();
        private int _timingTrials;
        private long _totalDrawTime;
        private int _rasterX;
        private int _rasterY;
        private Color[] _screenData;
        private int _dataIndex;
        private RamPalette _currentPalette;
        private PaletteIndex _currentPaletteIndex;
        private List<IRasterInterrupt> _rasterInterrupts = new List<IRasterInterrupt>();
        private int _drawFrame;
        private IRasterEffect _currentEffect;
        public RasterBasedRenderService(GameSystem gameSystem, GraphicsDevice graphicsDevice)
        {
            _gameSystem = gameSystem;
            _spriteSize = _gameSystem.TileSize * 2;
            _canvas = new Texture2D(graphicsDevice, _gameSystem.Screen.Width, _gameSystem.Screen.Height);
            _screenData = new Color[_gameSystem.Screen.Width * _gameSystem.Screen.Height];
        }

        public void SetEffect(IRasterEffect rasterEffect)
        {
            _currentEffect = rasterEffect;
        }

        public void ClearInterrupts()
        {
            _rasterInterrupts.Clear();
        }

        public void AddInterrupt(IRasterInterrupt rasterInterrupt)
        {
            if(_rasterInterrupts.Any())
            {
                var previousLine = _rasterInterrupts.Last().VerticalLine;
                if (previousLine >= rasterInterrupt.VerticalLine)
                    throw new Exception("Illegal scanline for interrupt");
            }

            _rasterInterrupts.Add(rasterInterrupt);
        }

        public void DrawFrame(SpriteBatch spriteBatch)
        {
            _dataIndex = 0;
            _drawTimer.Restart();

            _currentPalette = _gameSystem.GetPalette(PaletteIndex.P1);
            _currentPaletteIndex = PaletteIndex.P1;

            var bgLayer = _gameSystem.GetLayer(LayerIndex.BG);
            var fgLayer = _gameSystem.GetLayer(LayerIndex.FG);
            var interfaceLayer = _gameSystem.GetLayer(LayerIndex.Interface);

            var backSprites = _gameSystem.GetBackSprites().ToArray();
            var frontSprites = _gameSystem.GetFrontSprites().ToArray();

            int nextInterruptIndex = 0;

            for (int rasterY = 0; rasterY < _gameSystem.Screen.Height; rasterY++)
            {
                if(nextInterruptIndex < _rasterInterrupts.Count && rasterY == _rasterInterrupts[nextInterruptIndex].VerticalLine)
                {
                    _rasterInterrupts[nextInterruptIndex].Handle(_drawFrame);
                    nextInterruptIndex++;
                }

                for (int rasterX = 0; rasterX < _gameSystem.Screen.Width; rasterX++)
                {
                    if (_currentEffect == null)
                    {
                        _rasterX = rasterX;
                        _rasterY = rasterY;
                    }
                    else
                    {
                        _rasterX = _currentEffect.AdjustX(rasterX, rasterY, _drawFrame);
                        _rasterY = _currentEffect.AdjustY(rasterX, rasterY, _drawFrame);
                    }

                    if (DrawLayerPixel(interfaceLayer))
                        continue;
                    if (DrawSpritePixel(frontSprites))
                        continue;
                    if (DrawLayerPixel(fgLayer))
                        continue;
                    if (DrawLayerPixel(bgLayer))
                        continue;
                    if (DrawSpritePixel(backSprites))
                        continue;

                    DrawPixel(PaletteIndex.P1, _gameSystem.BackgroundColorIndex);
                }
            }

            _canvas.SetData(_screenData,0,_screenData.Length);
            spriteBatch.Draw(_canvas, new Vector2(0, 0), Color.White);

            _drawTimer.Stop();
            _timingTrials++;
            _totalDrawTime += _drawTimer.ElapsedMilliseconds;
            if(_timingTrials == 100)
            {
                Debug.WriteLine($"Avg Draw Time={_totalDrawTime / _timingTrials} ms");
                _timingTrials = 0;
                _totalDrawTime = 0;
            }

            _drawFrame++;
        }

        private bool DrawSpritePixel(Sprite[] sprites)
        {
            foreach(var sprite in sprites)
            {
                if (DrawSpritePixel(sprite))
                    return true;
            }

            return false;
        }

        private bool DrawSpritePixel(Sprite sprite)
        {
            int screenX = _rasterX;
            int screenY = _rasterY;

            if (sprite.ScrollX + _spriteSize > sprite.ScrollX
                && screenX < sprite.ScrollX)
                return false;

            if (sprite.ScrollY + _spriteSize > sprite.ScrollY
                && screenY < sprite.ScrollY)
                return false;

            if (screenX >= sprite.ScrollX + _spriteSize
                || screenY >= sprite.ScrollY + _spriteSize)
                return false;

            var tileLocation = sprite.TilePointFromScreenPixelPoint(screenX, screenY);
            Tile tile;

            if ((sprite.Flip & Flip.H & Flip.V) > 0)
                tile = sprite.TileMap.GetTile(new Point(1-tileLocation.X,1-tileLocation.Y));
            else if ((sprite.Flip & Flip.H) > 0)
                tile = sprite.TileMap.GetTile(new Point(1 - tileLocation.X, tileLocation.Y));
            else if ((sprite.Flip & Flip.V) > 0)
                tile = sprite.TileMap.GetTile(new Point(tileLocation.X, 1 - tileLocation.Y));
            else
                tile = sprite.TileMap.GetTile(tileLocation);

            if (tile.Index == 255)
                return false;

            bool flipX = false;
            bool flipY = false;

            if (((sprite.Flip & Flip.H) > 0) ^ ((tile.Flags & TileFlags.FlipH) > 0))
                flipX = true;

            if (((sprite.Flip & Flip.V) > 0) ^ ((tile.Flags & TileFlags.FlipV) > 0))
                flipY = true;

            var tileSrcRec = _gameSystem.GetPatternTableBlock((byte)(sprite.TileOffset + tile.Index));

            int tileScreenX = sprite.ScrollX + tileLocation.X * _gameSystem.TileSize;
            int tileScreenY = sprite.ScrollY + tileLocation.Y * _gameSystem.TileSize;

            if (tileScreenX + _gameSystem.TileSize >= _gameSystem.LayerPixelWidth)
                tileScreenX -= _gameSystem.LayerPixelWidth;

            if (tileScreenY + _gameSystem.TileSize >= _gameSystem.LayerPixelHeight)
                tileScreenY -= _gameSystem.LayerPixelHeight;

            int tileSrcX; int tileSrcY;

            if (flipX)
                tileSrcX = tileSrcRec.Right - (screenX - tileScreenX) -1;
            else
                tileSrcX = tileSrcRec.X + (screenX - tileScreenX);

            if (flipY)
                tileSrcY = tileSrcRec.Bottom - (screenY - tileScreenY) -1;
            else
                tileSrcY = tileSrcRec.Y + (screenY - tileScreenY);


            var colorByte = _gameSystem.GetPatternTableData(new Point(tileSrcX, tileSrcY));
            if (colorByte == 0)
                return false;

            DrawPixel(sprite.Palette, colorByte);
            return true;
        }

        private bool DrawLayerPixel(Layer layer)
        {
            int screenX = _rasterX;
            int screenY = _rasterY;

            var tileLocation = layer.TilePointFromScreenPixelPoint(screenX, screenY);
            var tile = layer.TileMap.GetTile(tileLocation);

            if (tile == null || tile.Index == 255)
                return false;

            var tileSrcRec = _gameSystem.GetPatternTableBlock((byte)(layer.TileOffset + tile.Index));

            var tileScreenX = layer.ScrollX.Value + tileLocation.X * _gameSystem.TileSize;
            var tileScreenY = layer.ScrollY.Value + tileLocation.Y * _gameSystem.TileSize;

            if (tileScreenX > _gameSystem.Screen.Width)
                tileScreenX -= _gameSystem.LayerPixelWidth;

            if (tileScreenY > _gameSystem.Screen.Height)
                tileScreenX -= _gameSystem.LayerPixelHeight;

            int tileSrcX; int tileSrcY;

            if ((tile.Flags & TileFlags.FlipH) > 0)
                tileSrcX = tileSrcRec.Right - (screenX - tileScreenX)-1;
            else
                tileSrcX = tileSrcRec.X + (screenX - tileScreenX);

            if ((tile.Flags & TileFlags.FlipV) > 0)
                tileSrcY = tileSrcRec.Bottom - (screenY - tileScreenY)-1;
            else
                tileSrcY = tileSrcRec.Y + (screenY - tileScreenY);
        

            var colorByte = _gameSystem.GetPatternTableData(new Point(tileSrcX,tileSrcY));
            if (colorByte == 0)
                return false;

            DrawPixel(layer.Palette, colorByte);

            return true;
        }
    
        private void DrawPixel(PaletteIndex palette, byte color)
        {
            if (palette != _currentPaletteIndex)
            {
                _currentPalette = _gameSystem.GetPalette(palette);
                _currentPaletteIndex = palette;
            }

            _screenData[_dataIndex++] = _currentPalette[color];
        }
    }
}
