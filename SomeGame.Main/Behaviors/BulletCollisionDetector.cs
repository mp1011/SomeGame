using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class BulletCollisionDetector : ICollisionDetector
    {
        private readonly ActorManager _actorManager;
        private readonly ActorType _collidesWith;

        public BulletCollisionDetector(ActorManager actorManager, ActorType collidesWith)
        {
            _actorManager = actorManager;
            _collidesWith = collidesWith;
        }

        public CollisionInfo DetectCollisions(Actor actor, GameRectangleWithSubpixels frameStartPosition)
        {
            foreach(var otherActor in _actorManager.GetActors(_collidesWith))
            {
                if(otherActor.WorldPosition.IntersectsWith(actor.WorldPosition))                
                    return new CollisionInfo(otherActor);                
            }

            return new CollisionInfo();
        }
    }
}
