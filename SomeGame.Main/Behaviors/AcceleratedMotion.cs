using Microsoft.Xna.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class AcceleratedMotion : Behavior
    {
        public Orientation Orientation { get; }

        public byte SubPixelAcceleration { get; set; }

        public int CoarseAcceleration { get; set; }

        public AcceleratedMotion(Orientation orientation)
        {
            Orientation = orientation;
        }

        public override void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo collisionInfo)
        {
            actor.MotionVector = actor.MotionVector.Offset(Orientation, CoarseAcceleration, SubPixelAcceleration);
        }
    }
}
