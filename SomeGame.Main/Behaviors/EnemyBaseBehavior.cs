using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class EnemyBaseBehavior : Behavior
    {
        private RamEnum<StandardEnemyState> _state;
        private RamPixelValue _walkSpeed;

        public StandardEnemyState CurrentState
        {
            get => _state;
            set => _state.Set(value);
        }

        public PixelValue WalkSpeed
        {
            get => _walkSpeed;
            set => _walkSpeed.Set(value);
        }
        
        public EnemyBaseBehavior(GameSystem gameSystem)
        {
            _state = gameSystem.RAM.DeclareEnum(StandardEnemyState.Idle);
            _walkSpeed = gameSystem.RAM.DeclarePixelValue(0, 50);
        }
        protected override void DoUpdate()
        {
        }

        protected override void OnCollision(CollisionInfo collisionInfo)
        {
            if (collisionInfo.XCorrection != 0)
                Actor.MotionVector.X.Set(0);

            if (collisionInfo.YCorrection != 0)
                Actor.MotionVector.Y.Set(0);
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
