using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.RasterEffects;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class PlayerDeathBehavior : IDestroyedBehavior
    {
        private readonly SceneManager _sceneManager;
        private readonly RamByte _timer;
        private readonly PlayerStateManager _playerStateManager;
        private readonly AudioService _audioService;
        private readonly RasterBasedRenderService _rasterBasedRenderService;
        private readonly Dissolve _dissolve;

        public PlayerDeathBehavior(GameSystem gameSystem, SceneManager sceneManager,
            PlayerStateManager playerStateManager, AudioService audioService, RasterBasedRenderService rasterBasedRenderService)
        {
            _timer = gameSystem.RAM.DeclareByte();
            _playerStateManager = playerStateManager;
            _sceneManager = sceneManager;
            _audioService = audioService;
            _rasterBasedRenderService = rasterBasedRenderService;
            _dissolve = new Dissolve();
        }

        public void OnDestroyed(Actor actor)
        {
            _timer.Set(0);

            _playerStateManager.CurrentState.Lives.Dec();
            _audioService.StopMusic();
            actor.CurrentAnimation = AnimationKey.Hurt;
            actor.MotionVector.Set(new PixelPoint(0, 0));

            _rasterBasedRenderService.SetEffect(_dissolve);
            _dissolve.Amount = 0;
        }

        public DestroyedState Update(Actor actor)
        {
            _timer.Inc();

            if (_timer > 100)
            {
                if (_dissolve.Amount < 252)
                    _dissolve.Amount += 3;
                else
                    _dissolve.Amount = 255;
            }

            if (_timer == 50)
                _audioService.Play(SoundContentKey.PlayerLose);
            
            
            if (_timer < 250)
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
