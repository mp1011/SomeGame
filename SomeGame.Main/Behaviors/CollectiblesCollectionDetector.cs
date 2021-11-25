using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class CollectiblesCollectionDetector : ICollisionDetector
    {
        private readonly CollectiblesService _collectiblesService;

        public CollectiblesCollectionDetector(CollectiblesService collectiblesService)
        {
            _collectiblesService = collectiblesService;
        }

        public CollisionInfo DetectCollisions(Actor actor)
        {
            return _collectiblesService.HandleCollectibleCollision(actor.WorldPosition);
        }
    }
}
