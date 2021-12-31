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

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            if (_timer == 0 && collisionInfo.Actor != null)
                _timer.Inc();
            else if (_timer > 0)
            {
                actor.Palette = _gameSystem.GetLayer(LayerIndex.FG).Palette.Next();
                actor.Visible = (_timer % 4) <= 1;
                _timer.Inc();
            }

            if (_timer >= 20)
            {
                SetTiles(actor, false);
                actor.Destroy();
            }
        }

        protected override void OnBlockCreated(Actor actor)
        {
            SetTiles(actor, show: true);
        }
    }
}
