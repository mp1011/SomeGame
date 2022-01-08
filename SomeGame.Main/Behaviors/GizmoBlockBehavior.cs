using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    abstract class GizmoBlockBehavior : Behavior
    {
        protected readonly GameSystem _gameSystem;
        protected readonly RamByte _timer;
        protected readonly AudioService _audioService;
        protected readonly Scroller _scroller;

        public GizmoBlockBehavior(GameSystem gameSystem, Scroller scroller, AudioService audioService)
        {
            _gameSystem = gameSystem;
            _timer = gameSystem.RAM.DeclareByte();
            _audioService = audioService;
            _scroller = scroller;
        }
        protected override void OnCreated()
        {
            _timer.Set(0);
            Actor.Visible = false;
            OnBlockCreated(Actor);
        }

        protected abstract void OnBlockCreated(Actor actor);

        protected void SetTiles(Actor actor, bool show)
        {
            SetTiles(actor, show ? TileFlags.Solid : TileFlags.None);
        }

        protected void SetTiles(Actor actor, TileFlags flags)
        {
            var layer = _gameSystem.GetLayer(LayerIndex.FG);

            var tileX = actor.WorldPosition.X.Pixel / _gameSystem.TileSize;
            var tileY = actor.WorldPosition.Y.Pixel / _gameSystem.TileSize;

            if (flags != TileFlags.None)
            {
                actor.Animator.Update(actor.CurrentAnimation);
                var animationFrame = actor.Animator.GetCurrentFrame(actor.CurrentAnimation);

                byte offset = (byte)(_gameSystem.GetTileOffset(Content.TilesetContentKey.Gizmos) - layer.TileOffset);

                if ((actor.Flip & Flip.FlipH) > 0)
                    flags |= TileFlags.FlipH;
                if ((actor.Flip & Flip.FlipV) > 0)
                    flags |= TileFlags.FlipV;

                SetTile(tileX, tileY,animationFrame.TopLeft.Index,offset, flags);
                SetTile(tileX + 1, tileY,animationFrame.TopRight.Index,offset, flags);
                SetTile(tileX, tileY + 1,animationFrame.BottomLeft.Index ,offset, flags);
                SetTile(tileX + 1, tileY + 1,animationFrame.BottomRight.Index , offset, flags);
            }
            else
            {
                SetTile(tileX, tileY, 0, 0, TileFlags.None);
                SetTile(tileX + 1, tileY, 0, 0, TileFlags.None);
                SetTile(tileX, tileY + 1, 0, 0, TileFlags.None);
                SetTile(tileX + 1, tileY + 1, 0, 0, TileFlags.None);
            }
        }

        private void SetTile(int tileX, int tileY, byte tileIndex, byte offset, TileFlags flags)
        {
            _scroller.SetTile(LayerIndex.FG, tileX, tileY, new Tile(tileIndex + offset, flags));
        }

        protected bool IsBlockVisible(Actor actor)
        {
            var tileX = actor.WorldPosition.X.Pixel / _gameSystem.TileSize;
            var tileY = actor.WorldPosition.Y.Pixel / _gameSystem.TileSize;

            var layer = _gameSystem.GetLayer(LayerIndex.FG);
            var block = layer.TileMap.GetTile(tileX, tileY);

            return block.IsSolid;
        }
    }
}
