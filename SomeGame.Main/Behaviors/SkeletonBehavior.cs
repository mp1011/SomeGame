using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class SkeletonBehavior : Behavior
    {
        private readonly Gravity _gravity;
        private readonly EnemyBaseBehavior _enemyMotionBehavior;
        private readonly Actor _projectile;
        private readonly RamByte _timer;

        public SkeletonBehavior(GameSystem gameSystem, Gravity gravity, EnemyBaseBehavior enemyMotionBehavior, Actor projectile)
        {
            _timer = gameSystem.RAM.DeclareByte();
            _gravity = gravity;
            _enemyMotionBehavior = enemyMotionBehavior;
            _projectile = projectile;
        }

        public override void OnCreated(Actor actor)
        {
            _enemyMotionBehavior.SetIdle(actor);
            _timer.Set(100);
        }

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            _gravity.Update(actor, collisionInfo);
            _enemyMotionBehavior.Update(actor, collisionInfo);

            if(_enemyMotionBehavior.CurrentState == StandardEnemyState.Moving && 
                (collisionInfo.IsFacingLedge || collisionInfo.XCorrection != 0))
            {
                if (actor.FacingDirection == Direction.Right)
                    _enemyMotionBehavior.SetMoving(actor, Direction.Left);
                else
                    _enemyMotionBehavior.SetMoving(actor, Direction.Right);

                _timer.Set(10);
            }

            if (_timer == 0)
            {
                if (_enemyMotionBehavior.CurrentState == StandardEnemyState.Idle)
                {
                    _enemyMotionBehavior.SetMoving(actor, RandomUtil.RandomItem(Direction.Left, Direction.Right));
                    _timer.Set(RandomUtil.RandomItem(100, 150, 200));
                }
                else if (_enemyMotionBehavior.CurrentState == StandardEnemyState.Moving)
                {
                    DoAttack(actor);
                    _timer.Set(30);
                }
                else if (_enemyMotionBehavior.CurrentState == StandardEnemyState.Attacking)
                {
                    _enemyMotionBehavior.SetIdle(actor);
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


        public override void HandleCollision(Actor actor, Actor other)
        {
            if (other.ActorType == (ActorType.Player | ActorType.Bullet))
                actor.Destroy();
        }
    }
}
