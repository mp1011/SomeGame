using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class GhostBulletBehavior : Behavior
    {
        public GhostBulletBehavior()
        {
        }

        public override void OnCreated(Actor actor)
        {
            actor.CurrentAnimation = AnimationKey.Moving;           
        }

        public override void Update(Actor actor, CollisionInfo collisionInfo)
        {
          
        }
    }
}
