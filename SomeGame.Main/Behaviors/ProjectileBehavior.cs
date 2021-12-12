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

        public override void OnCreated(Actor actor)
        {
            _destroyTimer.Set(0);
            actor.CurrentAnimation = AnimationKey.Moving;           
        }

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            if(_destroyTimer == 0)
            {
                var direction = actor.Flip == Flip.H ? Direction.Left : Direction.Right;
                var pt = direction.ToPoint();
                actor.MotionVector.X.Set((PixelValue)_speed * pt.X);
                actor.MotionVector.Y.Set((PixelValue)_speed * pt.Y);
            }

            _destroyTimer++;
            if(_destroyTimer == _duration)
                actor.Destroy();
            
            if (collisionInfo.Actor != null)
                actor.Destroy();
        }
    }
}
