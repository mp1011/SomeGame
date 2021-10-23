using SomeGame.Main.Models;
using SomeGame.Main.Scenes;

namespace SomeGame.Main.Services
{
    class SceneManager
    {
        private readonly GameSystem _gameSystem;

        private Scene _nextScene;
        public Scene CurrentScene { get; private set; }

        public SceneManager(GameSystem gameSystem)
        {
            _gameSystem = gameSystem;
        }

        public void QueueNextScene(Scene scene)
        {
            _nextScene = scene;
        }

        public void RestartCurrentScene() => QueueNextScene(CurrentScene);

       
        public SceneUpdateResult Update()
        {
            if (_nextScene != null)
            {
                CurrentScene = _nextScene;
                _nextScene = null;
                return new SceneUpdateResult(LoadScene: CurrentScene);
            }

            return new SceneUpdateResult();
        }
    }
}
