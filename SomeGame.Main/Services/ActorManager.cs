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
        private List<ActorWithSprite> _actors = new List<ActorWithSprite>();

        public ActorManager(GameSystem gameSystem, Scroller scroller)
        {
            _scroller = scroller;
            _gameSystem = gameSystem;
        }

        public IEnumerable<Actor> GetActors(ActorType actorType)
        {
            return _actors
                .Select(a=>a.Actor)
                .Where(a => a != null && a.Enabled && (a.ActorType & actorType) == actorType);
        }

        public IEnumerable<Actor> GetActors()
        {
            return _actors
                .Select(a => a.Actor)
                .Where(a => a != null && a.Enabled);
        }

        public void UnloadAll()
        {
            foreach (var actor in _actors)
                _gameSystem.GetSprite(actor.SpriteIndex).Enabled = false;

            _actors.Clear();
        }

        public void AddActor(Actor actor)
        {
            _actors.Add(new ActorWithSprite(actor, SpriteIndex.Sprite8));
            DebugService.Actors.Add(actor);
        }

        private void TryAssignSprite(ActorWithSprite actorWithSprite)
        {
            var spriteIndex = _gameSystem.GetFreeSpriteIndex() ?? SpriteIndex.Sprite8;

            if(spriteIndex == SpriteIndex.Sprite8)
            {
                if (RandomUtil.RandomBit())
                    return;
            }

            var sprite = _gameSystem.GetSprite(spriteIndex);
            sprite.TileOffset = _gameSystem.GetTileOffset(actorWithSprite.Actor.Tileset);
            sprite.Priority = SpritePriority.Front;
            sprite.Palette = actorWithSprite.Actor.Palette;
            sprite.Enabled = actorWithSprite.Actor.Enabled;
            actorWithSprite.SpriteIndex = spriteIndex;
            actorWithSprite.NeedsSprite = false;            
        }

        public void Update(Scene currentScene)
        {
            var extraSprite = _gameSystem.GetSprite(SpriteIndex.Sprite8);
            extraSprite.Enabled = false;

            foreach (var actorWithSprite in _actors)
            {
                if (ShouldActivate(actorWithSprite.Actor))
                    actorWithSprite.Actor.Create();

                if (actorWithSprite.SpriteIndex == SpriteIndex.Sprite8)
                    actorWithSprite.NeedsSprite = true;

                if (actorWithSprite.Actor.Enabled && actorWithSprite.NeedsSprite)                
                    TryAssignSprite(actorWithSprite);                   
                

                var spriteIndex = actorWithSprite.SpriteIndex;
                if (actorWithSprite.NeedsSprite)
                {
                    var actor = actorWithSprite.Actor;
                    if (actor.Enabled || actor.Destroying)                    
                        UpdateActor(actor, spriteIndex, currentScene, actorWithSprite.NeedsSprite);                    
                    else
                        actorWithSprite.NeedsSprite = true;
                }
                else
                {
                    var actor = actorWithSprite.Actor;

                    var sprite = _gameSystem.GetSprite(spriteIndex);
                    sprite.Enabled = actor.Enabled;

                    if (actor.Enabled)
                    {
                        UpdateActor(actor, spriteIndex, currentScene, actorWithSprite.NeedsSprite);
                        sprite.Flip = actor.Flip;
                    }
                    else
                        actorWithSprite.NeedsSprite = true;
                }
            }
        }

        private bool ShouldActivate(Actor a)
        {
            if (a.HasBeenActivated)
                return false;

            //todo, Y axis
            int padding = _gameSystem.Screen.Width / 4;
            return (a.WorldPosition.Right > _scroller.Camera.Left - padding)
                && (a.WorldPosition.Left < _scroller.Camera.Right + padding);
        }

        private void UpdateActor(Actor actor, SpriteIndex spriteIndex, Scene scene, bool needsSprite)
        {
            var sprite = _gameSystem.GetSprite(spriteIndex);

            if(!needsSprite)
                sprite.Palette = actor.Palette;

            actor.WorldPosition.XPixel += actor.MotionVector.X;
            actor.WorldPosition.YPixel += actor.MotionVector.Y;

            var animationState = actor.Animator.Update(spriteIndex, actor.CurrentAnimation, needsSprite);
            actor.IsAnimationFinished = animationState == AnimationState.Finished;

            var collisionInfo = actor.CollisionDetector.DetectCollisions(actor);
            DebugService.ShowPosition(actor);

            if(actor.Destroying)
                actor.OnBeingDestroyed();
            else 
                actor.Behavior.Update(actor, collisionInfo);

            if(!needsSprite)
                _scroller.ScrollActor(actor, sprite);
        }

    }
}
