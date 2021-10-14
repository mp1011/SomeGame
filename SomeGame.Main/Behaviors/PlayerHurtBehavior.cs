using Microsoft.Xna.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class PlayerHurtBehavior : Behavior
    {
        private int _hurtTimer;
        private const int _recoilTime = 20;
        private const int _invulnerableTime = 200;

        public bool IsRecoiling => _hurtTimer > 0 && _hurtTimer < _recoilTime;

        public override void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo collisionInfo)
        {
            if (IsRecoiling)
            {
                actor.MotionVector = new PixelPoint(0, actor.MotionVector.Y);
                actor.CurrentAnimation = AnimationKey.Hurt;
            }
            
            if(_hurtTimer >= _recoilTime)
            {
                if (actor.CurrentAnimation == AnimationKey.Hurt)
                    actor.CurrentAnimation = AnimationKey.Idle;

                if ((_hurtTimer % 2) == 0)
                    actor.Palette = PaletteIndex.P1;
                else
                    actor.Palette = PaletteIndex.P2;
            }
            else
                actor.Palette = PaletteIndex.P2;

            if (_hurtTimer > 0)
                _hurtTimer++;

            if (_hurtTimer == _invulnerableTime)
                _hurtTimer = 0;
        }

        public override void HandleCollision(Actor actor, Actor other)
        {
            if((other.ActorType & ActorType.Enemy) > 0)
                _hurtTimer = 1;
        }
    }
}
