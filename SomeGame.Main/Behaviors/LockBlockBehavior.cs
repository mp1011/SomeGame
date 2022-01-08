using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class LockBlockBehavior : GizmoBlockBehavior
    {
        private ActorManager _actorManager;
        private Actor _lock;

        public LockBlockBehavior(GameSystem gameSystem, Scroller scroller, AudioService audioService, ActorManager actorManager)
            : base(gameSystem, scroller, audioService)
        {
            _actorManager = actorManager;
        }

        protected override void DoUpdate()
        {
            if (_lock != null && _lock.State == ActorState.Destroyed)
            {
                SetTiles(Actor, false);
                Actor.Destroy();
            }
        }

        protected override void OnBlockCreated(Actor actor)
        {
            if (_lock == null)
            {
                var lockFinder = new LockFinder(_actorManager, actor.WorldPosition);
                _lock = lockFinder.FindActor(enabledOnly:false);
            }

            SetTiles(actor, true);
        }
    }
}
