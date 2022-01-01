using SomeGame.Main.Models;
using SomeGame.Main.Services;

namespace SomeGame.Main.Behaviors
{
    class DebrisBehavior : Behavior
    {
        private readonly Scroller _scroller;

        public DebrisBehavior(Gravity gravity, Scroller scroller) : base(gravity)
        {
            _scroller = scroller;
        }

        protected override void DoUpdate()
        {
            if (Actor.WorldPosition.Y > _scroller.Camera.Bottom())
                Actor.Destroy();
        }
    }
}
