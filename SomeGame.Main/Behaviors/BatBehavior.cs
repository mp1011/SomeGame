using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class BatBehavior : Behavior
    {
        private readonly EnemyBaseBehavior _baseBehavior;
        private readonly PlayerFinder _playerFinder;
        private int _timer;
        private const int _minDistance = 120;
        private PixelValue _speed = new PixelValue(1, 0);
        private PixelValue _riseSpeed = new PixelValue(0, -50);

        private int _riseHeight;

        public BatBehavior(EnemyBaseBehavior baseBehavior, PlayerFinder playerFinder)
        {
            _baseBehavior = baseBehavior;
            _playerFinder = playerFinder;
        }

        public override void OnCreated(Actor actor)
        {
            ResetTimer();
        }

        private void ResetTimer() => _timer = RandomUtil.RandomItem(60, 120, 120, 120, 240);

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            if (_baseBehavior.CurrentState == StandardEnemyState.Idle)
            {
                if (actor.WorldPosition.Top < _riseHeight)
                    actor.MotionVector = new PixelPoint(0, 0);

                var player = _playerFinder.FindActor();
                if (player == null)
                    return;

                actor.FacingDirection = actor.WorldPosition.GetHorizontalDirectionTo(player.WorldPosition);

                if (--_timer <= 0)
                {
                    ResetTimer();

                    if (actor.WorldPosition.GetAbsoluteXDistance(player.WorldPosition) <= _minDistance)
                        _baseBehavior.SetMoving(actor, actor.WorldPosition.GetDirectionTo(player.WorldPosition), _speed);
                }
            }
            else if (_baseBehavior.CurrentState == StandardEnemyState.Moving)
            {
                if (--_timer <= 0)
                {
                    _baseBehavior.SetIdle(actor, stopMotion:false);
                    actor.MotionVector = new PixelPoint(new PixelValue(0, 0), _riseSpeed);
                    var player = _playerFinder.FindActor();
                    if(player != null)
                        _riseHeight = player.WorldPosition.Top - 50;
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
