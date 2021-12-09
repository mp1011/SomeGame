namespace SomeGame.Main.SceneControllers
{
    interface ISceneController
    {
        void Update();
    }

    class EmptySceneController : ISceneController
    {
        public void Update() { }
    }
}
