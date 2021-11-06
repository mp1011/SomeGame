using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class ActorCollisionDetector : ICollisionDetector
    {
        private readonly ActorManager _actorManager;
        private readonly ActorType _collidesWith;

        public ActorCollisionDetector(ActorManager actorManager, ActorType collidesWith)
        {
            _actorManager = actorManager;
            _collidesWith = collidesWith;
        }

        public CollisionInfo DetectCollisions(Actor actor)
        {
            foreach(var otherActor in _actorManager.GetActors(_collidesWith))
            {
                if (otherActor.WorldPosition.IntersectsWith(actor.WorldPosition))
                {
                    otherActor.Behavior.HandleCollision(otherActor, actor);
                    return new CollisionInfo(Actor:otherActor);
                }
            }

            return new CollisionInfo();
        }
    }
}
