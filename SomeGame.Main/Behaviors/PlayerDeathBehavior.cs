using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class PlayerDeathBehavior : IDestroyedBehavior
    {
        private readonly SceneManager _sceneManager;
        private int timer = 0;

        public PlayerDeathBehavior(SceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        public void OnDestroyed(Actor actor)
        {
            timer = 0;
            actor.CurrentAnimation = AnimationKey.Hurt;
            actor.MotionVector.Set(new PixelPoint(0, 0));
        }

        public DestroyedState Update(Actor actor)
        {
            timer++;
            if (timer < 200)
                return DestroyedState.Destroying;
            else
            {
                _sceneManager.RestartCurrentScene();
                return DestroyedState.Destroyed;
            }
        }
    }
}
