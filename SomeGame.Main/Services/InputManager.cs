using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Models;

namespace SomeGame.Main.Services
{
    class InputManager
    {
        private readonly GameSystem _gameSystem;

        public InputModel Input { get; private set; } = new InputModel(ButtonState.None, ButtonState.None, ButtonState.None, ButtonState.None, ButtonState.None, ButtonState.None, ButtonState.None, 0, 0);
        private double _mouseScale = 1.0;

        public InputManager(GameSystem gameSystem)
        {
            _gameSystem = gameSystem;
        }

        public void AdjustMouseScale(GameSystem gameSystem, Viewport viewport)
        {
            _mouseScale = gameSystem.Screen.Width / (double)viewport.Width;
        }

        public void Update()
        {
            Input = _gameSystem.Input.ReadInput(_mouseScale);
        }
    }
}
