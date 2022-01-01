using Microsoft.Xna.Framework;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class CameraBehavior : Behavior
    {
        private readonly Scroller _scroller;
        private readonly GameSystem _gameSystem;

        public CameraBehavior(Scroller scroller, GameSystem gameSystem)
        {
            _scroller = scroller;
            _gameSystem = gameSystem;
        }

        protected override void DoUpdate()
        {
            _scroller.Camera.X = Actor.WorldPosition.X.Pixel - (_gameSystem.Screen.Width / 2);
            _scroller.Camera.Y = Actor.WorldPosition.Y.Pixel - (_gameSystem.Screen.Height / 2);
        }
    }
}
