using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class ProjectileBehavior : Behavior
    {
        private PixelPoint _motionVector;
        private int _destroyTimer;
        private int _duration;

        public ProjectileBehavior(Direction direction, PixelValue speed, int duration)
        {
            var pt = direction.ToPoint();
            _motionVector = new PixelPoint(speed * pt.X, speed * pt.Y);
            _duration = duration;
        }

        public override void OnCreated(Actor actor)
        {
            _destroyTimer = 0;
        }

        public override void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo collisionInfo)
        {
            if (actor.Flip == Flip.H)
                actor.MotionVector = new PixelPoint(_motionVector.X * -1, _motionVector.Y);
            else
                actor.MotionVector = _motionVector;

            actor.CurrentAnimation = AnimationKey.Moving;

            _destroyTimer++;
            if(_destroyTimer == _duration)
                actor.Destroy();
            
            if (collisionInfo.Actor != null)
                actor.Destroy();
        }
    }
}
