using SomeGame.Main.Models;

namespace SomeGame.Main.Services
{
    class PlayerStateManager
    {
        public PlayerState CurrentState { get; private set; }

        public PlayerStateManager(GameSystem gameSystem)
        {
            CurrentState = new PlayerState(gameSystem);
            ResetPlayerState();
        }

        public void ResetPlayerState()
        {
            CurrentState.Health.Max = 12;
            CurrentState.Health.Set(12);
            CurrentState.Lives.Set(3);
            CurrentState.Score.Set(0);
        }
    }
}
