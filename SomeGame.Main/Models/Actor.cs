using Microsoft.Xna.Framework;
using SomeGame.Main.Behaviors;
using SomeGame.Main.Content;
using SomeGame.Main.Services;
using System.Collections.Generic;

namespace SomeGame.Main.Models
{
    class Actor
    {
        public ActorType ActorType { get; }
        public Behavior Behavior { get; }
        public Rectangle LocalHitbox { get; }

        public SpriteAnimator Animator { get; }

        public ICollisionDetector CollisionDetector { get; }
        public GameRectangleWithSubpixels WorldPosition { get; set; }
        public PixelPoint MotionVector { get; set; } = new PixelPoint(0, 0);
        public TilesetContentKey Tileset { get; }
        public PaletteIndex Palette { get; set; }

        public Flip Flip { get; set; } 

        public bool Enabled { get; set; }

        public AnimationKey CurrentAnimation { get; set; }

        public bool IsAnimationFinished { get; set; }

        public Actor(ActorType actorType,
                     TilesetContentKey tilesetKey, 
                     PaletteIndex palette,
                     Behavior behavior,
                     ICollisionDetector collisionDetector,
                     Rectangle localHitbox,
                     SpriteAnimator animator)
        {
            ActorType = actorType;
            WorldPosition = new GameRectangleWithSubpixels(0, 0, 16, 16);
            Behavior = behavior;
            CollisionDetector = collisionDetector;
            Palette = palette;
            Tileset = tilesetKey;
            LocalHitbox = localHitbox;
            Animator = animator;
        }
    }
}
