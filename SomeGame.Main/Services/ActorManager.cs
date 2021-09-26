using SomeGame.Main.Models;
using System;
using System.Linq;

namespace SomeGame.Main.Services
{
    class ActorManager
    {
        private readonly SpriteAnimator _spriteAnimator;
        private Actor[] _actors = new Actor[Enum.GetValues<SpriteIndex>().Length];

        public ActorManager(SpriteAnimator spriteAnimator)
        {
            _spriteAnimator = spriteAnimator;
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

        public void Update(GameSystem gameSystem, Scene currentScene)
        {
            foreach (SpriteIndex spriteIndex in Enum.GetValues<SpriteIndex>())
            {
                var actor = _actors[(int)spriteIndex];
                if (actor == null)
                    continue;

                UpdateActor(actor, spriteIndex, currentScene, gameSystem);
            }
        }

        private void UpdateActor(Actor actor, SpriteIndex spriteIndex, Scene currentScene, GameSystem gameSystem)
        {
            var sprite = gameSystem.GetSprite(spriteIndex);

            sprite.ScrollX = sprite.ScrollX.Set(actor.WorldPosition.X - currentScene.Camera.X);
            sprite.ScrollY = sprite.ScrollX.Set(actor.WorldPosition.Y - currentScene.Camera.Y);

            _spriteAnimator.SetSpriteAnimation(spriteIndex, actor.CurrentAnimationIndex);
        }
    }
}
