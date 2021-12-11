using Microsoft.Xna.Framework;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class DebrisBehavior : Behavior
    {
        private readonly Gravity _gravity;
        private readonly Scroller _scroller;

        public DebrisBehavior(Gravity gravity, Scroller scroller)
        {
            _gravity = gravity;
            _scroller = scroller;
        }

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            _gravity.Update(actor, collisionInfo);
            if (actor.WorldPosition.Y > _scroller.Camera.Bottom())
                actor.Destroy();
        }
    }
}
