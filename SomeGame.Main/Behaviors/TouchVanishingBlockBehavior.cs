using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class TouchVanishingBlockBehavior : GizmoBlockBehavior
    {
        public TouchVanishingBlockBehavior(GameSystem gameSystem, Scroller scroller, AudioService audioService) 
            : base(gameSystem, scroller, audioService)
        {
        }

        protected override void OnCollision(CollisionInfo collisionInfo)
        {
            if (_timer == 0 && collisionInfo.Actor != null)
                _timer.Inc();
        }

        protected override void DoUpdate()
        {            
            if (_timer > 0)
            {
                Actor.Palette = _gameSystem.GetLayer(LayerIndex.FG).Palette.Next();
                Actor.Visible = (_timer % 4) <= 1;
                _timer.Inc();
            }

            if (_timer >= 20)
            {
                SetTiles(Actor, false);
                Actor.Destroy();
            }
        }

        protected override void OnBlockCreated(Actor actor)
        {
            SetTiles(actor, show: true);
        }
    }
}
