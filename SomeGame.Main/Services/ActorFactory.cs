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

        public ActorFactory(ActorManager actorManager, GameSystem gameSystem)
        {
            _actorManager = actorManager;
            _gameSystem = gameSystem;
        }

        public Actor CreateActor(
            TilesetContentKey tileset,
            PaletteIndex paletteIndex,
            Dictionary<AnimationKey, byte> animations,
            Behavior behavior,
            ICollisionDetector collisionDetector,
            GameRectangleWithSubpixels position)
        {
            var actor = new Actor(tileset, paletteIndex, behavior, collisionDetector, animations);
            actor.WorldPosition = position;

            _actorManager.TryAddActor(_gameSystem, actor);

            return actor;
        }
    }
}
