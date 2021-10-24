using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeGame.Main.Models
{
    class ScrollingLayer
    {
        private readonly GameSystem _gameSystem;
        private readonly Layer _layer;
        public TileMap TileMap { get; }

        public Point TopLeftTile { get; private set; }
        private BoundedGameRectangle _scrollBounds;

        public ScrollingLayer(GameSystem gameSystem, TileMap tileMap, Layer layer)
        {
            _gameSystem = gameSystem;
            _layer = layer;
            TileMap = tileMap;
            _scrollBounds = new BoundedGameRectangle(
                x: 0,
                y: 0,
                width: gameSystem.LayerPixelWidth,
                height: gameSystem.LayerPixelHeight,
                maxX: (tileMap.TilesX * gameSystem.TileSize) - gameSystem.Screen.Width,
                maxY: (tileMap.TilesY * gameSystem.TileSize) - gameSystem.Screen.Height);
            SetTopLeftTile(0, 0);
        }

        public GameRectangleWithSubpixels WorldPositionToLayerPosition(GameRectangleWithSubpixels worldPosition)
        {
            var layerPos = worldPosition.Copy();
            layerPos.XPixel -= _scrollBounds.X;
            layerPos.YPixel -= _scrollBounds.Y;
            return layerPos;
        }

        public Rectangle LayerPositionToWorldPosition(Rectangle layerPosition)
        {
            return new Rectangle(
                layerPosition.X + _scrollBounds.X,
                layerPosition.Y + _scrollBounds.Y,
                layerPosition.Width,
                layerPosition.Height);
        }


        private void SetTopLeftTile(int x, int y)
        {
            TopLeftTile = new Point(x,y);

            _scrollBounds.TopLeft = new Point(x * _gameSystem.TileSize, y * _gameSystem.TileSize);

            _layer.TileMap.SetEach((x, y) =>
            {
                int srcX = x + TopLeftTile.X;
                int srcY = y + TopLeftTile.Y;
                return TileMap.GetTile(srcX, srcY);
            });
        }

        public void ScrollLayer(Rectangle cameraPosition)
        {
            if(cameraPosition.Right > _scrollBounds.Right
                || cameraPosition.Bottom > _scrollBounds.Bottom
                || cameraPosition.Left < _scrollBounds.Left
                || cameraPosition.Top < _scrollBounds.Top)
            {
                var tileX = new BoundedInt(cameraPosition.X / _gameSystem.TileSize, TileMap.TilesX);
                var tileY = new BoundedInt(cameraPosition.Y / _gameSystem.TileSize, TileMap.TilesY);

                if (cameraPosition.Right > _scrollBounds.Right)
                    tileX -= 8;

                if (cameraPosition.Left < _scrollBounds.Left)
                    tileX -= _gameSystem.LayerTileWidth / 2;

                if (cameraPosition.Bottom > _scrollBounds.Bottom)
                    tileY += 8;

                if (cameraPosition.Top > _scrollBounds.Top)
                    tileY -= _gameSystem.LayerTileHeight / 2;

                SetTopLeftTile(tileX, tileY);
            }

            if (_layer.ScrollFactor == 100)
            {
                _layer.ScrollX = _layer.ScrollX.Set(-(cameraPosition.X - _scrollBounds.X));
                _layer.ScrollY = _layer.ScrollY.Set(-(cameraPosition.Y - _scrollBounds.Y));
            }
            else if (_layer.ScrollFactor > 0)
            {
                var factor = _layer.ScrollFactor / -100.0;
                _layer.ScrollX = _layer.ScrollX.Set(factor * (cameraPosition.X - _scrollBounds.X));
                _layer.ScrollY = _layer.ScrollY.Set(factor * (cameraPosition.Y - _scrollBounds.Y));
            }
        }
    }
}
