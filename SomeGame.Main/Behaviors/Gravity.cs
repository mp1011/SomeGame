using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class Gravity : Behavior
    {
        private byte _acceleration = 15;

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            if (collisionInfo.IsOnGround)
                return;

            actor.MotionVector.Offset(Orientation.Vertical, 0, _acceleration);
        }
    }
}
