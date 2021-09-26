using SomeGame.Main.Models;
using System;

namespace SomeGame.Main.Services
{
    class SpriteAnimator
    {
        private readonly SpriteFrame[] _spriteFrames;
        private readonly AnimationFrame[] _animationFrames;
        private readonly Animation[] _animations;

        private SpriteAnimation[] _spriteAnimations = new SpriteAnimation[Enum.GetValues<SpriteIndex>().Length];

        public SpriteAnimator(SpriteFrame[] spriteFrames, 
                              AnimationFrame[] animationFrames,
                              Animation[] animations)
        {
            _spriteFrames = spriteFrames;
            _animationFrames = animationFrames;
            _animations = animations;
        }

        public void SetSpriteAnimation(SpriteIndex spriteIndex, byte animationIndex)
        {
            var current = _spriteAnimations[(int)spriteIndex];
            if (current == null || current.AnimationIndex != animationIndex)
                _spriteAnimations[(int)spriteIndex] = new SpriteAnimation(animationIndex);
        }

        public void Update(GameSystem gameSystem)
        {
            foreach(SpriteIndex spriteIndex in Enum.GetValues<SpriteIndex>())
            {
                var spriteAnimation = _spriteAnimations[(int)spriteIndex];
                if (spriteAnimation == null)
                    continue;

                UpdateAnimation(spriteAnimation, spriteIndex, gameSystem);
            }
        }

        private void UpdateAnimation(SpriteAnimation spriteAnimation, SpriteIndex spriteIndex, GameSystem gameSystem)
        {
            var animation = _animations[spriteAnimation.AnimationIndex];
            bool newFrame = false;

            if (spriteAnimation.FramesRemaining == 0)
            {
                spriteAnimation.CurrentIndex++;
                newFrame=true;
            }

            if (spriteAnimation.CurrentIndex >= animation.FrameIndices.Length)
            {
                spriteAnimation.CurrentIndex = 0;
                newFrame = true;
            }
           
            if (newFrame)
            {
                var animationFrame = _animationFrames[animation.FrameIndices[spriteAnimation.CurrentIndex]];
                var spriteFrame = _spriteFrames[animationFrame.SpriteFrameIndex];

                spriteAnimation.FramesRemaining = animationFrame.Duration;

                gameSystem.GetSprite(spriteIndex).SetTiles(
                    topLeft: spriteFrame.TopLeft,
                    topRight: spriteFrame.TopRight,
                    bottomLeft: spriteFrame.BottomLeft,
                    bottomRight: spriteFrame.BottomRight);
            }
            else
                spriteAnimation.FramesRemaining--;
        }


    }
}
