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

        public override void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo collisionInfo)
        {
            _scroller.Camera.X = actor.WorldPosition.X - _gameSystem.Screen.Width / 2;
            _scroller.Camera.Y = actor.WorldPosition.Y - _gameSystem.Screen.Height / 2;
        }
    }
}
