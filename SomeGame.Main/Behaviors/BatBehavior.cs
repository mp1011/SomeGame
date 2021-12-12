using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class BatBehavior : Behavior
    {
        private readonly EnemyBaseBehavior _baseBehavior;
        private readonly PlayerFinder _playerFinder;
        private RamByte _timer;
        private RamByte _riseHeight;

        private const int _minDistance = 120;
        private readonly PixelValue _speed = new PixelValue(1, 0);
        private readonly PixelValue _riseSpeed = new PixelValue(0, -50);
        
        public BatBehavior(GameSystem gameSystem, EnemyBaseBehavior baseBehavior, PlayerFinder playerFinder)
        {
            _baseBehavior = baseBehavior;
            _playerFinder = playerFinder;
            _timer = gameSystem.RAM.DeclareByte();
            _riseHeight = gameSystem.RAM.DeclareByte();
        }

        public override void OnCreated(Actor actor)
        {
            ResetTimer();
        }

        private void ResetTimer() => _timer.Set(RandomUtil.RandomItem<byte>(60, 120, 120, 120, 240));

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            if (_baseBehavior.CurrentState == StandardEnemyState.Idle)
            {
                if (actor.WorldPosition.Y < _riseHeight)
                    actor.MotionVector.Set(new PixelPoint(0, 0));

                var player = _playerFinder.FindActor();
                if (player == null)
                    return;

                actor.FacingDirection = actor.WorldPosition.GetHorizontalDirectionTo(player.WorldPosition);

                if (_timer.Dec() <= 0)
                {
                    ResetTimer();

                    if (actor.WorldPosition.GetAbsoluteXDistance(player.WorldPosition) <= _minDistance)
                        _baseBehavior.SetMoving(actor, actor.WorldPosition.GetDirectionTo(player.WorldPosition), _speed);
                }
            }
            else if (_baseBehavior.CurrentState == StandardEnemyState.Moving)
            {
                if (_timer.Dec() <= 0)
                {
                    _baseBehavior.SetIdle(actor, stopMotion:false);
                    actor.MotionVector.Set(new PixelPoint(new PixelValue(0, 0), _riseSpeed));
                    var player = _playerFinder.FindActor();
                    if(player != null)
                        _riseHeight.Set(player.WorldPosition.Y.Pixel - 50);
                    ResetTimer();
                }
            }
        }

        public override void HandleCollision(Actor actor, Actor other)
        {
            if (other.ActorType == (ActorType.Player | ActorType.Bullet))
                actor.Destroy();
        }
    }
}
