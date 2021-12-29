using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class SkeletonDestroyedBehavior : EnemyDestroyedBehavior
    {
        private readonly Actor _skull;
        private readonly ActorPool _bones;

        public SkeletonDestroyedBehavior(int score, PlayerStateManager playerStateManager,
            Actor skull, ActorPool bones, AudioService audioService) : base(score, playerStateManager, audioService)
        {
            _skull = skull;
            _bones = bones;
        }

        protected override void OnDestroyedStart(Actor actor)
        {
            _skull.Create();
            _skull.WorldPosition.Center = actor.WorldPosition.Center;

            AddBone(-2, actor);
            AddBone(1, actor);
            AddBone(2, actor);
        }

        private void AddBone(int initialX, Actor actor)
        {
            var bone = _bones.ActivateNext();
            if (bone == null)
                return;
            bone.WorldPosition.Center = actor.WorldPosition.Center;
            bone.MotionVector.Set(new PixelPoint(initialX, -2));
        }
    }
}
