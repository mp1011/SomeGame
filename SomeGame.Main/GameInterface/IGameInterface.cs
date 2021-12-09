namespace SomeGame.Main.GameInterface
{
    public interface IGameInterface
    {
        void Update();
    }

    class EmptyGameInterface : IGameInterface
    {
        public void Update()
        {
        }
    }
}
