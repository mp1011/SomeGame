﻿using SomeGame.Main.Models;
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
        }


        public AnimationState Update(SpriteIndex spriteIndex, AnimationKey animationKey, bool needsSprite)
        {
            if(_spriteAnimation == null || _spriteAnimation.Key != animationKey)
                _spriteAnimation = new SpriteAnimation(animationKey);
             
            var animation = _animations[animationKey];
            bool newFrame = false;
            bool finished = false;

            if (_spriteAnimation.FramesRemaining == 0)
            {
                _spriteAnimation.CurrentIndex++;
                newFrame = true;
            }

            if (_spriteAnimation.CurrentIndex >= animation.Frames.Length)
            {
                finished = _spriteAnimation.CurrentIndex == animation.Frames.Length;
                _spriteAnimation.CurrentIndex = 0;
                newFrame = true;
            }

            var animationFrame = animation.Frames[_spriteAnimation.CurrentIndex];
            var spriteFrame = _spriteFrames[animationFrame.SpriteFrameIndex];

            if (newFrame)
                _spriteAnimation.FramesRemaining = animationFrame.Duration;         
            else
                _spriteAnimation.FramesRemaining--;


            if (!needsSprite)
            {
                _gameSystem.GetSprite(spriteIndex).SetTiles(
                    topLeft: spriteFrame.TopLeft,
                    topRight: spriteFrame.TopRight,
                    bottomLeft: spriteFrame.BottomLeft,
                    bottomRight: spriteFrame.BottomRight);
            }

            if (finished)
                return AnimationState.Finished;
            else
                return AnimationState.Playing;
        }


    }
}
