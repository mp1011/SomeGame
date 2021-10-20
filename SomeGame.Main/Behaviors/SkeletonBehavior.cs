using Microsoft.Xna.Framework;
using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class SkeletonBehavior : Behavior
    {
        private readonly Gravity _gravity;
        private readonly EnemyBaseBehavior _enemyMotionBehavior;
        private readonly Actor _projectile;

        private int _timer;

        public SkeletonBehavior(Gravity gravity, EnemyBaseBehavior enemyMotionBehavior, Actor projectile)
        {
            _gravity = gravity;
            _enemyMotionBehavior = enemyMotionBehavior;
            _projectile = projectile;
        }

        public override void OnCreated(Actor actor)
        {
            _enemyMotionBehavior.SetIdle(actor);
            _timer = 100;
        }

        public override void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo collisionInfo)
        {
            _gravity.Update(actor, frameStartPosition, collisionInfo);
            _enemyMotionBehavior.Update(actor, frameStartPosition, collisionInfo);

            if(_enemyMotionBehavior.CurrentState == StandardEnemyState.Moving && 
                (collisionInfo.IsFacingLedge || collisionInfo.XCorrection.Pixel != 0 || collisionInfo.XCorrection.SubPixel != 0))
            {
                if (actor.MotionVector.X > 0)
                    _enemyMotionBehavior.SetMoving(actor, Direction.Left);
                else
                    _enemyMotionBehavior.SetMoving(actor, Direction.Right);

                _timer = 10;
            }

            if (_timer == 0)
            {
                if (_enemyMotionBehavior.CurrentState == StandardEnemyState.Idle)
                {
                    _enemyMotionBehavior.SetMoving(actor, RandomUtil.RandomItem(Direction.Left, Direction.Right));
                    _timer = RandomUtil.RandomItem(100, 150, 200);
                }
                else if (_enemyMotionBehavior.CurrentState == StandardEnemyState.Moving)
                {
                    DoAttack(actor);
                    _timer = 30;
                }
                else if (_enemyMotionBehavior.CurrentState == StandardEnemyState.Attacking)
                {
                    _enemyMotionBehavior.SetIdle(actor);
                    _timer = RandomUtil.RandomItem(20, 50);
                }
            }

            _timer--;
        }

        private void DoAttack(Actor actor)
        {
            _enemyMotionBehavior.SetAttacking(actor);

            _projectile.Flip = actor.Flip;           
            _projectile.WorldPosition.X = actor.WorldPosition.X;
            _projectile.WorldPosition.Y = actor.WorldPosition.Y;

            _projectile.Create();

        }


        public override void HandleCollision(Actor actor, Actor other)
        {
            if (other.ActorType == (ActorType.Player | ActorType.Bullet))
                actor.Destroy();
        }
    }
}
