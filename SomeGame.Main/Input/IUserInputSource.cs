using Microsoft.Xna.Framework;
using SomeGame.Main.Models;
using ButtonState = SomeGame.Main.Models.ButtonState;

namespace SomeGame.Main.Input
{
    interface IUserInputSource
    {
        void Initialize(Rectangle screenSize);
        InputModel ReadInput(double mouseScale);       
    }

    abstract class InputSourceBase<TIn> : IUserInputSource
    {
        protected Rectangle ScreenSize { get; private set; }

        protected InputBinding<TIn> Bindings { get; private set; }

        protected abstract InputBinding<TIn> SetBindings();

        public void Initialize(Rectangle screenSize)
        {
            ScreenSize = screenSize;
            Bindings = SetBindings();
        }

        protected ButtonState GetButtonState(ButtonState oldState, bool isPressed)
        {
            bool wasPressed = (oldState & ButtonState.Down) != 0;

            var state = ButtonState.None;
            if (isPressed)
                state |= ButtonState.Down;
            if (wasPressed != isPressed)
                state |= ButtonState.Changed;
            return state;
        }

        public abstract InputModel ReadInput(double mouseScale);
    }
}
