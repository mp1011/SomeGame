using SomeGame.Main.Models;

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
        private PlayerState _playerState;
        private int _score;

        public EnemyDestroyedBehavior(int score, PlayerState playerState)
        {
            _playerState = playerState;
            _score = score;
        }

        public void OnDestroyed(Actor actor)
        {
            _playerState.Score += _score;
        }

        public virtual DestroyedState Update(Actor actor) => DestroyedState.Destroyed;
    }
}
