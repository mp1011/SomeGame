using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using System;

namespace SomeGame.Main.Editor
{
    class UIBlockSelect
    {
        private Point? _dragStart;
        private Point? _dragEnd;
        private Action<Point, Point> _action;

        public bool HasSelection => _dragEnd != null;

        public UIBlockSelect(Action<Point, Point> action)
        {
            _action = action;
        }

        public void Update(Point mouseTile, InputModel input, Layer foreground, Layer background)
        {
            if (input.A.IsPressed())
            {
                _dragStart = mouseTile;
                _dragEnd = null;
            }
            else if (input.A.IsReleased() && _dragStart.HasValue)
            {
                _dragEnd = mouseTile;
            }

            if (input.A.IsDown() && _dragStart.HasValue)
                ShowDragArea(_dragStart.Value, mouseTile, foreground, background);

            if (input.B.IsPressed() && _dragEnd.HasValue)
                _action(_dragStart.Value, _dragEnd.Value);
        }

        public void ClearSelection(Layer foreground)
        {
            _dragStart = null;
            _dragEnd = null;
            foreground.TileMap.SetEach((x, y) => new Tile(-1, TileFlags.None));
        }

        private void ShowDragArea(Point start, Point end, Layer foreground, Layer background)
        {
            foreground.TileMap.SetEach((x, y) =>
            {
                if (x >= start.X
                    && x <= end.X
                    && y >= start.Y
                    && y <= end.Y)
                {
                    return background.TileMap.GetTile(x, y);
                }
                else
                {
                    return new Tile(-1, TileFlags.None);
                }
            });
        }

    }
}
