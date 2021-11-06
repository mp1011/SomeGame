using Microsoft.Xna.Framework;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class PlayerHurtBehavior : Behavior
    {
        private PlayerStateManager _playerStateManager;
        private int _hurtTimer;
        private const int _recoilTime = 20;
        private const int _invulnerableTime = 200;
         
        public bool IsRecoiling => _hurtTimer > 0 && _hurtTimer < _recoilTime;
        public bool IsInvulnerable => _hurtTimer > 0 && _hurtTimer < _invulnerableTime;

        public PlayerHurtBehavior(PlayerStateManager playerStateManager)
        {
            _playerStateManager = playerStateManager;
        }

        public override void Update(Actor actor, CollisionInfo collisionInfo)
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
            if (IsInvulnerable || other == null || (other.ActorType & ActorType.Enemy) == 0)
                return;

            if (_hurtTimer == 0)
            {
                _playerStateManager.CurrentState.Health -= 5;
                if (_playerStateManager.CurrentState.Health == 0)
                    actor.Destroy();
                _hurtTimer = 1;
            }
        }
    }
}
