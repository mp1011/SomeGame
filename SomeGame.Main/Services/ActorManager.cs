using SomeGame.Main.Models;
using SomeGame.Main.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Services
{
    class ActorManager
    {
        private readonly Scroller _scroller;
        private readonly GameSystem _gameSystem;
        private Actor[] _actors = new Actor[Enum.GetValues<SpriteIndex>().Length];

        public ActorManager(GameSystem gameSystem, Scroller scroller)
        {
            _scroller = scroller;
            _gameSystem = gameSystem;
        }

        public IEnumerable<Actor> GetActors(ActorType actorType)
        {
            return _actors.Where(a => a != null && a.Enabled && a.ActorType == actorType);
        }

        public void UnloadAll()
        {
            foreach (SpriteIndex spriteIndex in Enum.GetValues<SpriteIndex>())
            {
                _actors[(int)spriteIndex] = null;
                _gameSystem.GetSprite(spriteIndex).Enabled = false;
            }
        }

        public bool TryAddActor(GameSystem system, Actor actor)
        {
            var spriteIndex = system.GetFreeSpriteIndex();
            if (!spriteIndex.HasValue)
                return false;

            var sprite = system.GetSprite(spriteIndex.Value);
            sprite.TileOffset = system.GetTileOffset(actor.Tileset); 
            sprite.Priority = SpritePriority.Front;
            sprite.Palette = actor.Palette;
            sprite.Enabled = true;

            _actors[(int)spriteIndex] = actor;

            DebugService.Actors.Add(actor);

            return true;
        }

        public void Update(Scene currentScene)
        {
            foreach (SpriteIndex spriteIndex in Enum.GetValues<SpriteIndex>())
            {
                var actor = _actors[(int)spriteIndex];
                if (actor == null)
                    continue;

                var sprite = _gameSystem.GetSprite(spriteIndex);
                sprite.Enabled = actor.Enabled;

                if (actor.Enabled)
                {
                    UpdateActor(actor, spriteIndex, currentScene);
                    sprite.Flip = actor.Flip;
                }
            }
        }

        private void UpdateActor(Actor actor, SpriteIndex spriteIndex, Scene scene)
        {
            var sprite = _gameSystem.GetSprite(spriteIndex);
            sprite.Palette = actor.Palette;
            var frameStartPosition = actor.WorldPosition.Copy();   

            actor.WorldPosition.XPixel += actor.MotionVector.X;
            actor.WorldPosition.YPixel += actor.MotionVector.Y;

            var animationState = actor.Animator.Update(spriteIndex, actor.CurrentAnimation);
            actor.IsAnimationFinished = animationState == AnimationState.Finished;

            var collisionInfo = actor.CollisionDetector.DetectCollisions(actor, frameStartPosition);

            if(actor.Destroying)
            {
                if (actor.DestroyedBehavior.Update(actor) == DestroyedState.Destroyed)
                    actor.Enabled = false;
            }
            else 
                actor.Behavior.Update(actor, frameStartPosition, collisionInfo);

            _scroller.ScrollActor(actor, sprite);
        }
    }
}
