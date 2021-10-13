namespace SomeGame.Main.Models
{
    class SpriteAnimation
    {
        public AnimationKey Key { get; }
        public byte FramesRemaining { get; set; }
        public byte CurrentIndex { get; set; }

        public SpriteAnimation(AnimationKey key)
        {
            Key = key;
            CurrentIndex = 254;
        }
    }
}
