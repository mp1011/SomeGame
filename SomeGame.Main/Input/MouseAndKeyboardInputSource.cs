using Microsoft.Xna.Framework.Input;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using ButtonState = SomeGame.Main.Models.ButtonState;
using XNAButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace SomeGame.Main.Input
{


    class MouseAndKeyboardInputSource : InputSourceBase<Keys>
    {
        private InputModel _lastInput = new InputModel(
            ButtonState.None, ButtonState.None, ButtonState.None, ButtonState.None,
            ButtonState.None, ButtonState.None, ButtonState.None, 0, 0);

        protected override InputBinding<Keys> SetBindings()
        {
            var bindings = new InputBinding<Keys>();
            bindings.SetBinding(Keys.Up, InputButton.Up);
            bindings.SetBinding(Keys.Down, InputButton.Down);
            bindings.SetBinding(Keys.Left, InputButton.Left);
            bindings.SetBinding(Keys.Right, InputButton.Right);
            bindings.SetBinding(Keys.Enter, InputButton.Start);
            bindings.SetBinding(Keys.A, InputButton.A);
            bindings.SetBinding(Keys.S, InputButton.B);
            return bindings;
        }

        private ButtonState GetButtonState(KeyboardState keyboardState, ButtonState lastState, InputButton inputButton)
        {
            bool isPressed = keyboardState.IsKeyDown(Bindings[inputButton]);
            return GetButtonState(lastState, isPressed);
        }

        private ButtonState GetButtonState(KeyboardState keyboardState, XNAButtonState mouseButtonState, ButtonState lastState, InputButton inputButton)
        {
            bool isPressed = mouseButtonState == XNAButtonState.Pressed || keyboardState.IsKeyDown(Bindings[inputButton]);
            return GetButtonState(lastState, isPressed);
        }


        public override InputModel ReadInput(double mouseScale)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            var newInput = new InputModel(
                Up: GetButtonState(keyboardState, _lastInput.Up, InputButton.Up),
                Down: GetButtonState(keyboardState, _lastInput.Down, InputButton.Down),
                Left: GetButtonState(keyboardState, _lastInput.Left, InputButton.Left),
                Right: GetButtonState(keyboardState, _lastInput.Right, InputButton.Right),
                Start: GetButtonState(keyboardState, _lastInput.Start, InputButton.Start),
                A: GetButtonState(keyboardState, mouseState.LeftButton, _lastInput.A, InputButton.A),
                B: GetButtonState(keyboardState, mouseState.RightButton, _lastInput.B, InputButton.B),
                MouseX: (mouseState.X * mouseScale).Clamp(0, ScreenSize.Width),
                MouseY: (mouseState.Y * mouseScale).Clamp(0, ScreenSize.Height));

            _lastInput = newInput;
            return newInput;
        }
    }
}
