using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
namespace SomeGame.Main.Modules
{
    abstract class TileEditorBaseModule : EditorModule
    {
        private Point _lastMouseTile;

        protected TileEditorBaseModule(GameStartup gameStartup) 
            : base(gameStartup)
        {
        }

        protected Tile SelectedTile { get; set; } = new Tile(1, TileFlags.None);

        protected Point GetCurrentMouseTile(LayerIndex layerIndex)
        {
            var layer = GameSystem.GetLayer(layerIndex);
            return layer.TilePointFromScreenPixelPoint(Input.MouseX, Input.MouseY);
        }

        protected void HandleStandardInput()
        {
            if (GetCurrentMouseTile(LayerIndex.Interface).Y < 2)
                return;

            var foreground = GameSystem.GetLayer(LayerIndex.FG);
            var background = GameSystem.GetLayer(LayerIndex.BG);
            var mouseTile = GetCurrentMouseTile(LayerIndex.BG);

            foreground.TileMap.SetEach((x, y) => {
                if (x == mouseTile.X && y == mouseTile.Y)
                    return SelectedTile;
                else
                    return new Tile(-1, TileFlags.None);
            });

            var existingTile = background.TileMap.GetTile(mouseTile.X, mouseTile.Y);

            if (Input.A.IsPressed())
            {
                if (existingTile.Index == SelectedTile.Index)
                {
                    background.TileMap.SetTile(mouseTile.X, mouseTile.Y, existingTile.NextFlip());
                    AfterTilePlaced(mouseTile);
                }
            }

            if (Input.A.IsDown() && existingTile.Index != SelectedTile.Index)
            {
                background.TileMap.SetTile(mouseTile.X, mouseTile.Y, SelectedTile);
                AfterTilePlaced(mouseTile);
            }
            if (Input.B.IsDown())
            {
                background.TileMap.SetTile(mouseTile.X, mouseTile.Y, new Tile(-1, TileFlags.None));
                AfterTilePlaced(mouseTile);
            }

            if (_lastMouseTile != mouseTile)
            {
                OnMouseTileChanged(mouseTile);
                _lastMouseTile = mouseTile;
            }
        }

        protected virtual void AfterTilePlaced(Point location)
        {
        }

        protected virtual void OnMouseTileChanged(Point mouseTile) { }

    }
}
