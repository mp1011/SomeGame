using Microsoft.Xna.Framework;

namespace SomeGame.Main.Models
{
    class Scene
    {
        public Rectangle Bounds { get; }
        public GameRectangle Camera { get; }

        public Scene(Rectangle bounds, GameSystem gameSystem)
        {
            Bounds = bounds;
            Camera = new GameRectangle(0, 0, gameSystem.Screen.Width, gameSystem.Screen.Height);
        }

        public void Update(GameSystem gameSystem)
        {
            var bg = gameSystem.GetLayer(LayerIndex.BG);
            var fg = gameSystem.GetLayer(LayerIndex.FG);

            bg.ScrollX = bg.ScrollX.Set(-Camera.X);
            bg.ScrollY = bg.ScrollY.Set(-Camera.Y);
            fg.ScrollX = fg.ScrollX.Set(-Camera.X);
            fg.ScrollY = fg.ScrollY.Set(-Camera.Y);
        }


    }
}
