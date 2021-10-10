using SomeGame.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Services
{
    class ActorManager
    {
        private readonly GameSystem _gameSystem;
        private readonly SceneManager _sceneManager;
        private readonly SpriteAnimator _spriteAnimator;
        private Actor[] _actors = new Actor[Enum.GetValues<SpriteIndex>().Length];

        public ActorManager(GameSystem gameSystem, SpriteAnimator spriteAnimator, SceneManager sceneManager)
        {
            _spriteAnimator = spriteAnimator;
            _gameSystem = gameSystem;
            _sceneManager = sceneManager;
        }

        public IEnumerable<Actor> GetActors(ActorType actorType)
        {
            return _actors.Where(a => a != null && a.Enabled && a.ActorType == actorType);
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
            actor.Enabled = true;

            _actors[(int)spriteIndex] = actor;

            DebugService.Actors.Add(actor);

            return true;
        }

        public void Update()
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
                    UpdateActor(actor, spriteIndex);
                    sprite.Flip = actor.Flip;
                }
            }
        }

        private void UpdateActor(Actor actor, SpriteIndex spriteIndex)
        {
            var sprite = _gameSystem.GetSprite(spriteIndex);
            var frameStartPosition = actor.WorldPosition.Copy();   

            actor.WorldPosition.XPixel += actor.MotionVector.X;
            actor.WorldPosition.YPixel += actor.MotionVector.Y;

            var animationState = _spriteAnimator.Update(spriteIndex, actor.CurrentAnimationIndex);
            actor.IsAnimationFinished = animationState == AnimationState.Finished;

            var collisionInfo = actor.CollisionDetector.DetectCollisions(actor, frameStartPosition);
            actor.Behavior.Update(actor, frameStartPosition, collisionInfo);

            var scene = _sceneManager.CurrentScene;
            var actorScreenX = sprite.ScrollX.Set(actor.WorldPosition.X - scene.Camera.X);
            var actorScreenY = sprite.ScrollX.Set(actor.WorldPosition.Y - scene.Camera.Y);

            sprite.ScrollX = actorScreenX - actor.LocalHitbox.X;
            sprite.ScrollY = actorScreenY - actor.LocalHitbox.Y;


        }
    }
}
