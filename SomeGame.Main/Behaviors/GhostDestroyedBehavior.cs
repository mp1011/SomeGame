using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class GhostDestroyedBehavior : EnemyDestroyedBehavior
    {
        private readonly Actor _bullet;
        private int _timer = 0;
        private byte _soundTimer = 0;

        public GhostDestroyedBehavior(int score, Actor bullet, PlayerStateManager playerStateManager, AudioService audioService) 
            : base(score, playerStateManager, audioService)
        {
            _bullet = bullet;
        }

        protected override void OnDestroyedStart(Actor actor)
        {
            actor.MotionVector.Set(new PixelPoint(0, 0));
            _timer = 0;
        }

        public override DestroyedState Update(Actor actor)
        {
            _soundTimer++;
            if(_soundTimer == 10)
            {
                _soundTimer = 0;
                PlayDestroyedSound();
            }

            _timer++;
            actor.Palette = (PaletteIndex)(_timer % 4);
            _bullet.Palette = (PaletteIndex)((_timer+1) % 4);

            _bullet.WorldPosition.Center = actor.WorldPosition.Center.Offset(
                RandomUtil.RandomItem(-8, -4, 0, 4, 8),
                RandomUtil.RandomItem(-8, -4, 0, 4, 8));

            if (_timer == 50)
            {
                _bullet.Destroy();
                return DestroyedState.Destroyed;
            }
            else
                return DestroyedState.Destroying;
        }
    }
}
