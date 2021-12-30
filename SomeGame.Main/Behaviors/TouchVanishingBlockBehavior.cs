using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class TouchVanishingBlockBehavior : Behavior
    {
        private readonly GameSystem _gameSystem;
        private readonly RamByte _timer;
        private readonly AudioService _audioService;

        public TouchVanishingBlockBehavior(GameSystem gameSystem, AudioService audioService)
        {
            _gameSystem = gameSystem;
            _timer = gameSystem.RAM.DeclareByte();
            _audioService = audioService;
        }
        public override void OnCreated(Actor actor)
        {
            _timer.Set(0);
            SetTiles(actor, true);
            actor.Visible = false;
        }

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            if (_timer == 0 && collisionInfo.Actor != null)
                _timer.Inc();
            else if(_timer > 0)
            {
                actor.Palette = _gameSystem.GetLayer(LayerIndex.FG).Palette.Next();
                actor.Visible = (_timer % 4) <= 1;
                _timer.Inc();
            }

            if(_timer >= 20)
            {
                SetTiles(actor, false);
                actor.Destroy();
            }
        }

        private void SetTiles(Actor actor, bool show)
        {
            var layer = _gameSystem.GetLayer(LayerIndex.FG);

            var tileX = actor.WorldPosition.X.Pixel / _gameSystem.TileSize;
            var tileY = actor.WorldPosition.Y.Pixel / _gameSystem.TileSize;

            if (show)
            {
                actor.Animator.Update(AnimationKey.Idle);
                var animationFrame = actor.Animator.GetCurrentFrame(AnimationKey.Idle);

                int offset = _gameSystem.GetTileOffset(Content.TilesetContentKey.Gizmos) - layer.TileOffset;

                layer.TileMap.SetTile(tileX, tileY, new Tile(animationFrame.TopLeft.Index + offset, TileFlags.Solid));
                layer.TileMap.SetTile(tileX + 1, tileY, new Tile(animationFrame.TopRight.Index + offset, TileFlags.Solid));
                layer.TileMap.SetTile(tileX, tileY + 1, new Tile(animationFrame.BottomLeft.Index + offset, TileFlags.Solid));
                layer.TileMap.SetTile(tileX + 1, tileY + 1, new Tile(animationFrame.BottomRight.Index + offset, TileFlags.Solid));
            }
            else
            {
                layer.TileMap.SetTile(tileX, tileY, new Tile(-1, TileFlags.None));
                layer.TileMap.SetTile(tileX + 1, tileY, new Tile(-1, TileFlags.None));
                layer.TileMap.SetTile(tileX, tileY + 1, new Tile(-1, TileFlags.None));
                layer.TileMap.SetTile(tileX + 1, tileY + 1, new Tile(-1, TileFlags.None));
            }
        }
    }
}
