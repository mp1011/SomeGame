namespace SomeGame.Main.Models.AnimationModels
{
    class SpriteAnimation
    {
        public byte AnimationIndex { get; }
        public byte FramesRemaining { get; set; }
        public byte CurrentIndex { get; set; }

        public SpriteAnimation(byte animationIndex)
        {
            AnimationIndex = animationIndex;
            CurrentIndex = 254;
        }
    }
}
