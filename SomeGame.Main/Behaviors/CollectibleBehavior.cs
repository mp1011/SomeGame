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
        private readonly RamByte _timer;

        protected abstract SoundContentKey CollectSound { get; }
        protected abstract void OnCollected(PlayerState playerState);

        public CollectibleBehavior(GameSystem gameSystem, AudioService audioService, PlayerStateManager playerStateManager)
        {
            _audioService = audioService;
            _playerStateManager = playerStateManager;
            _timer = gameSystem.RAM.DeclareByte();
        }

        protected override void OnCreated( )
        {
            OnCollected(_playerStateManager.CurrentState);
            _audioService.Play(CollectSound);
            Actor.MotionVector.Set(new PixelPoint(0, -2));
            _timer.Set(0);
        }

        protected override void DoUpdate()
        {
            _timer.Inc();
            if (_timer >= 5)
                Actor.Destroy();
        }
    }

    class CoinBehavior : CollectibleBehavior
    {
        public CoinBehavior(GameSystem gameSystem, AudioService audioService, PlayerStateManager playerStateManager) 
            : base(gameSystem, audioService, playerStateManager)
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
        public GemBehavior(GameSystem gameSystem, AudioService audioService, PlayerStateManager playerStateManager) 
            : base(gameSystem, audioService, playerStateManager)
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
        public AppleBehavior(GameSystem gameSystem, AudioService audioService, PlayerStateManager playerStateManager) 
            : base(gameSystem, audioService, playerStateManager)
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
        public MeatBehavior(GameSystem gameSystem, AudioService audioService, PlayerStateManager playerStateManager) 
            : base(gameSystem, audioService, playerStateManager)
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
        public KeyBehavior(GameSystem gameSystem, AudioService audioService, PlayerStateManager playerStateManager) 
            : base(gameSystem, audioService, playerStateManager)
        {
        }

        protected override SoundContentKey CollectSound => SoundContentKey.Collect;

        protected override void OnCollected(PlayerState playerState)
        {
            playerState.HasKey = true;
        }
    }

}
