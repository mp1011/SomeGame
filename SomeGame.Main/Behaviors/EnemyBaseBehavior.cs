using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class EnemyBaseBehavior : Behavior
    {
        public StandardEnemyState CurrentState { get; private set; }

        public PixelValue WalkSpeed { get; set; } = new PixelValue(0, 100);
        
        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
            if (collisionInfo.XCorrection != 0)
                actor.MotionVector = new PixelPoint(0, actor.MotionVector.Y);

            if (collisionInfo.YCorrection != 0)
                actor.MotionVector = new PixelPoint(actor.MotionVector.X, 0);
        }

        public void SetIdle(Actor actor, bool stopMotion=true)
        {
            CurrentState = StandardEnemyState.Idle;
            actor.CurrentAnimation = AnimationKey.Idle;

            if(stopMotion)
                actor.MotionVector = new PixelPoint(0, actor.MotionVector.Y);
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

            actor.FacingDirection = direction;
            actor.MotionVector = new PixelPoint(WalkSpeed * direction.GetSpeedMod(), actor.MotionVector.Y);          
        }

        public void SetMoving(Actor actor, PixelPoint unitVector, PixelValue speed)
        {
            CurrentState = StandardEnemyState.Moving;
            actor.CurrentAnimation = AnimationKey.Moving;
            actor.MotionVector = unitVector * speed;
        }
    }
}
