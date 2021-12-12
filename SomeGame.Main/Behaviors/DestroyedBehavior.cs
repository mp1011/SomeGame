using SomeGame.Main.Content;
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
        private AudioService _audioService;
        private PlayerStateManager _playerStateManager;
        protected virtual SoundContentKey DestroySound { get; set; } = SoundContentKey.Destroy;

        private int _score;

        public EnemyDestroyedBehavior(int score, PlayerStateManager playerStateManager, AudioService audioService)
        {
            _audioService = audioService;
            _playerStateManager = playerStateManager;
            _score = score;
        }

        public void OnDestroyed(Actor actor)
        {
            _audioService.Play(DestroySound);
            _playerStateManager.CurrentState.Score.Add(_score);
            OnDestroyedStart(actor);
        }

        protected void PlayDestroyedSound() => _audioService.Play(DestroySound);

        protected virtual void OnDestroyedStart(Actor actor)
        {

        }

        public virtual DestroyedState Update(Actor actor) => DestroyedState.Destroyed;
    }
}
