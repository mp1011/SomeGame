using Microsoft.Xna.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class EnemyBaseBehavior : Behavior
    {
        public StandardEnemyState CurrentState { get; private set; }

        private readonly PixelValue _walkSpeed = new PixelValue(0, 100);

        public override void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo collisionInfo)
        {
            if (collisionInfo.XCorrection != 0)
                actor.MotionVector = new PixelPoint(0, actor.MotionVector.Y);

            if (collisionInfo.YCorrection != 0)
                actor.MotionVector = new PixelPoint(actor.MotionVector.X, 0);
        }

        public void SetIdle(Actor actor)
        {
            CurrentState = StandardEnemyState.Idle;
            actor.MotionVector = new PixelPoint(0, actor.MotionVector.Y);
            actor.CurrentAnimation = AnimationKey.Idle;
        }

        public void SetAttacking(Actor actor)
        {
            CurrentState = StandardEnemyState.Attacking;
            actor.CurrentAnimation = AnimationKey.Attacking;
            actor.MotionVector = new PixelPoint(0, actor.MotionVector.Y);
        }

        public void SetMoving(Actor actor, Direction direction)
        {
            CurrentState = StandardEnemyState.Moving;
            actor.CurrentAnimation = AnimationKey.Moving;

            if (direction == Direction.Left)
            {
                actor.Flip = Flip.H;
                actor.MotionVector = new PixelPoint(_walkSpeed * -1, actor.MotionVector.Y);
            }
            else
            {
                actor.Flip = Flip.None;
                actor.MotionVector = new PixelPoint(_walkSpeed, actor.MotionVector.Y);
            }
        }
    }
}
