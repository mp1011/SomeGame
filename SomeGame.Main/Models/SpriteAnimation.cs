namespace SomeGame.Main.Models
{
    class SpriteAnimation
    {

        private readonly RamEnum<AnimationKey> _key;
        public AnimationKey Key => _key;
        public RamByte FramesRemaining { get; }
        public RamByte CurrentIndex { get;  }

        public SpriteAnimation(GameSystem gameSystem, AnimationKey key)
        {
            _key = gameSystem.RAM.DeclareEnum(key);
            CurrentIndex = gameSystem.RAM.DeclareByte(254);
            FramesRemaining = gameSystem.RAM.DeclareByte();
        }

        public void ChangeKey(AnimationKey key)
        {
            _key.Set(key);
            FramesRemaining.Set(0);
            CurrentIndex.Set(254);
        }
    }
}
