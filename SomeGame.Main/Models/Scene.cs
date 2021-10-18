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

            ScrollLayer(bg);
            ScrollLayer(fg);
        }

        private void ScrollLayer(Layer layer)
        {
            if(layer.ScrollFactor == 100)
            {
                layer.ScrollX = layer.ScrollX.Set(-Camera.X);
                layer.ScrollY = layer.ScrollY.Set(-Camera.Y);
            }
            else if(layer.ScrollFactor > 0)
            {
                var factor = layer.ScrollFactor / -100.0;
                layer.ScrollX = layer.ScrollX.Set(factor * Camera.X);
                layer.ScrollY = layer.ScrollY.Set(factor * Camera.Y);
            }
        }


    }
}
