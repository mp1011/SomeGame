using Microsoft.Xna.Framework;
using SomeGame.Main.Extensions;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;

namespace SomeGame.Main.Behaviors
{
    class PlayerBehavior : Behavior
    {
        private readonly PlatformerPlayerMotionBehavior _motionBehavior;
        private readonly PlayerHurtBehavior _playerHurtBehavior;
        private readonly CameraBehavior _cameraBehavior;
        private readonly AcceleratedMotion _gravity;
        private readonly InputManager _inputManager;
        private readonly Actor _bullet;
        private PlayerState _playerState;

        public PlayerBehavior(PlatformerPlayerMotionBehavior motionBehavior, PlayerHurtBehavior playerHurtBehavior, CameraBehavior cameraBehavior, 
            AcceleratedMotion gravity, InputManager inputManager, Actor bullet, PlayerState playerState)
        {
            _motionBehavior = motionBehavior;
            _cameraBehavior = cameraBehavior;
            _playerHurtBehavior = playerHurtBehavior;
            _gravity = gravity;
            _bullet = bullet;
            _inputManager = inputManager;
            _playerState = playerState;
        }

        public override void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo collisionInfo)
        {
            _playerHurtBehavior.Update(actor, frameStartPosition, collisionInfo);
            _motionBehavior.Update(actor, frameStartPosition, collisionInfo);
            _gravity.Update(actor, frameStartPosition, collisionInfo);
            _cameraBehavior.Update(actor, frameStartPosition, collisionInfo);


            if(!_bullet.Enabled && _inputManager.Input.B.IsPressed())
            {
                actor.CurrentAnimation = AnimationKey.Attacking;               
            }

            if (actor.CurrentAnimation == AnimationKey.Attacking && actor.IsAnimationFinished)
            {
                actor.CurrentAnimation = AnimationKey.Idle;
                _bullet.WorldPosition.X = actor.WorldPosition.X;
                _bullet.WorldPosition.Y = actor.WorldPosition.Y;
                _bullet.Enabled = true;

                _bullet.Flip = actor.Flip;
            }
        }

        public override void HandleCollision(Actor actor, Actor other)
        {
            _playerHurtBehavior.HandleCollision(actor, other);
            _playerState.Health -= 5;
        }
    }
}
