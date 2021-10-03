using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
namespace SomeGame.Main.Modules
{
    abstract class TileEditorBaseModule : GameModuleBase
    {
        protected readonly DataSerializer _dataSerializer;
        private Point _lastMouseTile;

        public TileEditorBaseModule()
        {
            _dataSerializer = new DataSerializer();
        }

        protected Tile SelectedTile { get; set; } = new Tile(1, TileFlags.None);

        protected override Palette CreatePalette(IndexedTilesetImage[] tilesetImages, PaletteIndex index)
        {
            return tilesetImages[0].Palette;
        }

        protected Point GetCurrentMouseTile()
        {
            var foreground = GameSystem.GetLayer(LayerIndex.FG);
            return foreground.TilePointFromScreenPixelPoint(Input.MouseX, Input.MouseY);
        }

        protected void HandleStandardInput()
        {
            var foreground = GameSystem.GetLayer(LayerIndex.FG);
            var background = GameSystem.GetLayer(LayerIndex.BG);
            var mouseTile = GetCurrentMouseTile();

            if (mouseTile.Y < 2)
                return;

            foreground.TileMap.SetEach((x, y) => {
                if (x == mouseTile.X && y == mouseTile.Y)
                    return SelectedTile;
                else
                    return new Tile(-1, TileFlags.None);
            });

            if (Input.A.IsDown())
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
                MouseTileChanged(mouseTile);
                _lastMouseTile = mouseTile;
            }
        }

        protected virtual void AfterTilePlaced(Point location)
        {
        }

        protected virtual void MouseTileChanged(Point mouseTile) { }

    }
}
