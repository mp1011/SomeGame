using Microsoft.Xna.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    abstract class Behavior
    {
        private readonly Behavior[] _subBehaviors;

        protected Actor Actor { get; private set; }
        public void Update()
        {
            foreach (var subBehavior in _subBehaviors)
                subBehavior.Update();

            DoUpdate();
        }

        protected abstract void DoUpdate();

        public void OnCreated(Actor actor)
        {
            Actor = actor;
            foreach (var subBehavior in _subBehaviors)
                subBehavior.OnCreated(actor);
            OnCreated();
        }

        public Behavior(params Behavior[] subBehaviors)
        {
            _subBehaviors = subBehaviors;
        }

        protected virtual void OnCreated() { }
        public void HandleCollision(CollisionInfo collisionInfo) 
        {
            foreach (var subBehavior in _subBehaviors)
                subBehavior.HandleCollision(collisionInfo);

            OnCollision(collisionInfo);
        }

        protected virtual void OnCollision(CollisionInfo collisionInfo) { }
    }

    class EmptyBehavior : Behavior
    {
        protected override void DoUpdate()
        {
        }
    }
}
