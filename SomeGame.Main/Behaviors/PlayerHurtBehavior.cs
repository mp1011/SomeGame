using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class PlayerHurtBehavior : Behavior
    {
        private AudioService _audioService;
        private PlayerStateManager _playerStateManager;
        private int _hurtTimer;
        private const int _recoilTime = 20;
        private const int _invulnerableTime = 200;
        private PaletteIndex _normalPalette;
        private PaletteIndex _flashPalette;
         
        public bool IsRecoiling => _hurtTimer > 0 && _hurtTimer < _recoilTime;
        public bool IsInvulnerable => _hurtTimer > 0 && _hurtTimer < _invulnerableTime;

        public PlayerHurtBehavior(PlayerStateManager playerStateManager, AudioService audioService)
        {
            _playerStateManager = playerStateManager;
            _audioService = audioService;
        }

        public override void OnCreated(Actor actor)
        {
            _normalPalette = actor.Palette;
            _flashPalette = PaletteIndex.P4;
        }

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            if (IsRecoiling)
            {
                actor.MotionVector.X.Set(0);
                actor.CurrentAnimation = AnimationKey.Hurt;
            }
            
            if(_hurtTimer >= _recoilTime)
            {
                if (actor.CurrentAnimation == AnimationKey.Hurt)
                    actor.CurrentAnimation = AnimationKey.Idle;

                if ((_hurtTimer % 2) == 0)
                    actor.Palette = _flashPalette;
                else
                    actor.Palette = _normalPalette;
            }
            else
                actor.Palette = _normalPalette;

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
                _audioService.Play(SoundContentKey.Hurt);
                _playerStateManager.CurrentState.Health -= 5;
                if (_playerStateManager.CurrentState.Health == 0)
                    actor.Destroy();
                _hurtTimer = 1;
            }
        }
    }
}
