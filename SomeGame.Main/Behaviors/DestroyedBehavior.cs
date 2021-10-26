using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    internal interface IDestroyedBehavior
    {
        void OnDestroyed(Actor actor);
        DestroyedState Update(Actor actor);
    }

    class EmptyDestroyedBehavior : IDestroyedBehavior
    {
        public void OnDestroyed(Actor actor)
        {
        }

        public DestroyedState Update(Actor actor) => DestroyedState.Destroyed;
    }

    class EnemyDestroyedBehavior : IDestroyedBehavior
    {
        private PlayerStateManager _playerStateManager;
        private int _score;

        public EnemyDestroyedBehavior(int score, PlayerStateManager playerStateManager)
        {
            _playerStateManager = playerStateManager;
            _score = score;
        }

        public void OnDestroyed(Actor actor)
        {
            _playerStateManager.CurrentState.Score += _score;
            OnDestroyedStart(actor);
        }

        protected virtual void OnDestroyedStart(Actor actor)
        {

        }

        public virtual DestroyedState Update(Actor actor) => DestroyedState.Destroyed;
    }
}
