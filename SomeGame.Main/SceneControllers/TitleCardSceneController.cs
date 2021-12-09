using SomeGame.Main.Content;
using SomeGame.Main.Extensions;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.SceneControllers
{
    class TitleCardSceneController : ISceneController
    {
        private readonly SceneContentKey _nextScene;
        private readonly InputManager _inputManager;
        private readonly SceneManager _sceneManager;

        public TitleCardSceneController(SceneContentKey nextScene, InputManager inputManager, SceneManager sceneManager)
        {
            _nextScene = nextScene;
            _inputManager = inputManager;
            _sceneManager = sceneManager;
        }

        public void Update()
        {
            if(_inputManager.Input.A.IsDown() 
                || _inputManager.Input.B.IsDown() 
                || _inputManager.Input.Start.IsDown())
            {
                _sceneManager.QueueNextScene(_nextScene);
            }

        }
    }
}
