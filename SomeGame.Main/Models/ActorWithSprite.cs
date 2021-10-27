namespace SomeGame.Main.Models
{
    class ActorWithSprite 
    {
        public Actor Actor { get; }
        public SpriteIndex SpriteIndex { get; set; }
        public bool NeedsSprite { get; set; } = true;
        public ActorWithSprite(Actor actor, SpriteIndex spriteIndex)
        {
            Actor = actor;
            SpriteIndex = spriteIndex;
        }

        public override string ToString() => $"{Actor} {SpriteIndex} N={NeedsSprite}";
    }
}
