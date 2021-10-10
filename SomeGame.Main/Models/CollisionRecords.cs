namespace SomeGame.Main.Models
{
    record CollisionInfo(PixelValue XCorrection, PixelValue YCorrection, bool IsOnGround = false)
    {
        public CollisionInfo(bool IsOnGround=false) : this(new PixelValue(0,0), new PixelValue(0, 0), IsOnGround)
        {
        }

        public static CollisionInfo operator +(CollisionInfo c1, CollisionInfo c2)
        {
            return new CollisionInfo(c1.XCorrection + c2.XCorrection, c1.YCorrection + c2.YCorrection, c1.IsOnGround || c2.IsOnGround);
        }

        public override string ToString()
        {
            var groundOrAir = IsOnGround ? "On Ground" : "In Air";
            return $"{groundOrAir} X={XCorrection} Y={YCorrection}";
        }
    }
}
