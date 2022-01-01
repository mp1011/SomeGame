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
        private readonly RamByte _hurtTimer;
        private const int _recoilTime = 20;
        private const int _invulnerableTime = 200;
        private PaletteIndex _normalPalette;
        private PaletteIndex _flashPalette;
         
        public bool IsRecoiling => _hurtTimer > 0 && _hurtTimer < _recoilTime;
        public bool IsInvulnerable => _hurtTimer > 0 && _hurtTimer < _invulnerableTime;

        public PlayerHurtBehavior(GameSystem gameSystem, PlayerStateManager playerStateManager, AudioService audioService)
        {
            _hurtTimer = gameSystem.RAM.DeclareByte();
            _playerStateManager = playerStateManager;
            _audioService = audioService;
        }

        protected override void OnCreated()
        {
            _normalPalette = Actor.Palette;
            _flashPalette = PaletteIndex.P4;
        }

        protected override void DoUpdate()
        {
            if (IsRecoiling)
            {
                Actor.MotionVector.X.Set(0);
                Actor.CurrentAnimation = AnimationKey.Hurt;
            }
            
            if(_hurtTimer >= _recoilTime)
            {
                if (Actor.CurrentAnimation == AnimationKey.Hurt)
                    Actor.CurrentAnimation = AnimationKey.Idle;

                if ((_hurtTimer % 2) == 0)
                    Actor.Palette = _flashPalette;
                else
                    Actor.Palette = _normalPalette;
            }
            else
                Actor.Palette = _normalPalette;

            if (_hurtTimer > 0)
                _hurtTimer.Inc();

            if (_hurtTimer == _invulnerableTime)
                _hurtTimer.Set(0);
        }

        protected override void OnCollision(CollisionInfo collisionInfo)
        {
            if (IsInvulnerable)
                return;

            bool hitObject = collisionInfo.Actor != null
                && (collisionInfo.Actor.ActorType & ActorType.Enemy) != 0;

            if (!hitObject && !collisionInfo.Harmful)
                return;

            if (_hurtTimer == 0)
            {
                _audioService.Play(SoundContentKey.Hurt);
                _playerStateManager.CurrentState.Health.Subtract(1);
                if (_playerStateManager.CurrentState.Health == 0)
                    Actor.Destroy();
                _hurtTimer.Set(1);
            }
        }
    }
}
