using Microsoft.Xna.Framework;
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

        public override void Update(Actor actor, Rectangle frameStartPosition)
        {
            actor.MotionVector = GetVectorFromInput(_inputManger.Input);

            if (actor.MotionVector.X == 0 && actor.MotionVector.Y == 0)
                actor.CurrentAnimation = AnimationKey.Idle;
            else
                actor.CurrentAnimation = AnimationKey.Moving;
        }

        private Point GetVectorFromInput(InputModel input)
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

            return new Point(x, y);
        }
    }
}
