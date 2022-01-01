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
        private readonly RamByte _timer;
        private readonly RamByte _attackTimer;
        private readonly RamEnum<Angle> _bulletAngle;
        private readonly RamByte _bulletDisplayIndex;
        private readonly RamByte _bulletTravelTimer;
        private readonly RamPoint _anchor;

        public GhostBehavior(GameSystem gameSystem, EnemyBaseBehavior baseBehavior, PlayerFinder playerFinder, Actor bullet)
        {
            _baseBehavior = baseBehavior;
            _playerFinder = playerFinder;
            _bullet = bullet;

            _timer = gameSystem.RAM.DeclareByte();
            _attackTimer = gameSystem.RAM.DeclareByte();
            _bulletDisplayIndex = gameSystem.RAM.DeclareByte();
            _bulletTravelTimer = gameSystem.RAM.DeclareByte();
            _anchor = gameSystem.RAM.DeclarePoint();
            _bulletAngle = gameSystem.RAM.DeclareEnum(Angle.Right);


            _baseBehavior.WalkSpeed = new PixelValue(0, 50);
        }

        protected override void DoUpdate()
        {
            var player = _playerFinder.FindActor();
            if (player == null)
                return;

            if(_bulletTravelTimer == 0)
                PlaceOrbitingBullet(Actor);
            else
            {
                _bulletTravelTimer.Inc();
                if(_bulletTravelTimer == 100)
                {
                    _bulletTravelTimer.Set(0);
                    _bullet.MotionVector.Set(new PixelPoint(0, 0));
                    _baseBehavior.SetMoving(Actor, Direction.Left);
                }
            }

            if (_baseBehavior.CurrentState == StandardEnemyState.Moving)
            {
                if(_timer % 2 == 0)
                    _attackTimer.Inc();

                if (_attackTimer == 255)
                {
                    _attackTimer.Set(0);
                    _baseBehavior.SetAttacking(Actor);
                    Actor.FacingDirection = Actor.WorldPosition.GetHorizontalDirectionTo(player.WorldPosition);
                    Actor.MotionVector.Set(new PixelPoint(0, 0));

                    return;
                }
               
                if (_timer.Inc() == 30)
                {
                    _anchor.Set(player.WorldPosition.Center.Offset(0, -20));
                    Actor.FacingDirection = Actor.WorldPosition.GetHorizontalDirectionTo(player.WorldPosition);
                    _timer.Set(0);
                }

                var targetAngle = Actor.WorldPosition.Center.GetAngleTo(_anchor);
                var currentAngle = Actor.MotionVector.ToAngle();

                var newAngle = currentAngle.RotateToward(targetAngle, 4);
                Actor.MotionVector.Set(newAngle.ToPixelPoint(_baseBehavior.WalkSpeed));
            }
            else if(_baseBehavior.CurrentState == StandardEnemyState.Attacking)
            {
                if(_bulletTravelTimer == 0 && Actor.IsAnimationFinished)
                {
                    Actor.CurrentAnimation = AnimationKey.Moving;
                    _bulletTravelTimer.Set(1);
                    _bullet.WorldPosition.Center = Actor.WorldPosition.Center;
                    _bullet.MotionVector.Set(_bullet.WorldPosition.Center
                                                  .GetAngleTo(player.WorldPosition.Center)
                                                  .ToPixelPoint(new PixelValue(2, 0)));
                }
            }
        }

        protected override void OnCreated()
        {
            _bullet.Create();
            _anchor.Set(Actor.WorldPosition.Center);
            _baseBehavior.SetMoving(Actor, Direction.Left);
        }

        private void PlaceOrbitingBullet(Actor actor)
        {
            _bulletAngle.Inc();

            if (_timer % 2 == 0)
            {
                _bulletDisplayIndex.Inc();
                if (_bulletDisplayIndex == 4)
                    _bulletDisplayIndex.Set(0);
            }

            var displayAngle = (Angle)(_bulletAngle + (byte)(64 * _bulletDisplayIndex));
            _bullet.WorldPosition.Center = actor.WorldPosition.Center.Offset(displayAngle, 30);
        }

        protected override void OnCollision(CollisionInfo collisionInfo)
        {
            if (collisionInfo.Actor != null && collisionInfo.Actor.ActorType == (ActorType.Player | ActorType.Bullet))
                Actor.Destroy();
        }
    }
}
