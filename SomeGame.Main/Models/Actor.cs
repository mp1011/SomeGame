using Microsoft.Xna.Framework;
using SomeGame.Main.Behaviors;
using SomeGame.Main.Content;
using System.Collections.Generic;

namespace SomeGame.Main.Models
{
    class Actor
    {
        public Behavior Behavior { get; }
        public GameRectangle WorldPosition { get; set; }
        public Point MotionVector { get; set; }
        public TilesetContentKey Tileset { get; }
        public PaletteIndex Palette { get; }

        public AnimationKey CurrentAnimation { get; set; }

        private Dictionary<AnimationKey, byte> _animationSet = new Dictionary<AnimationKey, byte>();
        public byte CurrentAnimationIndex => _animationSet[CurrentAnimation];

        public Actor(TilesetContentKey tilesetKey, 
                     PaletteIndex palette,
                     Behavior behavior,
                     Dictionary<AnimationKey, byte> animationSet)
        {
            WorldPosition = new GameRectangle(0, 0, 16, 16);
            Behavior = behavior;
            Palette = palette;
            Tileset = tilesetKey;
            _animationSet = animationSet;
        }
    }
}
