using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class PlayerDeathBehavior : IDestroyedBehavior
    {
        private readonly SceneManager _sceneManager;
        private readonly RamByte _timer;
        private readonly PlayerStateManager _playerStateManager;

        public PlayerDeathBehavior(GameSystem gameSystem, SceneManager sceneManager,
            PlayerStateManager playerStateManager)
        {
            _timer = gameSystem.RAM.DeclareByte();
            _playerStateManager = playerStateManager;
            _sceneManager = sceneManager;
        }

        public void OnDestroyed(Actor actor)
        {
            _timer.Set(0);

            _playerStateManager.CurrentState.Lives.Dec();

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
                if (_playerStateManager.CurrentState.Lives == 0)
                    _sceneManager.QueueNextScene(SceneContentKey.GameOver);
                else
                    _sceneManager.RestartCurrentScene();

                return DestroyedState.Destroyed;
            }
        }
    }
}
