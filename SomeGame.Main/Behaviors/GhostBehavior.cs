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

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            var player = _playerFinder.FindActor();
            if (player == null)
                return;

            if(_bulletTravelTimer == 0)
                PlaceOrbitingBullet(actor);
            else
            {
                _bulletTravelTimer.Inc();
                if(_bulletTravelTimer == 100)
                {
                    _bulletTravelTimer.Set(0);
                    _bullet.MotionVector.Set(new PixelPoint(0, 0));
                    _baseBehavior.SetMoving(actor, Direction.Left);
                }
            }

            if (_baseBehavior.CurrentState == StandardEnemyState.Moving)
            {
                if(_timer % 2 == 0)
                    _attackTimer.Inc();

                if (_attackTimer == 255)
                {
                    _attackTimer.Set(0);
                    _baseBehavior.SetAttacking(actor);
                    actor.FacingDirection = actor.WorldPosition.GetHorizontalDirectionTo(player.WorldPosition);
                    actor.MotionVector.Set(new PixelPoint(0, 0));

                    return;
                }
               
                if (_timer.Inc() == 30)
                {
                    _anchor.Set(player.WorldPosition.Center.Offset(0, -20));
                    actor.FacingDirection = actor.WorldPosition.GetHorizontalDirectionTo(player.WorldPosition);
                    _timer.Set(0);
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
                    _bulletTravelTimer.Set(1);
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
            _anchor.Set(actor.WorldPosition.Center);

            _baseBehavior.SetMoving(actor, Direction.Left);
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

        public override void HandleCollision(Actor actor, Actor other)
        {
            if (other.ActorType == (ActorType.Player | ActorType.Bullet))
                actor.Destroy();
        }
    }
}
