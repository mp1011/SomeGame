namespace SomeGame.Main.Models
{
    record CollisionInfo(PixelValue XCorrection, PixelValue YCorrection, bool IsOnGround = false, bool IsFacingLedge=false, Actor Actor=null)
    {
        public CollisionInfo(bool IsOnGround=false, bool IsFacingLedge=false) : 
            this(new PixelValue(0,0), new PixelValue(0, 0), IsOnGround, IsFacingLedge)
        {
        }

        public CollisionInfo(Actor actor) : this(new PixelValue(0, 0), new PixelValue(0, 0), IsOnGround:false, Actor:actor )
        {
        }

        public static CollisionInfo operator +(CollisionInfo c1, CollisionInfo c2)
        {
            return new CollisionInfo(XCorrection: c1.XCorrection + c2.XCorrection, 
                                     YCorrection: c1.YCorrection + c2.YCorrection, 
                                     IsOnGround: c1.IsOnGround || c2.IsOnGround,
                                     IsFacingLedge: c1.IsFacingLedge || c2.IsFacingLedge,
                                     Actor: c1.Actor ?? c2.Actor);
        }

        public override string ToString()
        {
            var groundOrAir = IsOnGround ? "On Ground" : "In Air";
            return $"{groundOrAir} X={XCorrection} Y={YCorrection}";
        }
    }
}
