using Microsoft.Xna.Framework;
using SomeGame.Main.Content;
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
        private readonly AudioService _audioService;
        private readonly DestroyOnFall _destroyOnFall;
        private readonly SceneManager _sceneManager;
        private readonly TransitionInfo _incomingTransition;

        private int _attackCooldown;
        private bool _queueAttack;

        public PlayerBehavior(PlatformerPlayerMotionBehavior motionBehavior, PlayerHurtBehavior playerHurtBehavior, 
            CameraBehavior cameraBehavior, AcceleratedMotion gravity, InputManager inputManager, ActorPool bullets,  
            DestroyOnFall destroyOnFall, SceneManager sceneManager, AudioService audioService, TransitionInfo incomingTransition)
        {
            _destroyOnFall = destroyOnFall;
            _motionBehavior = motionBehavior;
            _cameraBehavior = cameraBehavior;
            _playerHurtBehavior = playerHurtBehavior;
            _gravity = gravity;
            _bullets = bullets;
            _inputManager = inputManager;
            _audioService = audioService;
            _sceneManager = sceneManager;
            _incomingTransition = incomingTransition;
        }

        public override void OnCreated(Actor actor)
        {
            if (_incomingTransition.ExitSide.Width == 0)
                return;

            var relativeExitPosition = new Point(
                _incomingTransition.PlayerExitPosition.X - _incomingTransition.ExitSide.Center.X,
                _incomingTransition.PlayerExitPosition.Y - _incomingTransition.ExitSide.Center.Y);

            var enterSide = _sceneManager.CurrentScene.GetEdge(_incomingTransition.Direction.Opposite());

            var relativeEnterPosition = new Point(
                enterSide.Center.X + relativeExitPosition.X,
                enterSide.Center.Y + relativeExitPosition.Y);

            switch(_incomingTransition.Direction)
            {
                case Direction.Left:
                    relativeEnterPosition = new Point(enterSide.Left - actor.WorldPosition.Width - 5, relativeEnterPosition.Y);
                    break;
                case Direction.Right:
                    relativeEnterPosition = new Point(enterSide.Right + actor.WorldPosition.Width + 5, relativeEnterPosition.Y);
                    break;
                case Direction.Up:
                    relativeEnterPosition = new Point(relativeEnterPosition.X, enterSide.Top - actor.WorldPosition.Height - 5);
                    break;
                case Direction.Down:
                    relativeEnterPosition = new Point(relativeEnterPosition.X, enterSide.Bottom + actor.WorldPosition.Height + 5);
                    break;
            }

            actor.WorldPosition.Center = relativeEnterPosition;
        }

        public override void Update(Actor actor, Rectangle frameStartPosition, CollisionInfo collisionInfo)
        {
            if (actor.CurrentAnimation == AnimationKey.Attacking && actor.IsAnimationFinished)
            {
                actor.CurrentAnimation = AnimationKey.Idle;
                var bullet = _bullets.ActivateNext();
                if (bullet != null)
                {
                    bullet.WorldPosition.Center = actor.WorldPosition.Center
                        .GetRelativePosition(8, 4, actor.Flip);

                    bullet.Flip = actor.Flip;
                }
            }


            _destroyOnFall.Update(actor);
            _playerHurtBehavior.Update(actor, frameStartPosition, collisionInfo);
            _motionBehavior.Update(actor, frameStartPosition, collisionInfo);
            _gravity.Update(actor, frameStartPosition, collisionInfo);
            _cameraBehavior.Update(actor, frameStartPosition, collisionInfo);


            if(_attackCooldown == 0 && (_queueAttack || _inputManager.Input.B.IsPressed()))
            {
                _queueAttack = false;
                actor.CurrentAnimation = AnimationKey.Attacking;
                _attackCooldown = 15;
                _audioService.Play(SoundContentKey.Swish);
            }
            else if(_inputManager.Input.B.IsPressed())
            {
                _queueAttack = true;
            }

            if (_attackCooldown > 0)
                _attackCooldown--;

            _sceneManager.CheckLevelTransitions(actor);
        }

        public override void HandleCollision(Actor actor, Actor other)
        {
            _playerHurtBehavior.HandleCollision(actor, other);            
        }
    }
}
