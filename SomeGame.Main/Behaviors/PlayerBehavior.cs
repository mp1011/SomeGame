using SomeGame.Main.Models;
using System;

namespace SomeGame.Main.Behaviors
{
    class PlayerBehavior : Behavior
    {
        private readonly EightDirPlayerMotionBehavior _motionBehavior;
        private readonly CameraBehavior _cameraBehavior;

        public PlayerBehavior(EightDirPlayerMotionBehavior motionBehavior, CameraBehavior cameraBehavior)
        {
            _motionBehavior = motionBehavior;
            _cameraBehavior = cameraBehavior;
        }

        public override void Update(Actor actor)
        {
            _motionBehavior.Update(actor);
            _cameraBehavior.Update(actor);
        }
    }
}
