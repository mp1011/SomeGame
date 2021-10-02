using Microsoft.Xna.Framework;
using SomeGame.Main.Behaviors;
using SomeGame.Main.Content;
using System.Collections.Generic;

namespace SomeGame.Main.Models
{
    class Actor
    {
        public Behavior Behavior { get; }
        public GameRectangleWithSubpixels WorldPosition { get; set; }
        public PixelPoint MotionVector { get; set; } = new PixelPoint(0, 0);
        public TilesetContentKey Tileset { get; }
        public PaletteIndex Palette { get; }

        public Flip Flip { get; set; } 

        public AnimationKey CurrentAnimation { get; set; }

        private Dictionary<AnimationKey, byte> _animationSet = new Dictionary<AnimationKey, byte>();
        public byte CurrentAnimationIndex => _animationSet[CurrentAnimation];

        public Actor(TilesetContentKey tilesetKey, 
                     PaletteIndex palette,
                     Behavior behavior,
                     Dictionary<AnimationKey, byte> animationSet)
        {
            WorldPosition = new GameRectangleWithSubpixels(0, 0, 16, 16);
            Behavior = behavior;
            Palette = palette;
            Tileset = tilesetKey;
            _animationSet = animationSet;
        }
    }
}
