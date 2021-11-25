using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class PlayerCollisionDetector : ICollisionDetector
    {
        private readonly BgCollisionDetector _bgCollisionDetector;
        private readonly CollectiblesCollectionDetector _collectiblesCollectionDetector;

        public PlayerCollisionDetector(BgCollisionDetector bgCollisionDetector, CollectiblesCollectionDetector collectiblesCollectionDetector)
        {
            _bgCollisionDetector = bgCollisionDetector;
            _collectiblesCollectionDetector = collectiblesCollectionDetector;
        }

        public CollisionInfo DetectCollisions(Actor actor)
        {
            return _bgCollisionDetector.DetectCollisions(actor) 
                    + _collectiblesCollectionDetector.DetectCollisions(actor);
        }
    }
}
