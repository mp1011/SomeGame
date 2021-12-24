using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.SceneControllers
{
    class PlayableSceneController : ISceneController
    {
        private readonly InputManager _inputManger;
        private readonly AudioService _audioService;
        private readonly GameSystem _gameSystem;

        public PlayableSceneController(GameSystem gameSystem, InputManager inputManger, AudioService audioService)
        {
            _gameSystem = gameSystem;
            _inputManger = inputManger;
            _audioService = audioService;
        }

        public void Update()
        {
            if(_inputManger.Input.Start.IsPressed())
            {
                _audioService.Play(SoundContentKey.Pause);
                _gameSystem.Paused = !_gameSystem.Paused;
                if (_gameSystem.Paused)
                    _audioService.PauseMusic();
                else
                    _audioService.ResumeMusic();
            }
        }
    }
}
