using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Models;

namespace SomeGame.Main.Services
{
    class RenderService
    {
        private readonly GameSystem _gameSystem;

        public RenderService(GameSystem gameSystem)
        {
            _gameSystem = gameSystem;
        }

        public void DrawLayer(SpriteBatch spriteBatch, Layer layer)
        {
            DrawWrappingTileMap(spriteBatch, 
                layer.TileMap, 
                _gameSystem.GetTileSet(layer.Palette),
                layer.ScrollX, 
                layer.ScrollY);
        }

        private void DrawWrappingTileMap(SpriteBatch spriteBatch, TileMap tileMap, TileSet tileSet, RotatingInt x, RotatingInt y)
        {
            tileMap.ForEach((tileX, tileY, tile) =>
            {
                var screenX = (int)(x + (tileX * _gameSystem.TileSize));
                var screenY = (int)(y + (tileY * _gameSystem.TileSize));

                if (screenX + _gameSystem.TileSize > _gameSystem.LayerPixelWidth)
                    screenX -= _gameSystem.LayerPixelWidth;

                if (screenY + _gameSystem.TileSize > _gameSystem.LayerPixelHeight)
                    screenY -= _gameSystem.LayerPixelHeight;

                if (screenX > _gameSystem.Screen.Width || screenY > _gameSystem.Screen.Height)
                    return;

                DrawTile(
                    spriteBatch: spriteBatch,
                    tileX: tileX,
                    tileY: tileY,
                    screenX: screenX,
                    screenY: screenY,
                    tile: tile,
                    tileSet: tileSet);
            });
        }
   

        private void DrawTile(SpriteBatch spriteBatch, int tileX, int tileY, int screenX, int screenY, 
                             Tile tile, TileSet tileSet)
        {
            if (tile.Index < 0)
                return;

            var srcRec = tileSet.GetSrcRec(tile);
            var destRec = new Rectangle(x: screenX, 
                                        y: screenY,
                                        width: _gameSystem.TileSize,
                                        height: _gameSystem.TileSize);
            if (!destRec.Intersects(_gameSystem.Screen))
                return;

            var effects = SpriteEffects.None;

            if ((tile.Flags & TileFlags.FlipH) > 0)
                effects |= SpriteEffects.FlipHorizontally;

            if ((tile.Flags & TileFlags.FlipV) > 0)
                effects |= SpriteEffects.FlipVertically;

            spriteBatch.Draw(tileSet.Texture, 
                                destinationRectangle: destRec,
                                sourceRectangle: srcRec, 
                                Color.White, 
                                rotation: 0, 
                                origin: Vector2.Zero, 
                                effects: effects, 
                                layerDepth: 0);
        }
    }
}
