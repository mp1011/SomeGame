using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    abstract class CollectibleBehavior : Behavior
    {
        private readonly AudioService _audioService;
        private readonly PlayerStateManager _playerStateManager;
        private int _timer = 0;

        protected abstract SoundContentKey CollectSound { get; }
        protected abstract void OnCollected(PlayerState playerState);

        public CollectibleBehavior(AudioService audioService, PlayerStateManager playerStateManager)
        {
            _audioService = audioService;
            _playerStateManager = playerStateManager;
        }

        protected override void OnCreated( )
        {
            OnCollected(_playerStateManager.CurrentState);
            _audioService.Play(CollectSound);
            Actor.MotionVector.Set(new PixelPoint(0, -2));
            _timer = 0;
        }

        protected override void DoUpdate()
        {
            _timer++;
            if (_timer >= 5)
                Actor.Destroy();
        }
    }

    class CoinBehavior : CollectibleBehavior
    {
        public CoinBehavior(AudioService audioService, PlayerStateManager playerStateManager) : base(audioService, playerStateManager)
        {
        }

        protected override SoundContentKey CollectSound => SoundContentKey.Collect;

        protected override void OnCollected(PlayerState playerState)
        {
            playerState.Score.Add(25);
        }
    }

    class GemBehavior : CollectibleBehavior
    {
        public GemBehavior(AudioService audioService, PlayerStateManager playerStateManager) : base(audioService, playerStateManager)
        {
        }

        protected override SoundContentKey CollectSound => SoundContentKey.Collect;

        protected override void OnCollected(PlayerState playerState)
        {
            playerState.Score.Add(500);
        }
    }

    class AppleBehavior : CollectibleBehavior
    {
        public AppleBehavior(AudioService audioService, PlayerStateManager playerStateManager) : base(audioService, playerStateManager)
        {
        }

        protected override SoundContentKey CollectSound => SoundContentKey.Collect;

        protected override void OnCollected(PlayerState playerState)
        {
            playerState.Health.Add(4);
        }
    }

    class MeatBehavior : CollectibleBehavior
    {
        public MeatBehavior(AudioService audioService, PlayerStateManager playerStateManager) : base(audioService, playerStateManager)
        {
        }

        protected override SoundContentKey CollectSound => SoundContentKey.Collect;

        protected override void OnCollected(PlayerState playerState)
        {
            playerState.Health.Add(20);
        }
    }

    class KeyBehavior : CollectibleBehavior
    {
        public KeyBehavior(AudioService audioService, PlayerStateManager playerStateManager) : base(audioService, playerStateManager)
        {
        }

        protected override SoundContentKey CollectSound => SoundContentKey.Collect;

        protected override void OnCollected(PlayerState playerState)
        {
            //todo
        }
    }

}
