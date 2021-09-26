using SomeGame.Main.Models;

namespace SomeGame.Main.Services
{
    class SceneManager
    {
        public Scene CurrentScene { get; private set; }

        public void SetScene(Scene scene)
        {
            CurrentScene = scene;
        }

        public void Update()
        {
            CurrentScene.Update();
        }
    }
}
