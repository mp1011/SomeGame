using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class Gravity : Behavior
    {
        private byte _acceleration = 15;

        protected override void DoUpdate()
        {
        }

        protected override void OnCollision(CollisionInfo collisionInfo)
        {
            if (collisionInfo.IsOnGround || collisionInfo.Actor != null || Actor.Destroying)
                return;

            Actor.MotionVector.Offset(Orientation.Vertical, 0, _acceleration);
        }
    }
}
