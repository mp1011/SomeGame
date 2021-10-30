using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class MovingBlockCollisionDetector : BlockCollisionDetector, ICollisionDetector
    {
        private readonly ActorManager _actorManager;

        public MovingBlockCollisionDetector(ActorManager actorManager)
        {
            _actorManager = actorManager;
        }

        public CollisionInfo DetectCollisions(Actor movingBlock, GameRectangleWithSubpixels frameStartPosition)
        {
            CollisionInfo collisionInfo = null;

            foreach (var character in _actorManager.GetActors(ActorType.Character))
            {
                if (character.WorldPosition.IntersectsWith(movingBlock.WorldPosition))
                {
                    collisionInfo = new CollisionInfo(character);

                    if (character.WorldPosition.IntersectsWith(movingBlock.WorldPosition))
                        collisionInfo += HandleCollision(character, movingBlock.WorldPosition, frameStartPosition);

                    collisionInfo += CheckTouchingGround(character, movingBlock.WorldPosition);

                    if (collisionInfo.IsOnGround)
                        collisionInfo += CheckOnLedge(character, movingBlock.WorldPosition, false,false);

                    character.Behavior.HandleCollision(character, movingBlock);
                }
            }

            return collisionInfo;
        }
    }
}
