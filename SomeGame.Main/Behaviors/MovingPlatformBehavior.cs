using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class MovingPlatformBehavior : Behavior
    {
        private Point _origin;
        private readonly int _xSpan = 100;
       // private readonly int _ySpan = 0;

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            if(actor.MotionVector.X < 0 && actor.WorldPosition.X < _origin.X)
                actor.MotionVector = new PixelPoint(new PixelValue(0, 100), new PixelValue(0, 0));
            else if(actor.MotionVector.X > 0 && actor.WorldPosition.X > _origin.X + _xSpan)
                actor.MotionVector = new PixelPoint(new PixelValue(0, -100), new PixelValue(0, 0));
        }

        public override void OnCreated(Actor actor)
        {
            _origin = actor.WorldPosition.Center;
            actor.MotionVector = new PixelPoint(new PixelValue(0, -100), new PixelValue(0, 0));
        }
    }
}
