using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class ProjectileBehavior : Behavior
    {
        private int _destroyTimer;
        private int _duration;
        private PixelValue _speed;

        public ProjectileBehavior(PixelValue speed, int duration)
        {
            _duration = duration;
            _speed = speed;
        }

        public override void OnCreated(Actor actor)
        {
            _destroyTimer = 0;
            actor.CurrentAnimation = AnimationKey.Moving;           
        }

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            if(_destroyTimer == 0)
            {
                var direction = actor.Flip == Flip.H ? Direction.Left : Direction.Right;
                var pt = direction.ToPoint();
                actor.MotionVector = new PixelPoint(_speed * pt.X, _speed * pt.Y);
            }

            _destroyTimer++;
            if(_destroyTimer == _duration)
                actor.Destroy();
            
            if (collisionInfo.Actor != null)
                actor.Destroy();
        }
    }
}
