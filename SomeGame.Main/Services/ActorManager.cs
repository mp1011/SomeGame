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
        private List<Actor> _actors = new List<Actor>();

        private Actor[] _actorSpriteSlots;

        public ActorManager(GameSystem gameSystem, Scroller scroller)
        {
            _scroller = scroller;
            _gameSystem = gameSystem;
            _actorSpriteSlots = new Actor[Enum.GetValues<SpriteIndex>().Length];
        }

        public IEnumerable<Actor> GetActors(ActorType actorType)
        {
            return _actors
                .Where(a => a.Enabled && (a.ActorType & actorType) == actorType);
        }

        public IEnumerable<Actor> GetActors()
        {
            return _actors
                .Where(a => a.Enabled);
        }

        public void Update(Scene currentScene)
        {
            if (_gameSystem.Paused)
                return;

            foreach (var actor in _actors)
                Update(actor, currentScene);

            foreach (var spriteIndex in Enum.GetValues<SpriteIndex>())
            {
                var sprite = _gameSystem.GetSprite(spriteIndex);
                var actor = _actorSpriteSlots[(int)spriteIndex];

                sprite.Enabled = actor != null && actor.Enabled && actor.Visible;

                if (sprite.Enabled)
                {
                    _scroller.ScrollActorSprite(actor, sprite);                    
                    sprite.Palette = actor.Palette;
                    sprite.Flip = actor.Flip;
                    actor.Animator.ApplyToSprite(sprite, actor.CurrentAnimation);
                }
                else
                {
                    _actorSpriteSlots[(int)spriteIndex] = null;
                    if (actor != null)
                        actor.NeedsSprite = true;
                }
            }
        }

        public void UnloadAll()
        {
            foreach(var spriteIndex in Enum.GetValues<SpriteIndex>())
                _gameSystem.GetSprite(spriteIndex).Enabled = false;

            _actors.Clear();
        }

        public void AddActor(Actor actor)
        {
            _actors.Add(actor);
            DebugService.Actors.Add(actor);
        }

        
        private void Update(Actor actor, Scene currentScene)
        {
            if (actor.State == ActorState.Destroyed)
                return;

            if (actor.State == ActorState.WaitingForActivation
                && IsNearScreen(actor))
            {
                actor.Create();
            }

            if (actor.Visible && actor.NeedsSprite)
                AssignSprite(actor);

            if (actor.State != ActorState.WaitingForActivation)
            {
                UpdateActorBehavior(actor);
                UpdateAnimation(actor);
            }

            if (actor.State == ActorState.Active && !IsNearScreen(actor))
                actor.State = ActorState.WaitingForActivation;
        }

        private void UpdateAnimation(Actor actor)
        {
            var animationState = actor.Animator.Update(actor.CurrentAnimation);
            actor.IsAnimationFinished = animationState == AnimationState.Finished;
        }

        private void AssignSprite(Actor actor)
        {
            foreach(var spriteIndex in Enum.GetValues<SpriteIndex>())
            {
                if(AssignSprite(actor, spriteIndex))              
                    return;              
            }

            //steal a random sprite
            var stealIndex = RandomUtil.RandomEnumValue<SpriteIndex>();
            var stealActor = _actorSpriteSlots[(int)stealIndex];
            if (stealActor != null)
                stealActor.NeedsSprite = true;

            _actorSpriteSlots[(int)stealIndex] = null;
            AssignSprite(actor, stealIndex);
        }

        private bool AssignSprite(Actor actor, SpriteIndex spriteIndex)
        {
            var sprite = _gameSystem.GetSprite(spriteIndex);
            if (sprite.Enabled && _actorSpriteSlots[(int)spriteIndex] != null)
                return false;

            sprite.Enabled = true;
            sprite.Priority = true;
            sprite.TileOffset = _gameSystem.GetTileOffset(actor.Tileset);
            _actorSpriteSlots[(int)spriteIndex] = actor;
            actor.NeedsSprite = false;

            return true;
        }

        private void StealRandomSprite(Actor actor)
        {

        }

        private void UpdateActorBehavior(Actor actor)
        {
            actor.WorldPosition.X.Add(actor.MotionVector.X);
            actor.WorldPosition.Y.Add(actor.MotionVector.Y);
           
            var collisionInfo = actor.CollisionDetector.DetectCollisions(actor);
            actor.Behavior.HandleCollision(collisionInfo);

            if (actor.Destroying)
            {
                if (actor.DestroyedBehavior == null || actor.DestroyedBehavior.Update(actor) == DestroyedState.Destroyed)
                    actor.State = ActorState.Destroyed;
            }
            else
            {
                actor.Behavior.Update();
            }

        }
        private bool IsNearScreen(Actor a)
        {
            //todo, Y axis
            int padding = _gameSystem.Screen.Width / 4;
            return (a.WorldPosition.Right() > _scroller.Camera.Left() - padding)
                && (a.WorldPosition.Left() < _scroller.Camera.Right() + padding);
        }
    }
}
