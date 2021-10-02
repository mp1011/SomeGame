using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class PlatformerPlayerMotionBehavior : Behavior
    {
        private readonly InputManager _inputManger;

        public PlatformerPlayerMotionBehavior(InputManager inputManger)
        {
            _inputManger = inputManger;
        }

        public override void Update(Actor actor, Rectangle frameStartPosition)
        {
            if (_inputManger.Input.A.IsPressed())
                actor.MotionVector = new PixelPoint(actor.MotionVector.X, -2);

            if (_inputManger.Input.Left.IsDown())
            {
                actor.MotionVector = new PixelPoint(-1, actor.MotionVector.Y);
                actor.Flip = Flip.H;
                actor.CurrentAnimation = AnimationKey.Moving;
            }
            else if (_inputManger.Input.Right.IsDown())
            {
                actor.MotionVector = new PixelPoint(1, actor.MotionVector.Y);
                actor.Flip = Flip.None;
                actor.CurrentAnimation = AnimationKey.Moving;
            }
            else
            {
                actor.MotionVector = new PixelPoint(0, actor.MotionVector.Y);
                actor.CurrentAnimation = AnimationKey.Idle;
            }
        }
    }
}
