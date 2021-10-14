using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class PlatformerPlayerMotionBehavior : Behavior
    {
        private readonly InputManager _inputManger;

        public PlatformerPlayerMotionBehavior(InputManager inputManger)
        {
            _inputManger = inputManger;
        }

        public override void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo backgroundCollisionInfo)
        {
            if (backgroundCollisionInfo.XCorrection != 0)
                actor.MotionVector = new PixelPoint(0, actor.MotionVector.Y);

            if (backgroundCollisionInfo.YCorrection != 0)
                actor.MotionVector = new PixelPoint(actor.MotionVector.X, 0);

            if (_inputManger.Input.A.IsPressed() && backgroundCollisionInfo.IsOnGround)
                actor.MotionVector = new PixelPoint(actor.MotionVector.X, -2);

            bool isMoving = false;

            if (_inputManger.Input.Left.IsDown())
            {
                actor.MotionVector = new PixelPoint(-1, actor.MotionVector.Y);
                actor.Flip = Flip.H;
                isMoving = true;
            }
            else if (_inputManger.Input.Right.IsDown())
            {
                actor.MotionVector = new PixelPoint(1, actor.MotionVector.Y);
                actor.Flip = Flip.None;
                isMoving = true;
            }
            else
            {
                actor.MotionVector = new PixelPoint(0, actor.MotionVector.Y);
            }

            if(actor.CurrentAnimation != AnimationKey.Attacking && actor.CurrentAnimation != AnimationKey.Hurt)
                actor.CurrentAnimation = GetAnimation(isMoving, backgroundCollisionInfo.IsOnGround);
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
