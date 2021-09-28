using Microsoft.Xna.Framework;
using SomeGame.Main.Models;
using System;
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

            
            return true;
        }

        public void Update()
        {
            foreach (SpriteIndex spriteIndex in Enum.GetValues<SpriteIndex>())
            {
                var actor = _actors[(int)spriteIndex];
                if (actor == null)
                    continue;

                UpdateActor(actor, spriteIndex);
            }
        }

        private void UpdateActor(Actor actor, SpriteIndex spriteIndex)
        {
            var sprite = _gameSystem.GetSprite(spriteIndex);
            var frameStartPosition = (Rectangle)actor.WorldPosition;    

            actor.WorldPosition.X += actor.MotionVector.X;
            actor.WorldPosition.Y += actor.MotionVector.Y;
         
            _spriteAnimator.SetSpriteAnimation(spriteIndex, actor.CurrentAnimationIndex);

            actor.Behavior.Update(actor, frameStartPosition);

            var scene = _sceneManager.CurrentScene;
            sprite.ScrollX = sprite.ScrollX.Set(actor.WorldPosition.X - scene.Camera.X);
            sprite.ScrollY = sprite.ScrollX.Set(actor.WorldPosition.Y - scene.Camera.Y);
        }
    }
}
