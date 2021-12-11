using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class DestroyOnFall
    {
        private readonly SceneManager _sceneManager;

        public DestroyOnFall(SceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        public void Update(Actor a)
        {
            if (a.WorldPosition.Top() > _sceneManager.CurrentScene.Bounds.Bottom)
                a.Destroy();
        }
    }
}
