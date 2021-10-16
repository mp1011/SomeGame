using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Editor
{
    class UIButton
    {
        public string Text { get; }
        public Point Location { get; }

        public UIButton(string text, Point location, Layer layer, Font font)
        {
            Text = text;
            Location = location;

            font.WriteToLayer(text, layer, location);
        }

        private bool CheckMouseOver(Point mouseTile)
        {
            return mouseTile.X >= Location.X
                && mouseTile.X < Location.X + Text.Length
                && mouseTile.Y == Location.Y;                               
        }

        public UIButtonState Update(Layer interfaceLayer, InputModel input)
        {
            var mouseTile = interfaceLayer.TilePointFromScreenPixelPoint(input.MouseX, input.MouseY);

            if (!CheckMouseOver(mouseTile))
                return UIButtonState.None;

            if (input.A.IsPressed())
                return UIButtonState.Pressed;
            else
                return UIButtonState.MouseOver;
        }
    }
}
