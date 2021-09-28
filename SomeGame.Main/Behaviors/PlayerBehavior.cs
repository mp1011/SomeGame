using Microsoft.Xna.Framework;
using SomeGame.Main.Models;
using System;

namespace SomeGame.Main.Behaviors
{
    class PlayerBehavior : Behavior
    {
        private readonly EightDirPlayerMotionBehavior _motionBehavior;
        private readonly CameraBehavior _cameraBehavior;
        private readonly BgCollisionBehavior _bgCollisionBehavior;

        public PlayerBehavior(EightDirPlayerMotionBehavior motionBehavior, CameraBehavior cameraBehavior, BgCollisionBehavior bgCollisionBehavior)
        {
            _motionBehavior = motionBehavior;
            _cameraBehavior = cameraBehavior;
            _bgCollisionBehavior = bgCollisionBehavior;
        }

        public override void Update(Actor actor, Rectangle frameStartPosition)
        {
            _bgCollisionBehavior.Update(actor,frameStartPosition);
            _motionBehavior.Update(actor, frameStartPosition);            
            _cameraBehavior.Update(actor, frameStartPosition);
        }
    }
}
