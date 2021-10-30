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
        public IDestroyedBehavior DestroyedBehavior { get; }
        public Rectangle LocalHitbox { get; }

        public SpriteAnimator Animator { get; }

        public ICollisionDetector CollisionDetector { get; }
        public GameRectangleWithSubpixels WorldPosition { get; set; }
        public PixelPoint MotionVector { get; set; } = new PixelPoint(0, 0);
        public TilesetContentKey Tileset { get; }
        public PaletteIndex Palette { get; set; }

        public Flip Flip { get; set; } 

        public bool Enabled { get; private set; }

        public AnimationKey CurrentAnimation { get; set; }

        public bool IsAnimationFinished { get; set; }

        public bool HasBeenActivated { get; private set; }
        public bool Destroying { get; private set; }

        //todo, assumes left/right
        public Direction FacingDirection
        {
            get => Flip == Flip.None ? Direction.Right : Direction.Left;
            set
            {
                if (value == Direction.Right)
                    Flip = Flip.None;
                else if (value == Direction.Left)
                    Flip = Flip.H;
            }
        }

        public Actor(ActorType actorType,
                     TilesetContentKey tilesetKey, 
                     PaletteIndex palette,
                     Behavior behavior,
                     IDestroyedBehavior destroyedBehavior,
                     ICollisionDetector collisionDetector,
                     Rectangle localHitbox,
                     SpriteAnimator animator)
        {
            ActorType = actorType;
            WorldPosition = new GameRectangleWithSubpixels(0, 0, 16, 16);
            Behavior = behavior;
            DestroyedBehavior = destroyedBehavior;
            CollisionDetector = collisionDetector;
            Palette = palette;
            Tileset = tilesetKey;
            LocalHitbox = localHitbox;
            Animator = animator;
        }

        public void Destroy()
        {
            Enabled = false;

            if (DestroyedBehavior == null)
                return;
                           
            Destroying = true;
            DestroyedBehavior.OnDestroyed(this);            
        }

        public void OnBeingDestroyed()
        {
            if (DestroyedBehavior.Update(this) == DestroyedState.Destroyed)
                Destroying = false;
        }

        public void Create()
        {
            HasBeenActivated = true;
            Destroying = false;
            Behavior.OnCreated(this);
            Enabled = true;
        }

        public override string ToString()
        {
            if (Enabled)
                return $"{ActorType}";
            else
                return $"{ActorType} (disabled)";
        }
    }
}
