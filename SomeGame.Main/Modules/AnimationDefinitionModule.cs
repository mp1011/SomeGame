﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System.Collections.Generic;

namespace SomeGame.Main.Modules
{
    class AnimationDefinitionModule : IGameModule
    {
        private readonly DataSerializer _dataSerializer = new DataSerializer();
        public Rectangle Screen => new Rectangle(0, 0, 10, 10);

        public void Draw(SpriteBatch spriteBatch)
        {
        }

        public void Initialize()
        {
            var playerAnimations = new Dictionary<AnimationKey, Animation>();          
            playerAnimations.Add(AnimationKey.Idle,
                new Animation(
                    new AnimationFrame(SpriteFrameIndex: 0, Duration: 50),
                    new AnimationFrame(SpriteFrameIndex: 1, Duration: 50),
                    new AnimationFrame(SpriteFrameIndex: 2, Duration: 50),
                    new AnimationFrame(SpriteFrameIndex: 1, Duration: 50)));

            playerAnimations.Add(AnimationKey.Jumping,
               new Animation(
                   new AnimationFrame(SpriteFrameIndex: 4, Duration: 50),
                   new AnimationFrame(SpriteFrameIndex: 3, Duration: 50)));

            playerAnimations.Add(AnimationKey.Attacking,
                 new Animation(
                     new AnimationFrame(SpriteFrameIndex: 5, Duration: 5),
                     new AnimationFrame(SpriteFrameIndex: 6, Duration: 5)));

            playerAnimations.Add(AnimationKey.Moving,
                  new Animation(
                      new AnimationFrame(SpriteFrameIndex: 7, Duration: 10),
                      new AnimationFrame(SpriteFrameIndex: 8, Duration: 10),
                      new AnimationFrame(SpriteFrameIndex: 9, Duration: 10),
                      new AnimationFrame(SpriteFrameIndex: 10, Duration: 10),
                      new AnimationFrame(SpriteFrameIndex: 11, Duration: 10)));

            playerAnimations.Add(AnimationKey.Hurt,
                 new Animation(
                   new AnimationFrame(SpriteFrameIndex:12,Duration:10)));

            var skeletonAnimations = new Dictionary<AnimationKey, Animation>();
            skeletonAnimations.Add(AnimationKey.Idle,
                new Animation(
                     new AnimationFrame(SpriteFrameIndex: 0, Duration: 10)));

            skeletonAnimations.Add(AnimationKey.Moving,
                new Animation(
                    new AnimationFrame(SpriteFrameIndex: 1, Duration: 20),
                    new AnimationFrame(SpriteFrameIndex: 2, Duration: 20)));

            skeletonAnimations.Add(AnimationKey.Attacking,
                new Animation(
                    new AnimationFrame(SpriteFrameIndex: 3, Duration: 50)));

            var skeletonBoneAnimations = new Dictionary<AnimationKey, Animation>();
            skeletonBoneAnimations.Add(AnimationKey.Moving,
                new Animation(
                    new AnimationFrame(SpriteFrameIndex: 4, Duration: 10),
                    new AnimationFrame(SpriteFrameIndex: 5, Duration: 10),
                    new AnimationFrame(SpriteFrameIndex: 6, Duration: 10),
                    new AnimationFrame(SpriteFrameIndex: 7, Duration: 10)));

            var skullAnimations = new Dictionary<AnimationKey, Animation>();
            skullAnimations.Add(AnimationKey.Moving,
                new Animation(
                    new AnimationFrame(SpriteFrameIndex: 9, Duration: 10)));

            var deadSkeletonBoneAnimations = new Dictionary<AnimationKey, Animation>();
            deadSkeletonBoneAnimations.Add(AnimationKey.Moving,
                new Animation(
                    new AnimationFrame(SpriteFrameIndex: 4, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 5, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 6, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 7, Duration: 5)));


            var playerBulletAnimations = new Dictionary<AnimationKey, Animation>();
            playerBulletAnimations.Add(AnimationKey.Moving,
                new Animation(
                    new AnimationFrame(SpriteFrameIndex: 0, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 1, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 2, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 1, Duration: 5)));


            var coinAnimations = new Dictionary<AnimationKey, Animation>();
            coinAnimations.Add(AnimationKey.Idle,
                new Animation(
                    new AnimationFrame(SpriteFrameIndex: 5, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 6, Duration: 5)));

            _dataSerializer.SaveAnimations(ActorId.Player, playerAnimations);
            _dataSerializer.SaveAnimations(ActorId.PlayerBullet, playerBulletAnimations);
            _dataSerializer.SaveAnimations(ActorId.Skeleton, skeletonAnimations);
            _dataSerializer.SaveAnimations(ActorId.SkeletonBone, skeletonBoneAnimations);
            _dataSerializer.SaveAnimations(ActorId.Coin, coinAnimations);
            _dataSerializer.SaveAnimations(ActorId.Skull, skullAnimations);
            _dataSerializer.SaveAnimations(ActorId.DeadSkeletonBone, deadSkeletonBoneAnimations);

        }

        public void OnWindowSizeChanged(Viewport viewport)
        {
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
