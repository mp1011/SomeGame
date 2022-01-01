using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class PlatformerPlayerMotionBehavior : Behavior
    {
        private readonly InputManager _inputManger;
        private readonly AudioService _audioService;
        private RamByte _exJumpCounter;
        private RamEnum<InputQueue> _inputQueue;
        
        private bool _jumpQueued
        {
            get => _inputQueue.GetFlag(InputQueue.Jump);
            set => _inputQueue.SetFlag(InputQueue.Jump, value);
        }

        public PlatformerPlayerMotionBehavior(GameSystem gameSystem, InputManager inputManger, AudioService audioService,
            RamEnum<InputQueue> inputQueue)
        {
            _exJumpCounter = gameSystem.RAM.DeclareByte();
            _inputManger = inputManger;
            _audioService = audioService;
            _inputQueue = inputQueue;
        }

        protected override void DoUpdate()
        {
        }

        protected override void OnCollision(CollisionInfo backgroundCollisionInfo)
        {
            if (Actor.CurrentAnimation == AnimationKey.Attacking && !Actor.IsAnimationFinished)
            {
                if (_inputManger.Input.A.IsPressed() && backgroundCollisionInfo.IsOnGround)
                    _jumpQueued = true;

                Actor.MotionVector.X.Set(0);
                return;
            }

            if (backgroundCollisionInfo.XCorrection != 0)
                Actor.MotionVector.X.Set(0);

            if (backgroundCollisionInfo.YCorrection != 0)
                Actor.MotionVector.Y.Set(0);

            if ((_jumpQueued || _inputManger.Input.A.IsPressed()) && backgroundCollisionInfo.IsOnGround)
            {
                _audioService.Play(SoundContentKey.Jump);
                _jumpQueued = false;
                Actor.MotionVector.Y.Set(new PixelValue(-2, 0));
                _exJumpCounter.Set(15);
            }

            if (!_inputManger.Input.A.IsDown())
                _exJumpCounter.Set(0);
            else if(_exJumpCounter > 0)
            {
                _exJumpCounter.Dec();
                Actor.MotionVector.Y.Set(new PixelValue(-2, 0));
            }

            bool isMoving = false;

            if (_inputManger.Input.Left.IsDown())
            {
                Actor.MotionVector.X.Set(-1);
                Actor.Flip = Flip.FlipH;
                isMoving = true;
            }
            else if (_inputManger.Input.Right.IsDown())
            {
                Actor.MotionVector.X.Set(1);
                Actor.Flip = Flip.None;
                isMoving = true;
            }
            else
            {
                Actor.MotionVector.X.Set(0);
            }

            if(Actor.CurrentAnimation != AnimationKey.Attacking && Actor.CurrentAnimation != AnimationKey.Hurt)
                Actor.CurrentAnimation = GetAnimation(isMoving, backgroundCollisionInfo.IsOnGround);
        }

        private AnimationKey GetAnimation(bool isMoving, bool isOnGround)
        {
            if (!isOnGround)
                return AnimationKey.Jumping;
            else if (isMoving)
                return AnimationKey.Moving;
            else
                return AnimationKey.Idle;
        }
    }
}
