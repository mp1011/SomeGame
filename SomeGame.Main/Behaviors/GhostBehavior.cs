using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class GhostBehavior : Behavior
    {
        private readonly EnemyBaseBehavior _baseBehavior;
        private readonly PlayerFinder _playerFinder;
        private readonly Actor _bullet;
        private Point _anchor;
        private int _timer = 0;
        private int _attackTimer = 0;

        private Angle _bulletAngle = 0;
        private byte _bulletDisplayIndex;
        private int _bulletTravelTimer;

        public GhostBehavior(EnemyBaseBehavior baseBehavior, PlayerFinder playerFinder, Actor bullet)
        {
            _baseBehavior = baseBehavior;
            _playerFinder = playerFinder;
            _bullet = bullet;

            _baseBehavior.WalkSpeed = new PixelValue(0, 50);
        }

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            var player = _playerFinder.FindActor();
            if (player == null)
                return;

            if(_bulletTravelTimer == 0)
                PlaceOrbitingBullet(actor);
            else
            {
                _bulletTravelTimer++;
                if(_bulletTravelTimer == 100)
                {
                    _bulletTravelTimer = 0;
                    _bullet.MotionVector.Set(new PixelPoint(0, 0));
                    _baseBehavior.SetMoving(actor, Direction.Left);
                }
            }

            if (_baseBehavior.CurrentState == StandardEnemyState.Moving)
            {
                _attackTimer++;
                if(_attackTimer == 500)
                {
                    _attackTimer = 0;
                    _baseBehavior.SetAttacking(actor);
                    actor.FacingDirection = actor.WorldPosition.GetHorizontalDirectionTo(player.WorldPosition);
                    actor.MotionVector.Set(new PixelPoint(0, 0));

                    return;
                }
               
                if (++_timer == 30)
                {
                    _anchor = player.WorldPosition.Center.Offset(0, -20);
                    actor.FacingDirection = actor.WorldPosition.GetHorizontalDirectionTo(player.WorldPosition);
                    _timer = 0;
                }

                var targetAngle = actor.WorldPosition.Center.GetAngleTo(_anchor);
                var currentAngle = actor.MotionVector.ToAngle();

                var newAngle = currentAngle.RotateToward(targetAngle, 4);
                actor.MotionVector.Set(newAngle.ToPixelPoint(_baseBehavior.WalkSpeed));
            }
            else if(_baseBehavior.CurrentState == StandardEnemyState.Attacking)
            {
                if(_bulletTravelTimer == 0 && actor.IsAnimationFinished)
                {
                    actor.CurrentAnimation = AnimationKey.Moving;
                    _bulletTravelTimer = 1;
                    _bullet.WorldPosition.Center = actor.WorldPosition.Center;
                    _bullet.MotionVector.Set(_bullet.WorldPosition.Center
                                                  .GetAngleTo(player.WorldPosition.Center)
                                                  .ToPixelPoint(new PixelValue(2, 0)));
                }
            }
        }

        public override void OnCreated(Actor actor)
        {
            _bullet.Create();
            _anchor = actor.WorldPosition.Center;

            _baseBehavior.SetMoving(actor, Direction.Left);
        }

        private void PlaceOrbitingBullet(Actor actor)
        {
            _bulletAngle++;

            _bulletDisplayIndex++;
            if (_bulletDisplayIndex == 4)
                _bulletDisplayIndex = 0;

            var displayAngel = _bulletAngle + (byte)(64 * _bulletDisplayIndex);
            _bullet.WorldPosition.Center = actor.WorldPosition.Center.Offset(displayAngel, 30);
        }

        public override void HandleCollision(Actor actor, Actor other)
        {
            if (other.ActorType == (ActorType.Player | ActorType.Bullet))
                actor.Destroy();
        }
    }
}
