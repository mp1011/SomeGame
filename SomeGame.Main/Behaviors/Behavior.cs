﻿using Microsoft.Xna.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    abstract class Behavior
    {
        public abstract void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo collisionInfo);
    }
}
