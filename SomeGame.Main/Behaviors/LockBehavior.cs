using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class LockBehavior : GizmoBlockBehavior
    {
        private readonly PlayerStateManager _playerStateManager;

        public LockBehavior(GameSystem gameSystem, Scroller scroller, AudioService audioService, PlayerStateManager playerStateManager) 
            : base(gameSystem, scroller, audioService)
        {
            _playerStateManager = playerStateManager;
        }

        protected override void DoUpdate()
        {
        }

        protected override void OnBlockCreated(Actor actor)
        {
            SetTiles(actor, true);
        }

        protected override void OnCollision(CollisionInfo collisionInfo)
        {
            if(collisionInfo.Actor != null && _playerStateManager.CurrentState.HasKey)
            {
                _playerStateManager.CurrentState.HasKey = false;
                SetTiles(Actor, false);
                Actor.Destroy();
            }
        }
    }
}
