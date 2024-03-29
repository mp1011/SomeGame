﻿using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class EightDirPlayerMotionBehavior : Behavior
    {
        private readonly InputManager _inputManger;
        
        public EightDirPlayerMotionBehavior(InputManager inputManger)
        {
            _inputManger = inputManger;
        }

        protected override void DoUpdate()
        {
            Actor.MotionVector.Set(GetVectorFromInput(_inputManger.Input));

            if (Actor.MotionVector.X == 0 && Actor.MotionVector.Y == 0)
                Actor.CurrentAnimation = AnimationKey.Idle;
            else
                Actor.CurrentAnimation = AnimationKey.Moving;
        }

        private PixelPoint GetVectorFromInput(InputModel input)
        {
            int x = 0, y = 0;
            if (input.Right.IsDown())
                x = 1;
            else if (input.Left.IsDown())
                x = -1;
            if (input.Down.IsDown())
                y = 1;
            else if (input.Up.IsDown())
                y = -1;

            return new PixelPoint(x, y);
        }
    }
}
