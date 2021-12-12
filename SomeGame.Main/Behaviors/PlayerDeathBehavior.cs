using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class PlayerDeathBehavior : IDestroyedBehavior
    {
        private readonly SceneManager _sceneManager;
        private readonly RamByte _timer;

        public PlayerDeathBehavior(GameSystem gameSystem, SceneManager sceneManager)
        {
            _timer = gameSystem.RAM.DeclareByte();
            _sceneManager = sceneManager;
        }

        public void OnDestroyed(Actor actor)
        {
            _timer.Set(0);
            actor.CurrentAnimation = AnimationKey.Hurt;
            actor.MotionVector.Set(new PixelPoint(0, 0));
        }

        public DestroyedState Update(Actor actor)
        {
            _timer.Inc();
            if (_timer < 200)
                return DestroyedState.Destroying;
            else
            {
                _sceneManager.RestartCurrentScene();
                return DestroyedState.Destroyed;
            }
        }
    }
}
