namespace SomeGame.Main.Models
{
    record CollisionInfo(bool IsOnGround = false)
    {
        public static CollisionInfo operator +(CollisionInfo c1, CollisionInfo c2)
        {
            return new CollisionInfo(c1.IsOnGround || c2.IsOnGround);
        }
    }
}
