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

        protected override void DoUpdate()
        {
            _timer.Inc();

            if(_timer == 128)
            {
                bool visible = IsBlockVisible(Actor);
                SetTiles(Actor, !visible);
                _timer.Set(0);
            }
        }

    
    }
}
