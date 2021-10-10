using SomeGame.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Services
{
    class SpriteAnimator
    {
        private readonly GameSystem _gameSystem;
        private readonly SpriteFrame[] _spriteFrames;
        private readonly AnimationFrame[] _animationFrames;
        private readonly Animation[] _animations;

        private SpriteAnimation[] _spriteAnimations = new SpriteAnimation[Enum.GetValues<SpriteIndex>().Length];

        public SpriteAnimator(GameSystem gameSystem,
                              IEnumerable<SpriteFrame> spriteFrames, 
                              AnimationFrame[] animationFrames,
                              Animation[] animations)
        {
            _gameSystem = gameSystem;
            _spriteFrames = spriteFrames.ToArray();
            _animationFrames = animationFrames;
            _animations = animations;
        }

        private void SetSpriteAnimation(SpriteIndex spriteIndex, byte animationIndex)
        {
            var current = _spriteAnimations[(int)spriteIndex];
            if (current == null || current.AnimationIndex != animationIndex)
                _spriteAnimations[(int)spriteIndex] = new SpriteAnimation(animationIndex);
        }

        public AnimationState Update(SpriteIndex spriteIndex, byte animationIndex)
        {
            SetSpriteAnimation(spriteIndex, animationIndex);
            var spriteAnimation = _spriteAnimations[(int)spriteIndex];
            if (spriteAnimation == null)
                return AnimationState.None;

            return UpdateAnimation(spriteAnimation, spriteIndex);
        }

        private AnimationState UpdateAnimation(SpriteAnimation spriteAnimation, SpriteIndex spriteIndex)
        {
            var animation = _animations[spriteAnimation.AnimationIndex];
            bool newFrame = false;
            bool finished = false;

            if (spriteAnimation.FramesRemaining == 0)
            {
                spriteAnimation.CurrentIndex++;
                newFrame=true;
            }

            if (spriteAnimation.CurrentIndex >= animation.FrameIndices.Length)
            {
                finished = spriteAnimation.CurrentIndex == animation.FrameIndices.Length;
                spriteAnimation.CurrentIndex = 0;
                newFrame = true;                
            }
           
            if (newFrame)
            {
                var animationFrame = _animationFrames[animation.FrameIndices[spriteAnimation.CurrentIndex]];
                var spriteFrame = _spriteFrames[animationFrame.SpriteFrameIndex];

                spriteAnimation.FramesRemaining = animationFrame.Duration;

                _gameSystem.GetSprite(spriteIndex).SetTiles(
                    topLeft: spriteFrame.TopLeft,
                    topRight: spriteFrame.TopRight,
                    bottomLeft: spriteFrame.BottomLeft,
                    bottomRight: spriteFrame.BottomRight);
            }
            else
                spriteAnimation.FramesRemaining--;

            if (finished)
                return AnimationState.Finished;
            else
                return AnimationState.Playing;
        }


    }
}
