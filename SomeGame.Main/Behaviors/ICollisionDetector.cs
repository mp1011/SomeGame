using Microsoft.Xna.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    interface ICollisionDetector
    {
        CollisionInfo DetectCollisions(Actor actor);
    }
}
