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

        public override void OnCreated(Actor actor)
        {
            OnCollected(_playerStateManager.CurrentState);
            _audioService.Play(CollectSound);
            actor.MotionVector = new PixelPoint(0, -2);
            _timer = 0;
        }

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            _timer++;
            if (_timer >= 5)
                actor.Destroy();
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
            playerState.Score += 25;
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
            playerState.Score += 500;
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
            playerState.Health += 4;
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
            playerState.Health += 20;
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
