using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SomeGame.Main.Content;
using SomeGame.Main.Models;
using SomeGame.Main.Services;
using System;
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

        public void Initialize(ResourceLoader resourceLoader, GraphicsDevice graphicsDevice)
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

            var playerBulletAnimations = new Dictionary<AnimationKey, Animation>();
            playerBulletAnimations.Add(AnimationKey.Moving,
                new Animation(
                    new AnimationFrame(SpriteFrameIndex: 0, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 1, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 2, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 1, Duration: 5)));

            _dataSerializer.SaveAnimations(ActorId.Player, playerAnimations);
            _dataSerializer.SaveAnimations(ActorId.PlayerBullet, playerBulletAnimations);
            _dataSerializer.SaveAnimations(ActorId.Skeleton, skeletonAnimations);
            _dataSerializer.SaveAnimations(ActorId.SkeletonBone, skeletonBoneAnimations);
        }

        public void OnWindowSizeChanged(Viewport viewport)
        {
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
