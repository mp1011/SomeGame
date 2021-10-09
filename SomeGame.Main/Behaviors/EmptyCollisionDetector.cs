using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class EmptyCollisionDetector : ICollisionDetector
    {
        public CollisionInfo DetectCollisions(Actor actor, GameRectangleWithSubpixels frameStartPosition)
        {
            return new CollisionInfo();
        }
    }
}
