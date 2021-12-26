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

        public SpringBehavior(GameSystem gameSystem, AudioService audioService)
        {
            _gameSystem = gameSystem;
            _springTimer = gameSystem.RAM.DeclareByte();
            _originalY = gameSystem.RAM.DeclareInt();
            _audioService = audioService;
        }

        public override void OnCreated(Actor actor)
        {
            SetTiles(actor,false);
            _originalY.Set(actor.WorldPosition.Y.Pixel);
        }

        private void SetTiles(Actor actor, bool springActive)
        {
            var layer = _gameSystem.GetLayer(LayerIndex.FG);

            int offset = _gameSystem.GetTileOffset(Content.TilesetContentKey.Gizmos) - layer.TileOffset;

            var tileX = actor.WorldPosition.X.Pixel / _gameSystem.TileSize;
            var tileY = actor.WorldPosition.Y.Pixel / _gameSystem.TileSize;

            int tileIndex = offset + (springActive ? 8 : 1);

            layer.TileMap.SetTile(tileX, tileY, new Tile(tileIndex, TileFlags.Solid));
            layer.TileMap.SetTile(tileX + 1, tileY, new Tile(tileIndex + 1, TileFlags.Solid));
        }

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            if (_springTimer == 0)
                actor.Visible = false;

            if(collisionInfo.Actor != null && _springTimer == 0)
            {
                collisionInfo.Actor.MotionVector.Y.Set(-4);
                _springTimer.Inc();
                SetTiles(actor, true);
                actor.Visible = true;
                _audioService.Play(SoundContentKey.Bounce);
            }
            else if(_springTimer > 0)
            {
                _springTimer.Inc();

                int yAdjust = _springTimer % 16;
                if (yAdjust >= 8)
                    yAdjust = 16 - yAdjust;
                actor.WorldPosition.Y.Set(_originalY - yAdjust);

                if (_springTimer == 60)
                {
                    _springTimer.Set(0);
                    actor.WorldPosition.Y.Set(_originalY);
                    SetTiles(actor, false);                    
                }
            }
         
        }
    }
}
