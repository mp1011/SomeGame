using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    class Gravity : AcceleratedMotion
    {
        public Gravity() : base(Orientation.Vertical)
        {
            SubPixelAcceleration = 30;
        }
    }
}
