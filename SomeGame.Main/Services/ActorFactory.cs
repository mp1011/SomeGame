using Microsoft.Xna.Framework;
using SomeGame.Main.Behaviors;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using System.Collections.Generic;

namespace SomeGame.Main.Services
{
    class ActorFactory
    {
        private readonly ActorManager _actorManager;
        private readonly GameSystem _gameSystem;
        private readonly DataSerializer _dataSerializer;

        public ActorFactory(ActorManager actorManager, GameSystem gameSystem, DataSerializer dataSerializer)
        {
            _actorManager = actorManager;
            _gameSystem = gameSystem;
            _dataSerializer = dataSerializer;
        }

        public Actor CreateActor(
            ActorId actorId,
            ActorType actorType,
            TilesetContentKey tileset,
            PaletteIndex paletteIndex,
            Behavior behavior,
            IDestroyedBehavior destroyedBehavior,
            ICollisionDetector collisionDetector,
            PixelPoint position,
            Rectangle hitBox)
        {
            var actor = new Actor(actorType, tileset, paletteIndex, behavior, destroyedBehavior, collisionDetector, hitBox, CreateAnimator(actorId, tileset));
            actor.WorldPosition = new GameRectangleWithSubpixels(position.X,position.Y, hitBox.Width,hitBox.Height);

            _actorManager.TryAddActor(_gameSystem, actor);

            return actor;
        }

        private SpriteAnimator CreateAnimator(ActorId actorId, TilesetContentKey tileset)
        {
            return new SpriteAnimator(_gameSystem,
                _dataSerializer.LoadSpriteFrames(tileset),
                _dataSerializer.LoadAnimations(actorId));
        }
    }
}
