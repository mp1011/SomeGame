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

        public override void Update(Actor actor)
        {
            actor.MotionVector = GetVectorFromInput(_inputManger.Input);
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
