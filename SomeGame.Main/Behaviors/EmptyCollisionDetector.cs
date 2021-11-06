using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class EmptyCollisionDetector : ICollisionDetector
    {
        public CollisionInfo DetectCollisions(Actor actor)
        {
            return new CollisionInfo();
        }
    }
}
