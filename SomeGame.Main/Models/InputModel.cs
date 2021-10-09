using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;

namespace SomeGame.Main.Models
{
    record InputModel(ButtonState Up, ButtonState Down, ButtonState Left, ButtonState Right,
        ButtonState Start, ButtonState A, ButtonState B, int MouseX, int MouseY)
    {
        public Point HeldDirectionVector
        {
            get
            {
                int y = 0;
                if (Up.IsDown())
                    y = -1;
                else if (Down.IsDown())
                    y = 1;

                int x = 0;
                if (Left.IsDown())
                    x = -1;
                else if (Right.IsDown())
                    x = 1;

                return new Point(x, y);
            }
        }

        public Point PressedDirectionVector
        {
            get
            {
                int y = 0;
                if (Up.IsPressed())
                    y = -1;
                else if (Down.IsPressed())
                    y = 1;

                int x = 0;
                if (Left.IsPressed())
                    x = -1;
                else if (Right.IsPressed())
                    x = 1;

                return new Point(x, y);
            }
        }
    }
}
