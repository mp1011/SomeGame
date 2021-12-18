using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;

namespace SomeGame.Main.Services
{
    class TextureBasedRenderService : IRenderService
    {
        private readonly GameSystem _gameSystem;

        public TextureBasedRenderService(GameSystem gameSystem)
        {
            _gameSystem = gameSystem;
        }

        public void DrawFrame(SpriteBatch spriteBatch)
        {
            foreach (var sprite in _gameSystem.GetBackSprites())
                DrawSprite(spriteBatch, sprite);

            DrawLayer(spriteBatch, _gameSystem.GetLayer(LayerIndex.BG));
            DrawLayer(spriteBatch, _gameSystem.GetLayer(LayerIndex.FG));

            foreach (var sprite in _gameSystem.GetFrontSprites())
                DrawSprite(spriteBatch, sprite);

            DrawLayer(spriteBatch, _gameSystem.GetLayer(LayerIndex.Interface));
        }

        private void DrawLayer(SpriteBatch spriteBatch, Layer layer)
        {
            DrawWrappingTileMap(spriteBatch,
                layer.TileMap,
                _gameSystem.GetTileSet(),
                x: layer.ScrollX,
                y: layer.ScrollY,
                tileOffset: layer.TileOffset);
        }

        private void DrawSprite(SpriteBatch spriteBatch, Sprite sprite)
        {
            var tileSet = _gameSystem.GetTileSet();
            DrawWrappingTileMap(spriteBatch, sprite.TileMap, tileSet, sprite.ScrollX, sprite.ScrollY, sprite.TileOffset, sprite.Flip);
        }

        private void DrawWrappingTileMap(SpriteBatch spriteBatch, TileMap tileMap, TileSet tileSet, RotatingInt x, RotatingInt y, int tileOffset, Flip flip = Flip.None)
        {
            tileMap.ForEach((tileX, tileY, tile) =>
            {
                if ((flip & Flip.H) > 0)
                    tileX = tileMap.TilesX - tileX - 1;

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
                    screenX: screenX,
                    screenY: screenY,
                    tile: tile,
                    tileSet: tileSet,
                    tileOffset: tileOffset,
                    flip: flip);
            });
        }


        private void DrawTile(SpriteBatch spriteBatch,
                              int screenX,
                              int screenY,
                              Tile tile,
                              TileSet tileSet,
                              int tileOffset,
                              Flip flip)
        {
            if (tile.Index < 0)
                return;

            var srcRec = tileSet.GetSrcRec(tileOffset + tile.Index);
            var destRec = new Rectangle(x: screenX,
                                        y: screenY,
                                        width: _gameSystem.TileSize,
                                        height: _gameSystem.TileSize);
            if (!destRec.Intersects(_gameSystem.Screen))
                return;

            var effects = SpriteEffects.None;

            if (((flip & Flip.H) > 0) ^ ((tile.Flags & TileFlags.FlipH) > 0))
                effects |= SpriteEffects.FlipHorizontally;

            if (((flip & Flip.V) > 0) ^ ((tile.Flags & TileFlags.FlipV) > 0))
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
