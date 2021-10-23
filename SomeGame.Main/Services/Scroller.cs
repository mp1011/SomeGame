using Microsoft.Xna.Framework;
using SomeGame.Main.Models;
using SomeGame.Main.Scenes;

namespace SomeGame.Main.Services
{
    class Scroller
    {
        private readonly GameSystem _gameSystem;
        public GameRectangle Camera { get; private set; }

        public Scroller(GameSystem gameSystem)
        {
            _gameSystem = gameSystem;
            SetCameraBounds(gameSystem.Screen);
        }

        public void SetCameraBounds(Rectangle bounds)
        {
            Camera = new BoundedGameRectangle(bounds.X, bounds.Y, _gameSystem.Screen.Width, _gameSystem.Screen.Height,
                    maxX: bounds.Width - _gameSystem.Screen.Width,
                    maxY: bounds.Height - _gameSystem.Screen.Height);
        }


        public void Update()
        {
            var bg = _gameSystem.GetLayer(LayerIndex.BG);
            var fg = _gameSystem.GetLayer(LayerIndex.FG);
            ScrollLayer(bg);
            ScrollLayer(fg);
        }

        public void ScrollActor(Actor actor, Sprite sprite)
        {
            var actorScreenX = sprite.ScrollX.Set(actor.WorldPosition.X - Camera.X);
            var actorScreenY = sprite.ScrollX.Set(actor.WorldPosition.Y - Camera.Y);

            sprite.ScrollX = actorScreenX - actor.LocalHitbox.X;
            sprite.ScrollY = actorScreenY - actor.LocalHitbox.Y;
        }

        private void ScrollLayer(Layer layer)
        {
            var camera = Camera;

            if (layer.ScrollFactor == 100)
            {
                layer.ScrollX = layer.ScrollX.Set(-camera.X);
                layer.ScrollY = layer.ScrollY.Set(-camera.Y);
            }
            else if (layer.ScrollFactor > 0)
            {
                var factor = layer.ScrollFactor / -100.0;
                layer.ScrollX = layer.ScrollX.Set(factor * camera.X);
                layer.ScrollY = layer.ScrollY.Set(factor * camera.Y);
            }
        }

    }
}
