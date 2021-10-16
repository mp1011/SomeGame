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
        private readonly ActorPool _bullets;
        private PlayerState _playerState;
        private int _attackCooldown;

        public PlayerBehavior(PlatformerPlayerMotionBehavior motionBehavior, PlayerHurtBehavior playerHurtBehavior, CameraBehavior cameraBehavior, 
            AcceleratedMotion gravity, InputManager inputManager, ActorPool bullets, PlayerState playerState)
        {
            _motionBehavior = motionBehavior;
            _cameraBehavior = cameraBehavior;
            _playerHurtBehavior = playerHurtBehavior;
            _gravity = gravity;
            _bullets = bullets;
            _inputManager = inputManager;
            _playerState = playerState;
        }

        public override void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo collisionInfo)
        {
            _playerHurtBehavior.Update(actor, frameStartPosition, collisionInfo);
            _motionBehavior.Update(actor, frameStartPosition, collisionInfo);
            _gravity.Update(actor, frameStartPosition, collisionInfo);
            _cameraBehavior.Update(actor, frameStartPosition, collisionInfo);


            if(_attackCooldown == 0 && _inputManager.Input.B.IsPressed())
            {
                actor.CurrentAnimation = AnimationKey.Attacking;
                _attackCooldown = 30;
            }

            if (_attackCooldown > 0)
                _attackCooldown--;

            if (actor.CurrentAnimation == AnimationKey.Attacking && actor.IsAnimationFinished)
            {
                actor.CurrentAnimation = AnimationKey.Idle;
                var bullet = _bullets.ActivateNext();
                if (bullet != null)
                {
                    bullet.WorldPosition.Center = actor.WorldPosition.Center
                        .GetRelativePosition(4, 4, actor.Flip);

                    bullet.Flip = actor.Flip;
                }
            }
        }

        public override void HandleCollision(Actor actor, Actor other)
        {
            _playerHurtBehavior.HandleCollision(actor, other);
            _playerState.Health -= 5;
        }
    }
}
