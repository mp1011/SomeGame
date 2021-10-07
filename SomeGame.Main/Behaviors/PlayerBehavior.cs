using Microsoft.Xna.Framework;
using SomeGame.Main.Models;
using System;

namespace SomeGame.Main.Behaviors
{
    class PlayerBehavior : Behavior
    {
        private readonly PlatformerPlayerMotionBehavior _motionBehavior;
        private readonly CameraBehavior _cameraBehavior;
        private readonly AcceleratedMotion _gravity;

        public PlayerBehavior(PlatformerPlayerMotionBehavior motionBehavior, CameraBehavior cameraBehavior, 
            AcceleratedMotion gravity)
        {
            _motionBehavior = motionBehavior;
            _cameraBehavior = cameraBehavior;
            _gravity = gravity;
        }

        public override void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo collisionInfo)
        {
            _motionBehavior.Update(actor, frameStartPosition, collisionInfo);
            _gravity.Update(actor, frameStartPosition, collisionInfo);
            _cameraBehavior.Update(actor, frameStartPosition, collisionInfo);
        }
    }
}
