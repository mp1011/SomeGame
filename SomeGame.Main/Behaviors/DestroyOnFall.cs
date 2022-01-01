using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class DestroyOnFall : Behavior
    {
        private readonly SceneManager _sceneManager;

        public DestroyOnFall(SceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        protected override void DoUpdate()
        {
            if (Actor.WorldPosition.Top() > _sceneManager.CurrentScene.Bounds.Bottom)
                Actor.Destroy();
        }
    }
}
