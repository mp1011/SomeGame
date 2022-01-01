using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class ProjectileBehavior : Behavior
    {
        private RamByte _destroyTimer;
        private RamByte _duration;
        private RamPixelValue _speed;

        public ProjectileBehavior(GameSystem gameSystem, PixelValue speed, byte duration)
        {
            _destroyTimer = gameSystem.RAM.DeclareByte();
            _duration = gameSystem.RAM.DeclareByte(duration);
            _speed = gameSystem.RAM.DeclarePixelValue(speed.Pixel,speed.SubPixel);
        }

        protected override void OnCreated()
        {
            _destroyTimer.Set(0);
            Actor.CurrentAnimation = AnimationKey.Moving;           
        }

        protected override void DoUpdate()
        {
            if(_destroyTimer == 0)
            {
                var direction = Actor.Flip == Flip.H ? Direction.Left : Direction.Right;
                var pt = direction.ToPoint();
                Actor.MotionVector.X.Set((PixelValue)_speed * pt.X);
                Actor.MotionVector.Y.Set((PixelValue)_speed * pt.Y);
            }

            _destroyTimer++;
            if(_destroyTimer == _duration)
                Actor.Destroy();
        }

        protected override void OnCollision(CollisionInfo collisionInfo)
        {
            if (collisionInfo.Actor != null)
                Actor.Destroy();
        }
    }
}
