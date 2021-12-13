using SomeGame.Main.Models;

namespace SomeGame.Main.Services
{
    class PlayerStateManager
    {
        public PlayerState CurrentState { get; }

        public PlayerStateManager(GameSystem gameSystem)
        {
            CurrentState = new PlayerState(gameSystem);
            ResetPlayerState();
        }

        public void ResetPlayerState()
        {
            ResetPlayerHealth();
            CurrentState.Lives.Set(3);
            CurrentState.Score.Set(0);
        }

        public void ResetPlayerHealth()
        {
            CurrentState.Health.Max = 12;
            CurrentState.Health.Set(12);
        }
    }
}
