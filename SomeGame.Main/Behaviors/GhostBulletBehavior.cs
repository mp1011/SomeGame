using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class GhostBulletBehavior : Behavior
    {
        public GhostBulletBehavior()
        {
        }

        protected override void OnCreated()
        {
            Actor.CurrentAnimation = AnimationKey.Moving;           
        }

        protected override void DoUpdate()
        {
        }
    }
}
