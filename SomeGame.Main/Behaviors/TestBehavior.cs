using Microsoft.Xna.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class TestBehavior : Behavior
    {
        private int _counter = 0;

        public override void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo collisionInfo)
        {
            if(_counter==0)
                actor.MotionVector = new PixelPoint(2, 1);
            if (_counter == 50)
                actor.MotionVector = new PixelPoint(-2, -1);
          
            _counter++;
            if (_counter == 100)
                _counter = 0;
        }
    }
}
