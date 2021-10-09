using Microsoft.Xna.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class SkeletonBehavior : Behavior
    {
        private readonly Gravity _gravity;
        private readonly EnemyBaseBehavior _enemyMotionBehavior;
        private readonly Actor _projectile;

        private int _counter;

        public SkeletonBehavior(Gravity gravity, EnemyBaseBehavior enemyMotionBehavior, Actor projectile)
        {
            _gravity = gravity;
            _enemyMotionBehavior = enemyMotionBehavior;
            _projectile = projectile;
        }

        public override void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo collisionInfo)
        {
            _gravity.Update(actor, frameStartPosition, collisionInfo);

            if (_counter == 0)
                _enemyMotionBehavior.SetIdle(actor);
            if (_counter == 100)
                _enemyMotionBehavior.SetMoving(actor, Direction.Left);
            if (_counter == 200)
                DoAttack(actor);
            if (_counter == 250)
                _enemyMotionBehavior.SetIdle(actor);
            if (_counter == 300)
                _enemyMotionBehavior.SetMoving(actor, Direction.Right);
          
            _counter++;
            if (_counter == 400)
                _counter = 0;            
        }

        private void DoAttack(Actor actor)
        {
            _enemyMotionBehavior.SetAttacking(actor);
            _projectile.Enabled = true;
            _projectile.WorldPosition.X = actor.WorldPosition.X;
            _projectile.WorldPosition.Y = actor.WorldPosition.Y;           
        }
    }
}
