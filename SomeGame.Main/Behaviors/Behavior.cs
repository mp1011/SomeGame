﻿using Microsoft.Xna.Framework;
using SomeGame.Main.Models;

namespace SomeGame.Main.Behaviors
{
    abstract class Behavior
    {
        public abstract void Update(Actor actor, CollisionInfo collisionInfo);
        public virtual void OnCreated(Actor actor) { }
        public virtual void HandleCollision(Actor actor, Actor other) { }
    }
}
