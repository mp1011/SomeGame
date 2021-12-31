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
        public override void OnCreated(Actor actor)
        {
            _timer.Set(0);
            actor.Visible = false;
            OnBlockCreated(actor);
        }

        protected abstract void OnBlockCreated(Actor actor);

        protected void SetTiles(Actor actor, bool show)
        {
            var layer = _gameSystem.GetLayer(LayerIndex.FG);

            var tileX = actor.WorldPosition.X.Pixel / _gameSystem.TileSize;
            var tileY = actor.WorldPosition.Y.Pixel / _gameSystem.TileSize;

            if (show)
            {
                actor.Animator.Update(AnimationKey.Idle);
                var animationFrame = actor.Animator.GetCurrentFrame(AnimationKey.Idle);

                int offset = _gameSystem.GetTileOffset(Content.TilesetContentKey.Gizmos) - layer.TileOffset;

                _scroller.SetTile(LayerIndex.FG, tileX, tileY, new Tile(animationFrame.TopLeft.Index + offset, TileFlags.Solid));
                _scroller.SetTile(LayerIndex.FG, tileX + 1, tileY, new Tile(animationFrame.TopRight.Index + offset, TileFlags.Solid));
                _scroller.SetTile(LayerIndex.FG, tileX, tileY + 1, new Tile(animationFrame.BottomLeft.Index + offset, TileFlags.Solid));
                _scroller.SetTile(LayerIndex.FG, tileX + 1, tileY + 1, new Tile(animationFrame.BottomRight.Index + offset, TileFlags.Solid));
            }
            else
            {
                _scroller.SetTile(LayerIndex.FG, tileX, tileY, new Tile(-1, TileFlags.None));
                _scroller.SetTile(LayerIndex.FG, tileX + 1, tileY, new Tile(-1, TileFlags.None));
                _scroller.SetTile(LayerIndex.FG, tileX, tileY + 1, new Tile(-1, TileFlags.None));
                _scroller.SetTile(LayerIndex.FG, tileX + 1, tileY + 1, new Tile(-1, TileFlags.None));
            }
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
