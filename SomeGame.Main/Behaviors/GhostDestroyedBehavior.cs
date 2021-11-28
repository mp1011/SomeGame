using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class GhostDestroyedBehavior : IDestroyedBehavior
    {
        private readonly Actor _bullet;
        private int _timer = 0;

        public GhostDestroyedBehavior(Actor bullet)
        {
            _bullet = bullet;
        }

        public void OnDestroyed(Actor actor)
        {
            actor.MotionVector = new PixelPoint(0, 0);
            _timer = 0;
        }

        public DestroyedState Update(Actor actor)
        {
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
