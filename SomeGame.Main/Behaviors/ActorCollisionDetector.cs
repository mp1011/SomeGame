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
                if (otherActor.WorldPosition.IntersectsWithOrTouches(actor.WorldPosition))
                {                    
                    otherActor.Behavior.HandleCollision(new CollisionInfo(Actor: actor));
                    return new CollisionInfo(Actor: otherActor);
                }
            }

            return new CollisionInfo();
        }
    }
}
