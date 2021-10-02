using Microsoft.Xna.Framework;
using SomeGame.Main.Models;
using System;

namespace SomeGame.Main.Behaviors
{
    class PlayerBehavior : Behavior
    {
        private readonly PlatformerPlayerMotionBehavior _motionBehavior;
        private readonly CameraBehavior _cameraBehavior;
        private readonly BgCollisionBehavior _bgCollisionBehavior;
        private readonly AcceleratedMotion _gravity;

        public PlayerBehavior(PlatformerPlayerMotionBehavior motionBehavior, CameraBehavior cameraBehavior, 
            BgCollisionBehavior bgCollisionBehavior, AcceleratedMotion gravity)
        {
            _motionBehavior = motionBehavior;
            _cameraBehavior = cameraBehavior;
            _bgCollisionBehavior = bgCollisionBehavior;
            _gravity = gravity;
        }

        public override void Update(Actor actor, Rectangle frameStartPosition)
        {
            _bgCollisionBehavior.Update(actor,frameStartPosition);
            _motionBehavior.Update(actor, frameStartPosition);
            _gravity.Update(actor, frameStartPosition);
            _cameraBehavior.Update(actor, frameStartPosition);
        }
    }
}
