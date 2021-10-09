using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class ProjectileBehavior : Behavior
    {
        private PixelPoint _motionVector;
        private int _destroyTimer;

        public ProjectileBehavior(Direction direction, PixelValue speed)
        {
            var pt = direction.ToPoint();
            _motionVector = new PixelPoint(speed * pt.X, speed * pt.Y);
        }

        public override void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo collisionInfo)
        {
            actor.MotionVector = _motionVector;
            actor.CurrentAnimation = AnimationKey.Moving;

            _destroyTimer++;
            if(_destroyTimer == 100)
            {
                _destroyTimer = 0;
                actor.Enabled = false;
            }
        }
    }
}
