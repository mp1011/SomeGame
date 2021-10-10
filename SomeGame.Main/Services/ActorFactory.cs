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
            ActorType actorType,
            TilesetContentKey tileset,
            PaletteIndex paletteIndex,
            Dictionary<AnimationKey, byte> animations,
            Behavior behavior,
            ICollisionDetector collisionDetector,
            PixelPoint position,
            Rectangle hitBox)
        {
            var actor = new Actor(actorType, tileset, paletteIndex, behavior, collisionDetector, hitBox, animations);
            actor.WorldPosition = new GameRectangleWithSubpixels(position.X,position.Y, hitBox.Width,hitBox.Height);

            _actorManager.TryAddActor(_gameSystem, actor);

            return actor;
        }
    }
}
