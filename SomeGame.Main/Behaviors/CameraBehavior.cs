using Microsoft.Xna.Framework;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class CameraBehavior : Behavior
    {
        private readonly SceneManager _sceneManager;
        private readonly GameSystem _gameSystem;

        public CameraBehavior(SceneManager sceneManager, GameSystem gameSystem)
        {
            _sceneManager = sceneManager;
            _gameSystem = gameSystem;
        }

        public override void Update(Actor actor, Rectangle frameStartPosition)
        {
            var scene = _sceneManager.CurrentScene;
            scene.Camera.X = actor.WorldPosition.X - _gameSystem.Screen.Width / 2;
            scene.Camera.Y = actor.WorldPosition.Y - _gameSystem.Screen.Height / 2;
        }
    }
}
