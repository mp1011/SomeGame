using Microsoft.Xna.Framework;
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
        public Color BackgroundColor => Color.Black;

        public void Initialize()
        {
            SaveAnimations(ActorId.Player, 
                idle:
                    new Animation(
                        new AnimationFrame(SpriteFrameIndex: 0, Duration: 50),
                        new AnimationFrame(SpriteFrameIndex: 1, Duration: 50),
                        new AnimationFrame(SpriteFrameIndex: 2, Duration: 50),
                        new AnimationFrame(SpriteFrameIndex: 1, Duration: 50)),
                jumping:
                    new Animation(
                        new AnimationFrame(SpriteFrameIndex: 4, Duration: 50),
                        new AnimationFrame(SpriteFrameIndex: 3, Duration: 50)),
                attacking:
                    new Animation(
                        new AnimationFrame(SpriteFrameIndex: 5, Duration: 5),
                        new AnimationFrame(SpriteFrameIndex: 6, Duration: 5)),
                moving:
                  new Animation(
                      new AnimationFrame(SpriteFrameIndex: 7, Duration: 10),
                      new AnimationFrame(SpriteFrameIndex: 8, Duration: 10),
                      new AnimationFrame(SpriteFrameIndex: 9, Duration: 10),
                      new AnimationFrame(SpriteFrameIndex: 10, Duration: 10),
                      new AnimationFrame(SpriteFrameIndex: 11, Duration: 10)),
                hurt:  new Animation(SpriteFrameIndex:12,Duration:10));

            SaveAnimations(ActorId.Skeleton,
                idle: new Animation(SpriteFrameIndex: 0, Duration: 10),
                moving:
                    new Animation(
                        new AnimationFrame(SpriteFrameIndex: 1, Duration: 20),
                        new AnimationFrame(SpriteFrameIndex: 2, Duration: 20)),
                attacking: new Animation(SpriteFrameIndex: 3, Duration: 50));

            SaveAnimations(ActorId.SkeletonBone, AnimationKey.Moving,
                new Animation(
                    new AnimationFrame(SpriteFrameIndex: 4, Duration: 10),
                    new AnimationFrame(SpriteFrameIndex: 5, Duration: 10),
                    new AnimationFrame(SpriteFrameIndex: 6, Duration: 10),
                    new AnimationFrame(SpriteFrameIndex: 7, Duration: 10)));

            SaveAnimations(ActorId.Skull, AnimationKey.Moving,
                new Animation(SpriteFrameIndex: 9, Duration: 10));

            SaveAnimations(ActorId.DeadSkeletonBone, AnimationKey.Moving,
                new Animation(
                    new AnimationFrame(SpriteFrameIndex: 4, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 5, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 6, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 7, Duration: 5)));

            SaveAnimations(ActorId.PlayerBullet, AnimationKey.Moving,
                new Animation(
                    new AnimationFrame(SpriteFrameIndex: 0, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 1, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 2, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 1, Duration: 5)));

            SaveAnimations(ActorId.GhostBullet, AnimationKey.Moving,
               new Animation(
                   new AnimationFrame(SpriteFrameIndex: 0, Duration: 5),
                   new AnimationFrame(SpriteFrameIndex: 1, Duration: 5),
                   new AnimationFrame(SpriteFrameIndex: 2, Duration: 5),
                   new AnimationFrame(SpriteFrameIndex: 1, Duration: 5)));

            SaveAnimations(ActorId.Coin, AnimationKey.Idle,
                new Animation(
                    new AnimationFrame(SpriteFrameIndex: 5, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 6, Duration: 5)));

            SaveAnimations(ActorId.Gem, AnimationKey.Idle, 
                new Animation(
                    new AnimationFrame(SpriteFrameIndex: 0, Duration: 5),
                    new AnimationFrame(SpriteFrameIndex: 1, Duration: 5)));

            SaveAnimations(ActorId.Apple, AnimationKey.Idle, new Animation(SpriteFrameIndex: 2, Duration: 5));
            SaveAnimations(ActorId.Meat, AnimationKey.Idle, new Animation(SpriteFrameIndex: 3, Duration: 5));
            SaveAnimations(ActorId.Key, AnimationKey.Idle, new Animation(SpriteFrameIndex: 4, Duration: 5));

            SaveAnimations(ActorId.MovingPlatform, AnimationKey.Moving, new Animation(SpriteFrameIndex: 0, Duration: 5));

            SaveAnimations(ActorId.Spring, AnimationKey.Idle, new Animation(SpriteFrameIndex: 4, Duration: 5));

            SaveAnimations(ActorId.TouchVanishingBlock, AnimationKey.Idle, new Animation(SpriteFrameIndex: 3, Duration: 5));
            SaveAnimations(ActorId.TimedVanishingBlock, AnimationKey.Idle, new Animation(SpriteFrameIndex: 3, Duration: 5));
            SaveAnimations(ActorId.LockBlock, AnimationKey.Idle, new Animation(SpriteFrameIndex: 3, Duration: 5));
            SaveAnimations(ActorId.Lock, AnimationKey.Idle, new Animation(SpriteFrameIndex: 2, Duration: 5));
            SaveAnimations(ActorId.SpikeBlock, AnimationKey.Idle, new Animation(SpriteFrameIndex: 3, Duration: 5));
           
            SaveAnimations(ActorId.SpikeV,
               idle:
                   new Animation(SpriteFrameIndex: 6, Duration: 0),
               disappearing:
                   new Animation(
                            new AnimationFrame(SpriteFrameIndex: 7, Duration: 4),
                            new AnimationFrame(SpriteFrameIndex: 8, Duration: 4),
                            new AnimationFrame(SpriteFrameIndex: 9, Duration: 4)),
               appearing:
                    new Animation(
                           new AnimationFrame(SpriteFrameIndex: 9, Duration: 4),
                            new AnimationFrame(SpriteFrameIndex: 8, Duration: 4),
                            new AnimationFrame(SpriteFrameIndex: 7, Duration: 4)));

            SaveAnimations(ActorId.SpikeH,
              idle:
                  new Animation(SpriteFrameIndex: 10, Duration: 0),
              disappearing:
                  new Animation(
                           new AnimationFrame(SpriteFrameIndex: 11, Duration: 4),
                           new AnimationFrame(SpriteFrameIndex: 12, Duration: 4),
                           new AnimationFrame(SpriteFrameIndex: 13, Duration: 4)),
              appearing:
                   new Animation(
                          new AnimationFrame(SpriteFrameIndex: 13, Duration: 4),
                           new AnimationFrame(SpriteFrameIndex: 12, Duration: 4),
                           new AnimationFrame(SpriteFrameIndex: 11, Duration: 4)));




            SaveAnimations(ActorId.Bat,
                idle: new Animation(
                        new AnimationFrame(SpriteFrameIndex: 0, Duration: 30),
                        new AnimationFrame(SpriteFrameIndex: 1, Duration: 30)),
                moving:
                    new Animation(
                        new AnimationFrame(SpriteFrameIndex: 0, Duration: 5),
                        new AnimationFrame(SpriteFrameIndex: 1, Duration: 5)));

            SaveAnimations(ActorId.Ghost,
               moving: new Animation(SpriteFrameIndex: 0, Duration: 30),
               attacking: new Animation(
                       new AnimationFrame(SpriteFrameIndex: 1, Duration: 10),
                       new AnimationFrame(SpriteFrameIndex: 2, Duration: 10),
                       new AnimationFrame(SpriteFrameIndex: 3, Duration: 10)));
        }

        private void SaveAnimations(ActorId actorId, AnimationKey key, Animation animation)
        {
            var d = new Dictionary<AnimationKey, Animation>();
            d.Add(key, animation);
            SaveAnimations(actorId, d);
        }

        private void SaveAnimations(ActorId actorId, Animation idle = null, Animation attacking = null, 
            Animation hurt = null, Animation jumping = null, Animation moving=null, Animation appearing=null, Animation disappearing=null)
        {
            var d = new Dictionary<AnimationKey, Animation>();
            if(idle != null)
                d.Add(AnimationKey.Idle, idle);
            if (attacking != null)
                d.Add(AnimationKey.Attacking, attacking);
            if (hurt != null)
                d.Add(AnimationKey.Hurt, hurt);
            if (jumping != null)
                d.Add(AnimationKey.Jumping, jumping);
            if (moving != null)
                d.Add(AnimationKey.Moving, moving);
            if (appearing != null)
                d.Add(AnimationKey.Appearing, appearing);
            if (disappearing != null)
                d.Add(AnimationKey.Disappearing, disappearing);

            SaveAnimations(actorId, d);
        }

        private void SaveAnimations(ActorId actorId, Dictionary<AnimationKey,Animation> animations)
        {
            _dataSerializer.SaveAnimations(actorId, animations);
        }

        public void OnWindowSizeChanged(Viewport viewport)
        {
        }

        public bool Update(GameTime gameTime)
        {
            return false;
        }
    }
}
