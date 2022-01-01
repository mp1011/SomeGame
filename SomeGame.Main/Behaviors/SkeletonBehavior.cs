using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class SkeletonBehavior : Behavior
    {
        private readonly EnemyBaseBehavior _enemyMotionBehavior;
        private readonly Actor _projectile;
        private readonly RamByte _timer;

        public SkeletonBehavior(GameSystem gameSystem, Gravity gravity, EnemyBaseBehavior enemyMotionBehavior, Actor projectile)
            : base(gravity, enemyMotionBehavior)
        {
            _timer = gameSystem.RAM.DeclareByte();
            _enemyMotionBehavior = enemyMotionBehavior;
            _projectile = projectile;
        }

        protected override void OnCreated()
        {
            _enemyMotionBehavior.SetIdle(Actor);
            _timer.Set(100);
        }

        protected override void OnCollision(CollisionInfo collisionInfo)
        {
            if (collisionInfo.Actor != null
                && collisionInfo.Actor.ActorType == (ActorType.Player | ActorType.Bullet))
            {
                Actor.Destroy();
            }

            if (_enemyMotionBehavior.CurrentState == StandardEnemyState.Moving &&
               (collisionInfo.IsFacingLedge || collisionInfo.XCorrection != 0))
            {
                if (Actor.FacingDirection == Direction.Right)
                    _enemyMotionBehavior.SetMoving(Actor, Direction.Left);
                else
                    _enemyMotionBehavior.SetMoving(Actor, Direction.Right);

                _timer.Set(10);
            }
        }

        protected override void DoUpdate()
        {
            if (_timer == 0)
            {
                if (_enemyMotionBehavior.CurrentState == StandardEnemyState.Idle)
                {
                    _enemyMotionBehavior.SetMoving(Actor, RandomUtil.RandomItem(Direction.Left, Direction.Right));
                    _timer.Set(RandomUtil.RandomItem(100, 150, 200));
                }
                else if (_enemyMotionBehavior.CurrentState == StandardEnemyState.Moving)
                {
                    DoAttack(Actor);
                    _timer.Set(30);
                }
                else if (_enemyMotionBehavior.CurrentState == StandardEnemyState.Attacking)
                {
                    _enemyMotionBehavior.SetIdle(Actor);
                    _timer.Set(RandomUtil.RandomItem(20, 50));
                }
            }

            _timer.Dec();
        }

        private void DoAttack(Actor actor)
        {
            _enemyMotionBehavior.SetAttacking(actor);

            _projectile.Flip = actor.Flip;           
            _projectile.WorldPosition.X.Set(actor.WorldPosition.X);
            _projectile.WorldPosition.Y.Set(actor.WorldPosition.Y);

            _projectile.Create();

        }


  
    }
}
