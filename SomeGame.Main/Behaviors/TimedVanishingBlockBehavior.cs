using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class TimedVanishingBlockBehavior : GizmoBlockBehavior
    {
        public TimedVanishingBlockBehavior(GameSystem gameSystem, Scroller scroller, AudioService audioService) 
            : base(gameSystem, scroller, audioService)
        {
        }

        protected override void OnBlockCreated(Actor actor)
        {
            SetTiles(actor, show: true);
        }

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            _timer.Inc();

            if(_timer == 128)
            {
                bool visible = IsBlockVisible(actor);
                SetTiles(actor, !visible);
                _timer.Set(0);
            }
        }

    
    }
}
