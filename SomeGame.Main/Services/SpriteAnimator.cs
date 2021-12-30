using SomeGame.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeGame.Main.Services
{
    class SpriteAnimator
    {
        private readonly GameSystem _gameSystem;
        private SpriteAnimation _spriteAnimation;
        private readonly SpriteFrame[] _spriteFrames;
        private readonly Dictionary<AnimationKey, Animation> _animations;

        public SpriteAnimator(GameSystem gameSystem, SpriteFrame[] spriteFrames, Dictionary<AnimationKey, Animation> animations)
        {
            _gameSystem = gameSystem;
            _animations = animations;
            _spriteFrames = spriteFrames;
            _spriteAnimation = new SpriteAnimation(_gameSystem, AnimationKey.Idle);
        }

        public AnimationState Update(AnimationKey animationKey)
        {
            if (_spriteAnimation.Key != animationKey)
                _spriteAnimation.ChangeKey(animationKey);
             
            var animation = _animations[animationKey];
            bool newFrame = false;
            bool finished = false;

            if (_spriteAnimation.FramesRemaining == 0)
            {
                _spriteAnimation.CurrentIndex.Inc();
                newFrame = true;
            }

            if (_spriteAnimation.CurrentIndex >= animation.Frames.Length)
            {
                finished = _spriteAnimation.CurrentIndex == animation.Frames.Length;
                _spriteAnimation.CurrentIndex.Set(0);
                newFrame = true;
            }

            var animationFrame = animation.Frames[_spriteAnimation.CurrentIndex];
            if (newFrame)
                _spriteAnimation.FramesRemaining.Set(animationFrame.Duration);
            else
                _spriteAnimation.FramesRemaining.Dec();

            if (finished)
                return AnimationState.Finished;
            else
                return AnimationState.Playing;
        }

        public SpriteFrame GetCurrentFrame(AnimationKey animationKey)
        {
            var animation = _animations[animationKey];
            var animationFrame = animation.Frames[_spriteAnimation.CurrentIndex];
            return _spriteFrames[animationFrame.SpriteFrameIndex];
        }

        public void ApplyToSprite(Sprite sprite, AnimationKey animationKey)
        {
            var spriteFrame = GetCurrentFrame(animationKey);
            sprite.SetTiles(
                topLeft: spriteFrame.TopLeft,
                topRight: spriteFrame.TopRight,
                bottomLeft: spriteFrame.BottomLeft,
                bottomRight: spriteFrame.BottomRight);
        }


    }
}
