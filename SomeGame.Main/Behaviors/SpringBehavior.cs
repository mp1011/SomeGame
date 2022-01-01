using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System.Diagnostics;

namespace SomeGame.Main.Behaviors
{
    class SpringBehavior : Behavior
    {
        private readonly GameSystem _gameSystem;
        private readonly RamByte _springTimer;
        private readonly RamInt _originalY;
        private readonly AudioService _audioService;
        private readonly Scroller _scroller;

        public SpringBehavior(GameSystem gameSystem, AudioService audioService, Scroller scroller)
        {
            _gameSystem = gameSystem;
            _springTimer = gameSystem.RAM.DeclareByte();
            _originalY = gameSystem.RAM.DeclareInt();
            _audioService = audioService;
            _scroller = scroller;
        }

        protected override void OnCreated()
        {
            SetTiles(Actor,false);
            _originalY.Set(Actor.WorldPosition.Y.Pixel);
        }

        private void SetTiles(Actor actor, bool springActive)
        {
            var layer = _gameSystem.GetLayer(LayerIndex.FG);

            int offset = _gameSystem.GetTileOffset(Content.TilesetContentKey.Gizmos) - layer.TileOffset;

            var tileX = actor.WorldPosition.X.Pixel / _gameSystem.TileSize;
            var tileY = actor.WorldPosition.Y.Pixel / _gameSystem.TileSize;

            int tileIndex = offset + (springActive ? 8 : 1);

            _scroller.SetTile(LayerIndex.FG, tileX, tileY, new Tile(tileIndex, TileFlags.Solid));
            _scroller.SetTile(LayerIndex.FG, tileX + 1, tileY, new Tile(tileIndex + 1, TileFlags.Solid));
        }

        protected override void OnCollision(CollisionInfo collisionInfo)
        {
            if (collisionInfo.Actor != null && _springTimer == 0)
            {
                collisionInfo.Actor.MotionVector.Y.Set(-4);
                _springTimer.Inc();
                SetTiles(Actor, true);
                Actor.Visible = true;
                _audioService.Play(SoundContentKey.Bounce);
            }
        }

        protected override void DoUpdate()
        {
            if (_springTimer == 0)
                Actor.Visible = false;

          
            if(_springTimer > 0)
            {
                _springTimer.Inc();

                int yAdjust = _springTimer % 16;
                if (yAdjust >= 8)
                    yAdjust = 16 - yAdjust;
                Actor.WorldPosition.Y.Set(_originalY - yAdjust);

                if (_springTimer == 60)
                {
                    _springTimer.Set(0);
                    Actor.WorldPosition.Y.Set(_originalY);
                    SetTiles(Actor, false);                    
                }
            }
         
        }
    }
}
