using Microsoft.Xna.Framework;
using SomeGame.Main.Behaviors;
using SomeGame.Main.Content;
using SomeGame.Main.Services;

namespace SomeGame.Main.Models
{
    class Actor
    {
        private readonly RamEnum<ActorFlags> _flags;
        private readonly RamEnum<PaletteIndex> _palette;
        private readonly RamEnum<AnimationKey> _animation;
        private readonly RamEnum<ActorState> _state;

        public ActorState State
        {
            get => _state;
            set => _state.Set(value);
        }

        public RamEnum<ActorType> ActorType { get; }
        public Behavior Behavior { get; }
        public IDestroyedBehavior DestroyedBehavior { get; }
        public RamRectangle LocalHitbox { get; }

        public SpriteAnimator Animator { get; }

        public ICollisionDetector CollisionDetector { get; }
        public RamGameRectangle WorldPosition { get; set; }
        public RamPixelPoint MotionVector { get; } 
        public RamEnum<TilesetContentKey> Tileset { get; }
      
        public PaletteIndex Palette
        {
            get => _palette;
            set => _palette.Set(value);
        }

       
        public Flip Flip
        {
            get
            {
                var ret = Flip.None;
                if ((_flags & ActorFlags.FlipH) > 0)
                    ret = ret | Flip.FlipH;
                if ((_flags & ActorFlags.FlipV) > 0)
                    ret = ret | Flip.FlipV;

                return ret;
            }
            set
            {
                _flags.SetFlag(ActorFlags.FlipH, (value & Flip.FlipH) > 0);
                _flags.SetFlag(ActorFlags.FlipV, (value & Flip.FlipV) > 0);
            }
        }

        public bool Enabled => State == ActorState.Active || State == ActorState.Destroying;

        public AnimationKey CurrentAnimation
        {
            get => _animation;
            set => _animation.Set(value);
        }

        public bool IsAnimationFinished
        {
            get => (_flags & ActorFlags.IsAnimationFinished) > 0;
            set => _flags.SetFlag(ActorFlags.IsAnimationFinished, value);
        }

        public bool NeedsSprite
        {
            get => (_flags & ActorFlags.SpriteRequired) > 0;
            set => _flags.SetFlag(ActorFlags.SpriteRequired, value);
        }

        public bool Destroying
        {
            get => State == ActorState.Destroying;
            set => State = ActorState.Destroying;
        }

        public bool Visible
        {
            get => (_flags & ActorFlags.Visible) > 0;
            set => _flags.SetFlag(ActorFlags.Visible, value);
        }

        //todo, assumes left/right
        public Direction FacingDirection
        {
            get => Flip == Flip.None ? Direction.Right : Direction.Left;
            set
            {
                if (value == Direction.Right)
                    Flip = Flip.None;
                else if (value == Direction.Left)
                    Flip = Flip.FlipH;
            }
        }

        public Actor(GameSystem gameSystem,
                     ActorType actorType,
                     TilesetContentKey tilesetKey, 
                     Behavior behavior,
                     IDestroyedBehavior destroyedBehavior,
                     ICollisionDetector collisionDetector,
                     Rectangle localHitbox,
                     SpriteAnimator animator)
        {
            _state = gameSystem.RAM.DeclareEnum(ActorState.WaitingForActivation);            
            _flags = gameSystem.RAM.DeclareEnum(ActorFlags.Visible);
            _palette = gameSystem.RAM.DeclareEnum(PaletteIndex.P1);
            _animation = gameSystem.RAM.DeclareEnum(AnimationKey.Idle);
            ActorType = gameSystem.RAM.DeclareEnum(actorType);
            WorldPosition = gameSystem.RAM.DeclareGameRectangleWithSubpixels(16,16);
            Tileset = gameSystem.RAM.DeclareEnum(tilesetKey);
            LocalHitbox = gameSystem.RAM.DeclareRectangle(localHitbox);
            MotionVector = gameSystem.RAM.DeclarePixelPoint();
            Animator = animator;
            Behavior = behavior;
            DestroyedBehavior = destroyedBehavior;
            CollisionDetector = collisionDetector;     
        }

        public void Destroy()
        {
            if (DestroyedBehavior == null)
                return;
                           
            Destroying = true;
            DestroyedBehavior.OnDestroyed(this);            
        }

        public void Create()
        {
            State = ActorState.Active;
            Behavior.OnCreated(this);
            NeedsSprite = true;
        }

        public override string ToString() => $"{ActorType} ({State})";
    }
}
