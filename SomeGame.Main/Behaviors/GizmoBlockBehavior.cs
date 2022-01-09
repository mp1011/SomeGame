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
                
                SetTile(tileX, tileY, animationFrame.TopLeft, offset, flags);
                SetTile(tileX + 1, tileY, animationFrame.TopRight, offset, flags);
                SetTile(tileX, tileY + 1, animationFrame.BottomLeft, offset, flags);
                SetTile(tileX + 1, tileY + 1, animationFrame.BottomRight, offset, flags);
            }
            else
            {
                ClearTile(tileX, tileY);
                ClearTile(tileX + 1, tileY);
                ClearTile(tileX, tileY + 1);
                ClearTile(tileX + 1, tileY + 1);
            }
        }

        private void SetTile(int tileX, int tileY, Tile tile, byte offset, TileFlags flags)
        {
            if (tile.Index == 255)
                ClearTile(tileX,tileY);
            else 
                _scroller.SetTile(LayerIndex.FG, tileX, tileY, new Tile(tile.Index + offset, tile.Flags | flags));
        }
        private void ClearTile(int tileX, int tileY)
        {
            _scroller.SetTile(LayerIndex.FG, tileX, tileY, new Tile(255, TileFlags.None));
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
