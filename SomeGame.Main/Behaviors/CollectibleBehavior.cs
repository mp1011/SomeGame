using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class CollectibleBehavior : Behavior
    {
        private readonly AudioService _audioService;
        private readonly PlayerStateManager _playerStateManager;
        private int _timer = 0;

        public CollectibleBehavior(AudioService audioService, PlayerStateManager playerStateManager)
        {
            _audioService = audioService;
            _playerStateManager = playerStateManager;
        }

        public override void OnCreated(Actor actor)
        {
            _playerStateManager.CurrentState.Score += 25;
            _audioService.Play(SoundContentKey.GetCoin);
            actor.MotionVector = new PixelPoint(0, -2);
            _timer = 0;
        }

        public override void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo collisionInfo)
        {
            _timer++;
            if (_timer >= 5)
                actor.Destroy();
        }
    }
}
