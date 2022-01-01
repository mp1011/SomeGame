using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class MovingPlatformBehavior : Behavior
    {
        private Point _origin;
        private readonly int _xSpan = 100;
        private const int _speed = 40;
        // private readonly int _ySpan = 0;

        protected override void DoUpdate()
        {
            if(Actor.MotionVector.X < 0 && Actor.WorldPosition.X < _origin.X)
                Actor.MotionVector.Set(new PixelPoint(new PixelValue(0, _speed), new PixelValue(0, 0)));
            else if(Actor.MotionVector.X > 0 && Actor.WorldPosition.X > _origin.X + _xSpan)
                Actor.MotionVector.Set(new PixelPoint(new PixelValue(0, -_speed), new PixelValue(0, 0)));
        }

        protected override void OnCreated()
        {
            _origin = Actor.WorldPosition.Center;
            Actor.MotionVector.Set(new PixelPoint(new PixelValue(0, -_speed), new PixelValue(0, 0)));
        }
    }
}
