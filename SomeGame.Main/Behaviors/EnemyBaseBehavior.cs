using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class EnemyBaseBehavior : Behavior
    {
        public StandardEnemyState CurrentState { get; private set; }

        public PixelValue WalkSpeed { get; set; } = new PixelValue(0, 50);
        
        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            if (collisionInfo.XCorrection != 0)
                actor.MotionVector.X.Set(0);

            if (collisionInfo.YCorrection != 0)
                actor.MotionVector.Y.Set(0);
        }

        public void SetIdle(Actor actor, bool stopMotion=true)
        {
            CurrentState = StandardEnemyState.Idle;
            actor.CurrentAnimation = AnimationKey.Idle;

            if (stopMotion)
                actor.MotionVector.X.Set(0);
        }

        public void SetAttacking(Actor actor)
        {
            CurrentState = StandardEnemyState.Attacking;
            actor.CurrentAnimation = AnimationKey.Attacking;
            actor.MotionVector.X.Set(0);
        }

        public void SetMoving(Actor actor, Direction direction)
        {
            CurrentState = StandardEnemyState.Moving;
            actor.CurrentAnimation = AnimationKey.Moving;

            actor.FacingDirection = direction;
            actor.MotionVector.X.Set(WalkSpeed * direction.GetSpeedMod());          
        }

        public void SetMoving(Actor actor, PixelPoint unitVector, PixelValue speed)
        {
            CurrentState = StandardEnemyState.Moving;
            actor.CurrentAnimation = AnimationKey.Moving;
            actor.MotionVector.Set(unitVector * speed);
        }
    }
}
