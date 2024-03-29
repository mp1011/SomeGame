﻿using Microsoft.Xna.Framework;
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
        private readonly Gravity _gravity;
        private readonly InputManager _inputManager;
        private readonly ActorPool _bullets;
        private readonly AudioService _audioService;
        private readonly DestroyOnFall _destroyOnFall;
        private readonly SceneManager _sceneManager;
        private readonly TransitionInfo _incomingTransition;

        private readonly RamByte _attackCooldown;
        private readonly RamEnum<InputQueue> _inputQueue;

        private bool _queueAttack
        {
            get => _inputQueue.GetFlag(InputQueue.Attack);
            set => _inputQueue.SetFlag(InputQueue.Attack, value);
        }

        public PlayerBehavior(GameSystem gameSystem, RamEnum<InputQueue> inputQueue, PlatformerPlayerMotionBehavior motionBehavior, PlayerHurtBehavior playerHurtBehavior, 
            CameraBehavior cameraBehavior, Gravity gravity, InputManager inputManager, ActorPool bullets,  
            DestroyOnFall destroyOnFall, SceneManager sceneManager, AudioService audioService, TransitionInfo incomingTransition) 
            :base(motionBehavior, playerHurtBehavior,cameraBehavior,gravity,destroyOnFall)
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
            _attackCooldown = gameSystem.RAM.DeclareByte();
            _inputQueue = inputQueue;
        }

        protected override void OnCreated()
        {
            _playerHurtBehavior.OnCreated(Actor);

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
                    relativeEnterPosition = new Point(enterSide.Left - Actor.WorldPosition.Width - 5, relativeEnterPosition.Y);
                    break;
                case Direction.Right:
                    relativeEnterPosition = new Point(enterSide.Right + Actor.WorldPosition.Width + 5, relativeEnterPosition.Y);
                    break;
                case Direction.Up:
                    relativeEnterPosition = new Point(relativeEnterPosition.X, enterSide.Top - Actor.WorldPosition.Height - 5);
                    break;
                case Direction.Down:
                    relativeEnterPosition = new Point(relativeEnterPosition.X, enterSide.Bottom + Actor.WorldPosition.Height + 5);
                    break;
            }

            Actor.WorldPosition.Center = relativeEnterPosition;
        }

        protected override void DoUpdate()
        {
            if (Actor.CurrentAnimation == AnimationKey.Attacking && Actor.IsAnimationFinished)
            {
                Actor.CurrentAnimation = AnimationKey.Idle;
                var bullet = _bullets.ActivateNext();
                if (bullet != null)
                {
                    bullet.WorldPosition.Center = Actor.WorldPosition.Center
                        .GetRelativePosition(8, 4, Actor.Flip);

                    bullet.Flip = Actor.Flip;
                }
            }

            if(_attackCooldown == 0 && (_queueAttack || _inputManager.Input.B.IsPressed()))
            {
                _queueAttack = false;
                Actor.CurrentAnimation = AnimationKey.Attacking;
                _attackCooldown.Set(15);
                _audioService.Play(SoundContentKey.Shoot);
            }
            else if(_inputManager.Input.B.IsPressed())
            {
                _queueAttack = true;
            }

            if (_attackCooldown > 0)
                _attackCooldown.Dec();

            _sceneManager.CheckLevelTransitions(Actor);
        }

        protected override void OnCollision(CollisionInfo collisionInfo)
        {
            _playerHurtBehavior.HandleCollision(collisionInfo);            
        }
    }
}
