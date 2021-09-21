using SomeGame.Main.Models;

namespace SomeGame.Main.Extensions
{
    public static class ButtonStateExtensions
    {
        public static bool IsDown(this ButtonState buttonState) => (buttonState & ButtonState.Down) != 0;
        public static bool IsUp(this ButtonState buttonState) => (buttonState & ButtonState.Down) == 0;
        public static bool IsChanged(this ButtonState buttonState) => (buttonState & ButtonState.Changed) != 0;

        public static bool IsPressed(this ButtonState buttonState) => buttonState.IsDown() && buttonState.IsChanged();
        public static bool IsReleased(this ButtonState buttonState) => buttonState.IsUp() && buttonState.IsChanged();
    }
}
