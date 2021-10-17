using Microsoft.Xna.Framework;

namespace SomeGame.Main.Models
{
    class Scene
    {
        private readonly GameSystem _gameSystem;

        public Rectangle Bounds { get; }
        public GameRectangle Camera { get; }

        public Scene(Rectangle bounds, GameSystem gameSystem)
        {
            _gameSystem = gameSystem;
            Bounds = bounds;
            Camera = new BoundedGameRectangle(bounds.X, bounds.Y, gameSystem.Screen.Width, gameSystem.Screen.Height, 
                maxX: bounds.Width - gameSystem.Screen.Width,
                maxY: bounds.Height - gameSystem.Screen.Height);
        }

        public void Update()
        {
            var bg = _gameSystem.GetLayer(LayerIndex.BG);
            var fg = _gameSystem.GetLayer(LayerIndex.FG);

            bg.ScrollX = bg.ScrollX.Set(-0.7 * Camera.X);
            bg.ScrollY = bg.ScrollY.Set(-0.7 * Camera.Y);
           
            fg.ScrollX = fg.ScrollX.Set(-Camera.X);
            fg.ScrollY = fg.ScrollY.Set(-Camera.Y);
        }


    }
}
