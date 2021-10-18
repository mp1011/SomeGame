using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class CollectibleBehavior : Behavior
    {
        private readonly AudioService _audioService;
        private readonly PlayerState _playerState;
        private int _timer = 0;

        public CollectibleBehavior(AudioService audioService, PlayerState playerState)
        {
            _audioService = audioService;
            _playerState = playerState;
        }

        public override void OnCreated(Actor actor)
        {
            _playerState.Score += 25;
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
