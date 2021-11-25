namespace SomeGame.Main.Models
{
    record CollisionInfo(int XCorrection=0, int YCorrection=0, bool IsOnGround = false, bool IsFacingLedge=false, Actor Actor=null)
    {
        public static CollisionInfo operator +(CollisionInfo c1, CollisionInfo c2)
        {
            if (c2 == null)
                return c1;
            else if (c1 == null)
                return c2;

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
