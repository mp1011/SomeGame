using SomeGame.Main.Models;

namespace SomeGame.Main.Services
{
    class PlayerStateManager
    {
        public PlayerState CurrentState { get; private set; }

        public PlayerStateManager()
        {
            ResetPlayerState();
        }

        public void ResetPlayerState()
        {
            CurrentState = new PlayerState { Health = new BoundedInt(12, 12), Lives = 3, Score = 0 };
        }
    }
}
